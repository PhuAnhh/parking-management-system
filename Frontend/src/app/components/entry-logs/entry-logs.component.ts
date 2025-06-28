import { Component, OnInit, ChangeDetectorRef, ViewChild, ElementRef } from '@angular/core';
import { EntryLogService } from '../../cores/services/entry-log.service';
import { ExitLogService } from '../../cores/services/exit-log.service';
import { CardService } from '../../cores/services/card.service';
import { CardGroupService } from '../../cores/services/card-group.service';
import { LaneService } from '../../cores/services/lane.service';
import { CustomerService } from '../../cores/services/customer.service';
import { LoginService } from '../../cores/services/login.service';
import { PlateRecognitionService } from '../../cores/services/plate-recognition.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { CardGroupVehicleType } from '../../cores/enums/card-group-vehicle-type';
import { LaneType } from '../../cores/enums/lane-type.enum';
import { CardStatus } from '../../cores/enums/card-status';

@Component({
  selector: 'app-entry-logs',
  standalone: false,
  templateUrl: './entry-logs.component.html',
  styleUrl: './entry-logs.component.scss'
})
export class EntryLogsComponent implements OnInit{
  
  @ViewChild('videoPlayer') videoPlayer!: ElementRef<HTMLVideoElement>;

  entryLogs: any[] = [];
  cards: any[] = [];
  cardGroups: any[] = [];
  lanes: any[] = [];
  customers: any[] = [];

  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  searchKeyword: string = '';
  selectedCardGroupId: number | null = null;
  selectedLaneId: number | null = null;
  selectedDateRange: Date[] | null = null;
  selectedEntryLogDetail: any = null;

  isAddEntryModalVisible = false; 
  isAddExitModalVisible = false;
  isShowDetailModalVisible = false;
  selectedEntryLog: any = null;
  filteredEntryLanes: any[] = [];
  filteredExitLanes: any[] = [];
  availableCards: any[] = []; 
  usedCardIds: number[] = [];

  entryLogForm!: FormGroup; 
  exitLogForm!: FormGroup;

  totalVehicles: number = 0;
  totalCars: number = 0;
  totalMotorbikes: number = 0;
  totalBicycles: number = 0;

  videoElement!: HTMLVideoElement;
  capturedImage: string | null = null;
  cameraOn = false;
  recognizedPlate: string | null = null;

  vehicleTypes = [    
    { label: 'Ô tô', value: CardGroupVehicleType.CAR, color: '#d46b08' },
    { label: 'Xe máy', value: CardGroupVehicleType.MOTORBIKE, color: "#08979C" },
    { label: 'Xe đạp', value: CardGroupVehicleType.BICYCLE, color: '#389e0d' }
  ]

  constructor(
      private entryLogService: EntryLogService, 
      private exitLogService: ExitLogService,
      private cardService: CardService, 
      private cardGroupService: CardGroupService,
      private laneService: LaneService,
      private customerService: CustomerService,
      private cdr: ChangeDetectorRef,
      private modalService: NzModalService,
      private fb: FormBuilder,
      private notification: NzNotificationService,
      public loginService: LoginService,
      private plateService: PlateRecognitionService,
    ) {
      this.initForm();
    }  

  ngOnInit() {
    // Tải cards và cardGroups
    this.cardService.getCards().subscribe(data => {
      this.cards = data;
      
      // Sau khi có cards, tải cardGroups
      this.cardGroupService.getCardGroups().subscribe(data => {
        this.cardGroups = data;
        
        // Sau khi đã có cả cards và cardGroups, tải entryLogs
        this.loadEntryLogs();

        // Cập nhật danh sách thẻ có thể sử dụng
        this.updateAvailableCards();
      });
    });
    
    // Tải lanes và customers
    this.loadLanes();
    this.loadCustomers();
  }

  initForm() {
    this.entryLogForm = this.fb.group({
      plateNumber: [null, [Validators.required]],
      cardId: [null, [Validators.required]],
      cardGroupId: [null, [Validators.required]],
      laneId: [null, [Validators.required]],
      customerId: [null],
      imageUrl: [null],
      note: [null],
    });

    this.exitLogForm = this.fb.group({
      exitPlateNumber: [null],
      exitLaneId: [null, [Validators.required]],
      note: [null, [Validators.required]],
      imageUrl: [null]
    });
  }

  updateAvailableCards(): void {
    // Lấy danh sách ID thẻ đã được sử dụng (xe chưa ra khỏi bãi)
    this.usedCardIds = this.entryLogs.map(log => log.cardId);

    // Lọc ra những thẻ chưa được sử dụng và đang ở trạng thái Inactive
    this.availableCards = this.cards.filter(card =>
      !this.usedCardIds.includes(card.id) &&
      card.status === CardStatus.INACTIVE
    );
  }

  loadEntryLogs(searchKeyword: string = ''): void {
    this.loading = true;

    const serviceCall = (this.selectedDateRange?.length === 2)
      ? this.entryLogService.getEntryLogsByDateRange(
          this.selectedDateRange[0],
          this.selectedDateRange[1]
        )
      : this.entryLogService.getEntryLogs();

    serviceCall.subscribe({
      next: (data: any[]) => {
        const filteredLogs = this.filterEntryLogs(data, searchKeyword);

        filteredLogs.sort((a, b) =>
          new Date(b.entryTime + 'Z').getTime() - new Date(a.entryTime + 'Z').getTime()
        );

        this.total = filteredLogs.length;
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.entryLogs = filteredLogs.slice(start, end); 

        this.totalVehicles = filteredLogs.length;
        this.totalCars = this.countVehiclesByType(CardGroupVehicleType.CAR, filteredLogs);
        this.totalMotorbikes = this.countVehiclesByType(CardGroupVehicleType.MOTORBIKE, filteredLogs);
        this.totalBicycles = this.countVehiclesByType(CardGroupVehicleType.BICYCLE, filteredLogs);

        this.updateAvailableCards();

        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Lỗi khi lấy danh sách xe vào bãi:', err);
        this.notification.error('Lỗi', 'Không thể tải dữ liệu xe vào bãi');
        this.loading = false;
      }
    });
  }

  private filterEntryLogs(data: any[], keyword: string): any[] {
    let result = data;

    if (keyword) {
      result = result.filter(entry =>
        entry.plateNumber?.toLowerCase().includes(keyword.toLowerCase())
      );
    }

    if (this.selectedCardGroupId) {
      result = result.filter(entry => entry.cardGroupId === this.selectedCardGroupId);
    }

    if (this.selectedLaneId) {
      result = result.filter(entry => entry.laneId === this.selectedLaneId);
    }

    if (this.selectedDateRange && this.selectedDateRange.length === 2) {
      const startDate = new Date(this.selectedDateRange[0]);
      const endDate = new Date(this.selectedDateRange[1]);
      
      startDate.setHours(0, 0, 0, 0);
      endDate.setHours(23, 59, 59, 999);

      result = result.filter(entry => {
        if (!entry.entryTime) return false;
        
        const entryLocalTime = new Date(entry.entryTime + 'Z');
        
        return entryLocalTime >= startDate && entryLocalTime <= endDate;
      });
    }

    return result;
  }

  loadCards() {
    this.cardService.getCards().subscribe(data => {
      this.cards = data;
    });
  }

  loadCardGroups(){
    this.cardGroupService.getCardGroups().subscribe(data => {
      this.cardGroups = data;
    });
  }

  loadLanes(){
    this.laneService.getLanes().subscribe(data => {
      this.lanes = data;
    });
  }

  loadCustomers(){
    this.customerService.getCustomers().subscribe(data => {
      this.customers = data;
    });
  }

  loadExitLogs(){
    this.exitLogService.getExitLogs().subscribe(data => {
      this.entryLogs = data;
    });
  }

  countVehiclesByType(vehicleType: CardGroupVehicleType, logs: any[]): number {
    return logs.filter(log => {
      const card = this.cards.find(c => c.id === log.cardId);
      const cardGroup = card ? this.cardGroups.find(cg => cg.id === card.cardGroupId) : null;
      return cardGroup && cardGroup.vehicleType === vehicleType;
    }).length;
  }

  getFormattedTime(utcTimeString: string): Date | null {
    if (!utcTimeString) return null;
    return new Date(utcTimeString + 'Z');
  }

  getCardNameById(cardId: number): string {
    const card = this.cards.find(c => c.id === cardId);
    return card ? card.name : '';
  }

  getCardCodeById(cardId: number): string {
    const card = this.cards.find(c => c.id === cardId);
    return card ? card.code : '';
  }

  getCardGroupNameById(cardGroupId: number): string {
    const cardGroup = this.cardGroups.find(cg => cg.id === cardGroupId);
    return cardGroup ? cardGroup.name : '';
  }

  getVehicleTypeInfoByCardGroupId(cardGroupId: number): { label: string; color: string } | null {
    const cardGroup = this.cardGroups.find(g => g.id === cardGroupId);
    if (!cardGroup) return null;

    return this.vehicleTypes.find(v => v.value === cardGroup.vehicleType) || null;
  }

  getCustomerNameById(customerId: number): string {
    const customer = this.customers.find(c => c.id === customerId);
    return customer ? customer.name : '';
  }

  getLaneNameById(laneId: number): string {
    const lane = this.lanes.find(l => l.id === laneId);
    return lane ? lane.name : '';
  }

  get carPercent(): number {
    return this.totalVehicles === 0 ? 0 : Math.round((this.totalCars / this.totalVehicles) * 100);
  }

  get motorbikePercent(): number {
    return this.totalVehicles === 0 ? 0 : Math.round((this.totalMotorbikes / this.totalVehicles) * 100);
  }

  get bicyclePercent(): number {
    return this.totalVehicles === 0 ? 0 : Math.round((this.totalBicycles / this.totalVehicles) * 100);
  }

  getEntryLanes(): any[] {
    return this.lanes.filter(lane => lane.type !== LaneType.OUT);
  }

  getExitLanes(): any[] {
    return this.lanes.filter(lane => lane.type !== LaneType.IN);
  }

  onSearch() {
    console.log(this.searchKeyword);
    this.loadEntryLogs(this.searchKeyword); 
  }

  onCardGroupChange(): void {
    this.loadEntryLogs();
  }

  onLaneChange(): void {
    this.loadEntryLogs();
  }

  onDateRangeChange() : void {
    this.loadEntryLogs();
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadEntryLogs(this.searchKeyword);
  }

  showAddEntryLogModal() {
    this.isAddEntryModalVisible = true;
    this.entryLogForm.reset(); 

    this.updateAvailableCards();
  }

  showAddExitLogModal(entryLog?: any) {
    this.selectedEntryLog = entryLog;
    this.isAddExitModalVisible = true;
    this.exitLogForm.reset();
    this.filteredExitLanes = [];

    if (entryLog) {
      this.exitLogForm.patchValue({
        exitPlateNumber: entryLog.plateNumber
      });

      const selectedCard = this.cards.find(card => card.id === entryLog.cardId);
      const cardGroupId = selectedCard?.cardGroupId || null;

      const cardGroup = this.cardGroups.find(group => group.id === cardGroupId);

      if (cardGroup?.laneIds?.length) {
        this.filteredExitLanes = this.lanes.filter(lane =>
          cardGroup.laneIds.includes(lane.id) &&
          (lane.type === 'Out' || lane.type === 'Dynamic' || lane.type === 'KioskIn')
        );
      }
    }
  }

  showDetailModal(entryLogId: number): void {
    this.loading = true;
    
    this.entryLogService.getEntryLogById(entryLogId).subscribe({
      next: (data) => {
        if (data.imageUrl) {
          data.imageUrl = `http://localhost:5000${data.imageUrl}`;
        }
        this.selectedEntryLogDetail = data;
        this.isShowDetailModalVisible = true;
        this.loading = false;
      },
      error: (error) => {
        console.error('Lỗi khi lấy chi tiết sự kiện:', error);
        this.notification.error(
          'Lỗi',
          'Không thể tải chi tiết sự kiện',
          {
            nzPlacement: 'topRight',
            nzDuration: 3000
          }
        );
        this.loading = false;
      }
    });
  }

  handleCancel() {
    this.isAddEntryModalVisible = false;
    this.isAddExitModalVisible = false;
    this.selectedEntryLog = null;
  }

  handleDetailCancel(): void {
    this.isShowDetailModalVisible = false;
  }

  handleOk() {
    if (this.entryLogForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }

    const formValue = this.entryLogForm.value;

    const newEntryLog = {
      ...formValue,
      imageUrl: this.capturedImage 
    };

    this.entryLogService.addEntryLog(newEntryLog).subscribe({
      next: () => {
        this.loadEntryLogs();
        this.isAddEntryModalVisible = false; 

        this.capturedImage = null;
        this.cameraOn = false;

        this.notification.success(
          'Thành công',
          '',
          {
            nzPlacement: 'topRight',
            nzDuration: 3000
          }
        );
      },
      error: (error) => {
        let errorMessage = 'Có lỗi xảy ra khi ghi vé vào';
        
        if (error?.error?.message) {
          errorMessage = error.error.message;
        } else if (error?.error?.error) {
          errorMessage = error.error.error;
        } else if (error?.error && typeof error.error === 'string') {
          errorMessage = error.error;
        } else if (error?.message) {
          errorMessage = error.message;
        }
        
        if (errorMessage.includes('Invalid plate number format')) {
          errorMessage = 'Vui lòng nhập đúng định dạng biển số, ví dụ: "29M14838"';
        } else if (errorMessage.includes('Card not found')) {
          errorMessage = 'Không tìm thấy thẻ';
        } else if (errorMessage.includes('Card is locked')) {
          errorMessage = 'Thẻ bị khóa';
        } else if (errorMessage.includes('Card is already in use')) {
          errorMessage = 'Thẻ đang được sử dụng';
        } else if (errorMessage.includes('Card is not in an entry-allowed state')) {
          errorMessage = 'Thẻ không hợp lệ để vào bãi';
        } else if (errorMessage.includes('Card group not found')) {
          errorMessage = 'Không tìm thấy nhóm thẻ cho thẻ đã chọn';
        } else if (errorMessage.includes('Card group is not active')) {
          errorMessage = 'Nhóm thẻ không hoạt động';
        } else if (errorMessage.includes('Lane is not allowed')) {
          errorMessage = 'Nhóm thẻ không được sử dụng làn này';
        } else if (errorMessage.includes('Lane not found')) {
          errorMessage = 'Không tìm thấy làn';
        } else if (errorMessage.includes('Lane is not active')) {
          errorMessage = 'Làn không hoạt động';
        } else if (errorMessage.includes('Card already in use')) {
          errorMessage = 'Thẻ đang được sử dụng - xe chưa ra khỏi bãi';
        } else if (errorMessage.includes('Plate number is already in the parking lot')) {
          errorMessage = 'Biển số xe đã có trong bãi đỗ';
        } else if (errorMessage.includes('Plate number is required')) {
          errorMessage = 'Vui lòng nhập biển số xe';
        } else if (errorMessage.includes('The card has no valid usage period')) {
          errorMessage = 'Thẻ không có hiệu lực sử dụng';
        } else if (errorMessage.includes('The card has expired')) {
          errorMessage = 'Thẻ đã hết hạn';
        } else if (errorMessage.includes('Http failure response')) {
          errorMessage = 'Lỗi kết nối đến server';
        }

        this.notification.error(
          'Lỗi',
          errorMessage,
          {
            nzPlacement: 'topRight',
            nzDuration: 3000
          }
        );
      }
    });
  }

  handleExitOk() {
    if (this.exitLogForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        { nzDuration: 3000 }
      );
      return;
    }

    const formValue = this.exitLogForm.value;

    const entryPlate = this.selectedEntryLog?.plateNumber?.trim().toLowerCase();
    const exitPlate = formValue.exitPlateNumber?.trim().toLowerCase();

    const newExitLog = {
      entryLogId: this.selectedEntryLog?.id,
      exitPlateNumber: formValue.exitPlateNumber,
      exitLaneId: formValue.exitLaneId,
      note: formValue.note,
      imageUrl: this.capturedImage,
    };

    this.exitLogService.addExitLog(newExitLog).subscribe({
      next: () => {
        this.loadEntryLogs();
        this.isAddExitModalVisible = false;
        this.selectedEntryLog = null;
        this.capturedImage = null;
        this.cameraOn = false;
        
        this.notification.success(
          'Thành công',
          '',
          {
            nzPlacement: 'topRight',
            nzDuration: 3000
          }
        );

        if (entryPlate && exitPlate && entryPlate !== exitPlate) {
          this.notification.warning(
            'Biển số vào ra không khớp',
            '',
            {
              nzPlacement: 'topRight',
              nzDuration: 3000
            }
          );
        }
      },
      error: (error) => {
        let errorMessage = 'Có lỗi xảy ra khi ghi vé ra';
        
        if (error?.error?.message) {
          errorMessage = error.error.message;
        } else if (error?.error?.error) {
          errorMessage = error.error.error;
        } else if (error?.error && typeof error.error === 'string') {
          errorMessage = error.error;
        } else if (error?.message) {
          errorMessage = error.message;
        }
        
        if (errorMessage.includes('Invalid plate number format')) {
          errorMessage = 'Vui lòng nhập đúng định dạng biển số, ví dụ: "29M14838"';
        } else if (errorMessage.includes('Entry log not found')) {
          errorMessage = 'Không tìm thấy vé vào';
        } else if (errorMessage.includes('Card not found')) {
          errorMessage = 'Không tìm thấy thẻ';
        } else if (errorMessage.includes('Card is locked')) {
          errorMessage = 'Thẻ bị khóa';
        } else if (errorMessage.includes('Card is not in use')) {
          errorMessage = 'Thẻ chưa được sử dụng để vào bãi';
        } else if (errorMessage.includes('Card group not found')) {
          errorMessage = 'Không tìm thấy nhóm thẻ cho thẻ đã chọn';
        } else if (errorMessage.includes('Card group is not active')) {
          errorMessage = 'Nhóm thẻ không hoạt động';
        } else if (errorMessage.includes('Exit lane is not allowed for this card group')) {
          errorMessage = 'Nhóm thẻ không được sử dụng làn này';
        } else if (errorMessage.includes('Lane is not allowed')) {
          errorMessage = 'Nhóm thẻ không được sử dụng làn này';
        } else if (errorMessage.includes('Exit lane is invalid or inactive')) {
          errorMessage = 'Làn ra không hoạt động';
        } else if (errorMessage.includes('Lane not found')) {
          errorMessage = 'Không tìm thấy làn';
        } else if (errorMessage.includes('Lane is not active')) {
          errorMessage = 'Làn không hoạt động';
        } else if (errorMessage.includes('Entry lane is invalid or inactive')) {
          errorMessage = 'Làn vào không hoạt động';
        } else if (errorMessage.includes('Card already in use')) {
          errorMessage = 'Thẻ đang được sử dụng - xe chưa ra khỏi bãi';
        } else if (errorMessage.includes('Plate number is already in the parking lot')) {
          errorMessage = 'Biển số xe đã có trong bãi đỗ';
        } else if (errorMessage.includes('Plate number is required')) {
          errorMessage = 'Vui lòng nhập biển số xe';
        } else if (errorMessage.includes('The card has no valid usage period')) {
          errorMessage = 'Thẻ không có hiệu lực sử dụng';
        } else if (errorMessage.includes('The card has expired')) {
          errorMessage = 'Thẻ đã hết hạn';
        } else if (errorMessage.includes('Http failure response')) {
          errorMessage = 'Lỗi kết nối đến server';
        }

        this.notification.error(
          'Lỗi',
          errorMessage,
          {
            nzPlacement: 'topRight',
            nzDuration: 3000
          }
        );
      }
    });
  }

  onCardChange(cardId: number): void {
    const selectedCard = this.cards.find(card => card.id === cardId);

    const cardGroupId = selectedCard?.cardGroupId || null;
    const customerId = selectedCard?.customerId || null;

    this.entryLogForm.patchValue({
      cardGroupId,
      customerId
    });

    const cardGroup = this.cardGroups.find(group => group.id === cardGroupId);

    if (cardGroup?.laneIds?.length) {
      this.filteredEntryLanes = this.lanes.filter(lane =>
        cardGroup.laneIds.includes(lane.id) &&
        (lane.type === 'In' || lane.type === 'Dynamic' || lane.type === 'KioskIn')
      );
    } else {
      this.filteredEntryLanes = [];
    }
  }

  deleteEntryLog(id: number) {
    this.modalService.confirm({
      nzTitle:'Bạn có chắc chắn muốn xóa?',
      nzContent: `
        <div class="delete-warning-text">
          LƯU Ý: Bạn sẽ không thể hoàn tác khi xóa sự kiện
        </div>
      `,
      nzMaskClosable: true,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.entryLogService.deleteEntryLog(id).subscribe(
          () => {
            this.loadEntryLogs();
            this.notification.success(
              'Thành công',
              '',
              {
                nzPlacement: 'topRight',
                nzDuration: 3000
              }
            )
          },
          (error) => {
            console.error('Lỗi khi xóa xe vào bãi:', error);
            this.notification.error(
              'Lỗi',
              '', 
              {
                nzPlacement: 'topRight',
                nzDuration: 3000
              }
            );
          }
        );
      }
    });
  }

  // Hàm mở/tắt camera
  openCamera() {
    // Kiểm tra xem camera đã bật chưa
    if (this.cameraOn) {
      // Nếu camera đang bật thì tắt camera
      
      // Lấy element video từ template
      const videoElem = this.videoPlayer?.nativeElement;
      
      // Lấy stream (luồng video) từ element video
      const stream = videoElem?.srcObject as MediaStream;
      
      if (stream) {
        // Dừng tất cả các track (video/audio) trong stream
        stream.getTracks().forEach(track => track.stop());
        
        // Xóa stream khỏi element video
        videoElem.srcObject = null;
      }
      
      // Đặt trạng thái camera thành tắt
      this.cameraOn = false;
      return; // Thoát khỏi hàm
    }

    // Nếu camera chưa bật, thì bật camera
    this.cameraOn = true;

    // Sử dụng setTimeout để đảm bảo DOM đã được render
    setTimeout(() => {
      // Kiểm tra xem videoPlayer đã được khởi tạo chưa
      if (!this.videoPlayer) {
        console.error('videoPlayer chưa được khởi tạo');
        return;
      }

      // Yêu cầu quyền truy cập camera từ trình duyệt
      navigator.mediaDevices.getUserMedia({ video: true })
        .then(newStream => {
          // Khi được cấp quyền thành công
          const videoElem = this.videoPlayer.nativeElement;
          
          // Gán stream video vào element video
          videoElem.srcObject = newStream;
          
          // Bắt đầu phát video
          videoElem.play();
          
          console.log('Camera đã được bật');
        })
        .catch(err => {
          console.error('Lỗi mở camera:', err);
        });
    });
  }

  captureImage() {
    // Lấy element video
    const video = this.videoPlayer.nativeElement;
    
    // Tạo element canvas để vẽ ảnh
    const canvas = document.createElement('canvas');
    
    // Đặt kích thước canvas bằng kích thước video
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;

    // Lấy context 2D để vẽ trên canvas
    const ctx = canvas.getContext('2d');
    
    if (ctx) {
      // Vẽ frame hiện tại của video lên canvas
      ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
      
      // Chuyển đổi canvas thành chuỗi base64 (định dạng PNG)
      const base64Image = canvas.toDataURL('image/png');
      
      // Lưu ảnh đã chụp vào biến
      this.capturedImage = base64Image;

      // Dừng camera sau khi chụp
      const stream = video.srcObject as MediaStream;
      stream?.getTracks().forEach(track => track.stop());
      video.srcObject = null;

      // Đặt trạng thái camera thành tắt
      this.cameraOn = false;

      // Gửi ảnh đi nhận diện biển số
      this.sendCapturedImage();
    }
  }

  sendCapturedImage(): void {
    if (!this.capturedImage) {
      console.warn('Không có ảnh để gửi nhận diện!');
      return;
    }

    // Gọi service để nhận diện biển số từ ảnh
    this.plateService.recognizePlate(this.capturedImage).subscribe({
      next: (result) => {
        // Xử lý khi nhận diện thành công
        
        // Lấy biển số từ kết quả và chuyển thành chữ hoa
        const plate = result?.plate?.toUpperCase?.() || '';

        console.log('Biển số nhận được:', plate);

        // Lưu biển số đã nhận diện
        this.recognizedPlate = plate;
        
        this.entryLogForm.patchValue({
          plateNumber: plate
        });

        this.exitLogForm.patchValue({
          exitPlateNumber: plate
        });
      },
      error: (err) => {
        // Xử lý khi có lỗi trong quá trình nhận diện
        console.error('Lỗi khi gọi API nhận diện:', err);
        
        // Reset biển số về rỗng
        this.recognizedPlate = '';

        // Xóa giá trị trong các form
        this.entryLogForm.patchValue({ plateNumber: '' });
        this.exitLogForm.patchValue({ exitPlateNumber: '' });
      }
    });
  }

  retakePhoto() {
    this.capturedImage = null;
    this.openCamera(); 
  }
}
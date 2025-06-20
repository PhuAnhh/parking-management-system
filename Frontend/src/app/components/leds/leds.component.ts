import { Component, OnInit, ChangeDetectorRef, HostListener} from '@angular/core';
import { ComputerService } from '../../services/computer.service';
import { LedService } from '../../services/led.service';
import { LoginService } from '../../services/login.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';

enum LedType
{
    P10RED = 'P10Red',
    P10FULLCOLOR = 'P10FullColor',
    P762RGY = 'P762Rgy',
    DIRECTIONLED = 'DirectionLed',
    HUIDU = 'Huidu'
}

@Component({
  selector: 'app-leds',
  standalone: false,
  templateUrl: './leds.component.html',
  styleUrl: './leds.component.scss'
})
export class LedsComponent implements OnInit{
  computers: any[] = [];
  leds: any[] = [];
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  isVisible = false;
  isAddModalVisible = false; 
  isEditModalVisible = false; 
  ledForm!: FormGroup; 
  editLedForm!: FormGroup; 
  currentLedId: number | null = null;
  searchKeyword: string = ''

  ledTypes = [
    { label: 'P10Red', value: LedType.P10RED, color:'red' },
    { label: 'P10FullColor', value: LedType.P10FULLCOLOR, color: 'purple' },
    { label: 'P762Rgy', value: LedType.P762RGY, color: 'geekblue' },
    { label: 'DirectionLed', value: LedType.DIRECTIONLED, color: 'orange' },
    { label: 'Huidu', value: LedType.HUIDU,color: 'cyan' }
  ];

  getLedType(value:string){
    return this.ledTypes.find(opt => opt.value === value);
  }
  
  constructor(
    private ledService: LedService, 
    private computerService: ComputerService,
    private cdr: ChangeDetectorRef,
    private modalService: NzModalService,
    private fb:FormBuilder,
    private notification: NzNotificationService,
    public loginService: LoginService
  ) {
    this.initForm();
  } 

  ngOnInit() {
    this.loadLeds();
    this.loadComputers();
  }

  initForm() {
    this.ledForm = this.fb.group({
      name: [null, [Validators.required]],
      code: [null, [Validators.required]],
      computerId: [null, [Validators.required]],
      comport: [null],
      baudrate: [0],
      type: [LedType.DIRECTIONLED],
      status: [true]
    });

    this.editLedForm = this.fb.group({
      name: [null, [Validators.required]],
      code: [null, [Validators.required]],
      computerId: [null, [Validators.required]],
      comport: [null],
      baudrate: [null],
      type: [null],
      status: [true]
    });
  }

  loadLeds(searchKeyword: string = '') {
    this.loading = true;

    this.ledService.getLeds().subscribe(
      (data: any[]) => {
        data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        
        const filteredLeds = searchKeyword
          ? data.filter(led =>
              led.name.toLowerCase().includes(searchKeyword.toLowerCase()) ||
              led.code.toLowerCase().includes(searchKeyword.toLowerCase())
            )
          : data;
        this.total = filteredLeds.length; 
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.leds = filteredLeds.slice(start, end); 
        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách led:', error);
        this.loading = false
      }
    );
  }

  onSearch() {
    console.log(this.searchKeyword);
    this.loadLeds(this.searchKeyword); 
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadLeds(this.searchKeyword);
  }

  loadComputers() {
    this.computerService.getComputers().subscribe(data => {
      this.computers = data;
    });
  }

  getComputerNameById(computerId: number): string {
    const computer = this.computers.find(g => g.id === computerId);
    return computer ? computer.name : '';  
  }

  showAddLedModal() {
    this.isAddModalVisible = true;
    this.ledForm.reset({
      status: true,
      type: LedType.DIRECTIONLED,
      baudrate: 0
    }); 
  }
  
  showEditLedModal(led: any) {
    this.currentLedId = led.id;
    
    if (this.editLedForm) {
      this.editLedForm.patchValue({
        name: led.name,
        code: led.code,
        computerId: led.computerId,
        comport: led.comport,
        baudrate: led.baudrate,
        type: led.type,
        status: led.status 
      });
      this.isEditModalVisible = true;
    } else {
      console.error('Edit form is not initialized');
      this.initForm();
      this.showEditLedModal(led);
    }
  }

  handleCancel() {
    this.isAddModalVisible = false;
  }

  handleEditCancel() {
    this.isEditModalVisible = false;
    this.currentLedId = null;
  }

  handleOk() {
    if (this.ledForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }

    const newLed = this.ledForm.value;

    const isDupicateName = this.leds.some(led => led.name === newLed.name);
    const isDupicateCode = this.leds.some(led => led.code === newLed.code);

    if(isDupicateName) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    if(isDupicateCode) {
      this.notification.error(
        'Lỗi',
        'Mã: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }
    
    this.ledService.addLed(newLed).subscribe(
      () => {
        this.loadLeds();
        this.isAddModalVisible = false;
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
        console.error('Lỗi khi thêm led:', error);
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

  handleEditOk() {
    if (this.editLedForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }
    
    const updatedLed = this.editLedForm.value;

    const isDupicateName = this.leds.some(led =>
      led.name === updatedLed.name && led.id !== this.currentLedId
    );

    const isDupicateCode = this.leds.some(led =>
      led.code === updatedLed.code && led.id !== this.currentLedId
    );

    if(isDupicateName) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    if(isDupicateCode) {
      this.notification.error(
        'Lỗi',
        'Mã: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    console.log('Updated Led:', updatedLed)
    
    if (this.currentLedId) {
      this.ledService.updateLed(this.currentLedId, updatedLed).subscribe(
        () => {
          this.loadLeds();
          this.isEditModalVisible = false;
          this.currentLedId = null;
          this.notification.success(
            'Thành công',
            '',
            {nzDuration: 3000}
          );
        },
        (error) => {
          console.error('Lỗi khi cập nhật', error);

          if(error.error && error.error.message){
            this.notification.error(
              'Lỗi',
              error.error.message,
              {nzDuration: 3000}
            );
          } else {
            this.notification.error(
              'Lỗi',
              'Không thể cập nhật led. Vui lòng thử lại',
              {nzDuration: 3000}
            );
          }
        }
      );
    }
  }

  updateLed(id: number) {
    const led = this.leds.find(g => g.id === id);
    if (led) {
      this.showEditLedModal(led);
    } else {
      console.error(`Led with id ${id} not found`);
    }
  }

  deleteLed(id: number) {
    this.modalService.confirm({
      nzTitle:'Bạn có chắc chắn muốn xóa?',
      nzMaskClosable: true,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.ledService.deleteLed(id).subscribe(
          () => {
            this.loadLeds();
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

  toggleLedStatus(ledId: number) {
    const led = this.leds.find(l => l.id === ledId);
    if (!led) {
      console.error(`Không tìm thấy led với id ${ledId}`);
      return;
    }

    this.modalService.confirm({
      nzTitle: 'Xác nhận thay đổi trạng thái',
      nzMaskClosable: true,
      nzOkText: 'Xác nhận',
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        const newLed = !led.status;
    
        this.ledService.changeLedStatus(ledId, newLed).subscribe(
          () => {
            led.status = newLed;
            
            this.notification.success(
              'Thành công',
              '',
              {nzDuration: 3000}
            );
          },
          (error) => {
            console.error('Lỗi khi cập nhật trạng thái làn:', error);
            this.notification.error(
              'Lỗi',
              '',
              {nzDuration: 3000}
            );
          }
        );
      }
    });
  }
}

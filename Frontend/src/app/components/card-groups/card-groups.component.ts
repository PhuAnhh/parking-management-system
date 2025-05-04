import { Component, OnInit, ChangeDetectorRef, HostListener} from '@angular/core';
import { CardGroupService } from '../../services/card-group.service';
import { LaneService } from '../../services/lane.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { TransferItem, TransferChange } from 'ng-zorro-antd/transfer';

enum CardGroupType{
  MONTH = 'Month',
  DAY = 'Day'
}

enum CardGroupVehicleType{
  CAR = 'Car',
  MOTORBIKE = 'Motorbike',
  BICYCLE = 'Bicycle'
}

@Component({
  selector: 'app-card-groups',
  standalone: false,
  templateUrl: './card-groups.component.html',
  styleUrl: './card-groups.component.scss'
})
export class CardGroupsComponent {
  cardGroups: any[] = [];
  lanes: any[] = [];
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  isAddModalVisible = false; 
  isEditModalVisible = false; 
  cardGroupForm!: FormGroup; 
  editCardGroupForm!: FormGroup; 
  isVisible = false;
  currentCardGroupId: number | null = null;
  searchKeyword: string = '';
  cardGroupLanes: number[] = [];
  transferData: any[] = [];
  isNextBlockVisible = false;
  isFeeConfigExpanded = false;


cardGroupTypes = [
  { label: 'Tháng', value: CardGroupType.MONTH, color: 'pink'},
  { label: 'Ngày', value: CardGroupType.DAY, color: 'red'},
];

cardGroupVehicleTypes = [
  { label: 'Ô tô', value: CardGroupVehicleType.CAR, color: 'orange'},
  { label: 'Xe máy', value: CardGroupVehicleType.MOTORBIKE, color: 'red'},
  { label: 'Xe đạp', value: CardGroupVehicleType.BICYCLE, color: 'green'},
];

getCardGroupType(value: string){
  return this.cardGroupTypes.find(opt => opt.value === value);
}

getCardGroupVehicleType(value: string){
  return this.cardGroupVehicleTypes.find(opt => opt.value === value);
}

constructor(
    private cardGroupService: CardGroupService, 
    private laneService: LaneService,
    private cdr: ChangeDetectorRef,
    private modalService: NzModalService,
    private fb: FormBuilder,
    private notification: NzNotificationService
  ) {
    this.initForm();
  }  

  ngOnInit() {
    this.loadCardGroups();
    this.loadLanes();
  }

  initForm() {
    this.cardGroupForm = this.fb.group({
      code: [null, [Validators.required]],
      name: [null, [Validators.required]],
      type: [CardGroupType.MONTH, [Validators.required]], 
      vehicleType: [CardGroupVehicleType.CAR, [Validators.required]],  
      laneIds: [[]],
      freeMinutes: [null],
      firstBlockMinutes: [null],
      firstBlockPrice: [null],
      nextBlockMinutes: [null],
      nextBlockPrice: [null],
    });

    this.editCardGroupForm = this.fb.group({
      code: [null, [Validators.required]],
      name: [null, [Validators.required]],
      type: [CardGroupType.MONTH, [Validators.required]], 
      vehicleType: [CardGroupVehicleType.CAR, [Validators.required]],  
      laneIds: [[]],
      freeMinutes: [null],
      firstBlockMinutes: [null],
      firstBlockPrice: [null],
      nextBlockMinutes: [null],
      nextBlockPrice: [null],
    });
  }

  loadCardGroups(searchKeyword: string = '') {
    this.loading = true;
  
    this.cardGroupService.getCardGroups().subscribe(
      (data: any[]) => {
        console.log(data);
        const filteredCardGroups = searchKeyword
          ? data.filter(cardGroup =>
              cardGroup.name.toLowerCase().includes(searchKeyword.toLowerCase()) ||
              cardGroup.code.toLowerCase().includes(searchKeyword.toLowerCase())
            )
          : data;
          
          console.log('Kết quả đã lọc:', filteredCardGroups);
  
        this.total = filteredCardGroups.length;
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.cardGroups = filteredCardGroups.slice(start, end); 
        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách nhóm thẻ:', error);
        this.loading = false;
      }
    );
  }
  
  loadLanes() {
    this.laneService.getLanes().subscribe(data => {
      this.lanes = data;
      this.transferData = this.lanes.map(lane => ({
        key: lane.id,
        title: lane.name,
        direction: 'left',
        disabled: false
      }));
    });
  }

  onSearch() {
    console.log(this.searchKeyword);
    this.loadCardGroups(this.searchKeyword); 
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadCardGroups(this.searchKeyword);
  }

  transferChange(ret: TransferChange): void {
    // Lấy các key của items bên phải (đã chọn)
    const rightItems = ret.list.filter(item => item.direction === 'right');
    const selectedLaneIds = rightItems.map(item => item['key']);
    
    // Cập nhật form hiện tại (add hoặc edit)
    if (this.isAddModalVisible) {
      this.cardGroupForm.patchValue({ laneIds: selectedLaneIds });
    } else if (this.isEditModalVisible) {
      this.editCardGroupForm.patchValue({ laneIds: selectedLaneIds });
    }
    
    this.cardGroupLanes = selectedLaneIds;
  }

  showAddCardGroupModal() {
    this.isAddModalVisible = true;
    this.cardGroupForm.reset({
      status: true,
      type: CardGroupType.MONTH,
      vehicleType: CardGroupVehicleType.CAR,
      laneIds: [],
    });
    this.cardGroupLanes = [];
    
    // Đảm bảo transferData được khởi tạo đúng từ danh sách lanes
    this.transferData = this.lanes.map(lane => ({
      key: lane.id,
      title: lane.name,
      direction: 'left',
      disabled: false
    }));
  }

  showNextBlock() {
    this.isNextBlockVisible = true;
  }

  hideNextBlock() {
    this.isNextBlockVisible = false;
    
    // Xóa giá trị của các trường liên quan đến khoảng tiếp theo
    if (this.isEditModalVisible) {
      this.editCardGroupForm.patchValue({
        nextBlockMinutes: null,
        nextBlockPrice: null
      });
    } else if (this.isAddModalVisible) {
      this.cardGroupForm.patchValue({
        nextBlockMinutes: null,
        nextBlockPrice: null
      });
    }
  }

  showEditCardGroupModal(cardGroup: any) {
    this.currentCardGroupId = cardGroup.id;
    this.cardGroupLanes = cardGroup.laneIds || [];
  
    if (this.editCardGroupForm) {
      this.editCardGroupForm.patchValue({
        name: cardGroup.name,
        code: cardGroup.code,
        type: cardGroup.type,
        vehicleType: cardGroup.vehicleType,
        status: cardGroup.status,
        laneIds: this.cardGroupLanes,
        freeMinutes: cardGroup.freeMinutes,
        firstBlockMinutes: cardGroup.firstBlockMinutes,
        firstBlockPrice: cardGroup.firstBlockPrice,
        nextBlockMinutes: cardGroup.nextBlockMinutes,
        nextBlockPrice: cardGroup.nextBlockPrice
      });
  
      // Kiểm tra xem phần "Khoảng tiếp theo" có cần hiển thị không
      this.isNextBlockVisible = !!cardGroup.nextBlockMinutes || !!cardGroup.nextBlockPrice;
  
      // Kiểm tra nếu có bất kỳ cấu hình phí nào
      this.isFeeConfigExpanded =
        !!cardGroup.freeMinutes ||
        !!cardGroup.firstBlockMinutes ||
        !!cardGroup.firstBlockPrice ||
        !!cardGroup.nextBlockMinutes ||
        !!cardGroup.nextBlockPrice;
  
      // Cập nhật lại dữ liệu cho các lane
      this.transferData = this.lanes.map(lane => ({
        key: lane.id,
        title: lane.name,
        direction: this.cardGroupLanes.includes(lane.id) ? 'right' : 'left',
        disabled: false
      }));
  
      this.cdr.detectChanges();
      
      this.isEditModalVisible = true;
    } else {
      console.error('Edit form is not initialized');
      this.initForm();
      this.showEditCardGroupModal(cardGroup);
    }
  }
  

  handleCancel() {
    this.isAddModalVisible = false;
    this.cardGroupLanes = [];
  }
  
  handleEditCancel() {
    this.isEditModalVisible = false;
    this.currentCardGroupId = null;
    this.cardGroupLanes = [];
  }

  handleOk() {
    if (this.cardGroupForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }

    const newCardGroup = this.cardGroupForm.value;

    if (!newCardGroup.laneIds) {
      newCardGroup.laneIds = [];
    }

    const isDupicate = this.cardGroups.some(cardGroup => cardGroup.code === newCardGroup.code);

    if(isDupicate) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    this.cardGroupService.addCardGroup(newCardGroup).subscribe(() => {
      this.loadCardGroups();
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
      console.error('Lỗi khi thêm nhóm thẻ:', error);
      this.notification.error(
        'Lỗi',
        '', 
        {
          nzPlacement: 'topRight',
          nzDuration: 3000
        }
      )
    });
  }

  handleEditOk() {
    if (this.editCardGroupForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }
    
    const updatedCardGroup = {
      ...this.editCardGroupForm.value,
    };

    if (!updatedCardGroup.laneIds) {
      updatedCardGroup.laneIds = [];
    }

    if (!this.isNextBlockVisible) {
      updatedCardGroup.nextBlockMinutes = null;
      updatedCardGroup.nextBlockPrice = null;
    }
    
    console.log('Updated CardGroup:', updatedCardGroup);  

    const isDupicate = this.cardGroups.some(cardGroup =>
      cardGroup.code === updatedCardGroup.code && cardGroup.id !== this.currentCardGroupId
    );

    if (isDupicate) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    if (this.currentCardGroupId) {
      this.cardGroupService.updateCardGroup(this.currentCardGroupId, updatedCardGroup).subscribe(
        () => {
          this.loadCardGroups();
          this.isEditModalVisible = false;
          this.currentCardGroupId = null;
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
              'Không thể cập nhật nhóm thẻ. Vui lòng thử lại',
              {nzDuration: 3000}
            );
          }
        }
      );
    }
  }

  updateCardGroup(id: number) {
    const cardGroup = this.cardGroups.find(g => g.id === id);
    if (cardGroup) {
      this.showEditCardGroupModal(cardGroup);
    } else {
      console.error(`CardGroup with id ${id} not found`);
    }
  }

  deleteCardGroup(id: number) {
    this.modalService.confirm({
      nzTitle:'Bạn có chắc chắn muốn xóa?',
      nzMaskClosable: true,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.cardGroupService.deleteCardGroup(id).subscribe(
          () => {
            this.loadCardGroups();
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
            console.error('Lỗi khi xóa nhóm thẻ:', error);
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
}

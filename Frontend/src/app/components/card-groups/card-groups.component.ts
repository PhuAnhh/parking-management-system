import { Component, OnInit, ChangeDetectorRef, HostListener} from '@angular/core';
import { CardGroupService } from '../../services/card-group.service';
import { LaneService } from '../../services/lane.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';

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

  prepareLaneTransferData() {
    this.lanes = this.lanes.map(lane => ({
      key: lane.id.toString(),
      title: lane.name,
      description: lane.code,
      direction: this.cardGroupLanes.includes(lane.id) ? 'right' : 'left',
      disabled: false
    }));
  }
  
  selectLaneChange(ret: any): void {
    console.log('select change:', ret);
  }
  
  changeLaneTransfer(ret: any): void {
    console.log('change:', ret);
    
    const selectedLaneIds = this.lanes
      .filter(item => item.direction === 'right')
      .map(item => +item.key); 
      
    if (this.isAddModalVisible) {
      this.cardGroupForm.get('laneIds')?.setValue(selectedLaneIds);
    } else if (this.isEditModalVisible) {
      this.editCardGroupForm.get('laneIds')?.setValue(selectedLaneIds);
    }
  }

cardGroupTypes = [
  { label: 'Tháng', value: CardGroupType.MONTH, color: 'pink'},
  { label: 'Ngày', value: CardGroupType.DAY, color: 'red'},
];

cardGroupVehicleTypes = [
  { label: 'Ô tô', value: CardGroupVehicleType.CAR, color: 'pink'},
  { label: 'Xe máy', value: CardGroupVehicleType.MOTORBIKE, color: 'red'},
  { label: 'Xe đạp', value: CardGroupVehicleType.BICYCLE, color: 'yellow'},
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
      status: [true],
      laneIds: [[], []]
    });

    this.editCardGroupForm = this.fb.group({
      code: [null, [Validators.required]],
      name: [null, [Validators.required]],
      type: [CardGroupType.MONTH, [Validators.required]], 
      vehicleType: [CardGroupVehicleType.CAR, [Validators.required]],  
      status: [true],
      laneIds: [[], []]
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
      this.prepareLaneTransferData();
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

  showAddCardGroupModal() {
    this.isAddModalVisible = true;
    this.cardGroupForm.reset({
      status: true,
      type: CardGroupType.MONTH,
      vehicleType: CardGroupVehicleType.CAR
    });
    this.cardGroupLanes = [];
    this.prepareLaneTransferData();
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
        laneIds: this.cardGroupLanes
      });
      
      this.prepareLaneTransferData();
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
    this.prepareLaneTransferData();
  }
  
  handleEditCancel() {
    this.isEditModalVisible = false;
    this.currentCardGroupId = null;
    this.cardGroupLanes = [];
    this.prepareLaneTransferData();
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

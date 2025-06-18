import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CardGroupService } from '../../services/card-group.service';
import { LaneService } from '../../services/lane.service';
import { LoginService } from '../../services/login.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { TransferItem, TransferChange } from 'ng-zorro-antd/transfer';

enum CardGroupType {
  MONTH = 'Month',
  DAY = 'Day'
}

enum CardGroupVehicleType {
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
export class CardGroupsComponent implements OnInit {
  // Data properties
  cardGroups: any[] = [];
  lanes: any[] = [];
  transferData: TransferItem[] = [];
  cardGroupLanes: number[] = [];
  
  // UI state properties
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  isAddModalVisible = false;
  isEditModalVisible = false;
  isNextBlockVisible = false;
  isFeeConfigExpanded = false;
  currentCardGroupId: number | null = null;
  searchKeyword = '';
  
  // Form groups
  cardGroupForm!: FormGroup;
  editCardGroupForm!: FormGroup;
  
  // Options for dropdowns
  cardGroupTypes = [
    { label: 'Tháng', value: CardGroupType.MONTH, color: 'pink' },
    { label: 'Ngày', value: CardGroupType.DAY, color: 'red' }
  ];

  cardGroupVehicleTypes = [
    { label: 'Ô tô', value: CardGroupVehicleType.CAR, color: 'orange' },
    { label: 'Xe máy', value: CardGroupVehicleType.MOTORBIKE, color: 'cyan' },
    { label: 'Xe đạp', value: CardGroupVehicleType.BICYCLE, color: 'green' }
  ];

  // Formatter/Parser methods
  formatMinutes = (value: number): string => `${value} Phút`;
  parseMinutes = (value: string): number => value ? parseInt(value.replace(' Phút', ''), 10) : 0;

  formatPrice = (value: number): string => {
    if (value === null || value === undefined) return '';
    
    const fixedValue = value.toFixed(2);
    const parts = fixedValue.split('.');
    const formattedIntegerPart = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    const displayDecimal = (parts[1] === '00') ? '' : `.${parts[1]}`;
    
    return `${formattedIntegerPart}${displayDecimal}`;
  }
  
  parsePrice = (value: string): number => value ? parseFloat(value.replace(/,/g, '')) || 0 : 0;

  constructor(
    private cardGroupService: CardGroupService,
    private laneService: LaneService,
    private cdr: ChangeDetectorRef,
    private modalService: NzModalService,
    private fb: FormBuilder,
    private notification: NzNotificationService,
    public loginService: LoginService
  ) {
    this.initForm();
  }

  ngOnInit() {
    this.loadCardGroups();
    this.loadLanes();
  }

  // Helper methods
  getCardGroupType(value: string) {
    return this.cardGroupTypes.find(opt => opt.value === value);
  }

  getCardGroupVehicleType(value: string) {
    return this.cardGroupVehicleTypes.find(opt => opt.value === value);
  }

  // Form initialization
  initForm() {
    const defaultFormConfig = {
      code: [null, [Validators.required]],
      name: [null, [Validators.required]],
      type: [CardGroupType.MONTH, [Validators.required]],
      vehicleType: [CardGroupVehicleType.CAR, [Validators.required]],
      status: true,
      laneIds: [[]],
      freeMinutes: 0,
      firstBlockMinutes: 60,
      firstBlockPrice: 0.00,
      nextBlockMinutes: [null],
      nextBlockPrice: [null]
    };
    
    this.cardGroupForm = this.fb.group(defaultFormConfig);
    this.editCardGroupForm = this.fb.group(defaultFormConfig);
  }

  // Data loading methods
  loadCardGroups(searchKeyword: string = '') {
    this.loading = true;
  
    this.cardGroupService.getCardGroups().subscribe({
      next: (data: any[]) => {
        data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        
        const filteredCardGroups = searchKeyword
          ? data.filter(cardGroup =>
              cardGroup.name.toLowerCase().includes(searchKeyword.toLowerCase()) ||
              cardGroup.code.toLowerCase().includes(searchKeyword.toLowerCase())
            )
          : data;
  
        this.total = filteredCardGroups.length;
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.cardGroups = filteredCardGroups.slice(start, end);
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Lỗi khi lấy danh sách nhóm thẻ:', error);
        this.loading = false;
      }
    });
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

  // Event handlers
  onSearch() {
    this.loadCardGroups(this.searchKeyword);
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadCardGroups(this.searchKeyword);
  }

  transferChange(ret: TransferChange): void {
    // Update the direction of changed items in transferData
    ret.list.forEach(item => {
      const index = this.transferData.findIndex(data => data['key'] === item['key']);
      if (index !== -1) {
        this.transferData[index].direction = item.direction;
      }
    });
    
    // Get all right side items after the update
    const rightItems = this.transferData.filter(item => item.direction === 'right');
    const selectedLaneIds = rightItems.map(item => item['key']);
    
    // Update the current form
    const form = this.isAddModalVisible ? this.cardGroupForm : this.editCardGroupForm;
    form.patchValue({ laneIds: selectedLaneIds });
    this.cardGroupLanes = selectedLaneIds;
  }

  // Modal handling methods
  showAddCardGroupModal() {
    this.cardGroupForm.reset({
      status: true,
      type: CardGroupType.MONTH,
      vehicleType: CardGroupVehicleType.CAR,
      laneIds: [],
      freeMinutes: 0,
      firstBlockMinutes: 60,
      firstBlockPrice: 0
    });
    
    this.cardGroupLanes = [];
    this.isNextBlockVisible = false;
    
    // Reset all transfer items to left side
    this.transferData = this.lanes.map(lane => ({
      key: lane.id,
      title: lane.name,
      direction: 'left',
      disabled: false
    }));
    
    this.isAddModalVisible = true;
  }

  showEditCardGroupModal(cardGroup: any) {
    this.currentCardGroupId = cardGroup.id;
    this.cardGroupLanes = cardGroup.laneIds || [];
  
    const formValues = {
      name: cardGroup.name,
      code: cardGroup.code,
      type: cardGroup.type,
      vehicleType: cardGroup.vehicleType,
      status: cardGroup.status,
      laneIds: this.cardGroupLanes,
      freeMinutes: cardGroup.freeMinutes ?? 0,
      firstBlockMinutes: cardGroup.firstBlockMinutes ?? 60,
      firstBlockPrice: cardGroup.firstBlockPrice ?? 0,
      nextBlockMinutes: cardGroup.nextBlockMinutes ?? null,
      nextBlockPrice: cardGroup.nextBlockPrice ?? null
    };
    
    this.editCardGroupForm.patchValue(formValues);
    
    // Check if "Next Block" section should be visible
    this.isNextBlockVisible = !!cardGroup.nextBlockMinutes || !!cardGroup.nextBlockPrice;
    
    // Check if fee config should be expanded
    this.isFeeConfigExpanded =
      !!cardGroup.freeMinutes ||
      !!cardGroup.firstBlockMinutes ||
      !!cardGroup.firstBlockPrice ||
      !!cardGroup.nextBlockMinutes ||
      !!cardGroup.nextBlockPrice;
  
    // Update transfer data
    this.transferData = this.lanes.map(lane => ({
      key: lane.id,
      title: lane.name,
      direction: this.cardGroupLanes.includes(lane.id) ? 'right' : 'left',
      disabled: false
    }));
  
    this.isEditModalVisible = true;
    this.cdr.detectChanges();
  }
  
  showNextBlock() {
    this.isNextBlockVisible = true;
  }

  hideNextBlock() {
    this.isNextBlockVisible = false;
    
    // Clear next block fields
    const form = this.isEditModalVisible ? this.editCardGroupForm : this.cardGroupForm;
    form.patchValue({
      nextBlockMinutes: null,
      nextBlockPrice: null
    });
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

  // Form submission handlers
  handleOk() {
    if (this.cardGroupForm.invalid) {
      this.notification.warning('', 'Vui lòng nhập đủ thông tin', {nzDuration: 3000});
      return;
    }

    const newCardGroup = this.cardGroupForm.value;
    newCardGroup.laneIds = newCardGroup.laneIds || [];

    if (this.cardGroups.some(cardGroup => cardGroup.code === newCardGroup.code)) {
      this.notification.error('Lỗi', 'Tên: Trường bị trùng lặp', {nzDuration: 3000});
      return;
    }

    this.cardGroupService.addCardGroup(newCardGroup).subscribe({
      next: () => {
        this.loadCardGroups();
        this.isAddModalVisible = false;
        this.notification.success('Thành công', '', {
          nzPlacement: 'topRight',
          nzDuration: 3000
        });
      },
      error: (error) => {
        console.error('Lỗi khi thêm nhóm thẻ:', error);
        this.notification.error('Lỗi', '', {
          nzPlacement: 'topRight',
          nzDuration: 3000
        });
      }
    });
  }

  handleEditOk() {
    if (this.editCardGroupForm.invalid) {
      this.notification.warning('', 'Vui lòng nhập đủ thông tin', {nzDuration: 3000});
      return;
    }
    
    const updatedCardGroup = {...this.editCardGroupForm.value};
    updatedCardGroup.laneIds = updatedCardGroup.laneIds || [];

    if (!this.isNextBlockVisible) {
      updatedCardGroup.nextBlockMinutes = null;
      updatedCardGroup.nextBlockPrice = null;
    }

    const isDuplicate = this.cardGroups.some(cardGroup =>
      cardGroup.code === updatedCardGroup.code && cardGroup.id !== this.currentCardGroupId
    );

    if (isDuplicate) {
      this.notification.error('Lỗi', 'Tên: Trường bị trùng lặp', {nzDuration: 3000});
      return;
    }

    if (this.currentCardGroupId) {
      this.cardGroupService.updateCardGroup(this.currentCardGroupId, updatedCardGroup).subscribe({
        next: () => {
          this.loadCardGroups();
          this.isEditModalVisible = false;
          this.currentCardGroupId = null;
          this.notification.success('Thành công', '', {nzDuration: 3000});
        },
        error: (error) => {
          console.error('Lỗi khi cập nhật', error);
          const errorMessage = error.error?.message || 'Không thể cập nhật nhóm thẻ. Vui lòng thử lại';
          this.notification.error('Lỗi', errorMessage, {nzDuration: 3000});
        }
      });
    }
  }

  // Actions on card groups
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
      nzTitle: 'Bạn có chắc chắn muốn xóa?',
      nzMaskClosable: true,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.cardGroupService.deleteCardGroup(id).subscribe({
          next: () => {
            this.loadCardGroups();
            this.notification.success('Thành công', '', {
              nzPlacement: 'topRight',
              nzDuration: 3000
            });
          },
          error: (error) => {
            console.error('Lỗi khi xóa nhóm thẻ:', error);
            this.notification.error('Lỗi', '', {
              nzPlacement: 'topRight',
              nzDuration: 3000
            });
          }
        });
      }
    });
  }

  toggleCardGroupStatus(cardGroupId: number) {
    const cardGroup = this.cardGroups.find(c => c.id === cardGroupId);
    if (!cardGroup) {
      console.error(`Không tìm thấy nhóm thẻ với id ${cardGroupId}`);
      return;
    }

    this.modalService.confirm({
      nzTitle: 'Xác nhận khóa nhóm thẻ',
      nzMaskClosable: true,
      nzOkText: 'Xác nhận',
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        const updatedCardGroup = {
          ...cardGroup,
          status: !cardGroup.status,
          laneIds: cardGroup.laneIds || []
        };

        this.cardGroupService.updateCardGroup(cardGroupId, updatedCardGroup).subscribe({
          next: () => {
            cardGroup.status = !cardGroup.status;
            this.notification.success('Thành công', '', {nzDuration: 3000});
          },
          error: (error) => {
            console.error('Lỗi khi khóa nhóm thẻ:', error);
            this.notification.error('Lỗi', 'Đã có lỗi xảy ra', {nzDuration: 3000});
          }
        });
      }
    });
  }
}
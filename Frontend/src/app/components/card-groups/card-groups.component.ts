import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CardGroupService } from '../../cores/services/card-group.service';
import { LaneService } from '../../cores/services/lane.service';
import { LoginService } from '../../cores/services/login.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { TransferItem, TransferChange } from 'ng-zorro-antd/transfer';
import { CardGroupType } from '../../cores/enums/card-group-type';
import { CardGroupVehicleType } from '../../cores/enums/card-group-vehicle-type';

@Component({
  selector: 'app-card-groups',
  standalone: false,
  templateUrl: './card-groups.component.html',
  styleUrl: './card-groups.component.scss'
})
export class CardGroupsComponent implements OnInit {
  cardGroups: any[] = [];
  lanes: any[] = [];
  transferData: TransferItem[] = [];
  cardGroupLanes: number[] = [];
  
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
  cardGroupForm!: FormGroup;
  editCardGroupForm!: FormGroup;
  
  cardGroupTypes = [
    { label: 'Tháng', value: CardGroupType.MONTH},
    { label: 'Ngày', value: CardGroupType.DAY}
  ];

  cardGroupVehicleTypes = [
    { label: 'Ô tô', value: CardGroupVehicleType.CAR, color: 'orange' },
    { label: 'Xe máy', value: CardGroupVehicleType.MOTORBIKE, color: 'cyan' },
    { label: 'Xe đạp', value: CardGroupVehicleType.BICYCLE, color: 'green' }
  ];

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

  getCardGroupType(value: string) {
    return this.cardGroupTypes.find(opt => opt.value === value);
  }

  getCardGroupVehicleType(value: string) {
    return this.cardGroupVehicleTypes.find(opt => opt.value === value);
  }

  initForm() {
    const defaultFormConfig = {
      code: [null, [Validators.required]],
      name: [null, [Validators.required]],
      type: [null, [Validators.required]],
      vehicleType: [null, [Validators.required]],
      status: true,
      laneIds: [[]],
      freeMinutes: [null],
      firstBlockMinutes: [null],
      firstBlockPrice: [null],
      nextBlockMinutes: [null],
      nextBlockPrice: [null]
    };
    
    this.cardGroupForm = this.fb.group(defaultFormConfig);
    this.editCardGroupForm = this.fb.group(defaultFormConfig);
  }

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
        this.notification.error('Lỗi', 'Không thể tải dữ liệu nhóm thẻ');
        this.loading = false;
      }
    });
  }
  
  loadLanes() {
    this.laneService.getLanes().subscribe(data => {
      this.lanes = data;
    });
  }

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
    // update vị trí lane thay đổi 
    ret.list.forEach(changedItem => {
      const existingItem = this.transferData.find(item => item['key'] === changedItem['key']);
      if (existingItem) {
        existingItem.direction = changedItem['direction'];
      }
    });
    
    // lấy tất cả lane đã chọn
    const selectedLaneIds = this.transferData
      .filter(item => item.direction === 'right')
      .map(item => item['key'] as number);
    
    // lưu vào form
    const currentForm = this.isAddModalVisible ? this.cardGroupForm : this.editCardGroupForm;
    currentForm.patchValue({ laneIds: selectedLaneIds });
    
    this.cardGroupLanes = selectedLaneIds;
  }

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
    
    // hiển thị khoảng tiếp theo nếu có
    this.isNextBlockVisible = !!cardGroup.nextBlockMinutes || !!cardGroup.nextBlockPrice;
    
    this.transferData = this.lanes.map(lane => ({
      key: lane.id,
      title: lane.name,
      direction: this.cardGroupLanes.includes(lane.id) ? 'right' : 'left',
      disabled: false
    }));
  
    this.isEditModalVisible = true;
  }
  
  showNextBlock() {
    this.isNextBlockVisible = true;
  }

  hideNextBlock() {
    this.isNextBlockVisible = false;
    
    // xóa khoảng tiếp theo
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

  handleOk() {
    if (this.cardGroupForm.invalid) {
      this.notification.warning('', 'Vui lòng nhập đủ thông tin', {nzDuration: 3000});
      return;
    }

    const newCardGroup = this.cardGroupForm.value;
    newCardGroup.laneIds = newCardGroup.laneIds || [];

    if (this.cardGroups.some(cardGroup => cardGroup.code === newCardGroup.code)) {
      this.notification.error(
        'Lỗi', 
        'Tên bị trùng lặp', 
        {nzDuration: 3000}
      );
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
      this.notification.error(
        'Lỗi', 
        'Tên bị trùng lặp', 
        {nzDuration: 3000}
      );
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
          const errorMessage = error.error?.message || '';
          this.notification.error('Lỗi', errorMessage, {nzDuration: 3000});
        }
      });
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
            const message = error?.error?.message || 'Đã xảy ra lỗi khi cập nhật trạng thái thẻ.';
            this.notification.error('Lỗi', message, {
              nzPlacement: 'topRight',
              nzDuration: 3000
            });
          }
        });
      }
    });
  }

  toggleCardGroupStatus(cardGroupId: number): void {
    const cardGroup = this.cardGroups.find(c => c.id === cardGroupId);
    if (!cardGroup) {
      console.error(`Không tìm thấy nhóm thẻ với id ${cardGroupId}`);
      return;
    }

    const newStatus = !cardGroup.status;

    this.modalService.confirm({
      nzTitle: newStatus ? 'Xác nhận mở khóa nhóm thẻ' : 'Xác nhận khóa nhóm thẻ',
      nzMaskClosable: true,
      nzOkText: 'Xác nhận',
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.cardGroupService.changeCardGroupStatus(cardGroupId, newStatus).subscribe({
          next: () => {
            cardGroup.status = newStatus;
            this.notification.success('Thành công', '', { nzDuration: 3000 });
          },
          error: (error) => {
            console.error('Lỗi khi thay đổi trạng thái nhóm thẻ:', error);
            const message = error?.error?.message || 'Đã xảy ra lỗi khi cập nhật trạng thái thẻ.';
            this.notification.error('Lỗi', message, { nzDuration: 3000 });
          }
        });
      }
    });
  }
}
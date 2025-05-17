import { Component, OnInit, ChangeDetectorRef, HostListener} from '@angular/core';
import { EntryLogService } from '../../services/entry-log.service';
import { CardService } from '../../services/card.service';
import { CardGroupService } from '../../services/card-group.service';
import { LaneService } from '../../services/lane.service';
import { CustomerService } from '../../services/customer.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';

enum CardGroupVehicleType{
  CAR = 'Car',
  MOTORBIKE = 'Motorbike',
  BICYCLE = 'Bicycle'
}

@Component({
  selector: 'app-entry-logs',
  standalone: false,
  templateUrl: './entry-logs.component.html',
  styleUrl: './entry-logs.component.scss'
})
export class EntryLogsComponent implements OnInit{
  entryLogs: any[] = [];
  cards: any[] = [];
  cardGroups: any[] = [];
  lanes: any[] = [];
  customers: any[] = [];
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  isAddModalVisible = false; 
  isEditModalVisible = false; 
  entryLogForm!: FormGroup; 
  isVisible = false;
  currentEntryLogId: number | null = null;
  searchKeyword: string = '';

  totalVehicles: number = 0;
  totalCars: number = 0;
  totalMotorbikes: number = 0;
  totalBicycles: number = 0;

  vehicleTypes = [    
    { label: 'Ô tô', value: CardGroupVehicleType.CAR, color: '#d46b08' },
    { label: 'Xe máy', value: CardGroupVehicleType.MOTORBIKE, color: "#08979C" },
    { label: 'Xe đạp', value: CardGroupVehicleType.BICYCLE, color: '#389e0d' }
  ]

  constructor(
      private entryLogService: EntryLogService, 
      private cardService: CardService, 
      private cardGroupService: CardGroupService,
      private laneService: LaneService,
      private customerService: CustomerService,
      private cdr: ChangeDetectorRef,
      private modalService: NzModalService,
      private fb: FormBuilder,
      private notification: NzNotificationService
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
        entryTime: [null, [Validators.required]],
        imageUrl: [null],
        note: [null],
      });
    }

    loadEntryLogs(searchKeyword: string = '') {
    this.loading = true;
  
    this.entryLogService.getEntryLogs().subscribe(
      (data: any[]) => {
        console.log(data);
        const filteredEntryLogs = searchKeyword
          ? data.filter(entryLog =>
              entryLog.plateNumber.toLowerCase().includes(searchKeyword.toLowerCase()))
          : data;
          
          console.log('Kết quả đã lọc:', filteredEntryLogs);
  
        this.total = filteredEntryLogs.length;
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.entryLogs = filteredEntryLogs.slice(start, end); 
        
        this.totalVehicles = filteredEntryLogs.length;
        this.totalCars = this.countVehiclesByType(CardGroupVehicleType.CAR, filteredEntryLogs);
        this.totalMotorbikes = this.countVehiclesByType(CardGroupVehicleType.MOTORBIKE, filteredEntryLogs);
        this.totalBicycles = this.countVehiclesByType(CardGroupVehicleType.BICYCLE, filteredEntryLogs);

        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách xe vào bãi:', error);
        this.loading = false;
      }
    );
  }

  countVehiclesByType(vehicleType: CardGroupVehicleType, logs: any[]): number {
  return logs.filter(log => {
    const card = this.cards.find(c => c.id === log.cardId);
    const cardGroup = card ? this.cardGroups.find(cg => cg.id === card.cardGroupId) : null;
    return cardGroup && cardGroup.vehicleType === vehicleType;
  }).length;
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
  
  onSearch() {
    console.log(this.searchKeyword);
    this.loadEntryLogs(this.searchKeyword); 
  }
  
  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadEntryLogs(this.searchKeyword);
  }

  showAddEntryLogModal() {
    this.isAddModalVisible = true;
    this.entryLogForm.reset(); 
  }

  handleCancel() {
    this.isAddModalVisible = false;
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

    const newEntryLog = this.entryLogForm.value;

    this.entryLogService.addEntryLog(newEntryLog).subscribe(() => {
      this.loadEntryLogs();
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
      console.error('Lỗi khi thêm xe vào bãi:', error);
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

  deleteEntryLog(id: number) {
    this.modalService.confirm({
      nzTitle:'Bạn có chắc chắn muốn xóa?',
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
}

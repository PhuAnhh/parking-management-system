import { Component, OnInit, ChangeDetectorRef, HostListener} from '@angular/core';
import { ExitLogService } from '../../services/exit-log.service';
import { CardService } from '../../services/card.service';
import { CardGroupService } from '../../services/card-group.service';
import { LaneService } from '../../services/lane.service';
import { CustomerService } from '../../services/customer.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';

enum CardGroupVehicleType{
  CAR = 'Car',
  MOTORBIKE = 'Motorbike',
  BICYCLE = 'Bicycle'
}

interface DashboardItem {
  value: number;
  title: string;
  icon: string;
  gradient: string;
}

@Component({
  selector: 'app-exit-logs',
  standalone: false,
  templateUrl: './exit-logs.component.html',
  styleUrl: './exit-logs.component.scss'
})

export class ExitLogsComponent implements OnInit{
  exitLogs: any[] = [];
  cards: any[] = [];
  cardGroups: any[] = [];
  lanes: any[] = [];
  customers: any[] = [];
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  isVisible = false;
  currentEntryLogId: number | null = null;
  searchKeyword: string = '';
  dashboardItems: DashboardItem[] = [];


  vehicleTypes = [    
    { label: 'Ô tô', value: CardGroupVehicleType.CAR, color: '#d46b08' },
    { label: 'Xe máy', value: CardGroupVehicleType.MOTORBIKE, color: "#08979C" },
    { label: 'Xe đạp', value: CardGroupVehicleType.BICYCLE, color: '#389e0d' }
  ]

  constructor(
      private exitLogService: ExitLogService, 
      private cardService: CardService, 
      private cardGroupService: CardGroupService,
      private laneService: LaneService,
      private customerService: CustomerService,
      private cdr: ChangeDetectorRef,
    ) {
      this.dashboardItems = [
        {
          value: 0,
          title: 'Tổng số sự kiện xe ra',
          icon: '../../../assets/images/logo/ic-list.svg',
          gradient: 'linear-gradient(to right, #0c68e9, transparent)'
        },
        {
          value: 0,
          title: 'Tổng doanh thu',
          icon: '../../../assets/images/logo/ic-actual-revenue.svg',
          gradient: 'linear-gradient(to right, #22c55e, transparent)'
        }
      ];
    }  
  
  ngOnInit() {
    this.cardService.getCards().subscribe(data => {
      this.cards = data;
      
      this.cardGroupService.getCardGroups().subscribe(data => {
        this.cardGroups = data;
        
        this.loadExitLogs();
      });
    });
    
    this.loadLanes();
    this.loadCustomers();
  }
  
  initForm() {
  }

  loadExitLogs(searchKeyword: string = '') {
    this.loading = true;

    this.exitLogService.getExitLogs().subscribe(
      (data: any[]) => {
        console.log(data);

        const keyword = searchKeyword.toLowerCase();

        const filteredExitLogs = searchKeyword
          ? data.filter(exitLog =>
              (exitLog.entryLog?.plateNumber?.toLowerCase().includes(keyword)) ||
              (exitLog.exitPlateNumber?.toLowerCase().includes(keyword))
            )
          : data;

        console.log('Kết quả đã lọc:', filteredExitLogs);

        // Cập nhật dashboard items với dữ liệu thực tế
        this.updateDashboardItems(data);

        this.total = filteredExitLogs.length;
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.exitLogs = filteredExitLogs.slice(start, end);

        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách xe đã ra khỏi bãi:', error);
        this.loading = false;
      }
    );
  }

  updateDashboardItems(exitLogs: any[]) {
    const totalExitEvents = exitLogs.length;
    
    const totalRevenue = exitLogs.reduce((sum, exitLog) => {
      return sum + (exitLog.totalPrice || 0);
    }, 0);

    this.dashboardItems[0].value = totalExitEvents;
    this.dashboardItems[1].value = totalRevenue;
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

  onSearch() {
    console.log(this.searchKeyword);
    this.loadExitLogs(this.searchKeyword); 
  }
  
  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadExitLogs(this.searchKeyword);
  }

  formatDuration(milliseconds: number): string {
    if (!milliseconds || milliseconds < 0) return '—';

    const totalSeconds = Math.floor(milliseconds / 1000);
    const days = Math.floor(totalSeconds / 86400);
    const hours = Math.floor((totalSeconds % 86400) / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const seconds = totalSeconds % 60;

    const parts = [];
    if (days) parts.push(`${days} ngày`);
    if (hours) parts.push(`${hours} giờ`);
    if (minutes) parts.push(`${minutes} phút`);
    if (seconds || parts.length === 0) parts.push(`${seconds} giây`);

    return parts.join(' ');
  }
}

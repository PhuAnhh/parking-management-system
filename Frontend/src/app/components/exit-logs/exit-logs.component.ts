import { Component, OnInit, ChangeDetectorRef} from '@angular/core';
import { ExitLogService } from '../../cores/services/exit-log.service';
import { CardService } from '../../cores/services/card.service';
import { CardGroupService } from '../../cores/services/card-group.service';
import { LaneService } from '../../cores/services/lane.service';
import { CustomerService } from '../../cores/services/customer.service';
import { LoginService } from '../../cores/services/login.service';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { CardGroupVehicleType } from '../../cores/enums/card-group-vehicle-type';
import { forkJoin } from 'rxjs';

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
  dashboardItems: DashboardItem[] = [];

  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  searchKeyword: string = '';
  selectedCardGroupId: number | null = null;
  selectedLaneId: number | null = null;
  selectedDateRange: Date[] | null = null;
  selectedExitLogDetail: any = null;

  isVisible = false;
  currentEntryLogId: number | null = null;

  isShowDetailModalVisible = false;

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
      private notification: NzNotificationService,
      private cdr: ChangeDetectorRef,
      public loginService: LoginService
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
    this.loadAllData();
  }

  loadAllData(): void {
    this.loading = true;
    
    // Gọi tất cả API đồng thời
    forkJoin({
      cards: this.cardService.getCards(),
      cardGroups: this.cardGroupService.getCardGroups(),
      lanes: this.laneService.getLanes(),
      customers: this.customerService.getCustomers()
    }).subscribe({
      next: (data) => {
        this.cards = data.cards;
        this.cardGroups = data.cardGroups;
        this.lanes = data.lanes;
        this.customers = data.customers;
        
        // Sau khi có tất cả dữ liệu, load exitLogs
        this.loadExitLogs();
      },
      error: (error) => {
        console.error('Lỗi khi tải dữ liệu:', error);
        this.notification.error('Lỗi', 'Không thể tải dữ liệu');
        this.loading = false;
      }
    });
  }

  loadExitLogs(searchKeyword: string = ''): void {
    this.loading = true;

    const serviceCall = (this.selectedDateRange?.length === 2)
      ? this.exitLogService.getExitLogsByDateRange(
          this.selectedDateRange[0],
          this.selectedDateRange[1]
        )
      : this.exitLogService.getExitLogs();

    serviceCall.subscribe({
      next: (data: any[]) => {
        const filtered = this.filterExitLogs(data, searchKeyword);

        filtered.sort((a, b) =>
          new Date(b.exitTime + 'Z').getTime() - new Date(a.exitTime + 'Z').getTime()
        );

        this.updateDashboardItems(filtered);

        this.total = filtered.length;
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.exitLogs = filtered.slice(start, end);

        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Lỗi khi lấy danh sách xe đã ra khỏi bãi:', err);
        this.notification.error('Lỗi', 'Không thể tải dữ liệu xe đã ra');
        this.loading = false;
      }
    });
  }

  private filterExitLogs(data: any[], keyword: string): any[] {
    let result = data;

    if (keyword) {
      result = result.filter(exit =>
        (exit.entryLog?.plateNumber?.toLowerCase().includes(keyword.toLowerCase())) ||
        (exit.exitPlateNumber?.toLowerCase().includes(keyword.toLowerCase()))
      );
    }

    if (this.selectedCardGroupId) {
      result = result.filter(exit => exit.cardGroupId === this.selectedCardGroupId);
    }

    if (this.selectedLaneId) {
      result = result.filter(exit =>
        exit.entryLaneId === this.selectedLaneId || exit.exitLaneId === this.selectedLaneId
      );
    }

    if (this.selectedDateRange && this.selectedDateRange.length === 2) {
      const startDate = new Date(this.selectedDateRange[0]);
      const endDate = new Date(this.selectedDateRange[1]);
      
      startDate.setHours(0, 0, 0, 0);
      endDate.setHours(23, 59, 59, 999);

      result = result.filter(exit => {
        if (!exit.exitTime) return false;
        
        const exitLocalTime = new Date(exit.exitTime + 'Z');
        
        return exitLocalTime >= startDate && exitLocalTime <= endDate;
      });
    }

    return result;
  }

  updateDashboardItems(exitLogs: any[]) {
    const totalExitEvents = exitLogs.length;
    
    const totalRevenue = exitLogs.reduce((sum, exitLog) => {
      return sum + (exitLog.totalPrice || 0);
    }, 0);

    this.dashboardItems[0].value = totalExitEvents;
    this.dashboardItems[1].value = totalRevenue;
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

  onSearch() {
    console.log(this.searchKeyword);
    this.loadExitLogs(this.searchKeyword); 
  }

  onCardGroupChange(): void {
    this.loadExitLogs();
  }

  onLaneChange(): void {
    this.loadExitLogs();
  }

  onDateRangeChange(): void {
    this.loadExitLogs();
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadExitLogs(this.searchKeyword);
  }

  showDetailModal(exitLogId: number): void {
    this.loading = true;
    
    this.exitLogService.getExitLogById(exitLogId).subscribe({
      next: (data) => {
        if (data.imageUrl) {
          data.imageUrl = `http://localhost:5000${data.imageUrl}`;
        }

        if (data.entryLog?.imageUrl && !data.entryLog.imageUrl.startsWith('http')) {
          data.entryLog.imageUrl = `http://localhost:5000${data.entryLog.imageUrl}`;
        }
        
        this.selectedExitLogDetail = data;
        this.isShowDetailModalVisible = true;
        this.loading = false;
      },
      error: (error) => {
        console.error('Lỗi khi lấy chi tiết sự kiện:', error);
        this.notification.error(
          'Lỗi',
          '',
          {
            nzPlacement: 'topRight',
            nzDuration: 3000
          }
        );
        this.loading = false;
      }
    });
  }

  handleDetailCancel(): void {
    this.isShowDetailModalVisible = false;
  }
}
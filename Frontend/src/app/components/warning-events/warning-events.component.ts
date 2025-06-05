import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { WarningEventService } from '../../services/warning-event.service';
import { LaneService } from '../../services/lane.service';
import { NzTableQueryParams } from 'ng-zorro-antd/table';

enum WarningType {
  CARDLOCKED = "CardLocked",
  CARDGROUPNOTALLOWEDINLANE = "CardGroupNotAllowedInLane",
  LICENSEPLATEMISMATCH = "LicensePlateMismatch",
  TICKETISSUED = "TicketIssued",
  DELETEDWHILEVEHICLEINPARKING = "DeletedWhileVehicleInParking"
}

@Component({
  selector: 'app-revenue-reports',
  standalone: false,
  templateUrl: './warning-events.component.html',
  styleUrl: './warning-events.component.scss'
})
export class WarningEventsComponent implements OnInit {

  selectedWarningType: number | null = null;
  selectedLaneId: number | null = null;
  selectedDateRange: Date[] | null = null;
  searchKeyword = '';
  warnings: any[] = [];
  lanes: any[] = [];
  loading = false;
  pageIndex = 1;
  pageSize = 10;
  total = 0;

  warningType = [
    { label: 'Thẻ bị khóa', value: WarningType.CARDLOCKED, color: 'red' },
    { label: 'Nhóm thẻ không được sử dụng làn', value: WarningType.CARDGROUPNOTALLOWEDINLANE, color: 'yellow' },
    { label: 'Biển số vào ra không khớp', value: WarningType.LICENSEPLATEMISMATCH, color: 'orange' },
    { label: 'Ghi vé', value: WarningType.TICKETISSUED, color: 'green' },
    { label: 'Xóa sự kiện xe đang trong bãi', value: WarningType.DELETEDWHILEVEHICLEINPARKING, color: 'purple' }
  ];

  constructor(
    private warningEventService: WarningEventService,
    private laneService: LaneService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadWarningEvents();
    this.loadLanes();
  }

  loadWarningEvents(searchKeyword: string = ''): void {
    this.loading = true;

    const serviceCall = (this.selectedDateRange?.length === 2)
      ? this.warningEventService.getWarningsByDateRange(
          this.selectedDateRange[0],
          this.selectedDateRange[1]
        )
      : this.warningEventService.getWarnings();

    serviceCall.subscribe({
      next: (data: any[]) => {
        const filtered = this.filterWarningEvents(data, searchKeyword);

        filtered.sort((a, b) =>
          new Date(b.createdAt + 'Z').getTime() - new Date(a.createdAt + 'Z').getTime()
        );

        this.total = filtered.length;
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.warnings = filtered.slice(start, end);

        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Lỗi khi lấy danh sách cảnh báo:', err);
        this.loading = false;
      }
    });
  }

  private filterWarningEvents(data: any[], keyword: string): any[] {
    let result = data;

    if (keyword) {
      result = result.filter(warning =>
        warning.plateNumber?.toLowerCase().includes(keyword.toLowerCase())
      );
    }

    if (this.selectedWarningType) {
      result = result.filter(warning =>
        warning.warningType === this.selectedWarningType
      );
    }

    if (this.selectedLaneId) {
      result = result.filter(warning =>
        warning.laneId === this.selectedLaneId
      );
    }

    if (this.selectedDateRange && this.selectedDateRange.length === 2) {
      const startDate = new Date(this.selectedDateRange[0]);
      const endDate = new Date(this.selectedDateRange[1]);
      
      startDate.setHours(0, 0, 0, 0);
      endDate.setHours(23, 59, 59, 999);

      result = result.filter(warning => {
        if (!warning.createdAt) return false;
        
        const warningLocalTime = new Date(warning.createdAt + 'Z');
        
        return warningLocalTime >= startDate && warningLocalTime <= endDate;
      });
    }

    return result;
  }


  loadLanes() {
    this.laneService.getLanes().subscribe(data => {
      this.lanes = data;
    });
  }

  getFormattedTime(utcTimeString: string): Date | null {
    if (!utcTimeString) return null;
    return new Date(utcTimeString + 'Z');
  }

  getLaneNameById(laneId: number): string {
    const lane = this.lanes.find(l => l.id === laneId);
    return lane ? lane.name : '';
  }

  getWarningType(value: string) {
    return this.warningType.find(type => type.value === value);
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadWarningEvents(this.searchKeyword);
  }

  onSearch() {
    console.log(this.searchKeyword);
    this.loadWarningEvents(this.searchKeyword);
  }

  onWarningTypeChange(): void {
    this.loadWarningEvents();
  }

  onLaneChange(): void {
    this.loadWarningEvents();
  }

  onDateRangeChange(): void {
    this.loadWarningEvents();
  }
}

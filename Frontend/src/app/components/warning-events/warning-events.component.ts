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

  loadWarningEvents(searchKeyword: string = '') {
    this.loading = true;

    this.warningEventService.getWarnings().subscribe(
      (data: any[]) => {
        let filteredWarnings = data;

        if (searchKeyword) {
          filteredWarnings = filteredWarnings.filter(warning =>
            warning.plateNumber.toLowerCase().includes(searchKeyword.toLowerCase())
          );
        }

        if (this.selectedWarningType) {
          filteredWarnings = filteredWarnings.filter(warning =>
            warning.warningType === this.selectedWarningType
          );
        }

        if (this.selectedLaneId) {
          filteredWarnings = filteredWarnings.filter(warning =>
            warning.laneId === this.selectedLaneId
          );
        }

        this.total = filteredWarnings.length;
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.warnings = filteredWarnings.slice(start, end);
        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách cảnh báo:', error);
        this.loading = false;
      }
    );
  }

  loadLanes() {
    this.laneService.getLanes().subscribe(data => {
      this.lanes = data;
    });
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
}

import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { RevenueReportService } from '../../services/revenue-report';
import { CardGroupService } from '../../services/card-group.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-revenue-reports',
  standalone: false,
  templateUrl: './revenue-reports.component.html',
  styleUrl: './revenue-reports.component.scss'
})

export class RevenueReportsComponent implements OnInit {
  selectedCardGroupId: number | null = null;
  selectedDateRange: Date[] | null = null;

  revenueReports: any[] = [];
  cardGroups: any[] = [];

  loading = false;

  constructor(
    private revenueReportService: RevenueReportService,
    private cardGroupService: CardGroupService,
    private notification: NzNotificationService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadRevenueReports();
    this.loadCardGroups();
  }

  loadRevenueReports(): void {
    this.loading = true;

    let serviceCall;
    if (this.selectedDateRange && this.selectedDateRange.length === 2) {
      serviceCall = this.revenueReportService.getRevenueReportsByDateRange(
        this.selectedDateRange[0],
        this.selectedDateRange[1]
      );
    } else {
      serviceCall = this.revenueReportService.getRevenueReports();
    }

    serviceCall.subscribe({
      next: (data: any[]) => {
        let filteredReports = data;

        if (this.selectedCardGroupId !== null) {
          filteredReports = filteredReports.filter(report => report.cardGroupId === this.selectedCardGroupId);
        }

        this.revenueReports = filteredReports;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.notification.error('Lỗi', 'Không thể tải báo cáo doanh thu');
        this.loading = false;
      }
    });
  }

  onDateRangeChange(): void {
    this.loadRevenueReports();
  }

  onCardGroupChange(): void {
    this.loadRevenueReports();
  }

  loadCardGroups() {
    this.cardGroupService.getCardGroups().subscribe(data => {
      this.cardGroups = data;
    });
  }

  getCardGroupNameById(cardGroupId: number): string {
    const cardGroup = this.cardGroups.find(cg => cg.id === cardGroupId);
    return cardGroup ? cardGroup.name : '';
  }

  getTotalExitCount(): number {
    return this.revenueReports.reduce((sum, r) => sum + r.exitCount, 0);
  }

  getTotalRevenue(): number {
    return this.revenueReports.reduce((sum, r) => sum + r.revenue, 0);
  }
}
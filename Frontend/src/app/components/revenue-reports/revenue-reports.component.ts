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

    const serviceCall = (this.selectedDateRange?.length === 2)
      ? this.revenueReportService.getRevenueReportsByDateRange(
          this.selectedDateRange[0],
          this.selectedDateRange[1]
        )
      : this.revenueReportService.getRevenueReports();

    serviceCall.subscribe({
      next: (data: any[]) => {
        const sortedData = data.sort((a, b) =>
          new Date(b.createdAt + 'Z').getTime() - new Date(a.createdAt + 'Z').getTime()
        );
        
        this.revenueReports = this.filterRevenueReports(sortedData);
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.notification.error('Lỗi', 'Không thể tải báo cáo doanh thu');
        this.loading = false;
      }
    });
  }

  private filterRevenueReports(data: any[]): any[] {
    let result = data;

    if (this.selectedCardGroupId !== null) {
      result = result.filter(report => report.cardGroupId === this.selectedCardGroupId);
    }

    if (this.selectedDateRange && this.selectedDateRange.length === 2) {
      const startDate = new Date(this.selectedDateRange[0]);
      const endDate = new Date(this.selectedDateRange[1]);
      
      startDate.setHours(0, 0, 0, 0);
      endDate.setHours(23, 59, 59, 999);

      result = result.filter(report => {
        if (!report.createdAt) return false;
        
        const reportLocalTime = new Date(report.createdAt + 'Z');
        
        return reportLocalTime >= startDate && reportLocalTime <= endDate;
      });
    }

    return result;
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
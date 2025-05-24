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
  revenueReports: any[] = [];
  cardGroups: any[] = [];
  loading = false;
  searchKeyword = '';

  defaultRange: [Date, Date] = [new Date(), new Date()];

  constructor(
    private revenueReportService: RevenueReportService,
    private cardGroupService: CardGroupService,
    private notification: NzNotificationService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const start = new Date();
    start.setHours(0, 0, 0, 0);

    const end = new Date();
    end.setHours(23, 59, 59, 999);

    this.defaultRange = [start, end];

    this.loadRevenueReports();
    this.loadCardGroups();
  }

  loadRevenueReports(): void {
  this.loading = true;

  this.revenueReportService.getRevenueReports().subscribe({
    next: (data: any[]) => {
      let filtered = data;

      if (this.selectedCardGroupId !== null) {
        filtered = filtered.filter(rr => rr.cardGroupId === this.selectedCardGroupId);
      }

      this.revenueReports = filtered;

      this.loading = false;
      this.cdr.detectChanges();
    },
    error: () => {
      this.notification.error('Lỗi', 'Không thể tải báo cáo doanh thu');
      this.loading = false;
    }
  });
}

  onCardGroupChange(): void {
  this.loadRevenueReports();
}

  loadCardGroups(): void {
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

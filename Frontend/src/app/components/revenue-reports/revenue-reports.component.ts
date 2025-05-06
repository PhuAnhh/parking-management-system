import { Component } from '@angular/core';

@Component({
  selector: 'app-revenue-reports',
  standalone: false,
  templateUrl: './revenue-reports.component.html',
  styleUrl: './revenue-reports.component.scss'
})
export class RevenueReportsComponent {
    defaultRange: [Date, Date] = [new Date(), new Date()];

    ngOnInit(): void {
      const start = new Date();
      start.setHours(0, 0, 0, 0); // 00:00:00
    
      const end = new Date();
      end.setHours(23, 59, 59, 999); // 23:59:59.999
    
      this.defaultRange = [start, end];
    }
}

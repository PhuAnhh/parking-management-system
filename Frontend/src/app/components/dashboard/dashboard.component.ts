import { Component } from '@angular/core';
import dayjs from 'dayjs';

interface DashboardItem {
  value: number;
  title: string;
  icon: string;
  gradient: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent{
  today = new Date();
  dashboardItems: DashboardItem[] = [];

  constructor() {
    this.dashboardItems = [
      {
        value: 0,
        title: 'Tổng số xe đang gửi',
        icon: '../../../assets/images/logo/ic-list.svg',
        gradient: 'linear-gradient(to right, #0c68e9, transparent)'
      },
      {
        value: 0,
        title: 'Ô tô',
        icon: '../../../assets/images/logo/ic-car.svg',
        gradient: 'linear-gradient(to right, #ffab00, transparent)'
      },
      {
        value: 0,
        title: 'Xe máy',
        icon: '../../../assets/images/logo/ic-motor.svg',
        gradient: 'linear-gradient(to right, #00b8d9, transparent)'
      },
      {
        value: 0,
        title: 'Xe đạp',
        icon: '../../../assets/images/logo/ic-bike.svg',
        gradient: 'linear-gradient(to right, #22c55e, transparent)'
      }
    ];
  }
}

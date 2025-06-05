import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { EntryLogService } from '../../services/entry-log.service';
import { ExitLogService } from '../../services/exit-log.service';
import { CardService } from '../../services/card.service';
import { CardGroupService } from '../../services/card-group.service';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexXAxis,
  ApexDataLabels,
  ApexTooltip,
  ApexStroke,
  ApexGrid,
  ApexYAxis,
  ApexLegend,
  ApexFill
} from 'ng-apexcharts';

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis;
  dataLabels: ApexDataLabels;
  grid: ApexGrid;
  stroke: ApexStroke;
  tooltip: ApexTooltip;
  colors: string[];
  legend: ApexLegend;
};

enum CardGroupVehicleType {
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

interface HourlyData {
  hour: string;
  entryCount: number;
  exitCount: number;
}

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  today = new Date();
  loading = true;
  chartLoading = true;
  
  dashboardItems: DashboardItem[] = [
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
  
  cards: any[] = [];
  cardGroups: any[] = [];
  
  chartOptions: Partial<ChartOptions> = {};
  totalEntry = 0;
  totalExit = 0;
  totalRevenue = 0;

  constructor(
    private entryLogService: EntryLogService,
    private exitLogService: ExitLogService,
    private cardService: CardService,
    private cardGroupService: CardGroupService,
    private cdr: ChangeDetectorRef
  ) {
    this.initializeChart();
  }

  ngOnInit() {
    this.loadDashboardData();
    this.loadChartData();
  }

  private initializeChart() {
    this.chartOptions = {
      // Dữ liệu biểu đồ - 2 series cho xe vào và xe ra
      series: [
        { name: 'Xe vào', data: Array(24).fill(0) },
        { name: 'Xe ra', data: Array(24).fill(0) }
      ],
      
      // Cấu hình biểu đồ
      chart: {
        height: 350,
        type: 'line',
        zoom: { enabled: false },
        dropShadow: {
          enabled: true,
          color: '#000',
          top: 7, 
          left: 0,
          blur: 2,
          opacity: 0.5
        },
        toolbar: {
          show: true,
          tools: {
            download: false,
            selection: false,
            zoom: true,
            zoomin: false,
            zoomout: false,
            pan: false,
            reset: true
          }
        }
      },
      
      // Styling và hiển thị
      dataLabels: { enabled: false },
      stroke: { curve: 'smooth', width: 4 },
      grid: { row: { colors: ['#f3f3f3', 'transparent'], opacity: 0.2 } },
      
      // Trục X: 24 giờ trong ngày
      xaxis: { categories: this.generateHourLabels() },
      yaxis: { min: 0 },
      
      // Màu sắc và legend
      colors: ['#52c41a', '#ff4d4f'], 
      legend: { position: 'bottom', horizontalAlign: 'center' },
      
      tooltip: {
        y: {
          formatter: function (val: number) {
            return val + ' xe';
          }
        }
      }
    };
  }

  /**
   * Tạo labels cho 24 giờ (00h, 01h, ..., 23h)
   */
  private generateHourLabels(): string[] {
    return Array.from({ length: 24 }, (_, i) => `${i.toString().padStart(2, ' ')}h`);
  }

  loadDashboardData() {
    this.loading = true;
    
    // Bước 1: Tải danh sách thẻ
    this.cardService.getCards().subscribe(cards => {
      this.cards = cards;
      
      // Bước 2: Tải danh sách nhóm thẻ (chứa thông tin loại xe)
      this.cardGroupService.getCardGroups().subscribe(cardGroups => {
        this.cardGroups = cardGroups;
        
        // Bước 3: Tải logs xe vào để tính thống kê
        this.entryLogService.getEntryLogs().subscribe({
          next: (entryLogs: any[]) => {
            this.updateDashboardStats(entryLogs);
            this.loading = false;
            this.cdr.detectChanges();
          },
          error: () => this.loading = false
        });
      });
    });
  }

  private updateDashboardStats(entryLogs: any[]) {
    this.dashboardItems[0].value = entryLogs.length;
    this.dashboardItems[1].value = this.countVehiclesByType(CardGroupVehicleType.CAR, entryLogs);
    this.dashboardItems[2].value = this.countVehiclesByType(CardGroupVehicleType.MOTORBIKE, entryLogs); 
    this.dashboardItems[3].value = this.countVehiclesByType(CardGroupVehicleType.BICYCLE, entryLogs);
  }

  /**
   * Tải dữ liệu biểu đồ theo ngày được chọn
   * Hiển thị số xe vào/ra theo từng giờ trong ngày
   */
  loadChartData() {
    this.chartLoading = true;
    const selectedDate = this.today;
    
    // Validate ngày được chọn
    if (!this.isValidDate(selectedDate)) {
      this.chartLoading = false;
      return;
    }
    
    // Tạo khoảng thời gian cho cả ngày (00:00:00 - 23:59:59)
    const { startOfDay, endOfDay } = this.getDateRange(selectedDate);
    
    // Tải dữ liệu xe vào
    this.entryLogService.getEntryLogsByDateRange(startOfDay, endOfDay).subscribe({
      next: (entryLogs: any[]) => {
        // Tải dữ liệu xe ra
        this.exitLogService.getExitLogsByDateRange(startOfDay, endOfDay).subscribe({
          next: (exitLogs: any[]) => {
            this.processChartData(entryLogs, exitLogs);
            this.chartLoading = false;
            this.cdr.detectChanges();
          },
          error: () => {
            this.chartLoading = false;
            this.cdr.detectChanges();
          }
        });
      },
      error: () => {
        this.chartLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  /**
   * Xử lý dữ liệu và cập nhật biểu đồ
   */
  private processChartData(entryLogs: any[], exitLogs: any[]) {
    // Đảm bảo dữ liệu là mảng hợp lệ
    const validEntryLogs = Array.isArray(entryLogs) ? entryLogs : [];
    const validExitLogs = Array.isArray(exitLogs) ? exitLogs : [];
    
    // Xử lý dữ liệu theo giờ
    const hourlyData = this.processHourlyData(validEntryLogs, validExitLogs);
    
    // Cập nhật biểu đồ và tổng kết
    this.updateChart(hourlyData);
    this.calculateTotals(validEntryLogs, validExitLogs);
  }

  onDateChange() {
    if (!this.isValidDate(this.today)) {
      this.today = new Date();
    }
    
    setTimeout(() => this.loadChartData(), 100);
  }

  /**
   * Xử lý dữ liệu theo từng giờ trong ngày
   * Đếm số xe vào/ra cho mỗi giờ từ 0-23h
   */
  private processHourlyData(entryLogs: any[], exitLogs: any[]): HourlyData[] {
    // Khởi tạo dữ liệu cho 24 giờ với giá trị 0
    const hourlyData: HourlyData[] = Array.from({ length: 24 }, (_, hour) => ({
      hour: `${hour.toString().padStart(2, '0')}:00`,
      entryCount: 0,
      exitCount: 0
    }));

    // Xử lý logs xe vào
    this.processEntryLogs(entryLogs, hourlyData);
    
    // Xử lý logs xe ra
    this.processExitLogs(exitLogs, hourlyData);

    return hourlyData;
  }

  /**
   * Xử lý entry logs và cập nhật hourlyData
   */
  private processEntryLogs(entryLogs: any[], hourlyData: HourlyData[]) {
    entryLogs.forEach(log => {
      if (!log?.entryTime) return;
      
      const entryTime = this.getFormattedTime(log.entryTime);
      if (!entryTime || isNaN(entryTime.getTime())) return;
      
      const hour = entryTime.getHours();
      if (hour >= 0 && hour < 24) {
        hourlyData[hour].entryCount++;
      }
    });
  }

  /**
   * Xử lý exit logs và cập nhật hourlyData
   */
  private processExitLogs(exitLogs: any[], hourlyData: HourlyData[]) {
    exitLogs.forEach(log => {
      const exitTimeField = log.exitTime || log.createdAt;
      if (!exitTimeField) return;
      
      const exitTime = this.getFormattedTime(exitTimeField);
      if (!exitTime || isNaN(exitTime.getTime())) return;
      
      const hour = exitTime.getHours();
      if (hour >= 0 && hour < 24) {
        hourlyData[hour].exitCount++;
      }
    });
  }

  /**
   * Cập nhật biểu đồ với dữ liệu mới
   */
  private updateChart(hourlyData: HourlyData[]) {
    if (!hourlyData || !Array.isArray(hourlyData) || hourlyData.length !== 24) {
      return;
    }

    // Tách dữ liệu thành 2 mảng cho 2 series
    const entryData = hourlyData.map(data => data.entryCount || 0);
    const exitData = hourlyData.map(data => data.exitCount || 0);

    // Cập nhật cấu hình biểu đồ với dữ liệu mới
    this.chartOptions = {
      ...this.chartOptions,
      series: [
        { name: 'Xe vào', data: [...entryData] },
        { name: 'Xe ra', data: [...exitData] }
      ]
    };
    
    setTimeout(() => this.cdr.detectChanges(), 100);
  }

  /**
   * Tính tổng số xe vào/ra
   */
  private calculateTotals(entryLogs: any[], exitLogs: any[]) {
    this.totalEntry = entryLogs.length;
    this.totalExit = exitLogs.length;
  }

  countVehiclesByType(vehicleType: CardGroupVehicleType, logs: any[]): number {
    return logs.filter(log => {
      // Tìm thẻ tương ứng với log
      const card = this.cards.find(c => c.id === log.cardId);
      
      // Tìm nhóm thẻ để biết loại xe
      const cardGroup = card ? this.cardGroups.find(cg => cg.id === card.cardGroupId) : null;
      
      // Kiểm tra loại xe khớp với loại cần đếm
      return cardGroup && cardGroup.vehicleType === vehicleType;
    }).length;
  }

  getFormattedTime(utcTimeString: string): Date | null {
    return utcTimeString ? new Date(utcTimeString + 'Z') : null;
  }

  /**
   * Kiểm tra tính hợp lệ của ngày
   */
  private isValidDate(date: Date): boolean {
    return date && date instanceof Date && !isNaN(date.getTime());
  }

  /**
   * Tạo khoảng thời gian cho cả ngày (từ 00:00:00 đến 23:59:59)
   */
  private getDateRange(date: Date) {
    const startOfDay = new Date(date);
    startOfDay.setHours(0, 0, 0, 0);
    
    const endOfDay = new Date(date);
    endOfDay.setHours(23, 59, 59, 999);
    
    return { startOfDay, endOfDay };
  }
}
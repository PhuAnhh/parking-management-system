import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { EntryLogService } from '../../services/entry-log.service';
import { CardService } from '../../services/card.service';
import { CardGroupService } from '../../services/card-group.service';

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

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  today = new Date();
  dashboardItems: DashboardItem[] = [];
  cards: any[] = [];
  cardGroups: any[] = [];
  loading = true;

  constructor(
    private entryLogService: EntryLogService,
    private cardService: CardService,
    private cardGroupService: CardGroupService,
    private cdr: ChangeDetectorRef
  ) {
    // Initialize the dashboard items structure
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

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;
    
    // Load cards and card groups first
    this.cardService.getCards().subscribe(cards => {
      this.cards = cards;
      
      this.cardGroupService.getCardGroups().subscribe(cardGroups => {
        this.cardGroups = cardGroups;
        
        // Now load entry logs
        this.entryLogService.getEntryLogs().subscribe(
          (entryLogs: any[]) => {
            // Calculate totals
            const totalVehicles = entryLogs.length;
            const totalCars = this.countVehiclesByType(CardGroupVehicleType.CAR, entryLogs);
            const totalMotorbikes = this.countVehiclesByType(CardGroupVehicleType.MOTORBIKE, entryLogs);
            const totalBicycles = this.countVehiclesByType(CardGroupVehicleType.BICYCLE, entryLogs);
            
            // Update dashboard items with the calculated values
            this.dashboardItems[0].value = totalVehicles;
            this.dashboardItems[1].value = totalCars;
            this.dashboardItems[2].value = totalMotorbikes;
            this.dashboardItems[3].value = totalBicycles;
            
            this.loading = false;
            this.cdr.detectChanges();
          },
          (error) => {
            console.error('Error loading entry logs:', error);
            this.loading = false;
          }
        );
      });
    });
  }

  countVehiclesByType(vehicleType: CardGroupVehicleType, logs: any[]): number {
    return logs.filter(log => {
      const card = this.cards.find(c => c.id === log.cardId);
      const cardGroup = card ? this.cardGroups.find(cg => cg.id === card.cardGroupId) : null;
      return cardGroup && cardGroup.vehicleType === vehicleType;
    }).length;
  }
}
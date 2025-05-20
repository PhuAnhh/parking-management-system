import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { GatesComponent } from './components/gates/gates.component';
import { ComputersComponent } from './components/computers/computers.component';
import { CamerasComponent } from './components/cameras/cameras.component';
import { ControlUnitsComponent } from './components/control-units/control-units.component';
import { LanesComponent } from './components/lanes/lanes.component';
import { LedsComponent } from './components/leds/leds.component';
import { CustomersComponent } from './components/customers/customers.component';
import { CustomerGroupsComponent } from './components/customer-groups/customer-groups.component';
import { CardGroupsComponent } from './components/card-groups/card-groups.component';
import { CardsComponent } from './components/cards/cards.component';
import { EntryLogsComponent } from './components/entry-logs/entry-logs.component';
import { ExitLogsComponent } from './components/exit-logs/exit-logs.component';
import { WarningEventsComponent } from './components/warning-events/warning-events.component';
import { RevenueReportsComponent } from './components/revenue-reports/revenue-reports.component';

const routes: Routes = [
  // {path: '', redirectTo: 'login', pathMatch: 'full'},
  {path: '', redirectTo: 'dashboard', pathMatch: 'full'},
  {path: 'dashboard', component: DashboardComponent},
  {path: 'gates', component: GatesComponent},
  {path: 'computers', component: ComputersComponent},
  {path: 'cameras', component: CamerasComponent},
  {path: 'control_units', component: ControlUnitsComponent},
  {path: 'lanes', component: LanesComponent},
  {path: 'leds', component: LedsComponent},
  {path: 'customers', component: CustomersComponent},
  {path: 'customer-groups', component: CustomerGroupsComponent},
  {path: 'card-groups', component: CardGroupsComponent},
  {path: 'cards', component: CardsComponent},
  {path: 'entry-logs', component: EntryLogsComponent},
  {path: 'exit-logs', component: ExitLogsComponent},
  {path: 'warning-events', component: WarningEventsComponent},
  {path: 'revenue-reports', component: RevenueReportsComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {useHash: true})], /*, {useHash: true} */
  exports: [RouterModule]
})
export class AppRoutingModule { }

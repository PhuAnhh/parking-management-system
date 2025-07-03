import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './cores/guards/auth.guard';

import { MainLayoutComponent } from './cores/main-layout/main-layout.component';

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
import { LoginComponent } from './components/login/login.component';
import { UsersComponent } from './components/users/users.component';
import { RolePermissionsComponent } from './components/role-permissions/role-permissions.component';

const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  { path: 'login', component: LoginComponent },

  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'dashboard', component: DashboardComponent, data: { title: 'Trang chủ' } },
      { path: 'gates', component: GatesComponent, data: { title: 'Cổng' } },
      { path: 'computers', component: ComputersComponent, data: { title: 'Máy tính' } },
      { path: 'cameras', component: CamerasComponent, data: { title: 'Camera' } },
      { path: 'control_units', component: ControlUnitsComponent, data: { title: 'Bộ điều khiển' } },
      { path: 'lanes', component: LanesComponent, data: { title: 'Làn' } },
      { path: 'leds', component: LedsComponent, data: { title: 'Led' } },
      { path: 'customers', component: CustomersComponent, data: { title: 'Khách hàng' } },
      { path: 'customer-groups', component: CustomerGroupsComponent, data: { title: 'Nhóm khách hàng' } },
      { path: 'card-groups', component: CardGroupsComponent, data: { title: 'Nhóm thẻ' } },
      { path: 'cards', component: CardsComponent, data: { title: 'Thẻ' } },
      { path: 'entry-logs', component: EntryLogsComponent, data: { title: 'Xe vào bãi' } },
      { path: 'exit-logs', component: ExitLogsComponent, data: { title: 'Xe đã ra' } },
      { path: 'warning-events', component: WarningEventsComponent, data: { title: 'Sự kiện cảnh báo' } },
      { path: 'revenue-reports', component: RevenueReportsComponent, data: { title: 'Báo cáo doanh thu' } },
      { path: 'users', component: UsersComponent, data: { title: 'Người dùng' } },
      { path: 'role-permissions', component: RolePermissionsComponent, data: { title: 'Vai trò, quyền hạn' } }
    ]
  },

  { path: '**', redirectTo: 'dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

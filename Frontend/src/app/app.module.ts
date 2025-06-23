import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { NgApexchartsModule } from 'ng-apexcharts';

import { AppRoutingModule } from './app-routing.module';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { provideNzI18n } from 'ng-zorro-antd/i18n';
import { NZ_I18N, vi_VN } from 'ng-zorro-antd/i18n';
import vi from '@angular/common/locales/vi';
import { registerLocaleData } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { HttpClient, HttpClientModule, provideHttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';

import { NzBreadCrumbModule } from 'ng-zorro-antd/breadcrumb';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzNotificationModule } from 'ng-zorro-antd/notification';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzTransferModule, TransferItem } from 'ng-zorro-antd/transfer';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzProgressModule } from 'ng-zorro-antd/progress';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzImageModule } from 'ng-zorro-antd/image';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzTypographyModule } from 'ng-zorro-antd/typography';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { NzRateModule } from 'ng-zorro-antd/rate';

import { MainLayoutComponent } from './cores/main-layout/main-layout.component';
import { LoginComponent } from './components/login/login.component';
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
import { UsersComponent } from './components/users/users.component';
import { RolePermissionsComponent } from './components/role-permissions/role-permissions.component';
import { NzConfig, provideNzConfig } from 'ng-zorro-antd/core/config';

import { AuthInterceptor } from './cores/interceptors/auth.interceptor';

const ngZorroConfig: NzConfig = {
  pagination: {
    nzPageSizeOptions: [10, 20, 30]
  }
};

registerLocaleData(vi);

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DashboardComponent,
    GatesComponent,
    ComputersComponent,
    CamerasComponent,
    ControlUnitsComponent,
    LanesComponent,
    LedsComponent,
    CustomersComponent,
    CustomerGroupsComponent,
    CardGroupsComponent,
    CardsComponent,
    EntryLogsComponent,
    ExitLogsComponent,
    WarningEventsComponent,
    RevenueReportsComponent,
    UsersComponent,
    RolePermissionsComponent,
    MainLayoutComponent
  ],
  imports: [
    NgApexchartsModule,
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
    NzBreadCrumbModule,
    NzIconModule,
    NzLayoutModule,
    NzMenuModule,
    NzGridModule,
    NzCardModule,
    NzDatePickerModule,
    NzPaginationModule,
    NzModalModule,
    NzInputModule,
    NzButtonModule,
    NzFormModule,
    NzCheckboxModule,
    NzTableModule,
    NzSwitchModule,
    NzTagModule,
    NzToolTipModule,
    NzSelectModule,
    NzBadgeModule,
    FormsModule,
    NzNotificationModule,
    NzRadioModule,
    NzTransferModule,
    NzCollapseModule,
    NzInputNumberModule,
    NzProgressModule,
    NzListModule,
    NzDropDownModule,
    NzImageModule,
    NzSpinModule,
    NzTypographyModule,
    NzEmptyModule,
    NzAvatarModule,
    RouterModule,
    NzDrawerModule,
    NzRateModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    provideClientHydration(withEventReplay()),
    provideNzI18n(vi_VN),
    provideAnimationsAsync(),
    provideHttpClient(),
    provideNzConfig(ngZorroConfig)
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
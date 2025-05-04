import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { SignupComponent } from './components/signup/signup.component';
import { provideNzI18n } from 'ng-zorro-antd/i18n';
import { en_US } from 'ng-zorro-antd/i18n';
import { registerLocaleData } from '@angular/common';
import en from '@angular/common/locales/en';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { HttpClient, HttpClientModule, provideHttpClient } from '@angular/common/http';
import { DashboardComponent } from './components/dashboard/dashboard.component';

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
import { NzToolTipModule } from 'ng-zorro-antd/tooltip'
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzNotificationModule  } from 'ng-zorro-antd/notification';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzTransferModule, TransferItem } from 'ng-zorro-antd/transfer';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';


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
import { NzConfig, provideNzConfig } from 'ng-zorro-antd/core/config';

const ngZorroConfig: NzConfig = {
  pagination:{
    nzPageSizeOptions: [10,20,30]
  } 
};

registerLocaleData(en);

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    SignupComponent,
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
    CardsComponent
  ],
  imports: [
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
    NzCollapseModule

  ],
  providers: [
    provideClientHydration(withEventReplay()),
    provideNzI18n(en_US),
    provideAnimationsAsync(),
    provideHttpClient(),
    provideNzConfig(ngZorroConfig)
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }

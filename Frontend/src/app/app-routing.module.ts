import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { SignupComponent } from './components/signup/signup.component';
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

const routes: Routes = [
  // {path: '', redirectTo: 'login', pathMatch: 'full'},
  {path: '', redirectTo: 'dashboard', pathMatch: 'full'},
  {path: 'login', component: LoginComponent},
  {path: 'signup', component: SignupComponent},
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
  {path: 'cards', component: CardsComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {useHash: true})], /*, {useHash: true} */
  exports: [RouterModule]
})
export class AppRoutingModule { }

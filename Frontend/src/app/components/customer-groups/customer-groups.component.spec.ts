import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerGroupsComponent } from './customer-groups.component';

describe('CustomerGroupsComponent', () => {
  let component: CustomerGroupsComponent;
  let fixture: ComponentFixture<CustomerGroupsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CustomerGroupsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CustomerGroupsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

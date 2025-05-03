import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ControlUnitsComponent } from './control-units.component';

describe('ControlUnitsComponent', () => {
  let component: ControlUnitsComponent;
  let fixture: ComponentFixture<ControlUnitsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ControlUnitsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ControlUnitsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

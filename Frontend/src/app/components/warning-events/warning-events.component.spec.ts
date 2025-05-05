import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WarningEventsComponent } from './warning-events.component';

describe('WarningEventsComponent', () => {
  let component: WarningEventsComponent;
  let fixture: ComponentFixture<WarningEventsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [WarningEventsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WarningEventsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

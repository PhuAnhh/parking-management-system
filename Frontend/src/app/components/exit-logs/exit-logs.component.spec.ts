import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExitLogsComponent } from './exit-logs.component';

describe('ExitLogsComponent', () => {
  let component: ExitLogsComponent;
  let fixture: ComponentFixture<ExitLogsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ExitLogsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExitLogsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

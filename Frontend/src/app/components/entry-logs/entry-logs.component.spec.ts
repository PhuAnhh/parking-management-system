import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EntryLogsComponent } from './entry-logs.component';

describe('EntryLogsComponent', () => {
  let component: EntryLogsComponent;
  let fixture: ComponentFixture<EntryLogsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EntryLogsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EntryLogsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GatesComponent } from './gates.component';

describe('GatesComponent', () => {
  let component: GatesComponent;
  let fixture: ComponentFixture<GatesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GatesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { TestBed } from '@angular/core/testing';

import { ControlUnitService } from './control-unit.service';

describe('ControlUnitService', () => {
  let service: ControlUnitService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ControlUnitService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

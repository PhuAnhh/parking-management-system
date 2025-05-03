import { TestBed } from '@angular/core/testing';

import { LaneControlUnitService } from './lane-control-unit.service';

describe('LaneControlUnitService', () => {
  let service: LaneControlUnitService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LaneControlUnitService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

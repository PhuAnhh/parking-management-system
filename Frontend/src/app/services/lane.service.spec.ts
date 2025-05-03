import { TestBed } from '@angular/core/testing';

import { LaneService } from './lane.service';

describe('LaneService', () => {
  let service: LaneService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LaneService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

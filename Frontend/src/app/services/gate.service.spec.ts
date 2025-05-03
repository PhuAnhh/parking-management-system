import { TestBed } from '@angular/core/testing';

import { GateService } from './gate.service';

describe('GateService', () => {
  let service: GateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

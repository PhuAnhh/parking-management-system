import { TestBed } from '@angular/core/testing';

import { CustomerGroupService } from './customer-group.service';

describe('CustomerGroupService', () => {
  let service: CustomerGroupService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CustomerGroupService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

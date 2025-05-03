import { TestBed } from '@angular/core/testing';

import { CardGroupService } from './card-group.service';

describe('CardGroupService', () => {
  let service: CardGroupService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CardGroupService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

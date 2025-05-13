import { TestBed } from '@angular/core/testing';

import { EntryLogService } from './entry-log.service';

describe('EntryLogService', () => {
  let service: EntryLogService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EntryLogService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

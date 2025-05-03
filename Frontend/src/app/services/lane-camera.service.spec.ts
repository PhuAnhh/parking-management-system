import { TestBed } from '@angular/core/testing';

import { LaneCameraService } from './lane-camera.service';

describe('LaneCameraService', () => {
  let service: LaneCameraService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LaneCameraService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

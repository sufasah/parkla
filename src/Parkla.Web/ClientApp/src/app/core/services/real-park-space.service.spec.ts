import { TestBed } from '@angular/core/testing';

import { RealParkSpaceService } from './real-park-space.service';

describe('RealParkSpaceService', () => {
  let service: RealParkSpaceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RealParkSpaceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

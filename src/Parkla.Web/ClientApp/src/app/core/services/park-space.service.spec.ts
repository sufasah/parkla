import { TestBed } from '@angular/core/testing';

import { ParkSpaceService } from './park-space.service';

describe('ParkSpaceService', () => {
  let service: ParkSpaceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ParkSpaceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

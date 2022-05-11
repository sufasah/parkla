import { TestBed } from '@angular/core/testing';

import { ParkAreaService } from './park-area.service';

describe('ParkAreaService', () => {
  let service: ParkAreaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ParkAreaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

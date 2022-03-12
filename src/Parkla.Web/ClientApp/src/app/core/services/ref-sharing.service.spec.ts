import { TestBed } from '@angular/core/testing';

import { RefSharingService } from './ref-sharing.service';

describe('RefSharingService', () => {
  let service: RefSharingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RefSharingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

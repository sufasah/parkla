import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MNewParkComponent } from './m-new-park.component';

describe('MNewParkComponent', () => {
  let component: MNewParkComponent;
  let fixture: ComponentFixture<MNewParkComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MNewParkComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MNewParkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

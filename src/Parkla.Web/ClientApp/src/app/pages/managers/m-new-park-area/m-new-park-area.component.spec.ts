import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MNewParkAreaComponent } from './m-new-park-area.component';

describe('MNewParkAreaComponent', () => {
  let component: MNewParkAreaComponent;
  let fixture: ComponentFixture<MNewParkAreaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MNewParkAreaComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MNewParkAreaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

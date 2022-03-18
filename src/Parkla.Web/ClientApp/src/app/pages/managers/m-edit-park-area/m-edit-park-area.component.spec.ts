import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MEditParkAreaComponent } from './m-edit-park-area.component';

describe('MEditParkAreaComponent', () => {
  let component: MEditParkAreaComponent;
  let fixture: ComponentFixture<MEditParkAreaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MEditParkAreaComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MEditParkAreaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

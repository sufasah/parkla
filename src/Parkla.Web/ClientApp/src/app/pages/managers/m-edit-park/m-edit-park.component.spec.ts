import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MEditParkComponent } from './m-edit-park.component';

describe('MEditParkComponent', () => {
  let component: MEditParkComponent;
  let fixture: ComponentFixture<MEditParkComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MEditParkComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MEditParkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

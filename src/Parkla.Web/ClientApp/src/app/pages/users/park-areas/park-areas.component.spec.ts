import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParkAreasComponent } from './park-areas.component';

describe('ParkAreasComponent', () => {
  let component: ParkAreasComponent;
  let fixture: ComponentFixture<ParkAreasComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ParkAreasComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ParkAreasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MParkMapComponent } from './m-park-map.component';

describe('MParkMapComponent', () => {
  let component: MParkMapComponent;
  let fixture: ComponentFixture<MParkMapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MParkMapComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MParkMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

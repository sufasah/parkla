import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParkAreaFormComponent } from './park-area-form.component';

describe('ParkAreaFormComponent', () => {
  let component: ParkAreaFormComponent;
  let fixture: ComponentFixture<ParkAreaFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ParkAreaFormComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ParkAreaFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

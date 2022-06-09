import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParklaDialogComponent } from './parkla-dialog.component';

describe('ParklaDialogComponent', () => {
  let component: ParklaDialogComponent;
  let fixture: ComponentFixture<ParklaDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ParklaDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ParklaDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

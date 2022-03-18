import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MParkAreaQRComponent } from './m-park-area-qr.component';

describe('MParkAreaQRComponent', () => {
  let component: MParkAreaQRComponent;
  let fixture: ComponentFixture<MParkAreaQRComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MParkAreaQRComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MParkAreaQRComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

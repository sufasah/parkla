import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MParkAreaComponent } from './m-park-area.component';

describe('MParkAreaComponent', () => {
  let component: MParkAreaComponent;
  let fixture: ComponentFixture<MParkAreaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MParkAreaComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MParkAreaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MParkAreasComponent } from './m-park-areas.component';

describe('MParkAreasComponent', () => {
  let component: MParkAreasComponent;
  let fixture: ComponentFixture<MParkAreasComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MParkAreasComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MParkAreasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

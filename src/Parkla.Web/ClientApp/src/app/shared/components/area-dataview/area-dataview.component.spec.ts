import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AreaDataViewComponent } from './area-dataview.component';

describe('ParkAreasComponent', () => {
  let component: AreaDataViewComponent;
  let fixture: ComponentFixture<AreaDataViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AreaDataViewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AreaDataViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

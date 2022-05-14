import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MEditAreaTemplateComponent } from './m-edit-area-template.component';

describe('MEditAreaTemplateComponent', () => {
  let component: MEditAreaTemplateComponent;
  let fixture: ComponentFixture<MEditAreaTemplateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MEditAreaTemplateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MEditAreaTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

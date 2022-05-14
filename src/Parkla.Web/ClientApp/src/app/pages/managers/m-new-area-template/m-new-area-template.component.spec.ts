import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MNewAreaTemplateComponent } from './m-new-area-template.component';

describe('MNewAreaTemplateComponent', () => {
  let component: MNewAreaTemplateComponent;
  let fixture: ComponentFixture<MNewAreaTemplateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MNewAreaTemplateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MNewAreaTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

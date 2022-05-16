import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AreaTemplateFormComponent } from './area-template-form.component';

describe('AreaTemplateFormComponent', () => {
  let component: AreaTemplateFormComponent;
  let fixture: ComponentFixture<AreaTemplateFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AreaTemplateFormComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AreaTemplateFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

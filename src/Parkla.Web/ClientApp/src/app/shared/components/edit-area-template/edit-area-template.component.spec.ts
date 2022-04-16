import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditAreaTemplateComponent } from './edit-area-template.component';

describe('EditAreaTemplateComponent', () => {
  let component: EditAreaTemplateComponent;
  let fixture: ComponentFixture<EditAreaTemplateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditAreaTemplateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditAreaTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

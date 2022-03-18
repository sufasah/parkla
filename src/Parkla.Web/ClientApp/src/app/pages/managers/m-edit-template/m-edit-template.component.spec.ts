import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MEditTemplateComponent } from './m-edit-template.component';

describe('MEditTemplateComponent', () => {
  let component: MEditTemplateComponent;
  let fixture: ComponentFixture<MEditTemplateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MEditTemplateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MEditTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

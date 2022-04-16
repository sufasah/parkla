import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParkTemplateComponent } from './area-template.component';

describe('ParkTemplateComponent', () => {
  let component: ParkTemplateComponent;
  let fixture: ComponentFixture<ParkTemplateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ParkTemplateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ParkTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

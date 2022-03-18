import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoadMoneyComponent } from './load-money.component';

describe('LoadMoneyComponent', () => {
  let component: LoadMoneyComponent;
  let fixture: ComponentFixture<LoadMoneyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LoadMoneyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoadMoneyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

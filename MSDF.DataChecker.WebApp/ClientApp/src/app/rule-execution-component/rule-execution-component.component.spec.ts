import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RuleExecutionComponentComponent } from './rule-execution-component.component';

describe('RuleExecutionComponentComponent', () => {
  let component: RuleExecutionComponentComponent;
  let fixture: ComponentFixture<RuleExecutionComponentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RuleExecutionComponentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RuleExecutionComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

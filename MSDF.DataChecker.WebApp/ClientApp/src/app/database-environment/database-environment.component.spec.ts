import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DatabaseEnvironmentComponent } from './database-environment.component';

describe('DatabaseEnvironmentComponent', () => {
  let component: DatabaseEnvironmentComponent;
  let fixture: ComponentFixture<DatabaseEnvironmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DatabaseEnvironmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DatabaseEnvironmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

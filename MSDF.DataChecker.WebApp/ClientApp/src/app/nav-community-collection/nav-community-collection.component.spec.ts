import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NavCommunityCollectionComponent } from './nav-community-collection.component';

describe('NavCommunityCollectionComponent', () => {
  let component: NavCommunityCollectionComponent;
  let fixture: ComponentFixture<NavCommunityCollectionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NavCommunityCollectionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NavCommunityCollectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

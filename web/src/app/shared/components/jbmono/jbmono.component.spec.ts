import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { JbmonoComponent } from './jbmono.component';

describe('JbmonoComponent', () => {
  let component: JbmonoComponent;
  let fixture: ComponentFixture<JbmonoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ JbmonoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JbmonoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

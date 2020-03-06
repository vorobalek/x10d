import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { JBMonoComponent } from './jbmono.component';

describe('JBMonoComponent', () => {
  let component: JBMonoComponent;
  let fixture: ComponentFixture<JBMonoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ JBMonoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JBMonoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

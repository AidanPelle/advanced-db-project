import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReadMatrixComponent } from './read-matrix.component';

describe('ReadMatrixComponent', () => {
  let component: ReadMatrixComponent;
  let fixture: ComponentFixture<ReadMatrixComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReadMatrixComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ReadMatrixComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

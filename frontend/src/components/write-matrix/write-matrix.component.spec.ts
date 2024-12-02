import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WriteMatrixComponent } from './write-matrix.component';

describe('WriteMatrixComponent', () => {
  let component: WriteMatrixComponent;
  let fixture: ComponentFixture<WriteMatrixComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WriteMatrixComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WriteMatrixComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceUsageLineChartComponent } from './device-usage-line-chart.component';

describe('DeviceUsageLineChartComponent', () => {
  let component: DeviceUsageLineChartComponent;
  let fixture: ComponentFixture<DeviceUsageLineChartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeviceUsageLineChartComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DeviceUsageLineChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

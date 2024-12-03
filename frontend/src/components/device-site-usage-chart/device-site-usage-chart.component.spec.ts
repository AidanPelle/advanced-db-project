import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceSiteUsageChartComponent } from './device-site-usage-chart.component';

describe('DeviceSiteUsageChartComponent', () => {
  let component: DeviceSiteUsageChartComponent;
  let fixture: ComponentFixture<DeviceSiteUsageChartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeviceSiteUsageChartComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DeviceSiteUsageChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

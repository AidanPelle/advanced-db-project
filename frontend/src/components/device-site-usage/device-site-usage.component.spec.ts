import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceSiteUsageComponent } from './device-site-usage.component';

describe('DeviceSiteUsageComponent', () => {
  let component: DeviceSiteUsageComponent;
  let fixture: ComponentFixture<DeviceSiteUsageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeviceSiteUsageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DeviceSiteUsageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

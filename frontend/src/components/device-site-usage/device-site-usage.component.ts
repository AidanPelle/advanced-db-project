import {Component, inject} from '@angular/core';
import {ApiService} from "../../services/api.service";
import {DeviceSiteUsageDto} from "../../models/DeviceSiteUsageDto";
import {DeviceSiteUsageChartComponent} from "../device-site-usage-chart/device-site-usage-chart.component";

@Component({
  selector: 'app-device-site-usage',
  standalone: true,
  imports: [
    DeviceSiteUsageChartComponent
  ],
  templateUrl: './device-site-usage.component.html',
  styleUrl: './device-site-usage.component.scss'
})
export class DeviceSiteUsageComponent {
  apiService = inject(ApiService);
  devices: DeviceSiteUsageDto[] = [];

  ngOnInit(): void {
    this.apiService.getDeviceSiteUsages(30).subscribe(res => {
      this.devices = res;
    });
  }
}

import {Component, inject, Input, SimpleChanges} from '@angular/core';
import Chart, {ChartData} from 'chart.js/auto';
import {ChartComponent} from "chart.js/auto";
import {DeviceSiteUsageDto} from "../../models/DeviceSiteUsageDto";
import {ApiService} from "../../services/api.service";

@Component({
  selector: 'app-device-site-usage-chart',
  standalone: true,
  imports: [],
  templateUrl: './device-site-usage-chart.component.html',
  styleUrl: './device-site-usage-chart.component.scss'
})
export class DeviceSiteUsageChartComponent {
  @Input() name: string = "my-chart";
  @Input() deviceUsage: DeviceSiteUsageDto = {deviceName: "", siteUsage: []};
  public chart: any = [];

  ngAfterViewInit() {
    this.updateChart();
  }
  ngOnChanges(changes: SimpleChanges) {
    this.updateChart();
  }
  updateChart() {
    // Prepare data for Chart.js
    const labels = this.deviceUsage.siteUsage.map(usage => usage.siteName);
    const readUsages = this.deviceUsage.siteUsage.map(usage => usage.readUsage);
    const writeUsages = this.deviceUsage.siteUsage.map(usage => usage.writeUsage);
    const totalUsages = this.deviceUsage.siteUsage.map(usage => usage.readUsage + usage.writeUsage);

    console.log(readUsages);
    const data = {
      labels: labels,
      datasets: [
        {
          label: 'Read Usage',
          data: readUsages,
          backgroundColor: 'rgba(54, 162, 235, 0.6)',
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 1
        },
        {
          label: 'Write Usage',
          data: writeUsages,
          backgroundColor: 'rgba(255, 99, 132, 0.6)',
          borderColor: 'rgba(255, 99, 132, 1)',
          borderWidth: 1
        },
        {
          label: 'Total Usage',
          data: totalUsages,
          backgroundColor: 'rgba(75, 192, 192, 0.6)',
          borderColor: 'rgba(75, 192, 192, 1)',
          borderWidth: 1
        }
      ]
    };

    this.chart = new Chart(this.name, {
      type: "bar",
      data: data
    })
  }
}

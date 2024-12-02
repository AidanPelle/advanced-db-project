import { Component, inject, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { ActivatedRoute } from '@angular/router';
import { DeviceDto } from '../../models/DeviceDto';

@Component({
  selector: 'app-device-detail',
  templateUrl: './device-detail.component.html',
  styleUrl: './device-detail.component.scss'
})
export class DeviceDetailComponent implements OnInit {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);

  private deviceId: string = "";
  protected device?: DeviceDto;

  ngOnInit(): void {
    this.deviceId = this.route.snapshot.paramMap.get('id')!;
    this.api.getDeviceDetail(this.deviceId).subscribe(result => {
      this.device = result;
    });
  }
}

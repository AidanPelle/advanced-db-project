import { Component, inject, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { DeviceDto } from '../../models/DeviceDto';
import { Router } from '@angular/router';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrl: './device-list.component.scss',
})
export class DeviceListComponent implements OnInit {
  
  private apiService = inject(ApiService);
  private router = inject(Router);
  protected deviceList: DeviceDto[] = [];

  constructor() {
    console.log('plz');
  }

  ngOnInit(): void {
    this.getDeviceList();
  }

  getDeviceList() {
    console.log('device list called');
    this.apiService.getDeviceList().subscribe(result => {
      this.deviceList = result;
    });
  }

  navToDevice(id: number) {
    this.router.navigate(['devices/' + id]);
  }
}

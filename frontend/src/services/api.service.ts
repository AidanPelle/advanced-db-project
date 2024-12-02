import { inject, Injectable } from '@angular/core';
import { DeviceDto } from '../models/DeviceDto';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(
    private http: HttpClient,
  ) { }

  getDeviceList(): Observable<DeviceDto[]> {
    const url = environment.apiUrl + "/devices/all"
    const result = this.http.get<DeviceDto[]>(url);
    return result;
  }

  getDeviceDetail(id: string): Observable<DeviceDto> {
    const url = environment.apiUrl + "/devices/" + id;
    const result = this.http.get<DeviceDto>(url);
    return result;
  }
}

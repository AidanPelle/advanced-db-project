import {Injectable} from '@angular/core';
import {DeviceDto} from '../models/DeviceDto';
import {Observable} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {environment} from '../environment/environment';
import {WriteMatrixDto} from '../models/WriteMatrix';

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

  getWriteFrequencyMatrix(): Observable<WriteMatrixDto[]> {
    const url = environment.apiUrl + "/sites/write-matrix"
    const result = this.http.get<WriteMatrixDto[]>(url);
    return result;
  }

  getReadFrequencyMatrix(): Observable<WriteMatrixDto[]> {
    const url = environment.apiUrl + "/sites/read-matrix"
    return this.http.get<WriteMatrixDto[]>(url);
  }

  getDeviceDetail(id: string): Observable<DeviceDto> {
    const url = environment.apiUrl + "/devices/" + id;
    const result = this.http.get<DeviceDto>(url);
    return result;
  }

  setWriteFrequency(value: number, siteId: number, deviceId: string): Observable<void> {
    const url = environment.apiUrl + "/sites/write-matrix";
    let payload = {
      value: value,
      siteId: siteId,
      deviceId: deviceId,
    };
    return this.http.post<void>(url, payload);
  }

  setReadFrequency(value: number, siteId: number, deviceId: string): Observable<void> {
    const url = environment.apiUrl + "/sites/read-matrix";
    let payload = {
      value: value,
      siteId: siteId,
      deviceId: deviceId,
    };
    return this.http.post<void>(url, payload);
  }
}

import {Component, inject} from '@angular/core';
import {WriteMatrixDto} from "../../models/WriteMatrix";
import {ApiService} from "../../services/api.service";

@Component({
  selector: 'app-read-matrix',
  standalone: true,
  imports: [],
  templateUrl: './read-matrix.component.html',
  styleUrl: './read-matrix.component.scss'
})
export class ReadMatrixComponent {
  matrixList: WriteMatrixDto[] = [];

  apiService = inject(ApiService);

  ngOnInit(): void {
    this.apiService.getReadFrequencyMatrix().subscribe(res => {
      this.matrixList = res;
    });
  }

  saveFrequency(value: number, siteId: number, deviceId: string): void {
    this.apiService.setReadFrequency(value, siteId, deviceId).subscribe();
  }
}

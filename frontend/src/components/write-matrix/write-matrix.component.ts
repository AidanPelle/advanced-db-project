import { Component, inject, OnInit } from '@angular/core';
import { WriteMatrixDto } from '../../models/WriteMatrix';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-write-matrix',
  standalone: true,
  imports: [],
  templateUrl: './write-matrix.component.html',
  styleUrl: './write-matrix.component.scss'
})
export class WriteMatrixComponent implements OnInit {
  matrixList: WriteMatrixDto[] = [];

  apiService = inject(ApiService);

  ngOnInit(): void {
    this.apiService.getWriteFrequencyMatrix().subscribe(res => {
      this.matrixList = res;
    });
  }

  saveFrequency(value: number, siteId: number, deviceId: string): void {
    this.apiService.setWriteFrequency(value, siteId, deviceId).subscribe();
  }
}

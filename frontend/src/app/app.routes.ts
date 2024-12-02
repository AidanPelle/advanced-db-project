import { Routes } from '@angular/router';
import { DeviceListComponent } from '../components/device-list/device-list.component';
import { DeviceDetailComponent } from '../components/device-detail/device-detail.component';
import { WriteMatrixComponent } from '../components/write-matrix/write-matrix.component';

export const routes: Routes = [
    { path: 'writeMatrix', component: WriteMatrixComponent },
    { path: 'devices', component: DeviceListComponent },
    { path: 'devices/:id', component: DeviceDetailComponent },
];

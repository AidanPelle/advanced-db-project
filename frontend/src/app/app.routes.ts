import { Routes } from '@angular/router';
import { DeviceListComponent } from '../components/device-list/device-list.component';
import { DeviceDetailComponent } from '../components/device-detail/device-detail.component';
import { WriteMatrixComponent } from '../components/write-matrix/write-matrix.component';
import {ReadMatrixComponent} from "../components/read-matrix/read-matrix.component";
import {DeviceSiteUsageComponent} from "../components/device-site-usage/device-site-usage.component";

export const routes: Routes = [
    { path: 'readMatrix', component: ReadMatrixComponent},
    { path: 'writeMatrix', component: WriteMatrixComponent },
    { path: 'devices', component: DeviceListComponent },
    { path: 'devices/:id', component: DeviceDetailComponent },
    { path: 'trends/device-site-usage', component: DeviceSiteUsageComponent}
];

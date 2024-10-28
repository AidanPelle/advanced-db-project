import { Routes } from '@angular/router';
import { DeviceListComponent } from '../components/device-list/device-list.component';
import { DeviceDetailComponent } from '../components/device-detail/device-detail.component';

export const routes: Routes = [
    { path: 'devices', component: DeviceListComponent },
    { path: 'devices/:id', component: DeviceDetailComponent },
];

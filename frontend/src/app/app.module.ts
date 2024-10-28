import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeviceListComponent } from '../components/device-list/device-list.component';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { DeviceDetailComponent } from '../components/device-detail/device-detail.component';



@NgModule({
  declarations: [
    DeviceListComponent,
    DeviceDetailComponent,
  ],
  imports: [
    CommonModule,
    BrowserModule,
    RouterModule,
    HttpClientModule,
  ],
})
export class AppModule { }

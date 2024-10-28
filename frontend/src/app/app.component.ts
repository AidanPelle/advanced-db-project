import { HttpClient } from '@angular/common/http';
import { Component, inject, Inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  providers: [
    HttpClient,
  ]
})
export class AppComponent {

  router = inject(Router);

  title = 'frontend';

  navToDeviceList() {
    this.router.navigate(["devices"]);
  }
}

import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-main-layout',
  standalone: false,
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss'
})
export class MainLayoutComponent {
  displayName: string = 'Admin';

  logout(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('currentUser');
    localStorage.removeItem('permissions');
    this.router.navigate(['/login']);
  }

  constructor(private router: Router) {
    const currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
    this.displayName = currentUser.username;
  }
}

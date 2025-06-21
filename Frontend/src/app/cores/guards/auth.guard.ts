import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router) {}

  canActivate(): boolean | UrlTree {
    const token = localStorage.getItem('authToken');

    if (token) {
      return true; // Cho phép truy cập
    }

    // Nếu không có token thì chuyển về trang login
    return this.router.createUrlTree(['/login']);
  }
}

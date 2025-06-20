import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginService } from '../services/login.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private loginService: LoginService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Lấy token từ LoginService (localStorage)
    const token = this.loginService.getToken();

    // Nếu có token, thêm nó vào header Authorization
    if (token) {
      const requestWithToken = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });

      // Gửi request đã thêm token
      return next.handle(requestWithToken);
    }

    // Nếu không có token, gửi request gốc như bình thường
    return next.handle(request);
  }
}

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { LoginService } from '../../services/login.service';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private message: NzMessageService,
    private loginService: LoginService
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      
      const loginData = {
        username: this.loginForm.get('username')?.value,
        password: this.loginForm.get('password')?.value
      };

      this.loginService.login(loginData)
        .subscribe({
          next: (response) => {
            this.isLoading = false;
            
            if (response.success && response.token && response.user) {
              this.loginService.saveToken(response.token);
              localStorage.setItem('currentUser', JSON.stringify(response.user));

              this.message.success('Đăng nhập thành công!');
              this.router.navigate(['/dashboard']);
            }
          },
          error: (error) => {
            this.isLoading = false;
            this.showError(error.error?.message || 'Lỗi kết nối server');
          }
        });
    }
  }

  private showError(message: string): void {
    const vietnameseMessage = {
      'Invalid username or password': 'Tên đăng nhập hoặc mật khẩu không đúng',
      'Username and password are required': 'Vui lòng nhập tên đăng nhập và mật khẩu',
      'Account is inactive or deleted': 'Tài khoản đã bị khóa hoặc xóa',
      'An error occurred during login': 'Có lỗi xảy ra khi đăng nhập'
    }[message] || message;

    this.message.error(vietnameseMessage);
  }
}
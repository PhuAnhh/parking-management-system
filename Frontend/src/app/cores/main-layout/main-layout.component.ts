import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-main-layout',
  standalone: false,
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss'
})
export class MainLayoutComponent {
  displayName: string = 'Admin';       
  currentUser: any;                    
  
  isDrawerVisible = false;            
  
  showCurrentPassword = false;        
  showNewPassword = false;            
  showConfirmPassword = false;        
  
  changePasswordForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private userService: UserService,
    private notification: NzNotificationService
  ) {
    this.getUserInfo();
    
    this.initForm();
  }

  getUserInfo() {
    const userData = localStorage.getItem('currentUser');
    if (userData) {
      this.currentUser = JSON.parse(userData);
      this.displayName = this.currentUser.username || 'Admin';
    }
  }

  initForm() {
    this.changePasswordForm = this.fb.group({
      currentPassword: ['', Validators.required],       
      newPassword: ['', [Validators.required, Validators.minLength(6)]], 
      confirmPassword: ['', Validators.required]         
    });
  }

  checkPasswordMatch(): boolean {
    const newPass = this.changePasswordForm.get('newPassword')?.value;
    const confirmPass = this.changePasswordForm.get('confirmPassword')?.value;
    
    return newPass && confirmPass && newPass === confirmPass;
  }

  openDrawer() {
    this.isDrawerVisible = true;
  }

  closeDrawer() {
    this.isDrawerVisible = false;
    this.resetForm();
  }

  resetForm() {
    this.changePasswordForm.reset();    
    this.showCurrentPassword = false;   
    this.showNewPassword = false;     
    this.showConfirmPassword = false; 
  }

  changePassword() {
    if (!this.changePasswordForm.valid) {
      this.showNotification('warning', 'Vui lòng điền đầy đủ thông tin');
      return;
    }

    if (!this.checkPasswordMatch()) {
      this.showNotification('warning', 'Mật khẩu xác nhận không khớp');
      return;
    }

    const formData = this.changePasswordForm.value;
    const requestData = {
      currentPassword: formData.currentPassword,
      newPassword: formData.newPassword,
      confirmPassword: formData.confirmPassword
    };

    this.userService.changePassword(this.currentUser.id, requestData).subscribe({
      next: () => {
        this.showNotification('success', '');
        this.closeDrawer();
      },
      error: () => {
        this.showNotification('error', 'Mật khẩu hiện tại không đúng hoặc lỗi hệ thống');
      }
    });
  }

  showNotification(type: string, message: string) {
    const config = { nzDuration: 3000 };
    
    if (type === 'success') {
      this.notification.success('Thành công', message, config);
    } else if (type === 'warning') {
      this.notification.warning('Cảnh báo', message, config);
    } else if (type === 'error') {
      this.notification.error('Lỗi', message, config);
    }
  }

  logout() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('currentUser');
    localStorage.removeItem('permissions');
    this.router.navigate(['/login']);
  }
}
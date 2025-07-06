import { Component, OnInit, ChangeDetectorRef} from '@angular/core';
import { UserService } from '../../cores/services/user.service';
import { RoleService } from '../../cores/services/role.service';
import { LoginService } from '../../cores/services/login.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';
@Component({
  selector: 'app-users',
  standalone: false,
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss'
})
export class UsersComponent implements OnInit{
  roles: any[] = [];
  users: any[] = [];
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  isAddModalVisible = false; 
  isEditModalVisible = false; 
  isResetPasswordModalVisible = false;
  userForm!: FormGroup; 
  editUserForm!: FormGroup; 
  resetPasswordForm!: FormGroup;
  currentUserId: number | null = null;
  searchKeyword: string = '';

  constructor(
    private userService: UserService, 
    private roleService: RoleService,
    private cdr: ChangeDetectorRef,
    private modalService: NzModalService,
    private fb: FormBuilder,
    private notification: NzNotificationService,
    public loginService: LoginService
  ) {
    this.initForm();
  }  

  ngOnInit() {
    this.loadUsers();
    this.loadRoles();
  }

  initForm() {
    this.userForm = this.fb.group({
      username: [null, [Validators.required]],
      password: [null, [Validators.required]],
      name: [null],
      roleId: [null, [Validators.required]],
      status: [true]
    });

    this.editUserForm = this.fb.group({
      name: [null],
      roleId: [null, [Validators.required]],
      status: [true]
    });

    this.resetPasswordForm = this.fb.group({
      newPassword: [null, [Validators.required, Validators.minLength(6)]],
      confirmPassword: [null, [Validators.required]]
    })
  }

  passwordMatchValidator(form: FormGroup) {
    const newPassword = form.get('newPassword')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
    return newPassword === confirmPassword ? null : { passwordMismatch: true };
  }

  loadUsers(searchKeyword: string = '') {
    this.loading = true;
  
    this.userService.getUsers().subscribe(
      (data: any[]) => {
        data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        
        const filteredUsers = searchKeyword
          ? data.filter(user =>
              user.name.toLowerCase().includes(searchKeyword.toLowerCase()) ||
              user.username.toLowerCase().includes(searchKeyword.toLowerCase())
            )
          : data;
            
        this.total = filteredUsers.length;
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.users = filteredUsers.slice(start, end); 
        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách người dùng:', error);
        this.loading = false;
        if (error.status === 403) {
          this.notification.error(
            'Lỗi',
            'Bạn không có quyền xem danh sách người dùng',
            { nzDuration: 3000 }
          );
        } else {
          this.notification.error(
            'Lỗi',
            error.error?.message || 'Không thể tải dữ liệu người dùng',
            { nzDuration: 3000 }
          );
        }
      }
    );
  }

  loadRoles() {
    this.roleService.getRoles().subscribe(data => {
      this.roles = data;
    });
  }

  getRoleNameById(roleId: number): string {
    const role = this.roles.find(r => r.id === roleId);
    return role ? role.name : '';
  }
  
  onSearch() {
    this.loadUsers(this.searchKeyword); 
  }
  
  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadUsers(this.searchKeyword);
  }

  showAddUserModal() {
    this.isAddModalVisible = true;
    this.userForm.reset({status: true}); 
  }

  showEditUserModal(user: any) {
    this.currentUserId = user.id;
    
    if (this.editUserForm) {
      this.editUserForm.patchValue({
        name: user.name,
        roleId: user.roleId,
        status: user.status 
      });
      this.isEditModalVisible = true;
    } else {
      this.initForm();
      this.showEditUserModal(user);
    }
  }

  showResetPasswordModal(user: any) {
    this.currentUserId = user.id;
    this.resetPasswordForm.reset();
    this.isResetPasswordModalVisible = true;
  }

  handleCancel() {
    this.isAddModalVisible = false;
  }

  handleEditCancel() {
    this.isEditModalVisible = false;
    this.currentUserId = null;
  }

  handleResetPasswordCancel() {
    this.isResetPasswordModalVisible = false;
    this.currentUserId = null;
    this.resetPasswordForm.reset();
  }

  handleOk() {
    if (this.userForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }

    const newUser = this.userForm.value;

    const isDupicate = this.users.some(user => user.username === newUser.username);

    if(isDupicate) {
      this.notification.error(
        'Lỗi',
        'Tên bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    this.userService.addUser(newUser).subscribe(() => {
      this.loadUsers();
      this.isAddModalVisible = false; 
      this.notification.success(
        'Thành công',
        '',
        {
          nzPlacement: 'topRight',
          nzDuration: 3000
        }
      )
    },
    (error) => {
      console.error('Lỗi khi thêm người dùng:', error);
      this.notification.error(
        'Lỗi',
        '', 
        {
          nzPlacement: 'topRight',
          nzDuration: 3000
        }
      )
    });
  }

  handleEditOk() {
    if (this.editUserForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }
    
    const currentUser = this.users.find(u => u.id === this.currentUserId);

    const updatedUser = {
      ...this.editUserForm.value,
      username: currentUser?.username
    };
    
    const isDupicate = this.users.some(user =>
      user.name === updatedUser.name && user.id !== this.currentUserId
    );

    if (isDupicate) {
      this.notification.error(
        'Lỗi',
        'Tên bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    if (this.currentUserId) {
      this.userService.updateUser(this.currentUserId, updatedUser).subscribe(
        () => {
          this.loadUsers();
          this.isEditModalVisible = false;
          this.currentUserId = null;
          this.notification.success(
            'Thành công',
            '',
            {nzDuration: 3000}
          );
        },
        (error) => {
          console.error('Lỗi khi cập nhật', error);

          if(error.error && error.error.message){
            this.notification.error(
              'Lỗi',
              error.error.message,
              {nzDuration: 3000}
          );
          } else {
            this.notification.error(
              'Lỗi',
              'Không thể cập nhật người dùng. Vui lòng thử lại',
              {nzDuration: 3000}
            );
          }
        }
      );
    }
  }

  handleResetPasswordOk() {
    if (this.resetPasswordForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }

    const newPassword = this.resetPasswordForm.get('newPassword')?.value;
    const confirmPassword = this.resetPasswordForm.get('confirmPassword')?.value;

    if (newPassword !== confirmPassword) {
      this.notification.error(
        '',
        'Mật khẩu xác nhận không khớp',
        {nzDuration: 3000}
      );
      return;
    }

    if (this.currentUserId) {
      const resetPasswordData = {
        newPassword: newPassword,
        confirmPassword: confirmPassword
      };

      this.userService.resetPassword(this.currentUserId, resetPasswordData).subscribe(
        () => {
          this.isResetPasswordModalVisible = false;
          this.currentUserId = null;
          this.resetPasswordForm.reset();
          this.notification.success(
            'Thành công',
            '',
            {nzDuration: 3000}
          );
        },
        (error) => {
          console.error('Lỗi khi đặt lại mật khẩu:', error);
          
          if (error.error && error.error.message) {
            this.notification.error(
              'Lỗi',
              error.error.message,
              {nzDuration: 3000}
            );
          } else {
            this.notification.error(
              '',
              'Không thể đặt lại mật khẩu. Vui lòng thử lại',
              {nzDuration: 3000}
            );
          }
        }
      );
    }
  }

  updateUser(id: number) {
    const user = this.users.find(g => g.id === id);
    if (user) {
      this.showEditUserModal(user);
    } else {
      console.error(`User with id ${id} not found`);
    }
  }

  resetPassword(id: number) {
    const user = this.users.find(g => g.id === id);
    if (user) {
      this.showResetPasswordModal(user);
    } else {
      console.error(`User with id ${id} not found`);
    }
  }

  deleteUser(id: number) {
    this.modalService.confirm({
      nzTitle:'Bạn có chắc chắn muốn xóa?',
      nzMaskClosable: true,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.userService.deleteUser(id).subscribe(
          () => {
            this.loadUsers();
            this.notification.success(
              'Thành công',
              '',
              {
                nzPlacement: 'topRight',
                nzDuration: 3000
              }
            )
          },
          (error) => {
            console.error('Lỗi khi xóa người dùng:', error);
            this.notification.error(
              'Lỗi',
              '', 
              {
                nzPlacement: 'topRight',
                nzDuration: 3000
              }
            );
          }
        );
      }
    });
  }

  toggleUserStatus(userId: number) {
    const user = this.users.find(g => g.id === userId);
    if (!user) {
      return;
    }

    this.modalService.confirm({
      nzTitle: 'Xác nhận thay đổi trạng thái',
      nzMaskClosable: true,
      nzOkText: 'Xác nhận',
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        const newUser = !user.status;
    
        this.userService.changeUserStatus(userId, newUser).subscribe(
          () => {
            user.status = newUser;
            
            this.notification.success(
              'Thành công',
              '',
              {nzDuration: 3000}
            );
          },
          (error) => {
            console.error('Lỗi khi cập nhật trạng thái người dùng:', error);
            this.notification.error(
              'Lỗi',
              '',
              {nzDuration: 3000}
            );
          }
        );
      }
    });
  }
}

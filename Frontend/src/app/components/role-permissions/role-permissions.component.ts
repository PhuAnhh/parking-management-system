import { Component, OnInit, ChangeDetectorRef, HostListener} from '@angular/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { RoleService } from '../../services/role.service';

@Component({
  selector: 'app-role-permissions',
  standalone: false,
  templateUrl: './role-permissions.component.html',
  styleUrl: './role-permissions.component.scss'
})
export class RolePermissionsComponent implements OnInit{
  roles: any[] = [];
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  isAddModalVisible = false; 
  isEditModalVisible = false; 
  roleForm!: FormGroup; 
  editRoleForm!: FormGroup; 
  currentRoleId: number | null = null;
  searchKeyword: string = '';

  constructor(
    private roleService: RoleService, 
    private cdr: ChangeDetectorRef,
    private modalService: NzModalService,
    private fb: FormBuilder,
    private notification: NzNotificationService
  ) {
    this.initForm();
  }  

  ngOnInit() {
    this.loadRoles();
  }

  initForm() {
    this.roleForm = this.fb.group({
      name: [null, [Validators.required]],
      description: [null]
    });

    this.editRoleForm = this.fb.group({
      name: [null, [Validators.required]],
      description: [null]
    });
  }

  loadRoles(searchKeyword: string = '') {
    this.loading = true;
  
    this.roleService.getRoles().subscribe(
      (data: any[]) => {
        data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        
        const filteredRoles = searchKeyword
          ? data.filter(role =>
              role.name.toLowerCase().includes(searchKeyword.toLowerCase()) ||
              role.description.toLowerCase().includes(searchKeyword.toLowerCase())
            )
          : data;
            
        this.total = filteredRoles.length;
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.roles = filteredRoles.slice(start, end); 
        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách vai trò:', error);
        this.loading = false;
      }
    );
  }
  
  onSearch() {
    console.log(this.searchKeyword);
    this.loadRoles(this.searchKeyword); 
  }
  
  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadRoles(this.searchKeyword);
  }

  showAddRoleModal() {
    this.isAddModalVisible = true;
    this.roleForm.reset({status: true}); 
  }

  showEditRoleModal(role: any) {
    this.currentRoleId = role.id;
    
    if (this.editRoleForm) {
      this.editRoleForm.patchValue({
        name: role.name,
        description: role.description
      });
      this.isEditModalVisible = true;
    } else {
      console.error('Edit form is not initialized');
      this.initForm();
      this.showEditRoleModal(role);
    }
  }

  handleCancel() {
    this.isAddModalVisible = false;
  }

  handleEditCancel() {
    this.isEditModalVisible = false;
    this.currentRoleId = null;
  }

  handleOk() {
    if (this.roleForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }

    const newRole = this.roleForm.value;

    const isDupicate = this.roles.some(role => role.name === newRole.name);

    if(isDupicate) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    this.roleService.addRole(newRole).subscribe(() => {
      this.loadRoles();
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
      console.error('Lỗi khi thêm vai trò:', error);
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
    if (this.editRoleForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }
    
    const updatedRole = {
      ...this.editRoleForm.value,
    };
    
    console.log('Updated Role:', updatedRole);  

    const isDupicate = this.roles.some(role =>
      role.name === updatedRole.name && role.id !== this.currentRoleId
    );

    if (isDupicate) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    if (this.currentRoleId) {
      this.roleService.updateRole(this.currentRoleId, updatedRole).subscribe(
        () => {
          this.loadRoles();
          this.isEditModalVisible = false;
          this.currentRoleId = null;
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
              'Không thể cập nhật vai trò. Vui lòng thử lại',
              {nzDuration: 3000}
            );
          }
        }
      );
    }
  }

  updateRole(id: number) {
    const role = this.roles.find(g => g.id === id);
    if (role) {
      this.showEditRoleModal(role);
    } else {
      console.error(`User with id ${id} not found`);
    }
  }

  deleteRole(id: number) {
    this.modalService.confirm({
      nzTitle:'Bạn có chắc chắn muốn xóa?',
      nzMaskClosable: true,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.roleService.deleteRole(id).subscribe(
          () => {
            this.loadRoles();
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
            console.error('Lỗi khi xóa vai trò:', error);
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
}

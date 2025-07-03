import { Component, OnInit, ChangeDetectorRef} from '@angular/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder, FormArray } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { RoleService } from '../../cores/services/role.service';
import { LoginService } from '../../cores/services/login.service';
import { PermissionService } from '../../cores/services/permission.service';

interface Permission {
  id: number;
  name: string;
  endpoint: string;
  method: string;
}

interface GroupedPermissions {
  [category: string]: {
    [subcategory: string]: Permission[];
  };
}

@Component({
  selector: 'app-role-permissions',
  standalone: false,
  templateUrl: './role-permissions.component.html',
  styleUrl: './role-permissions.component.scss'
})
export class RolePermissionsComponent implements OnInit{
  roles: any[] = [];
  permissions: Permission[] = [];
  groupedPermissions: GroupedPermissions = {};
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  permissionsLoading = true;
  isAddModalVisible = false; 
  isEditModalVisible = false; 
  roleForm!: FormGroup; 
  editRoleForm!: FormGroup; 
  currentRoleId: number | null = null;
  searchKeyword: string = '';

  // Mapping cho categories và subcategories
  private readonly permissionConfig: { [key: string]: { category: string; subcategory: string } } = {
    'gate': { category: 'Quản lý thiết bị', subcategory: 'Cổng' },
    'computer': { category: 'Quản lý thiết bị', subcategory: 'Máy tính' },
    'camera': { category: 'Quản lý thiết bị', subcategory: 'Camera' },
    'controlunit': { category: 'Quản lý thiết bị', subcategory: 'Bộ điều khiển' },
    'lane': { category: 'Quản lý thiết bị', subcategory: 'Làn' },
    'led': { category: 'Quản lý thiết bị', subcategory: 'Led' },
    'card': { category: 'Quản lý thẻ', subcategory: 'Thẻ' },
    '/cardgroup': { category: 'Quản lý thẻ', subcategory: 'Nhóm thẻ' },
    'customer': { category: 'Quản lý khách hàng', subcategory: 'Khách hàng' },
    '/customergroup': { category: 'Quản lý khách hàng', subcategory: 'Nhóm khách hàng' },
    'entrylog': { category: 'Quản lý bãi xe', subcategory: 'Xe vào bãi' },
    'exitlog': { category: 'Quản lý bãi xe', subcategory: 'Xe ra bãi' },  
    'warningevent': { category: 'Quản lý bãi xe', subcategory: 'Cảnh báo' },
    'revenuereport': { category: 'Quản lý bãi xe', subcategory: 'Báo cáo doanh thu' },
    'role': { category: 'Quản lý tài khoản', subcategory: 'Vai trò, quyền hạn'},
    'user': { category: 'Quản lý tài khoản', subcategory: 'Người dùng' }
  };

  constructor(
    private roleService: RoleService,
    private permissionService: PermissionService,
    private cdr: ChangeDetectorRef,
    private modalService: NzModalService,
    private fb: FormBuilder,
    private notification: NzNotificationService,
    public loginService: LoginService
  ) {
    this.initForm();
  }  

  ngOnInit() {
    this.loadRoles();
    this.loadPermissions();
  }

  initForm() {
    this.roleForm = this.fb.group({
      name: [null, [Validators.required]],
      description: [null],
      permissionIds: this.fb.array([])
    });

    this.editRoleForm = this.fb.group({
      name: [null, [Validators.required]],
      description: [null],
      permissionIds: this.fb.array([])
    });
  }

  get permissionIdsFormArray(): FormArray {
    return this.roleForm.get('permissionIds') as FormArray;
  }

  get editPermissionIdsFormArray(): FormArray {
    return this.editRoleForm.get('permissionIds') as FormArray;
  }

  loadPermissions() {
    this.permissionService.getPermissions().subscribe(
      (data: Permission[]) => {
        this.permissions = data;
        this.groupPermissions();
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách quyền:', error);
      }
    );
  }

  groupPermissions() {
    this.groupedPermissions = {};
    
    this.permissions.forEach(permission => {
      const category = this.getCategoryFromPermission(permission);
      const subcategory = this.getSubcategoryFromPermission(permission);
      
      if (!this.groupedPermissions[category]) {
        this.groupedPermissions[category] = {};
      }
      
      if (!this.groupedPermissions[category][subcategory]) {
        this.groupedPermissions[category][subcategory] = [];
      }
      
      this.groupedPermissions[category][subcategory].push(permission);
    });
  }

  getCategoryFromPermission(permission: Permission): string {
    const key = this.getPermissionKey(permission);
    return this.permissionConfig[key]?.category || 'Khác';
  }

  getSubcategoryFromPermission(permission: Permission): string {
    const key = this.getPermissionKey(permission);
    return this.permissionConfig[key]?.subcategory || 'Khác';
  }

  private getPermissionKey(permission: Permission): string {
    const searchText = `${permission.endpoint} ${permission.name}`.toLowerCase();
    
    // Sắp xếp keys theo độ dài giảm dần để tìm match chính xác nhất trước
    const sortedKeys = Object.keys(this.permissionConfig).sort((a, b) => b.length - a.length);
    
    // Tìm key match chính xác nhất
    const foundKey = sortedKeys.find(key => 
      searchText.includes(key)
    );
    
    return foundKey || '';
  }

  getObjectKeys(obj: any): string[] {
    return Object.keys(obj);
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
        this.notification.error('Lỗi', 'Không thể tải dữ liệu vai trò');
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
    this.roleForm.reset();
    this.initPermissionCheckboxes();
  }

  showEditRoleModal(role: any) {
    this.currentRoleId = role.id;
    
    if (this.editRoleForm) {
      this.editRoleForm.patchValue({
        name: role.name,
        description: role.description
      });
      this.initEditPermissionCheckboxes(role.permissions || []);
      this.isEditModalVisible = true;
    } else {
      this.initForm();
      this.showEditRoleModal(role);
    }
  }

  initPermissionCheckboxes() {
    const permissionFormArray = this.permissionIdsFormArray;
    permissionFormArray.clear();
    
    this.permissions.forEach(() => {
      permissionFormArray.push(this.fb.control(false));
    });
  }

  initEditPermissionCheckboxes(rolePermissions: any[]) {
    const permissionFormArray = this.editPermissionIdsFormArray;
    permissionFormArray.clear();
    
    this.permissions.forEach(permission => {
      const isChecked = rolePermissions.some(rp => rp.id === permission.id);
      permissionFormArray.push(this.fb.control(isChecked));
    });
  }

  onPermissionChange(permissionIndex: number, checked: boolean, isEdit: boolean = false) {
    const formArray = isEdit ? this.editPermissionIdsFormArray : this.permissionIdsFormArray;
    formArray.at(permissionIndex).setValue(checked);
  }

  getSelectedPermissionIds(isEdit: boolean = false): number[] {
    const formArray = isEdit ? this.editPermissionIdsFormArray : this.permissionIdsFormArray;
    const selectedIds: number[] = [];
    
    formArray.controls.forEach((control, index) => {
      if (control.value) {
        selectedIds.push(this.permissions[index].id);
      }
    });
    
    return selectedIds;
  }

  isPermissionChecked(permissionId: number, isEdit: boolean = false): boolean {
    const formArray = isEdit ? this.editPermissionIdsFormArray : this.permissionIdsFormArray;
    const permissionIndex = this.permissions.findIndex(p => p.id === permissionId);
    return permissionIndex >= 0 ? formArray.at(permissionIndex).value : false;
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

    const formValue = this.roleForm.value;
    const selectedPermissionIds = this.getSelectedPermissionIds();
    
    const newRole = {
      name: formValue.name,
      description: formValue.description,
      permissionIds: selectedPermissionIds
    };

    const isDuplicate = this.roles.some(role => role.name === newRole.name);

    if(isDuplicate) {
      this.notification.error(
        'Lỗi',
        'Tên bị trùng lặp',
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
        error.error?.message || '', 
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
    
    const formValue = this.editRoleForm.value;
    const selectedPermissionIds = this.getSelectedPermissionIds(true);
    
    const updatedRole = {
      name: formValue.name,
      description: formValue.description,
      permissionIds: selectedPermissionIds
    };
    
    const isDuplicate = this.roles.some(role =>
      role.name === updatedRole.name && role.id !== this.currentRoleId
    );

    if (isDuplicate) {
      this.notification.error(
        'Lỗi',
        'Tên bị trùng lặp',
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
              '',
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
              error.error?.message || '', 
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

  trackByPermissionId(index: number, permission: Permission): number {
    return permission.id;
  }
}
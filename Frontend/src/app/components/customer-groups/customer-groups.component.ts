import { Component, ChangeDetectorRef} from '@angular/core';
import { CustomerGroupService } from '../../cores/services/customer-group.service';
import { LoginService } from '../../cores/services/login.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-customer-groups',
  standalone: false,
  templateUrl: './customer-groups.component.html',
  styleUrl: './customer-groups.component.scss'
})
export class CustomerGroupsComponent {
  customerGroups: any[] = [];
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  isAddModalVisible = false; 
  isEditModalVisible = false; 
  customerGroupForm!: FormGroup; 
  editCustomerGroupForm!: FormGroup; 
  isVisible = false;
  currentCustomerGroupId: number | null = null;
  searchKeyword: string = '';

  constructor(
    private customerGroupService: CustomerGroupService, 
    private cdr: ChangeDetectorRef,
    private modalService: NzModalService,
    private fb: FormBuilder,
    private notification: NzNotificationService,
    public loginService: LoginService
  ) {
    this.initForm();
  }  
  
  ngOnInit() {
    this.loadCustomerGroups();
  }

  initForm() {
    this.customerGroupForm = this.fb.group({
      code: [null, [Validators.required]],
      name: [null, [Validators.required]],
    });

    this.editCustomerGroupForm = this.fb.group({
      code: [null, [Validators.required]],
      name: [null, [Validators.required]],
    });
  }

  loadCustomerGroups(searchKeyword: string = '') {
    this.loading = true;
  
    this.customerGroupService.getCustomerGroups().subscribe(
      (data: any[]) => {
        data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        
        const filteredCustomerGroups = searchKeyword
          ? data.filter(customerGroup =>
              customerGroup.name.toLowerCase().includes(searchKeyword.toLowerCase()) ||
              customerGroup.code.toLowerCase().includes(searchKeyword.toLowerCase())
            )
          : data;
          
          console.log('Kết quả đã lọc:', filteredCustomerGroups);
  
        this.total = filteredCustomerGroups.length;
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.customerGroups = filteredCustomerGroups.slice(start, end); 
        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách nhóm khách hàng:', error);
        this.notification.error('Lỗi', 'Không thể tải dữ liệu nhóm khách hàng');
        this.loading = false;
      }
    );
  }

  onSearch() {
    console.log(this.searchKeyword);
    this.loadCustomerGroups(this.searchKeyword); 
  }
  
  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadCustomerGroups(this.searchKeyword);
  }

  showAddCustomerGroupModal() {
    this.isAddModalVisible = true;
    this.customerGroupForm.reset(); 
  }

  showEditCustomerGroupModal(customerGroup: any) {
    this.currentCustomerGroupId = customerGroup.id;
    
    if (this.editCustomerGroupForm) {
      this.editCustomerGroupForm.patchValue({
        name: customerGroup.name,
        code: customerGroup.code,
      });
      this.isEditModalVisible = true;
    } else {
      console.error('Edit form is not initialized');
      this.initForm();
      this.showEditCustomerGroupModal(customerGroup);
    }
  }

  handleCancel() {
    this.isAddModalVisible = false;
  }

  handleEditCancel() {
    this.isEditModalVisible = false;
    this.currentCustomerGroupId = null;
  }

  handleOk() {
    if (this.customerGroupForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }

    const newCustomerGroup = this.customerGroupForm.value;

    const isDupicate = this.customerGroups.some(customerGroup => customerGroup.code === newCustomerGroup.code);

    if(isDupicate) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    this.customerGroupService.addCustomerGroup(newCustomerGroup).subscribe(() => {
      this.loadCustomerGroups();
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
      console.error('Lỗi khi thêm nhóm khách hàng:', error);
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
    if (this.editCustomerGroupForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }
    
    const updatedCustomerGroup = {
      ...this.editCustomerGroupForm.value,
    };
    
    console.log('Updated CustomerGroup:', updatedCustomerGroup);  

    const isDupicate = this.customerGroups.some(customerGroup =>
      customerGroup.code === updatedCustomerGroup.code && customerGroup.id !== this.currentCustomerGroupId
    );

    if (isDupicate) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    if (this.currentCustomerGroupId) {
      this.customerGroupService.updateCustomerGroup(this.currentCustomerGroupId, updatedCustomerGroup).subscribe(
        () => {
          this.loadCustomerGroups();
          this.isEditModalVisible = false;
          this.currentCustomerGroupId = null;
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
              'Không thể cập nhật nhóm khách hàng. Vui lòng thử lại',
              {nzDuration: 3000}
            );
          }
        }
      );
    }
  }

  updateCustomerGroup(id: number) {
    const customerGroup = this.customerGroups.find(g => g.id === id);
    if (customerGroup) {
      this.showEditCustomerGroupModal(customerGroup);
    } else {
      console.error(`CustomerGroup with id ${id} not found`);
    }
  }

  deleteCustomerGroup(id: number) {
    this.modalService.confirm({
      nzTitle:'Bạn có chắc chắn muốn xóa?',
      nzMaskClosable: true,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.customerGroupService.deleteCustomerGroup(id).subscribe(
          () => {
            this.loadCustomerGroups();
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
            console.error('Lỗi khi xóa nhóm khách hàng:', error);
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

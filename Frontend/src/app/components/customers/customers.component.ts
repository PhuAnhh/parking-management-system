import { Component, OnInit, ChangeDetectorRef, HostListener} from '@angular/core';
import { CustomerService } from '../../services/customer.service';
import { CustomerGroupService } from '../../services/customer-group.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-customers',
  standalone: false,
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.scss'
})
export class CustomersComponent {
  customers: any[] = [];
  customerGroups: any[] = [];
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  isAddModalVisible = false; 
  isEditModalVisible = false; 
  customerForm!: FormGroup; 
  editCustomerForm!: FormGroup; 
  isVisible = false;
  currentCustomerId: number | null = null;
  searchKeyword: string = '';

  constructor(
    private customerService: CustomerService, 
    private customerGroupService: CustomerGroupService,
    private cdr: ChangeDetectorRef,
    private modalService: NzModalService,
    private fb: FormBuilder,
    private notification: NzNotificationService
  ) {
    this.initForm();
  }  

  ngOnInit() {
    this.loadCustomers();
    this.loadCustomerGroups();
  }

  initForm() {
    this.customerForm = this.fb.group({
      name: [null, [Validators.required]],
      code: [null, [Validators.required]],
      phone: [null],
      address: [null],
      customerGroupId: [null],
      status: [true]
    });

    this.editCustomerForm = this.fb.group({
      name: [null, [Validators.required]],
      code: [null, [Validators.required]],
      phone: [null],
      address: [null],
      customerGroupId: [null],
      status: [true]
    });
  }

  loadCustomers(searchKeyword: string = '') {
    this.loading = true;
  
    this.customerService.getCustomers().subscribe(
      (data: any[]) => {
        console.log(data);
        const filteredCustomers = searchKeyword
          ? data.filter(customer =>
              customer.name.toLowerCase().includes(searchKeyword.toLowerCase()) ||
              customer.code.toLowerCase().includes(searchKeyword.toLowerCase())
            )
          : data;
          
          console.log('Kết quả đã lọc:', filteredCustomers);
  
        this.total = filteredCustomers.length;
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.customers = filteredCustomers.slice(start, end); 
        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách khách hàng:', error);
        this.loading = false;
      }
    );
  }

  onSearch() {
    console.log(this.searchKeyword);
    this.loadCustomers(this.searchKeyword); 
  }

  loadCustomerGroups() {
    this.customerGroupService.getCustomerGroups().subscribe(data => {
      this.customerGroups = data;
    });
  }

  getCustomerGroupById(customerGroupId: number): string {
    const customerGroup = this.customerGroups.find(g => g.id === customerGroupId);
    return customerGroup ? customerGroup.name : '';  
  }
  
  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadCustomers(this.searchKeyword);
  }

  showAddCustomerModal() {
    this.isAddModalVisible = true;
    this.customerForm.reset(); 
  }

  showEditCustomerModal(customer: any) {
    this.currentCustomerId = customer.id;
    
    if (this.editCustomerForm) {
      this.editCustomerForm.patchValue({
        name: customer.name,
        code: customer.code,
        phone: customer.phone,
        address: customer.address,
        customerGroupId: customer.customerGroupId,
      });
      this.isEditModalVisible = true;
    } else {
      console.error('Edit form is not initialized');
      this.initForm();
      this.showEditCustomerModal(customer);
    }
  }

  handleCancel() {
    this.isAddModalVisible = false;
  }

  handleEditCancel() {
    this.isEditModalVisible = false;
    this.currentCustomerId = null;
  }

  handleOk() {
    if (this.customerForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }

    const newCustomer = this.customerForm.value;

    const isDupicate = this.customers.some(customer => customer.code === newCustomer.code);

    if(isDupicate) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    this.customerService.addCustomer(newCustomer).subscribe(() => {
      this.loadCustomers();
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
      console.error('Lỗi khi thêm khách hàng:', error);
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
    if (this.editCustomerForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }
    
    const updatedCustomer = {
      ...this.editCustomerForm.value,
    };
    
    console.log('Updated Customer:', updatedCustomer);  

    const isDupicate = this.customers.some(customer =>
      customer.code === updatedCustomer.code && customer.id !== this.currentCustomerId
    );

    if (isDupicate) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    if (this.currentCustomerId) {
      this.customerService.updateCustomer(this.currentCustomerId, updatedCustomer).subscribe(
        () => {
          this.loadCustomers();
          this.isEditModalVisible = false;
          this.currentCustomerId = null;
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
              'Không thể cập nhật cổng. Vui lòng thử lại',
              {nzDuration: 3000}
            );
          }
        }
      );
    }
  }

  updateCustomer(id: number) {
    const customer = this.customers.find(g => g.id === id);
    if (customer) {
      this.showEditCustomerModal(customer);
    } else {
      console.error(`Customer with id ${id} not found`);
    }
  }

  deleteCustomer(id: number) {
    this.modalService.confirm({
      nzTitle:'Bạn có chắc chắn muốn xóa?',
      nzMaskClosable: true,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.customerService.deleteCustomer(id).subscribe(
          () => {
            this.loadCustomers();
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
            console.error('Lỗi khi xóa khách hàng:', error);
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

  cardList = [
    {
      name: 'Thẻ VIP 1',
      code: 'VIP001',
      group: 'VIP',
      status: true
    },
    {
      name: 'Thẻ thường 2',
      code: 'STD002',
      group: 'Standard',
      status: false
    }
  ];
  
}


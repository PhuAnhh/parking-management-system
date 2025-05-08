import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CustomerService } from '../../services/customer.service';
import { CustomerGroupService } from '../../services/customer-group.service';
import { CardService } from '../../services/card.service';
import { CardGroupService } from '../../services/card-group.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';

enum CardStatus {
  LOCKED = 'Locked',
  ACTIVE = 'Active',
  INACTIVE = 'Inactive'
}

@Component({
  selector: 'app-customers',
  standalone: false,
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.scss'
})
export class CustomersComponent {
  customers: any[] = [];
  customerGroups: any[] = [];
  cards: any[] = [];
  cardGroups: any[] = [];
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

  // Card liên quan
  customerCards: any[] = [];
  isCardModalVisible = false;
  isAddCardModalVisible = false;
  selectedCustomer: any = null;
  cardForm!: FormGroup;
  availableCards: any[] = [];

  cardStatuses = [
    { label: 'Đã khóa', value: CardStatus.LOCKED},
    { label: 'Đang sử dụng', value: CardStatus.ACTIVE},
    { label: 'Chưa sử dụng', value: CardStatus.INACTIVE},
  ];

  constructor(
    private customerService: CustomerService, 
    private customerGroupService: CustomerGroupService,
    private cardService: CardService,
    private cardGroupService: CardGroupService, 
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
    this.loadAllCards();
    this.loadCardGroups();
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

    this.cardForm = this.fb.group({
      cardId: [null, [Validators.required]]
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

  loadAllCards() {
    this.cardService.getCards().subscribe(
      (data: any[]) => {
        this.cards = data;
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách thẻ:', error);
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

  loadCardGroups() {
    this.cardGroupService.getCardGroups().subscribe(data => {
      this.cardGroups = data;
    });
  }

  getCustomerGroupById(customerGroupId: number): string {
    const customerGroup = this.customerGroups.find(g => g.id === customerGroupId);
    return customerGroup ? customerGroup.name : '';  
  }

  getCardGroupNameById(cardGroupId: number): string {
    const cardGroup = this.cardGroups.find(g => g.id === cardGroupId);
    return cardGroup ? cardGroup.name : '';  
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
    this.selectedCustomer = customer;
    
    if (this.editCustomerForm) {
      this.editCustomerForm.patchValue({
        name: customer.name,
        code: customer.code,
        phone: customer.phone,
        address: customer.address,
        customerGroupId: customer.customerGroupId,
      });
  
      // Load danh sách thẻ của khách hàng này với timeout nhỏ để đảm bảo form được cập nhật trước
      setTimeout(() => {
        this.loadCustomerCards(customer.id);
      }, 100);
      
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

  // Phương thức quản lý thẻ cho khách hàng
  showCustomerCards(customer: any) {
    this.selectedCustomer = customer;
    this.loadCustomerCards(customer.id);
    this.isCardModalVisible = true;
  }

  loadCustomerCards(customerId: number) {
    this.cardService.getCards().subscribe(
      (cards: any[]) => {
        console.log("Tất cả thẻ từ API:", cards);
        console.log("Lọc thẻ cho khách hàng ID:", customerId);
        
        // Lấy tất cả thẻ của khách hàng này, bất kể trạng thái nào
        this.customerCards = cards.filter(card => card.customerId === customerId);
        console.log("Danh sách thẻ của khách hàng sau khi lọc:", this.customerCards);
  
        const assignedCardIds = this.customerCards.map(card => card.id);
        
        // Lọc các thẻ có thể được gán cho khách hàng
        this.availableCards = cards.filter(card => 
          // Thẻ chưa được gán cho khách hàng nào
          (!card.customerId || card.customerId === null) || 
          // HOẶC thẻ đã được gán cho khách hàng khác nhưng inactive
          (card.customerId !== customerId && card.status === CardStatus.INACTIVE && !assignedCardIds.includes(card.id))
        );
        
        // Đảm bảo UI được cập nhật
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách thẻ của khách hàng:', error);
        this.notification.error(
          'Lỗi',
          'Không thể tải danh sách thẻ',
          {nzDuration: 3000}
        );
      }
    );
  }

  showAddCardModal() {
    if (!this.selectedCustomer) {
      this.notification.error(
        'Lỗi',
        'Vui lòng chọn khách hàng trước',
        { nzDuration: 3000 }
      );
      return;
    }

    this.cardForm.reset();
    this.isAddCardModalVisible = true;
  }

  handleCardCancel() {
    this.isCardModalVisible = false;
    this.selectedCustomer = null;
  }

  handleAddCardCancel() {
    this.isAddCardModalVisible = false;
  }

  handleAddCardOk() {
    if (this.cardForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng chọn thẻ',
        {nzDuration: 3000}
      );
      return;
    }
  
    const cardId = this.cardForm.value.cardId;
    const card = this.availableCards.find(c => c.id === cardId);
    
    if (!card) {
      this.notification.error(
        'Lỗi',
        'Thẻ không tồn tại hoặc đã được gán cho khách hàng khác',
        { nzDuration: 3000 }
      );
      return;
    }
  
    const updatedCard = {
      ...card,
      customerId: this.selectedCustomer.id,
      status: CardStatus.ACTIVE
    };
  
    this.cardService.updateCard(cardId, updatedCard).subscribe(
      (response) => {
        // Đóng modal trước
        this.isAddCardModalVisible = false;
        this.cardForm.reset();
        
        // Thông báo thành công
        this.notification.success(
          'Thành công',
          'Đã gán thẻ cho khách hàng',
          {nzDuration: 3000}
        );
        
        // Quan trọng: Tải lại toàn bộ danh sách thẻ để đảm bảo đồng bộ
        this.loadCustomerCards(this.selectedCustomer.id);
      },
      (error) => {
        console.error('Lỗi khi gán thẻ cho khách hàng:', error);
        this.notification.error(
          'Lỗi',
          'Không thể gán thẻ cho khách hàng',
          {nzDuration: 3000}
        );
      }
    );
  }

  removeCardFromCustomer(cardId: number) {
    this.modalService.confirm({
      nzTitle: 'Bạn có chắc chắn muốn hủy liên kết thẻ này khỏi khách hàng?',
      nzMaskClosable: true,
      nzOkText: 'Xác nhận',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        const card = this.customerCards.find(c => c.id === cardId);
        if (!card) return;
  
        const updatedCard = {
          ...card,
          customerId: null,
          status: CardStatus.INACTIVE  // Thay đổi trạng thái thành INACTIVE khi hủy liên kết
        };
  
        this.cardService.updateCard(cardId, updatedCard).subscribe(
          (response) => {
            // Thông báo thành công
            this.notification.success(
              'Thành công',
              'Đã hủy liên kết thẻ',
              {nzDuration: 3000}
            );
            
            // Quan trọng: Tải lại toàn bộ danh sách thẻ để đảm bảo đồng bộ
            this.loadCustomerCards(this.selectedCustomer.id);
          },
          (error) => {
            console.error('Lỗi khi hủy liên kết thẻ:', error);
            this.notification.error(
              'Lỗi',
              'Không thể hủy liên kết thẻ. Vui lòng thử lại',
              {nzDuration: 3000}
            );
          }
        );
      }
    });
  }

  getCardStatusLabel(status: string): string {
    const cardStatus = this.cardStatuses.find(s => s.value === status);
    return cardStatus ? cardStatus.label : status;
  }

  getStatusColor(status: string): string {
    switch(status) {
      case 'Locked': return '#ff5500';
      case 'Active': return '#87d068';
      case 'Inactive': return '#108ee9';
      default: return '';
    }
  }
}
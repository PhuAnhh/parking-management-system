import { Component, OnInit, ChangeDetectorRef, HostListener} from '@angular/core';
import { CardService } from '../../services/card.service';
import { CardGroupService } from '../../services/card-group.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';

enum CardStatus{
  LOCKED = 'Locked',
  ACTIVE = 'Active',
  INACTIVE = 'Inactive'
}

@Component({
  selector: 'app-cards',
  standalone: false,
  templateUrl: './cards.component.html',
  styleUrl: './cards.component.scss'
})
export class CardsComponent {
  cards: any[] = [];
  cardGroups: any[] = [];
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  isVisible = false;
  isAddModalVisible = false; 
  isEditModalVisible = false; 
  cardForm!: FormGroup; 
  editCardForm!: FormGroup; 
  currentCardId: number | null = null;
  searchKeyword: string = '';

  cardStatuses = [
    { label: 'Đã khóa', value: CardStatus.LOCKED},
    { label: 'Đang sử dụng', value: CardStatus.ACTIVE},
    { label: 'Chưa sử dụng', value: CardStatus.INACTIVE},
  ]

  getCardStatus(value: string){
    return this.cardStatuses.find(opt => opt.value === value);
  }

  getStatusColor(status: string): string {
    switch(status) {
      case 'Locked': return '#ff5500';
      case 'Active': return '#87d068';
      case 'Inactive': return '#108ee9';
      default: return '';
    }
  }

  constructor(
    private cardService: CardService, 
    private cardGroupService: CardGroupService, 
    private cdr: ChangeDetectorRef,
    private modalService: NzModalService,
    private fb:FormBuilder,
    private notification: NzNotificationService
  ) {
    this.initForm();
  }

  ngOnInit() {
    this.loadCards();
    this.loadCardGroups();
  }

  initForm() {
    this.cardForm = this.fb.group({
      name: [null, [Validators.required]],
      code: [null, [Validators.required]],
      cardGroupId: [null, [Validators.required]],
      customerId: [null],
      note: [null],
      status: [CardStatus.INACTIVE, [Validators.required]]
    });

    this.editCardForm = this.fb.group({
      name: [null, [Validators.required]],
      code: [null, [Validators.required]],
      cardGroupId: [null, [Validators.required]],
      customerId: [null],
      note: [null],
      status: [CardStatus.INACTIVE, [Validators.required]]
    });
  }

  loadCards(searchKeyword: string = '') {
    this.loading = true;

    this.cardService.getCards().subscribe(
      (data: any[]) => {
        console.log(data);
        const filteredCards = searchKeyword
          ? data.filter(card =>
              card.code.toLowerCase().includes(searchKeyword.toLowerCase()) ||
              card.name.toLowerCase().includes(searchKeyword.toLowerCase()) 
            )
          : data;
        this.total = filteredCards.length; 

        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.cards = filteredCards.slice(start, end); 

        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách thẻ:', error);
        this.loading = true;
      }
    );
  }

  loadCardGroups() {
    this.cardGroupService.getCardGroups().subscribe(data => {
      this.cardGroups = data;
    });
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadCards(this.searchKeyword);
  }

  onSearch() {
    console.log(this.searchKeyword);
    this.loadCards(this.searchKeyword); 
  }

  getCardGroupNameById(cardGroupId: number): string {
    const cardGroup = this.cardGroups.find(g => g.id === cardGroupId);
    return cardGroup ? cardGroup.name : '';  
  }

  showAddCardModal() {
    this.isAddModalVisible = true;
    this.cardForm.reset({
      status: CardStatus.INACTIVE,
    }); 
  }

  showEditCardModal(card: any) {
    this.currentCardId = card.id;
    
    if (this.editCardForm) {
      this.editCardForm.patchValue({
        name: card.name,
        code: card.code,
        cardGroupId: card.cardGroupId,
        note: card.note,
        status: card.status 
      });
      this.isEditModalVisible = true;
    } else {
      console.error('Edit form is not initialized');
      this.initForm();
      this.showEditCardModal(card);
    }
  }

  handleCancel() {
    this.isAddModalVisible = false;
  }

  handleEditCancel() {
    this.isEditModalVisible = false;
    this.currentCardId = null;
  }

  handleOk() {
    if (this.cardForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }

    const newCard = this.cardForm.value;

    const isDuplicateName = this.cards.some(card => card.name === newCard.name);
    const isDuplicateCode = this.cards.some(card => card.code === newCard.code);
    
    if(isDuplicateName) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );  
      return;
    }

    if(isDuplicateCode) {
      this.notification.error(
        'Lỗi',
        'Mã: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );  
      return;
    }
    
    this.cardService.addCard(newCard).subscribe(
      () => {
        this.loadCards();
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
        console.error('Lỗi khi thêm thẻ:', error);
        this.notification.error(
          'Lỗi',
          'Không thể thêm thẻ. Vui lòng thử lại',
          {
            nzPlacement: 'topRight',
            nzDuration: 3000
          }
        );
      }
    );
  }

  handleEditOk() {
    if (this.editCardForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }
    
    const updatedCard = this.editCardForm.value;

    const isDupicateName = this.cards.some(card =>
      card.name === updatedCard.name && card.id !== this.currentCardId
    );
    const isDupicateCode = this.cards.some(card =>
      card.code === updatedCard.code && card.id !== this.currentCardId
    );

    if(isDupicateName) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );  
      return;
    }

    if(isDupicateCode) {
      this.notification.error(
        'Lỗi',
        'Mã: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );  
      return;
    }
    
    if (this.currentCardId) {
      this.cardService.updateCard(this.currentCardId, updatedCard).subscribe(
        () => {
          this.loadCards();
          this.isEditModalVisible = false;
          this.currentCardId = null;
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
              'Không thể cập nhật thẻ. Vui lòng thử lại',
              {nzDuration: 3000}
            );
          }
        }
      );
    }
  }

  updateCard(id: number) {
    const card = this.cards.find(g => g.id === id);
    if (card) {
      this.showEditCardModal(card);
    } else {
      console.error(`Card with id ${id} not found`);
    }
  }

  deleteCard(id: number) {
    this.modalService.confirm({
      nzTitle:'Bạn có chắc chắn muốn xóa?',
      nzMaskClosable: true,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.cardService.deleteCard(id).subscribe(
          () => {
            this.loadCards();
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
            console.error('Lỗi khi xóa thẻ:', error);
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

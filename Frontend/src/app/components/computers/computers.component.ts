import { Component, OnInit, ChangeDetectorRef, HostListener} from '@angular/core';
import { ComputerService } from '../../services/computer.service';
import { GateService } from '../../services/gate.service';
import { LoginService } from '../../services/login.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-computers',
  standalone: false,
  templateUrl: './computers.component.html',
  styleUrl: './computers.component.scss'
})
export class ComputersComponent {
  computers: any[] = [];
  gates: any[] = [];
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  isVisible = false;
  isAddModalVisible = false; 
  isEditModalVisible = false; 
  computerForm!: FormGroup; 
  editComputerForm!: FormGroup; 
  currentComputerId: number | null = null;
  searchKeyword: string = '';

  @HostListener('window:scroll')
  onWindowScroll() {
    const scrollPosition = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
    this.isVisible = scrollPosition > 300;
  }

  backToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  constructor(
      private gateService: GateService, 
      private computerService: ComputerService, 
      private cdr: ChangeDetectorRef,
      private modalService: NzModalService,
      private fb:FormBuilder,
      private notification: NzNotificationService,
      public loginService: LoginService
    ) {
      this.initForm();
    }

  ngOnInit() {
    this.loadComputers();
    this.loadGates();
  }

  initForm() {
    this.computerForm = this.fb.group({
      name: [null, [Validators.required]],
      ipAddress: [null, [Validators.required]],
      gateId: [null],
      status: [true]
    });

    this.editComputerForm = this.fb.group({
      name: [null, [Validators.required]],
      ipAddress: [null, [Validators.required]],
      gateId: [null],
      status: [true]
    });
  }
  

  loadComputers(searchKeyword: string = '') {
    this.loading = true;

    this.computerService.getComputers().subscribe(
      (data: any[]) => {
        data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        
        const filteredComputers = searchKeyword
          ? data.filter(computer =>
              computer.name.toLowerCase().includes(searchKeyword.toLowerCase()) 
            )
          : data;
        this.total = filteredComputers.length; 
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.computers = filteredComputers.slice(start, end); 
        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách máy tính:', error);
        this.loading = true;
      }
    );
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadComputers(this.searchKeyword);
  }

  onSearch() {
    console.log(this.searchKeyword);
    this.loadComputers(this.searchKeyword); 
  }

  loadGates() {
    this.gateService.getGates().subscribe(data => {
      this.gates = data;
    });
  }

  getGateNameById(gateId: number): string {
    const gate = this.gates.find(g => g.id === gateId);
    return gate ? gate.name : '';  
  }

  showAddComputerModal() {
    this.isAddModalVisible = true;
    this.computerForm.reset({status: true}); 
  }

  showEditComputerModal(computer: any) {
    this.currentComputerId = computer.id;
    
    if (this.editComputerForm) {
      this.editComputerForm.patchValue({
        name: computer.name,
        ipAddress: computer.ipAddress,
        gateId: computer.gateId,
        status: computer.status 
      });
      this.isEditModalVisible = true;
    } else {
      console.error('Edit form is not initialized');
      this.initForm();
      this.showEditComputerModal(computer);
    }
  }

  handleCancel() {
    this.isAddModalVisible = false;
  }

  handleEditCancel() {
    this.isEditModalVisible = false;
    this.currentComputerId = null;
  }

  handleOk() {
    if (this.computerForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
    }

    const newComputer = this.computerForm.value;
    
    this.computerService.addComputer(newComputer).subscribe(
      () => {
        this.loadComputers();
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
        console.error('Lỗi khi thêm máy tính:', error);
        this.notification.error(
          'Lỗi',
          'Không thể thêm máy tính. Vui lòng thử lại',
          {
            nzPlacement: 'topRight',
            nzDuration: 3000
          }
        );
      }
    );
  }

  handleEditOk() {
    if (this.editComputerForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }
    
    const updatedComputer = this.editComputerForm.value;

    const isDupicate = this.computers.some(computer =>
      computer.name === updatedComputer.name && computer.id !== this.currentComputerId
    );

    if(isDupicate) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );  
      return;
    }
    
    if (this.currentComputerId) {
      this.computerService.updateComputer(this.currentComputerId, updatedComputer).subscribe(
        () => {
          this.loadComputers();
          this.isEditModalVisible = false;
          this.currentComputerId = null;
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

  updateComputer(id: number) {
    const computer = this.computers.find(g => g.id === id);
    if (computer) {
      this.showEditComputerModal(computer);
    } else {
      console.error(`Computer with id ${id} not found`);
    }
  }

  deleteComputer(id: number) {
    this.modalService.confirm({
      nzTitle:'Bạn có chắc chắn muốn xóa?',
      nzMaskClosable: true,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.computerService.deleteComputer(id).subscribe(
          () => {
            this.loadComputers();
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
            console.error('Lỗi khi xóa máy tính:', error);
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

  toggleComputerStatus(computerId: number) {
    const computer = this.computers.find(c => c.id === computerId);
    if (!computer) {
      console.error(`Không tìm thấy camera với id ${computerId}`);
      return;
    }

    this.modalService.confirm({
      nzTitle: 'Xác nhận thay đổi trạng thái',
      nzMaskClosable: true,
      nzOkText: 'Xác nhận',
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        const newComputer = !computer.status;
    
        this.computerService.changeComputerStatus(computerId, newComputer).subscribe(
          () => {
            computer.status = newComputer;
            
            this.notification.success(
              'Thành công',
              '',
              {nzDuration: 3000}
            );
          },
          (error) => {
            console.error('Lỗi khi cập nhật trạng thái máy tính:', error);
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

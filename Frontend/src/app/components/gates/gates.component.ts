import { Component, OnInit, ChangeDetectorRef, HostListener} from '@angular/core';
import { GateService } from '../../services/gate.service';
import { LoginService } from '../../services/login.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-gates',
  standalone: false,
  templateUrl: './gates.component.html',
  styleUrls: ['./gates.component.scss']
})

export class GatesComponent implements OnInit{
  gates: any[] = [];
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  isAddModalVisible = false; 
  isEditModalVisible = false; 
  gateForm!: FormGroup; 
  editGateForm!: FormGroup; 
  isVisible = false;
  currentGateId: number | null = null;
  searchKeyword: string = '';

  @HostListener('window:scroll')
  onWindowScroll(){
    const scrollPosition = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
    this.isVisible = scrollPosition > 300;
  }

  backToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  constructor(
    private gateService: GateService, 
    private cdr: ChangeDetectorRef,
    private modalService: NzModalService,
    private fb: FormBuilder,
    private notification: NzNotificationService,
    public loginService: LoginService
  ) {
    this.initForm();
  }  

  ngOnInit() {
    this.loadGates();
  }

  initForm() {
    this.gateForm = this.fb.group({
      name: [null, [Validators.required]],
      code: [null, [Validators.required]],
      status: [true]
    });

    this.editGateForm = this.fb.group({
      name: [null, [Validators.required]],
      code: [null, [Validators.required]],
      status: [true]
    });
  }

  loadGates(searchKeyword: string = '') {
    this.loading = true;
  
    this.gateService.getGates().subscribe(
      (data: any[]) => {
        data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        
        const filteredGates = searchKeyword
          ? data.filter(gate =>
              gate.name.toLowerCase().includes(searchKeyword.toLowerCase()) ||
              gate.code.toLowerCase().includes(searchKeyword.toLowerCase())
            )
          : data;
          
          console.log('Kết quả đã lọc:', filteredGates);
  
        this.total = filteredGates.length;
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.gates = filteredGates.slice(start, end); 
        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách cổng:', error);
        this.loading = false;
      }
    );
  }
  
  onSearch() {
    console.log(this.searchKeyword);
    this.loadGates(this.searchKeyword); 
  }
  
  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadGates(this.searchKeyword);
  }


  showAddGateModal() {
    this.isAddModalVisible = true;
    this.gateForm.reset({status: true}); 
  }

  showEditGateModal(gate: any) {
    this.currentGateId = gate.id;
    
    if (this.editGateForm) {
      this.editGateForm.patchValue({
        name: gate.name,
        code: gate.code,
        status: gate.status 
      });
      this.isEditModalVisible = true;
    } else {
      console.error('Edit form is not initialized');
      this.initForm();
      this.showEditGateModal(gate);
    }
  }

  handleCancel() {
    this.isAddModalVisible = false;
  }

  handleEditCancel() {
    this.isEditModalVisible = false;
    this.currentGateId = null;
  }

  handleOk() {
    if (this.gateForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }

    const newGate = this.gateForm.value;

    const isDupicate = this.gates.some(gate => gate.code === newGate.code);

    if(isDupicate) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    this.gateService.addGate(newGate).subscribe(() => {
      this.loadGates();
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
      console.error('Lỗi khi thêm cổng:', error);
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
    if (this.editGateForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }
    
    const updatedGate = {
      ...this.editGateForm.value,
    };
    
    console.log('Updated Gate:', updatedGate);  

    const isDupicate = this.gates.some(gate =>
      gate.code === updatedGate.code && gate.id !== this.currentGateId
    );

    if (isDupicate) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    if (this.currentGateId) {
      this.gateService.updateGate(this.currentGateId, updatedGate).subscribe(
        () => {
          this.loadGates();
          this.isEditModalVisible = false;
          this.currentGateId = null;
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

  updateGate(id: number) {
    const gate = this.gates.find(g => g.id === id);
    if (gate) {
      this.showEditGateModal(gate);
    } else {
      console.error(`Gate with id ${id} not found`);
    }
  }

  deleteGate(id: number) {
    this.modalService.confirm({
      nzTitle:'Bạn có chắc chắn muốn xóa?',
      nzMaskClosable: true,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.gateService.deleteGate(id).subscribe(
          () => {
            this.loadGates();
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
            console.error('Lỗi khi xóa cổng:', error);
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

  toggleGateStatus(gateId: number) {
    const gate = this.gates.find(g => g.id === gateId);
    if (!gate) {
      console.error(`Không tìm thấy cổng với id ${gateId}`);
      return;
    }

    this.modalService.confirm({
      nzTitle: 'Xác nhận thay đổi trạng thái',
      nzMaskClosable: true,
      nzOkText: 'Xác nhận',
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        const updatedGate = {...gate, status: !gate.status};
    
        this.gateService.updateGate(gateId, updatedGate).subscribe(
          () => {
            gate.status = !gate.status;
            
            this.notification.success(
              'Thành công',
              '',
              {nzDuration: 3000}
            );
          },
          (error) => {
            console.error('Lỗi khi cập nhật trạng thái cổng:', error);
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
import { Component, OnInit, ChangeDetectorRef, HostListener  } from '@angular/core';
import { ControlUnitService } from '../../services/control-unit.service';
import { ComputerService } from '../../services/computer.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';

enum ControlUnitType {
  IDTECK = 'IDTECK',
  KZE02DOTNET = 'KZE02DOTNET',
  KZE16DOTNET = 'KZE16DOTNET',
  MT166 = 'MT166',
  INGRESSUS = 'INGRESSUS',
  E02DOTNET = 'E02DOTNET', 
  SC200 = 'SC200',
  Dahua = 'Dahua'
}

enum ControlUnitConnectionProtocolType
{
    TCP_IP = 'TCP_IP',
    RS232_485_422 = 'RS232_485_422'
}

@Component({
  selector: 'app-control-units',
  standalone: false,
  templateUrl: './control-units.component.html',
  styleUrl: './control-units.component.scss',
})
export class ControlUnitsComponent implements OnInit{
  computers: any[] = [];
  controlUnits: any[] = [];
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  isVisible = false;
  isAddModalVisible = false;
  isEditModalVisible = false; 
  controlUnitForm!: FormGroup; 
  editControlUnitForm!: FormGroup; 
  currentControlUnitId: number | null = null;
  searchKeyword: string = '';

  controlUnitTypes = [
    { label: 'IDTECK', value: ControlUnitType.IDTECK, color: 'lime' },
    { label: 'KZE02.NET', value: ControlUnitType.KZE02DOTNET, color: 'gold' },
    { label: 'KZE16.NET', value: ControlUnitType.KZE16DOTNET, color: 'volcano' },
    { label: 'MT166', value: ControlUnitType.MT166, color: 'magenta' },
    { label: 'INGRESSUS', value: ControlUnitType.INGRESSUS, color: 'geekblue' },
    { label: 'E02.NET', value: ControlUnitType.E02DOTNET, color: 'purple' },
    { label: 'SC200', value: ControlUnitType.SC200, color: 'cyan' },
    { label: 'Dahua', value: ControlUnitType.Dahua, color: 'pink' }
  ];

  getControlUnitType(value:string){
    return this.controlUnitTypes.find(opt => opt.value === value);
  }
  

  controlUnitConnectionProtocolTypes = [
    { label: 'TCP_IP', value: ControlUnitConnectionProtocolType.TCP_IP },
    { label: 'RS232_485_422', value: ControlUnitConnectionProtocolType.RS232_485_422 }
  ];
  
  @HostListener('window:scroll')
  onWindowScroll() {
    const scrollPosition = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
    this.isVisible = scrollPosition > 300;
  }

  backToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  constructor(
    private controlUnitService: ControlUnitService, 
    private computerService: ComputerService, 
    private cdr: ChangeDetectorRef,
    private modalService: NzModalService,
    private fb:FormBuilder,
    private notification: NzNotificationService
  ) {
    this.initForm();
  }  

  ngOnInit() {
    this.loadControlUnits();
    this.loadComputers();
  }

  initForm() {
    this.controlUnitForm = this.fb.group({
      name: [null, [Validators.required]],
      code: [null, [Validators.required]],
      username: [null],
      password: [null],
      type: [ControlUnitType.E02DOTNET],
      connectionProtocol: [ControlUnitConnectionProtocolType.TCP_IP],
      comport: [null],
      baudrate: [null],
      computerId: [null, [Validators.required]],
      status: [true]
    });

    this.editControlUnitForm = this.fb.group({
      name: [null, [Validators.required]],
      code: [null, [Validators.required]],
      username: [null],
      password: [null],
      type: [null],
      connectionProtocol: [null],
      comport: [null],
      baudrate: [null],
      computerId: [null, [Validators.required]],
      status: [true]
    });
  }
  

  loadControlUnits(searchKeyword: string = '') {
    this.loading = true;

    this.controlUnitService.getControlUnits().subscribe(
      (data: any[]) => {
        data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        
        const filteredControlUnits = searchKeyword
          ? data.filter(controlUnit =>
              controlUnit.name.toLowerCase().includes(searchKeyword.toLowerCase()) ||
              controlUnit.code.toLowerCase().includes(searchKeyword.toLowerCase())
            )
          : data;
        this.total = filteredControlUnits.length; 
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.controlUnits = filteredControlUnits.slice(start, end); 
        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách bộ điều khiển:', error);
        this.loading = false;
      }
    );
  }

  onSearch() {
    console.log(this.searchKeyword);
    this.loadControlUnits(this.searchKeyword); 
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadControlUnits(this.searchKeyword);
  }

  loadComputers() {
    this.computerService.getComputers().subscribe(data => {
      this.computers = data;
    });
  }

  getComputerNameById(computerId: number): string {
    const computer = this.computers.find(g => g.id === computerId);
    return computer ? computer.name : '';  
  }

  showAddControlUnitModal() {
    this.isAddModalVisible = true;
    this.controlUnitForm.reset({
      status: true,
      type: ControlUnitType.E02DOTNET,
      connectionProtocol: ControlUnitConnectionProtocolType.TCP_IP
    }); 
  }

  showEditControlUnitModal(controlUnit: any) {
    this.currentControlUnitId = controlUnit.id;
    
    if (this.editControlUnitForm) {
      this.editControlUnitForm.patchValue({
        name: controlUnit.name,
        code: controlUnit.code,
        username: controlUnit.username,
        password: controlUnit.password,
        type: controlUnit.type,
        connectionProtocol: controlUnit.connectionProtocol,
        comport: controlUnit.comport,
        baudrate: controlUnit.baudrate,
        computerId: controlUnit.computerId,
        status: controlUnit.status 
      });
      this.isEditModalVisible = true;
    } else {
      console.error('Edit form is not initialized');
      this.initForm();
      this.showEditControlUnitModal(controlUnit);
    }
  }

  handleCancel() {
    this.isAddModalVisible = false;
  }

  handleEditCancel() {
    this.isEditModalVisible = false;
    this.currentControlUnitId = null;
  }

  handleOk() {
    if (this.controlUnitForm.invalid) {
      this.modalService.warning({
        nzTitle: 'Vui lòng nhập đủ thông tin',
        nzContent: ''
      });
      return;
    }

    const newControlUnit = this.controlUnitForm.value;

    const isDupicateName = this.controlUnits.some(controlUnit => controlUnit.name === newControlUnit.name);
    const isDupicateCode = this.controlUnits.some(controlUnit => controlUnit.code === newControlUnit.code);

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

    this.controlUnitService.addControlUnit(newControlUnit).subscribe(
      () => {
        this.loadControlUnits();
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
        console.error('Lỗi khi thêm bộ điều khiển:', error);
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

  handleEditOk() {
    if (this.editControlUnitForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }
    
    const updatedControlUnit = this.editControlUnitForm.value;

    const isDupicateName = this.controlUnits.some(controlUnit =>
      controlUnit.name === updatedControlUnit.name && controlUnit.id !== this.currentControlUnitId
    );

    const isDupicateCode = this.controlUnits.some(controlUnit =>
      controlUnit.code === updatedControlUnit.code && controlUnit.id !== this.currentControlUnitId
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

    console.log('Updated ControlUnit:', updatedControlUnit)
    
    if (this.currentControlUnitId) {
      this.controlUnitService.updateControlUnit(this.currentControlUnitId, updatedControlUnit).subscribe(
        () => {
          this.loadControlUnits();
          this.isEditModalVisible = false;
          this.currentControlUnitId = null;
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
              'Không thể cập nhật bộ điều khiển. Vui lòng thử lại',
              {nzDuration: 3000}
            );
          }
        }
      );
    }
  }

  updateControlUnit(id: number) {
    const controlUnit = this.controlUnits.find(g => g.id === id);
    if (controlUnit) {
      this.showEditControlUnitModal(controlUnit);
    } else {
      console.error(`ControlUnit with id ${id} not found`);
    }
  }

  deleteControlUnit(id: number) {
    this.modalService.confirm({
      nzTitle:'Bạn có chắc chắn muốn xóa?',
      nzMaskClosable: true,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.controlUnitService.deleteControlUnit(id).subscribe(
          () => {
            this.loadControlUnits();
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
            console.error('Lỗi khi xóa bộ điều khiển:', error);
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

  toggleControlUnitStatus(controlUnitId: number) {
    const controlUnit = this.controlUnits.find(c => c.id === controlUnitId);
    if (!controlUnit) {
      console.error(`Không tìm thấy bộ điều khiển với id ${controlUnitId}`);
      return;
    }

    this.modalService.confirm({
      nzTitle: 'Xác nhận thay đổi trạng thái',
      nzMaskClosable: true,
      nzOkText: 'Xác nhận',
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        const updatedControlUnit = {...controlUnit, status: !controlUnit.status};
    
        this.controlUnitService.updateControlUnit(controlUnitId, updatedControlUnit).subscribe(
          () => {
            controlUnit.status = !controlUnit.status;
            
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

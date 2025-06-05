import { Component, OnInit, ChangeDetectorRef, HostListener} from '@angular/core';
import { ComputerService } from '../../services/computer.service';
import { CameraService } from '../../services/camera.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';

enum CameraType{
  SECUS = 'Secus',
  SHANY = 'Shany',
  BOSCH = 'Bosch',
  VANTECH = 'Vantech',
  CNB = 'CNB',
  HIK = 'HIK',
  ENSTER = 'Enster',
  DAHUA = 'Dahua',
  HANSE = 'Hanse',
  TIANDY = 'Tiandy',
  DMAX = 'DMAX',
  VIVANTEK = 'Vivantek'
}

@Component({
  selector: 'app-cameras',
  standalone: false,
  templateUrl: './cameras.component.html',
  styleUrl: './cameras.component.scss'
})
export class CamerasComponent {
  computers: any[] = [];
  cameras: any[] = [];
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = true;
  isVisible = false;
  isAddModalVisible = false; 
  isEditModalVisible = false; 
  cameraForm!: FormGroup;
  editCameraForm!: FormGroup; 
  currentCameraId: number | null = null;
  searchKeyword: string = '';

  cameraTypes = [
    { label: 'SECUS', value: CameraType.SECUS, color: 'pink'},
    { label: 'SHANY', value: CameraType.SHANY, color: 'red' },
    { label: 'BOSCH', value: CameraType.BOSCH, color: 'yellow' },
    { label: 'VANTECH', value: CameraType.VANTECH, color: 'orange' },
    { label: 'CNB', value: CameraType.CNB, color: 'cyan' },
    { label: 'HIK', value: CameraType.HIK, color: 'green' },
    { label: 'ENSTER', value: CameraType.ENSTER, color: 'blue' },
    { label: 'DAHUA', value: CameraType.DAHUA, color: 'purple' },
    { label: 'HANSE', value: CameraType.HANSE, color: 'geekblue' },
    { label: 'TIANDY', value: CameraType.TIANDY, color: 'magenta' },
    { label: 'DMAX', value: CameraType.DMAX, color: 'volcano' },
    { label: 'VIVANTEK', value: CameraType.VIVANTEK, color: 'lime' }
  ];

  getCameraType(value:string){
    return this.cameraTypes.find(opt => opt.value === value);
  }

  @HostListener('window:scroll')
  onWindowScroll() {
    const scrollPosition = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
    this.isVisible = scrollPosition > 300;
  }

  backToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  constructor(
      private cameraService: CameraService, 
      private computerService: ComputerService, 
      private cdr: ChangeDetectorRef,
      private modalService: NzModalService,
      private fb:FormBuilder,
      private notification: NzNotificationService

    ) {
      this.initForm();
    }

  ngOnInit() {
    this.loadCameras();
    this.loadComputers();
  }

  initForm() {
    this.cameraForm = this.fb.group({
      name: [null, [Validators.required]],
      code: [null, [Validators.required]],
      ipAddress: [null],
      type: [CameraType.TIANDY],
      username: [null, [Validators.required]],
      password: [null],
      computerId: [null, [Validators.required]],
      resolution: ['1280x720'],
      status: [true]
    });

    this.editCameraForm = this.fb.group({
      name: [null, [Validators.required]],
      code: [null, [Validators.required]],
      ipAddress: [null],
      type: [null],
      username: [null, [Validators.required]],
      password: [null],
      computerId: [null, [Validators.required]],
      resolution: [null],
      status: [true]
    });
  }
  

  loadCameras(searchKeyword: string = '') {
    this.loading = true;
  
    this.cameraService.getCameras().subscribe(
      (data: any[]) => {
        data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());

        const filteredCameras = searchKeyword
          ? data.filter(camera =>
              camera.name.toLowerCase().includes(searchKeyword.toLowerCase()) ||
              camera.code.toLowerCase().includes(searchKeyword.toLowerCase())
            )
          : data;
        this.total = filteredCameras.length;
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.cameras = filteredCameras.slice(start, end);
        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách camera:', error);
        this.loading = false;
      }
    );
  }

  onSearch() {
    console.log(this.searchKeyword);
    this.loadCameras(this.searchKeyword); 
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadCameras(this.searchKeyword);
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

  showAddCameraModal() {
    this.isAddModalVisible = true;
    this.cameraForm.reset({
      status: true,
      type: CameraType.TIANDY,
      resolution: '1280x720'
    });
  }

  showEditCameraModal(camera: any) {
    this.currentCameraId = camera.id;
    
    if (this.editCameraForm) {
      this.editCameraForm.patchValue({
        name: camera.name,
        code: camera.code,
        computerId: camera.computerId,
        ipAddress: camera.ipAddress,
        username: camera.username,
        password: camera.password,
        resolution: camera.resolution,
        type: camera.type,
        gateId: camera.gateId,
        status: camera.status 
      });
      this.isEditModalVisible = true;
    } else {
      console.error('Edit form is not initialized');
      this.initForm();
      this.showEditCameraModal(camera);
    }
  }

  handleCancel() {
    this.isAddModalVisible = false;
  }

  handleEditCancel() {
    this.isEditModalVisible = false;
    this.currentCameraId = null;
  }

  handleOk() {
    if (this.cameraForm.invalid) 
    {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }

    const newCamera = this.cameraForm.value;

    const isDuplicateName = this.cameras.some(camera => camera.name === newCamera.name);
    const isDuplicateCode = this.cameras.some(camera => camera.code === newCamera.code);

    if(isDuplicateName) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    if (isDuplicateCode) {
      this.notification.error(
        'Lỗi',
        'Mã camera: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    this.cameraService.addCamera(newCamera).subscribe(
      () => {
        this.loadCameras();
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
        console.error('Lỗi khi thêm camera:', error);
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
    if (this.editCameraForm.invalid) 
    {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      ); 
      return;
    }
    
    const updatedCamera = this.editCameraForm.value;

    const isDuplicateName = this.cameras.some(camera =>
      camera.name === updatedCamera.name && camera.id !== this.currentCameraId
    );

    const isDuplicateCode = this.cameras.some(camera =>
      camera.code === updatedCamera.code && camera.id !== this.currentCameraId
    );

    if(isDuplicateName) {
      this.notification.error(
        'Lỗi',
        'Tên: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }

    if (isDuplicateCode) {
      this.notification.error(
        'Lỗi',
        'Mã: Trường bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }
    
    if (this.currentCameraId) {
      this.cameraService.updateCamera(this.currentCameraId, updatedCamera).subscribe(
        () => {
          this.loadCameras();
          this.isEditModalVisible = false;
          this.currentCameraId = null;
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

  updateCamera(id: number) {
    const camera = this.cameras.find(g => g.id === id);
    if (camera) {
      this.showEditCameraModal(camera);
    } else {
      console.error(`Camera with id ${id} not found`);
    }
  }

  deleteCamera(id: number) {
    this.modalService.confirm({
      nzTitle:'Bạn có chắc chắn muốn xóa?',
      nzMaskClosable: true,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.cameraService.deleteCamera(id).subscribe(
          () => {
            this.loadCameras();
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
            this.notification.error(
              'Thất bại',
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

  toggleCameraStatus(cameraId: number) {
    const camera = this.cameras.find(c => c.id === cameraId);
    if (!camera) {
      console.error(`Không tìm thấy camera với id ${cameraId}`);
      return;
    }

    this.modalService.confirm({
      nzTitle: 'Xác nhận thay đổi trạng thái',
      nzMaskClosable: true,
      nzOkText: 'Xác nhận',
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        const updatedCamera = {...camera, status: !camera.status};
    
        this.cameraService.updateCamera(cameraId, updatedCamera).subscribe(
          () => {
            camera.status = !camera.status;
            
            this.notification.success(
              'Thành công',
              '',
              {nzDuration: 3000}
            );
          },
          (error) => {
            console.error('Lỗi khi cập nhật trạng thái camera:', error);
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

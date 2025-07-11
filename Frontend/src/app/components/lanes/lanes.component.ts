import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { LaneService } from '../../cores/services/lane.service';
import { ComputerService } from '../../cores/services/computer.service';
import { CameraService} from '../../cores/services/camera.service';
import { ControlUnitService } from '../../cores/services/control-unit.service';
import { LoginService } from '../../cores/services/login.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FormGroup, Validators, FormBuilder, FormArray } from '@angular/forms';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { LaneType } from '../../cores/enums/lane-type.enum';
import { LaneAutoOpenBarrier } from '../../cores/enums/lane-auto-open-barrier.enum';
import { LaneCameraPurpose } from '../../cores/enums/lane-camera-purpose.enum';

@Component({
  selector: 'app-lanes',
  standalone: false,
  templateUrl: './lanes.component.html',
  styleUrl: './lanes.component.scss'
})
export class LanesComponent implements OnInit{
  computers: any[] = [];
  cameras: any[] = [];
  controlUnits: any[] = [];
  lanes: any[] = [];
  pageIndex = 1;
  pageSize = 10;
  total = 0;
  loading = false;
  isVisible = false;
  isAddModalVisible = false;
  isEditModalVisible = false;
  laneForm!: FormGroup;
  editLaneForm!: FormGroup; 
  currentLaneId: number | null = null;
  filteredCameras: any[] = [];
  filteredControlUnits: any[] = [];
  searchKeyword: string = ''
  
  laneTypes = [
    { label: 'Làn vào', value: LaneType.IN, color: 'success' },
    { label: 'Làn ra', value: LaneType.OUT, color: 'error' },
    { label: 'Làn ki-ốt', value: LaneType.KIOSKIN, color: 'processing' },
    { label: 'Làn tự động', value: LaneType.DYNAMIC, color: 'warning' }
  ]

  getLaneTypeOption(value: string) {
    return this.laneTypes.find(x => x.value === value);
  }  

  getLaneTypeLabel(type: LaneType): string {
    const found = this.laneTypes.find(t => t.value === type);
    return found ? found.label : type;
  }
  
  laneAutoOpenBarrierOptions = [
    { label: 'Khi hợp lệ', shortLabel: 'Khi hợp lệ', value: LaneAutoOpenBarrier.WHENVALID, status: 'success' },
    { label: 'Không bao giờ', shortLabel: 'Không bao giờ', value: LaneAutoOpenBarrier.NEVER, status: 'error' },
    { label: 'Luôn luôn (Hệ thống sẽ bỏ qua kiểm tra biển số)', shortLabel: 'Luôn luôn', value: LaneAutoOpenBarrier.ALWAYS, status: 'processing' },
    { label: 'Khi xe đăng ký hợp lệ (Không tự mở cho thẻ lượt)', shortLabel: 'Khi xe đăng ký hợp lệ', value: LaneAutoOpenBarrier.WHENVALIDVEHICLE, status: 'warning' }
  ];

  getAutoOpenBarrierOption(value: string) {
    return this.laneAutoOpenBarrierOptions.find(o => o.value === value);
  }

  getAutoOpenBarrierShortLabel(value: string): string {
    const option = this.laneAutoOpenBarrierOptions.find(o => o.value === value);
    if (option) {
      return option.label.split(' (')[0];
    }
    return value;
  }  
  
  laneCameraPurposes = [
    { label: 'Biển số xe máy', value: LaneCameraPurpose.MOTORBIKEPLATENUMBER },
    { label: 'Biển số ô tô', value: LaneCameraPurpose.CARPLATENUMBER },
    { label: 'Toàn cảnh', value: LaneCameraPurpose.PANORAMA },
    { label: 'Khác', value: LaneCameraPurpose.OTHER }
  ]
  
  displayPositions = [0, 1, 2, 3, 4, 5];
  
  constructor(
    private laneService: LaneService, 
    private computerService: ComputerService,
    private cameraService: CameraService,
    private controlUnitService: ControlUnitService,
    private cdr: ChangeDetectorRef,
    private modalService: NzModalService,
    private fb: FormBuilder,
    private notification: NzNotificationService,
    public loginService: LoginService
  ) {
    this.initForm();
  }  

  ngOnInit() {
    this.initForm();
    this.loadLanes();
    this.loadComputers();
    this.loadCameras();
    this.loadControlUnits();


    this.laneForm.get('computerId')?.valueChanges.subscribe(computerId => {
      this.filteredCameras = this.cameras.filter(c => c.computerId === computerId);
      this.clearLaneCameras(); 

      this.filteredControlUnits = this.controlUnits.filter(cu => +cu.computerId === +computerId);
      this.clearLaneControlUnits();

      if(computerId) {
        this.filteredControlUnits.forEach(controlUnit => {
          this.addlaneControlUnit(controlUnit);
        });
      }
    });

    this.editLaneForm.get('computerId')?.valueChanges.subscribe(computerId => {
      this.filteredCameras = this.cameras.filter(c => c.computerId === computerId);
      this.clearEditLaneCameras();
  
      this.filteredControlUnits = this.controlUnits.filter(cu => +cu.computerId === +computerId);
      this.clearEditLaneControlUnits();
  
      if(computerId) {
        this.filteredControlUnits.forEach(controlUnit => {
          this.addEditLaneControlUnit(controlUnit);
        });
      }
    });
  }

  addlaneControlUnit(laneControlUnit: any) {
    const controlUnitGroup = this.fb.group({
      id: [laneControlUnit.id],
      name: [laneControlUnit.name],
      controlUnitId: [laneControlUnit.id],
      reader: [[]], 
      input: [[]],  
      barrier: [[]], 
      alarm: [[]]  
    });
  
    const laneControlUnits = this.laneForm.get('laneControlUnits') as FormArray;
    laneControlUnits.push(controlUnitGroup);
  }

  initForm() {
    this.laneForm = this.fb.group({
      name: [null, [Validators.required]],
      code: [null, [Validators.required]],
      computerId: [null, [Validators.required]],
      type: [null, [Validators.required]],
      autoOpenBarrier: [null, [Validators.required]],
      loop: [false],
      status: [true],
      laneCameras: this.fb.array([]),
      laneControlUnits: this.fb.array([])
    });

    this.editLaneForm = this.fb.group({
      name: [null, [Validators.required]],
      code: [null, [Validators.required]],
      computerId: [null, [Validators.required]],
      type: [null, [Validators.required]],
      autoOpenBarrier: [null, [Validators.required]],
      loop: [false],
      status: [true],
      laneCameras: this.fb.array([]),
      laneControlUnits: this.fb.array([])
    });
  }

  loadLanes(searchKeyword: string = '') {
    this.loading = true;
    
    this.laneService.getLanes().subscribe(
      (data: any[]) => {
        data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        
        const filteredLanes = searchKeyword
          ? data.filter(lane =>
              lane.name.toLowerCase().includes(searchKeyword.toLowerCase()) ||
              lane.code.toLowerCase().includes(searchKeyword.toLowerCase())
            )
          : data;
        this.total = filteredLanes.length; 
        const start = (this.pageIndex - 1) * this.pageSize;
        const end = start + this.pageSize;
        this.lanes = filteredLanes.slice(start, end); 
        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách làn:', error);
        this.notification.error('Lỗi', 'Không thể tải dữ liệu làn');
        this.loading = false;
      }
    );
  }

  onSearch() {
    console.log(this.searchKeyword);
    this.loadLanes(this.searchKeyword); 
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadLanes(this.searchKeyword);
  }

  loadComputers() {
    this.computerService.getComputers().subscribe(data => {
      this.computers = data;
    });
  }

  loadCameras() {
    this.cameraService.getCameras().subscribe(data => {
      this.cameras = data;
    });
  }

  loadControlUnits() {
    this.controlUnitService.getControlUnits().subscribe(data => {
      this.controlUnits = data;
    });
  }
  
  getComputerNameById(computerId: number): string {
    const computer = this.computers.find(g => g.id === computerId);
    return computer ? computer.name : '';  
  }

  showAddLaneModal() {
    this.isAddModalVisible = true;
    this.laneForm.reset({
      status: true,
      loop: false
    }); 
    this.loadLanes();
  }

  showEditLaneModal(lane: any) {
    if (!this.editLaneForm) {
      console.error('Edit form is not initialized');
      this.initForm();
    }
  
    this.currentLaneId = lane.id;
    this.editLaneForm.reset(); 
    this.clearEditLaneCameras();
    this.clearEditLaneControlUnits();
  
    this.editLaneForm.patchValue({
      name: lane.name,
      code: lane.code,
      computerId: lane.computerId,
      type: lane.type,
      autoOpenBarrier: lane.autoOpenBarrier,
      loop: lane.loop,
      status: lane.status
    });
  
    lane.laneCameras?.forEach((camera: any) => {
      this.editLaneCameras.push(this.fb.group({
        cameraId: [camera.cameraId, Validators.required],
        purpose: [camera.purpose],
        displayPosition: [camera.displayPosition]
      }));
    });
  
    const parseStringToNumberArray = (value: string | null | undefined) => {
      if (!value) return [];
      return value.split(', ')
        .filter(item => item.trim().length > 0)
        .map(item => Number(item)); 
    };

    this.clearEditLaneControlUnits();
  
    lane.laneControlUnits?.forEach((unit: any) => {
      const controlUnit = this.controlUnits.find(cu => +cu.id === +unit.controlUnitId);
      
      const readerArray = parseStringToNumberArray(unit.reader);
      const inputArray = parseStringToNumberArray(unit.input);
      const barrierArray = parseStringToNumberArray(unit.barrier);
      const alarmArray = parseStringToNumberArray(unit.alarm);
      
      console.log('Control unit values:', {
        reader: readerArray,
        input: inputArray,
        barrier: barrierArray,
        alarm: alarmArray
      });
  
      this.editLaneControlUnits.push(this.fb.group({
        id: [unit.controlUnitId],
        name: [controlUnit ? controlUnit.name : ''],
        reader: [readerArray],
        input: [inputArray], 
        barrier: [barrierArray],
        alarm: [alarmArray]
      }));
    });
  
    this.isEditModalVisible = true;
  }
  
  handleCancel() {
    this.isAddModalVisible = false;
  }

  handleEditCancel() {
    this.isEditModalVisible = false;
    this.currentLaneId = null;
  }

  handleOk() {
    if (this.laneForm.invalid) {
      this.notification.warning('', 'Vui lòng nhập đủ thông tin', { nzDuration: 3000 });
      return;
    }
  
    const rawLane = this.laneForm.value;
  
    const newLane = {
      ...rawLane,
      laneControlUnits: rawLane.laneControlUnits.map((unit: any) => ({
        controlUnitId: unit.controlUnitId || unit.id,
        reader: Array.isArray(unit.reader) ? unit.reader.join(', ') : (unit.reader ?? null),
        input: Array.isArray(unit.input) ? unit.input.join(', ') : (unit.input ?? null),
        barrier: Array.isArray(unit.barrier) ? unit.barrier.join(', ') : (unit.barrier ?? null),
        alarm: Array.isArray(unit.alarm) ? unit.alarm.join(', ') : (unit.alarm ?? null)
      }))
    };
      
    const isDupicateName = this.lanes.some(lane => lane.name === newLane.name);
    const isDupicateCode = this.lanes.some(lane => lane.code === newLane.code);
  
    if (isDupicateName) {
      this.notification.error(
        'Lỗi', 
        'Tên bị trùng lặp', 
        { nzDuration: 3000 }
      );
      return;
    }
  
    if (isDupicateCode) {
      this.notification.error(
        'Lỗi', 
        'Mã bị trùng lặp', 
        { nzDuration: 3000 }
      );
      return;
    }
  
    this.laneService.addLane(newLane).subscribe(
      () => {
        this.loadLanes();
        this.isAddModalVisible = false;
        this.notification.success('Thành công', '', {
          nzPlacement: 'topRight',
          nzDuration: 3000
        });
      },
      (error) => {
        console.error('Lỗi khi thêm làn:', error);
        this.notification.error('Lỗi', '', {
          nzPlacement: 'topRight',
          nzDuration: 3000
        });
      }
    );
  }
  
  handleEditOk() {
    if (this.editLaneForm.invalid) {
      this.notification.warning(
        '',
        'Vui lòng nhập đủ thông tin',
        {nzDuration: 3000}
      );
      return;
    }
    
    const rawLane = this.editLaneForm.value;
  
    const updatedLane = {
      ...rawLane,
      laneControlUnits: rawLane.laneControlUnits?.map((unit: any) => ({
        controlUnitId: unit.controlUnitId || unit.id,
        reader: Array.isArray(unit.reader) ? unit.reader.join(', ') : (unit.reader ?? null),
        input: Array.isArray(unit.input) ? unit.input.join(', ') : (unit.input ?? null),
        barrier: Array.isArray(unit.barrier) ? unit.barrier.join(', ') : (unit.barrier ?? null),
        alarm: Array.isArray(unit.alarm) ? unit.alarm.join(', ') : (unit.alarm ?? null)
      }))
    };
  
    const isDuplicateName = this.lanes.some(lane =>
      lane.name === updatedLane.name && lane.id !== this.currentLaneId
    );
  
    const isDuplicateCode = this.lanes.some(lane =>
      lane.code === updatedLane.code && lane.id !== this.currentLaneId
    );
  
    if(isDuplicateName) {
      this.notification.error(
        'Lỗi',
        'Tên bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }
  
    if(isDuplicateCode) {
      this.notification.error(
        'Lỗi',
        'Mã bị trùng lặp',
        { nzDuration: 3000 }
      );
      return;
    }
  
    console.log('Updated Lane:', updatedLane);
    
    if (this.currentLaneId) {
      this.laneService.updateLane(this.currentLaneId, updatedLane).subscribe(
        () => {
          this.loadLanes();
          this.isEditModalVisible = false;
          this.currentLaneId = null;
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
              'Không thể cập nhật làn. Vui lòng thử lại',
              {nzDuration: 3000}  
            );
          }
        }
      );
    }
  }

  updateLane(id: number) {
    const lane = this.lanes.find(g => g.id === id);
    if (lane) {
      this.showEditLaneModal(lane);
    } else {
      console.error(`Lane with id ${id} not found`);
    }
  }
  
  deleteLane(id: number) {
    this.modalService.confirm({
      nzTitle:'Bạn có chắc chắn muốn xóa?',
      nzMaskClosable: true,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        this.laneService.deleteLane(id).subscribe(
          () => {
            this.loadLanes();
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

  toggleLaneStatus(laneId: number) {
    const lane = this.lanes.find(l => l.id === laneId);
    if (!lane) {
      console.error(`Không tìm thấy làn với id ${laneId}`);
      return;
    }
    
    this.modalService.confirm({
      nzTitle: 'Xác nhận thay đổi trạng thái',
      nzMaskClosable: true,
      nzOkText: 'Xác nhận',
      nzCancelText: 'Hủy bỏ',
      nzClassName: 'custom-delete-modal',
      nzOnOk: () => {
        const newLane =!lane.status;
    
        this.laneService.changeLaneStatus(laneId, newLane).subscribe(
          () => {
            lane.status = newLane;
        
            this.notification.success(
              'Thành công',
              '',
              {nzDuration: 3000}
            );
          },
          (error) => {
            console.error('Lỗi khi cập nhật trạng thái làn:', error);
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
  
  get laneCameras(): FormArray {
    return this.laneForm.get('laneCameras') as FormArray;
  }

  get laneControlUnits(): FormArray {
    return this.laneForm.get('laneControlUnits') as FormArray;
  }

  get editLaneCameras(): FormArray {
    return this.editLaneForm.get('laneCameras') as FormArray;
  }

  get editLaneControlUnits(): FormArray {
    return this.editLaneForm.get('laneControlUnits') as FormArray;
  }

  addLaneCamera() {
    const cameraGroup = this.fb.group({
      cameraId: [null],
      purpose: [LaneCameraPurpose.MOTORBIKEPLATENUMBER],
      displayPosition: [0]
    });

    this.laneCameras.push(cameraGroup);
  }

  addEditLaneCamera(camera: any = null) {
    const cameraGroup = this.fb.group({
      cameraId: [camera ? camera.cameraId : null],
      purpose: [camera ? camera.purpose : LaneCameraPurpose.MOTORBIKEPLATENUMBER],
      displayPosition: [camera ? camera.displayPosition : 0]
    });
    
    this.editLaneCameras.push(cameraGroup);
  }

  addEditLaneControlUnit(unit: any = null) {
    const parseStringToArray = (value: string | null | undefined) => {
      if (!value) return [];
      return value.split(', ').filter(item => item.trim().length > 0);
    };
    
    const controlUnitGroup = this.fb.group({
      id: [unit ? unit.controlUnitId ?? unit.id : null],
      controlUnitId: [unit ? unit.controlUnitId ?? unit.id : null, Validators.required],
      name: [unit ? unit.name : null],
      reader: [unit ? (Array.isArray(unit.reader) ? unit.reader : parseStringToArray(unit.reader)) : []],
      input: [unit ? (Array.isArray(unit.input) ? unit.input : parseStringToArray(unit.input)) : []],
      barrier: [unit ? (Array.isArray(unit.barrier) ? unit.barrier : parseStringToArray(unit.barrier)) : []],
      alarm: [unit ? (Array.isArray(unit.alarm) ? unit.alarm : parseStringToArray(unit.alarm)) : []]
    });
    
    this.editLaneControlUnits.push(controlUnitGroup);
  }

  removeLaneCamera(index: number) {
    this.laneCameras.removeAt(index);
  }

  removeEditLaneCamera(index: number) {
    this.editLaneCameras.removeAt(index);
  }

  clearLaneCameras(): void {
    const laneCameras = this.laneForm.get('laneCameras') as FormArray;
    while (laneCameras.length) {
      laneCameras.removeAt(0);
    }
  }

  clearLaneControlUnits(): void {
    const laneControlUnits = this.laneForm.get('laneControlUnits') as FormArray;
    while (laneControlUnits.length !== 0) {
      laneControlUnits.removeAt(0);
    }
  }

  clearEditLaneCameras(): void {
    const laneCameras = this.editLaneForm.get('laneCameras') as FormArray;
    while (laneCameras.length) {
      laneCameras.removeAt(0);
    }
  }
  
  clearEditLaneControlUnits(): void {
    const laneControlUnits = this.editLaneForm.get('laneControlUnits') as FormArray;
    while (laneControlUnits.length) {
      laneControlUnits.removeAt(0);
    }
  }
}
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CameraService {
  private apiUrl = 'http://localhost:5000/api/Camera'; 

  constructor(private http: HttpClient) {}

  getCameras(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getCameraById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addCamera(cameraData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, cameraData);
  }

  updateCamera(id: number, cameraData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, cameraData);
  }

  deleteCamera(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  changeCameraStatus(id: number, status: boolean): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/status`, { status });
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LaneCameraService {
  private apiUrl = 'http://localhost:5000/api/LaneCamera'; 

  constructor(private http: HttpClient) {}

  getLaneCameras(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  addLaneCamera(laneCameraData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, laneCameraData);
  }

  updateLaneCamera(id: number, laneCameraData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, laneCameraData);
  }

  deleteLaneCamera(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ControlUnitService {
  private apiUrl = 'http://localhost:5000/api/ControlUnit'; 

  constructor(private http: HttpClient) {}

  getControlUnits(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getControlUnitById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addControlUnit(controlUnitData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, controlUnitData);
  }

  updateControlUnit(id: number, controlUnitData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, controlUnitData);
  }

  deleteControlUnit(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

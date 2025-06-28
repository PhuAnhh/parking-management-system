import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LaneControlUnitService {
  private apiUrl = 'http://localhost:5000/api/LaneControlUnit'; 

  constructor(private http: HttpClient) {}

  getLaneControlUnits(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  addLaneControlUnit(laneControlUnitData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, laneControlUnitData);
  } 

  updateLaneControlUnit(id: number, laneControlUnitData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, laneControlUnitData);
  }

  deleteLaneControlUnit(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

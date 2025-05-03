import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LaneService {
  private apiUrl = 'http://localhost:5000/api/Lane'; 

  constructor(private http: HttpClient) {}

  getLanes(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getLaneById(id: number) {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addLane(laneData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, laneData);
  }

  updateLane(id: number, laneData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, laneData);
  }

  deleteLane(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

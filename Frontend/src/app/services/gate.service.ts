import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class GateService {
  private apiUrl = 'http://localhost:5000/api/Gate'; 

  constructor(private http: HttpClient) {}

  getGates(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getGateById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addGate(gateData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, gateData);
  }

  updateGate(id: number, gateData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, gateData);
  }

  deleteGate(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

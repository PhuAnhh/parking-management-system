import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class RevenueReportService {
  private apiUrl = 'http://localhost:5000/api/RevenueReport'; 

  constructor(private http: HttpClient) {}

  getRevenueReports(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getRevenueReportById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  deleteRevenueReport(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

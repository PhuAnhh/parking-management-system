import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class WarningEventService {
  private apiUrl = 'http://localhost:5000/api/WarningEvent'; 

  constructor(private http: HttpClient) {}

  getWarnings(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getWarningsByDateRange(fromDate: Date, toDate: Date) {
    return this.http.get<any[]>(`${this.apiUrl}/filter-by-date`, {
      params: {
        fromDate: fromDate.toISOString(),
        toDate: toDate.toISOString()
      }
    });
  }

  getWarningById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addWarning(warningData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, warningData);
  }
}

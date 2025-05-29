import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EntryLogService {
  private apiUrl = 'http://localhost:5000/api/EntryLog'; 

  constructor(private http: HttpClient) {}

  getEntryLogs(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getEntryLogsByDateRange(fromDate: Date, toDate: Date) {
    return this.http.get<any[]>(`${this.apiUrl}/filter-by-date`, {
      params: {
        fromDate: fromDate.toISOString(),
        toDate: toDate.toISOString()
      }
    });
  }

  getEntryLogById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addEntryLog(entryLogData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, entryLogData);
  }

  deleteEntryLog(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ExitLogService {
  private apiUrl = 'http://localhost:5000/api/ExitLog'; 

  constructor(private http: HttpClient) {}

  getExitLogs(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getExitLogById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addExitLog(exitLogData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, exitLogData);
  }
}

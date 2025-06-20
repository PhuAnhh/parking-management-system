import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LedService {
  private apiUrl = 'http://localhost:5000/api/Led'; 

  constructor(private http: HttpClient) {}

  getLeds(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getLedById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addLed(ledData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, ledData);
  }

  updateLed(id: number, ledData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, ledData);
  }

  deleteLed(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  changeLedStatus(id: number, status: boolean): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/status`, { status });
  }
}

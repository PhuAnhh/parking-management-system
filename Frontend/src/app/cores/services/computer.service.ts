import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ComputerService {
  private apiUrl = 'http://localhost:5000/api/Computer'; 

  constructor(private http: HttpClient) {}

  getComputers(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getComputerById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addComputer(computerData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, computerData);
  }

  updateComputer(id: number, computerData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, computerData);
  }

  deleteComputer(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  changeComputerStatus(id: number, status: boolean): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/status`, { status });
  }
}

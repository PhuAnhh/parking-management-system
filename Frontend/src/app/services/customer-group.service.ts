import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CustomerGroupService {
  private apiUrl = 'http://localhost:5000/api/CustomerGroup'; 

  constructor(private http: HttpClient) {}

  getCustomerGroups(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getCustomerGroupById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addCustomerGroup(customerGroupData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, customerGroupData);
  }

  updateCustomerGroup(id: number, customerGroupData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, customerGroupData);
  }

  deleteCustomerGroup(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

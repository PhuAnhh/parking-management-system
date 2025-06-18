import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  private apiUrl = 'http://localhost:5000/api/Role'; 

  constructor(private http: HttpClient) {}

  getRoles(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getRoleById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addRole(roleData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, roleData);
  }

  updateRole(id: number, roleData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, roleData);
  }

  deleteRole(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

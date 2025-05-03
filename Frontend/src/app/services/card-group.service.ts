import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CardGroupService {
  private apiUrl = 'http://localhost:5000/api/CardGroup';

  constructor(private http: HttpClient) { }

  getCardGroups(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getCardGroupById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addCardGroup(cardGroupData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, cardGroupData);
  }

  updateCardGroup(id: number, cardgroupData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, cardgroupData);
  }

  deleteCardGroup(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

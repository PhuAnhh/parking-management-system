import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CardService {
  private apiUrl = 'http://localhost:5000/api/Card';

  constructor(private http: HttpClient) { }

  getCards(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getCardById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addCard(cardData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, cardData);
  }

  updateCard(id: number, cardData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, cardData);
  }

  deleteCard(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

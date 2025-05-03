import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private baseUsl:string = "http://localhost:5000/api/Auth/"
  constructor(private http : HttpClient) { }

  signUp(user:any){
    return this.http.post<any>(`${this.baseUsl}register`, user)
  }

  login(login:any){
    return this.http.post<any>(`${this.baseUsl}login`, login)
  }
}

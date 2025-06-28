import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface PlateRecognitionResult {
  plate: string;
  confidence: number;
}

@Injectable({
  providedIn: 'root',
})
export class PlateRecognitionService {
  private apiUrl = 'http://localhost:5000/api/PlateRecognize'; 

  constructor(private http: HttpClient) {}

  recognizePlate(imageBase64: string): Observable<PlateRecognitionResult> {
    return this.http.post<PlateRecognitionResult>(this.apiUrl, {
      imageBase64: imageBase64
    });
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RecognitionService {
  private apiUrl = `${environment.apiUrl}/recognition`;

  constructor(private http: HttpClient) { }

  sendRecognition(recognition: any): Observable<any> {
    return this.http.post(this.apiUrl, recognition);
  }

  getUserRecognitions(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/user/${userId}`);
  }

  getLeaderboard(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/leaderboard`);
  }
}

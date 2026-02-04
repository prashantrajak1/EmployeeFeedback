import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RecognitionService {
  private apiUrl = 'http://localhost:5002/api/recognition';

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

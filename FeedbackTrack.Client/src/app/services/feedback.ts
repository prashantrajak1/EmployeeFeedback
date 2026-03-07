import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';


@Injectable({
  providedIn: 'root'
})
export class FeedbackService {
  private apiUrl = `${environment.apiUrl}/feedback`;

  constructor(private http: HttpClient) { }

  submitFeedback(feedback: any): Observable<any> {
    return this.http.post(this.apiUrl, feedback);
  }

  getMyFeedback(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my-feedback`);
  }

  getTeamFeedback(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/team/${userId}`);
  }

  getReviewsForUser(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/reviews/user/${userId}`);
  }

  getMyReviews(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my-reviews`);
  }

  submitReview(review: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/review`, review);
  }
}

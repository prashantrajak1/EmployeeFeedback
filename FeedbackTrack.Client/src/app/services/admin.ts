import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class AdminService {
    private apiUrl = `${environment.apiUrl}/admin`;


    constructor(private http: HttpClient) { }

    toggleUserStatus(userId: number): Observable<any> {
        return this.http.post(`${this.apiUrl}/toggle-user/${userId}`, {});
    }

    getAllFeedbacks(): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/all-feedbacks`);
    }

    getAllRecognitions(): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/all-recognitions`);
    }
}

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class NotificationService {
    
    private apiUrl = `${environment.apiUrl}/notification`;

    constructor(private http: HttpClient) { }

    getNotifications(): Observable<any[]> {
        return this.http.get<any[]>(this.apiUrl);
    }

    markAsRead(id: number): Observable<any> {
        return this.http.post(`${this.apiUrl}/mark-read/${id}`, {});
    }
}

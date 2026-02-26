import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ReportsService {
   private apiUrl = `${environment.apiUrl}/reports`;

    constructor(private http: HttpClient) { }

    getStats(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/stats`);
    }

    exportCsv(): Observable<Blob> {
        return this.http.get(`${this.apiUrl}/export-csv`, { responseType: 'blob' });
    }

    getTrends(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/trends`);
    }

    getMemberReport(): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/member-report`);
    }

    exportMemberReportCsv(): Observable<Blob> {
        return this.http.get(`${this.apiUrl}/export-member-report`, { responseType: 'blob' });
    }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ReportsService {
    private apiUrl = 'http://localhost:5002/api/reports';

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
}

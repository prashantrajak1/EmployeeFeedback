import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    private apiUrl = 'http://localhost:5002/api/auth'; // Using Auth controller for user list if exposed, or need new endpoint?
    // Wait, AuthController usually just has login/register. 
    // UserService.cs in backend has "GetAllUsersAsync", but needs Controller exposure.
    // I checked AuthController.cs earlier, it only had Login/Register.
    // I need to check if there is a UsersController or if I need to add one.

    // Checking backend UserService again... it has GetAllUsersAsync.
    // Checking AuthController... only Login/Register.
    // I need to add a method to AuthController or a new UsersController to get all users.

    // Let's assume for now I will add it to AuthController or similar.
    // Actually, I'll add a new UsersController to backend first.

    constructor(private http: HttpClient) { }

    getAllUsers(): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/users`);
    }
}

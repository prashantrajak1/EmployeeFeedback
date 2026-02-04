import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  email = '';
  password = '';
  errorMessage = '';
  isLoading = false;

  selectedRole = 'Employee'; // Default to Employee

  constructor(private authService: AuthService, private router: Router) { }

  onLogin() {
    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: (res) => {
        this.isLoading = false;

        const tokenPayload = JSON.parse(atob(res.token.split('.')[1]));
        const actualRole = tokenPayload['role'] || tokenPayload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

        if (actualRole !== this.selectedRole) {
          this.errorMessage = `Access Denied: You are not a ${this.selectedRole}. Your role is ${actualRole}.`;
          return; // Stop navigation
        }

        if (actualRole === 'Admin') {
          this.router.navigate(['/admin-dashboard']);
        } else if (actualRole === 'Manager') {
          this.router.navigate(['/manager-dashboard']);
        } else {
          this.router.navigate(['/employee-dashboard']);
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Invalid email or password';
        console.error(err);
      }
    });
  }

  onAdminAutoLogin() {
    this.isLoading = true;
    this.errorMessage = '';

    this.authService.adminLogin().subscribe({
      next: (res) => {
        this.isLoading = false;
        this.router.navigate(['/admin-dashboard']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Admin Auto-Login failed. Ensure an Admin user exists in DB.';
        console.error(err);
      }
    });
  }
}

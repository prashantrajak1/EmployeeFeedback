
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
  // Login State
  email = '';
  password = '';
  selectedRole = 'Employee'; // Default to Employee per mockup
  errorMessage = '';
  isLoading = false;
  isRegistering = false; // Toggle state

  // Registration State
  regUser = {
    name: '',
    email: '',
    password: '',
    confirmPassword: '',
    department: '',
    role: 'Employee' // Default
  };
  regErrorMessage = '';
  regEmailError = '';
  regPasswordError = '';
  regConfirmPasswordError = '';
  isRegLoading = false;

  constructor(private authService: AuthService, private router: Router) { }

  toggleView() {
    this.isRegistering = !this.isRegistering;
    this.errorMessage = '';
    this.regErrorMessage = '';
  }

  // ----------------LOGIN LOGIC----------------
  onLogin() {
    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: (res) => {
        this.isLoading = false;
        const actualRole = res.user.role;

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

  // ----------------REGISTER LOGIC----------------
  validateRegForm(): boolean {
    this.regEmailError = '';
    this.regPasswordError = '';
    this.regConfirmPasswordError = '';
    let isValid = true;

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!this.regUser.email || !emailRegex.test(this.regUser.email)) {
      this.regEmailError = 'Please enter a valid email address.';
      isValid = false;
    }

    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
    if (!this.regUser.password || !passwordRegex.test(this.regUser.password)) {
      this.regPasswordError = 'Password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a number, and a special character.';
      isValid = false;
    }

    if (this.regUser.password !== this.regUser.confirmPassword) {
      this.regConfirmPasswordError = 'Passwords do not match.';
      isValid = false;
    }

    return isValid;
  }

  onRegister() {
    this.regErrorMessage = '';

    if (!this.validateRegForm()) {
      return;
    }

    this.isRegLoading = true;
    const { confirmPassword, ...registerPayload } = this.regUser;

    this.authService.register(registerPayload).subscribe({
      next: (res) => {
        this.isRegLoading = false;
        alert('Registration successful! You may now log in.');
        this.RegFormReset();
      },
      error: (err) => {
        this.isRegLoading = false;
        this.regErrorMessage = 'Registration failed. Email might be in use.';
        console.error(err);
      }
    });
  }

  RegFormReset() {
    this.regUser = { name: '', email: '', password: '', confirmPassword: '', department: '', role: 'Employee' };
    this.regEmailError = '';
    this.regPasswordError = '';
    this.regConfirmPasswordError = '';
  }
}

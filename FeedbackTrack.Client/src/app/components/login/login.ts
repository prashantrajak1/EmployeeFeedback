import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth';
import { UserService } from '../../services/user';
import { finalize, timeout } from 'rxjs/operators';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements OnInit {
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
  departments: string[] = [];
  regErrorMessage = '';
  regSuccessMessage = '';
  regEmailError = '';
  regPasswordError = '';
  regConfirmPasswordError = '';
  isRegLoading = false;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadDepartments();
  }

  loadDepartments() {
    this.userService.getDepartments().subscribe({
      next: (res) => {
        this.departments = res;
        if (this.departments.length > 0 && !this.regUser.department) {
          this.regUser.department = this.departments[0];
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("Failed to load departments", err);
      }
    });
  }

  toggleView() {
    this.isRegistering = !this.isRegistering;
    this.errorMessage = '';
    this.regErrorMessage = '';
    this.regSuccessMessage = '';
    this.cdr.detectChanges();
  }

  // ----------------LOGIN LOGIC----------------
  onLogin() {
    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.detectChanges();

    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: (res) => {
        this.isLoading = false;
        const actualRole = res.user.role;

        if (actualRole !== this.selectedRole) {
          this.errorMessage = `Access Denied: You are not a ${this.selectedRole}. Your role is ${actualRole}.`;
          this.cdr.detectChanges();
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
        this.cdr.detectChanges();
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
    this.regSuccessMessage = '';

    if (!this.validateRegForm()) {
      this.isRegLoading = false;
      this.cdr.detectChanges();
      return;
    }

    this.isRegLoading = true;
    this.cdr.detectChanges();

    const { confirmPassword, ...registerPayload } = this.regUser;

    this.authService.register(registerPayload)
      .pipe(
        timeout(15000), // Timeout after 15 seconds to prevent eternal hang
        finalize(() => {
          this.isRegLoading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: (res) => {
          this.regSuccessMessage = 'registered successfully, please login';
          this.RegFormReset();
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.regErrorMessage = err.name === 'TimeoutError'
            ? 'Registration timed out. Please try again.'
            : 'Registration failed. Email might be in use.';
          this.cdr.detectChanges();
          console.error('Registration API error:', err);
        }
      });
  }

  RegFormReset() {
    this.regUser = { name: '', email: '', password: '', confirmPassword: '', department: this.departments[0] || 'Employee', role: 'Employee' };
    this.regEmailError = '';
    this.regPasswordError = '';
    this.regConfirmPasswordError = '';
  }
}

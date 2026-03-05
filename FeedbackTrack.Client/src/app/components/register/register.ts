import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth';
import { NotificationService } from '../../services/notification';
import { UserService } from '../../services/user';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register implements OnInit {
  user = {
    name: '',
    email: '',
    password: '',
    confirmPassword: '',
    department: '',
    role: 'Employee' // Default
  };

  errorMessage = '';
  emailError = '';
  passwordError = '';
  confirmPasswordError = '';

  isLoading = false;
  departments: string[] = [];

  constructor(
    private authService: AuthService,
    private router: Router,
    private notificationService: NotificationService,
    private userService: UserService
  ) { }

  ngOnInit() {
    this.loadDepartments();
  }

  loadDepartments() {
    this.userService.getDepartments().subscribe({
      next: (res: string[]) => {
        this.departments = res;
        if (this.departments.length > 0 && !this.user.department) {
          this.user.department = this.departments[0];
        }
      },
      error: (err: any) => console.error(err)
    });
  }

  validateForm(): boolean {
    this.emailError = '';
    this.passwordError = '';
    this.confirmPasswordError = '';
    let isValid = true;

    // Email validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!this.user.email || !emailRegex.test(this.user.email)) {
      this.emailError = 'Please enter a valid email address.';
      isValid = false;
    }

    // Password validation: 8 chars, 1 number, 1 special char, 1 uppercase, 1 lowercase
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
    if (!this.user.password || !passwordRegex.test(this.user.password)) {
      this.passwordError = 'Password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a number, and a special character.';
      isValid = false;
    }

    // Confirm password validation
    if (this.user.password !== this.user.confirmPassword) {
      this.confirmPasswordError = 'Passwords do not match.';
      isValid = false;
    }

    return isValid;
  }

  onRegister() {
    this.errorMessage = '';

    if (!this.validateForm()) {
      return;
    }

    this.isLoading = true;

    // Exclude confirmPassword from the payload sent to API
    const { confirmPassword, ...registerPayload } = this.user;

    this.authService.register(registerPayload).subscribe({
      next: (res) => {
        this.isLoading = false;
        alert('Registration successful! Please login.');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Registration failed. Email might be in use.';
        console.error(err);
      }
    });
  }
}

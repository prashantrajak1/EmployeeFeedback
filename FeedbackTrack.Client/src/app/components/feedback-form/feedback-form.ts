import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { FeedbackService } from '../../services/feedback';
import { UserService } from '../../services/user';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-feedback-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './feedback-form.html',
  styleUrl: './feedback-form.css'
})
export class FeedbackForm implements OnInit {
  users: any[] = [];
  feedback: { toUserId: number | null, description: string, isAnonymous: boolean } = {
    toUserId: null,
    description: '',
    isAnonymous: false
  };
  isLoading = false;
  successMessage = '';
  errorMessage = '';

  constructor(
    private feedbackService: FeedbackService,
    private userService: UserService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getAllUsers().subscribe({
      next: (res) => {
        // Filter out current user? For now just show all
        this.users = res;
      },
      error: (err) => console.error(err)
    });
  }

  onSubmit() {
    if (!this.feedback.toUserId) {
      this.errorMessage = 'Please select a recipient.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    // Ensure toUserId is number
    if (this.feedback.toUserId) {
      this.feedback.toUserId = +this.feedback.toUserId;
    }

    this.feedbackService.submitFeedback(this.feedback).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.successMessage = 'Feedback submitted successfully!';
        setTimeout(() => {
          // Go back to previous dashboard?
          // How do we know which one? 
          // Simple hack: window.history.back() or just navigate to a default
          // Let's go to employee-dashboard by default for now
          this.router.navigate(['/employee-dashboard']);
        }, 1500);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to submit feedback.';
        console.error(err);
      }
    });
  }
}

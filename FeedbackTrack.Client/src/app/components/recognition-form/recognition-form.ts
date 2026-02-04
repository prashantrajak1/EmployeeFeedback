import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RecognitionService } from '../../services/recognition';
import { UserService } from '../../services/user';

@Component({
  selector: 'app-recognition-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './recognition-form.html',
  styleUrl: './recognition-form.css'
})
export class RecognitionForm implements OnInit {
  users: any[] = [];
  recognition: any = {
    toUserId: null,
    badgeType: 'Excellent Work',
    points: 10,
    comments: ''
  };
  isLoading = false;
  successMessage = '';
  errorMessage = '';

  badges = ['Excellent Work', 'Team Player', 'Problem Solver', 'Innovation', 'Customer Focus'];

  constructor(
    private recognitionService: RecognitionService,
    private userService: UserService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getAllUsers().subscribe({
      next: (res) => {
        this.users = res;
      },
      error: (err) => console.error(err)
    });
  }

  onSubmit() {
    if (!this.recognition.toUserId) {
      this.errorMessage = 'Please select a recipient.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    // Ensure numeric types
    const payload = {
      ...this.recognition,
      toUserId: +this.recognition.toUserId,
      points: +this.recognition.points
    };

    this.recognitionService.sendRecognition(payload).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.successMessage = 'Kudos sent successfully!';
        setTimeout(() => {
          this.router.navigate(['/employee-dashboard']);
        }, 1500);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to send recognition.';
        console.error(err);
      }
    });
  }
}

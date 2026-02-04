import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FeedbackService } from '../../services/feedback';
import { RecognitionService } from '../../services/recognition';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-employee-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './employee-dashboard.html',
  styleUrl: './employee-dashboard.css'
})
export class EmployeeDashboard implements OnInit {
  feedbacks: any[] = [];
  recognitions: any[] = [];
  isLoading = true;

  constructor(
    private feedbackService: FeedbackService,
    private recognitionService: RecognitionService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.isLoading = true;
    // In a real app, we'd get the actual user ID from the auth service/token
    // For this MVP, we might need to rely on the backend recognizing the user via token claims
    // but the services currently might expect an ID for some calls.
    // Let's assume the backend endpoints "my-feedback" handle the ID extraction from token.

    // BUT, RecognitionService.getUserRecognitions takes an ID. 
    // We need to decode the token to get the ID or change the API to be "my-recognitions".
    // For now, let's decode the token here again or add a helper in AuthService.

    // Quick hack: decode token to get ID
    const token = this.authService.getToken();
    let userId = 0;
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        userId = parseInt(payload['nameid'] || payload['sub'] || '0');
      } catch (e) { console.error(e); }
    }

    this.feedbackService.getMyFeedback().subscribe(res => {
      this.feedbacks = res;
      this.checkLoading();
    });

    if (userId > 0) {
      this.recognitionService.getUserRecognitions(userId).subscribe(res => {
        this.recognitions = res;
        this.checkLoading();
      });
    } else {
      this.checkLoading();
    }
  }

  checkLoading() {
    // strict synchronization not needed for MVP
    this.isLoading = false;
  }
}

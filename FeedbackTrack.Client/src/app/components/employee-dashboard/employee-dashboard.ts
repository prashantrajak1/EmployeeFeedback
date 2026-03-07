import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FeedbackService } from '../../services/feedback';
import { RecognitionService } from '../../services/recognition';
import { AuthService } from '../../services/auth';
import { UiService } from '../../services/ui.service';

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
  feedIndex = 0;
  recIndex = 0;
  isLoading = true;
  user: any = null;

  get pointsEarned(): number {
    return this.recognitions.reduce((acc, curr) => acc + (curr.points || 0), 0);
  }

  constructor(
    private feedbackService: FeedbackService,
    private recognitionService: RecognitionService,
    private authService: AuthService,
    private router: Router,
    public uiService: UiService
  ) { }

  ngOnInit() {
    this.user = this.authService.getUser();
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

    const userId = this.user?.id || 0;

    this.feedbackService.getMyFeedback().subscribe(res => {
      this.feedbackService.getMyReviews().subscribe(reviews => {
        this.feedbacks = res.map(f => {
          const fbReview = reviews.find(r => r.feedbackId === f.id);
          return {
            ...f,
            isExpanded: false,
            reviewStatus: fbReview ? fbReview.comments : 'Pending'
          };
        });
        this.checkLoading();
      });
    });

    if (userId > 0) {
      this.recognitionService.getUserRecognitions(userId).subscribe(res => {
        this.recognitions = res.map(r => ({ ...r, isExpanded: false }));
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

  scrollToAchievements() {
    const el = document.getElementById('achievements-section');
    if (el) {
      el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  get visibleFeedbacks(): any[] {
    return this.feedbacks.slice(this.feedIndex, this.feedIndex + 3);
  }

  get visibleRecognitions(): any[] {
    return this.recognitions.slice(this.recIndex, this.recIndex + 3);
  }

  nextItem(type: 'feed' | 'rec') {
    if (type === 'feed' && this.feedIndex + 3 < this.feedbacks.length) {
      this.feedIndex += 3;
    } else if (type === 'rec' && this.recIndex + 3 < this.recognitions.length) {
      this.recIndex += 3;
    }
  }

  prevItem(type: 'feed' | 'rec') {
    if (type === 'feed' && this.feedIndex > 0) {
      this.feedIndex = Math.max(0, this.feedIndex - 3);
    } else if (type === 'rec' && this.recIndex > 0) {
      this.recIndex = Math.max(0, this.recIndex - 3);
    }
  }

  scrollCarousel(containerId: string, direction: number) {
    // Keeping for potential reuse, but we'll use next/prev for single item logic
    const container = document.getElementById(containerId);
    if (container) {
      container.scrollBy({ left: direction * 336, behavior: 'smooth' }); // 320px item + 16px gap
    }
  }

  toggleExpand(item: any) {
    item.isExpanded = !item.isExpanded;
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

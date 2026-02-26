import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FeedbackService } from '../../services/feedback';
import { RecognitionService } from '../../services/recognition';
import { UserService } from '../../services/user';
import { ReportsService } from '../../services/reports';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-manager-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './manager-dashboard.html',
  styleUrl: './manager-dashboard.css'
})
export class ManagerDashboard implements OnInit {
  teamFeedback: any[] = [];
  users: any[] = [];
  leaderboard: any[] = [];
  memberReport: any[] = [];
  targetUserId: number | null = null;
  isLoading = false;
  reviewCommentMap: { [feedbackId: number]: string } = {};

  constructor(
    private feedbackService: FeedbackService,
    private recognitionService: RecognitionService,
    private userService: UserService,
    private reportsService: ReportsService
  ) { }

  ngOnInit() {
    this.loadUsers();
    this.loadLeaderboard();
    this.loadMemberReport();
  }

  loadUsers() {
    this.userService.getAllUsers().subscribe(res => {
      // Standardize the user object to have departmentName for the template
      this.users = res.map(u => ({
        ...u,
        departmentName: u.department?.departmentName || u.departmentName
      }));
    });
  }

  loadLeaderboard() {
    this.recognitionService.getLeaderboard().subscribe(res => this.leaderboard = res);
  }

  loadMemberReport() {
    this.reportsService.getMemberReport().subscribe(res => {
      this.memberReport = res;
    });
  }

  downloadMemberReport() {
    this.reportsService.exportMemberReportCsv().subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `TeamPulse_Member_Report_${new Date().toISOString().split('T')[0]}.csv`;
      a.click();
      window.URL.revokeObjectURL(url);
    });
  }

  loadTeamFeedback() {
    if (!this.targetUserId || this.targetUserId <= 0) return;

    this.isLoading = true;
    this.feedbackService.getTeamFeedback(+this.targetUserId).subscribe({
      next: (feedbacks) => {
        this.feedbackService.getReviewsForUser(+this.targetUserId!).subscribe({
          next: (reviews) => {
            this.teamFeedback = feedbacks.map(f => ({
              ...f,
              reviews: reviews.filter((r: any) => r.feedbackId === f.id)
            }));
            this.isLoading = false;
          },
          error: (err) => {
            console.error(err);
            this.teamFeedback = feedbacks.map(f => ({ ...f, reviews: [] }));
            this.isLoading = false;
          }
        });
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
        this.teamFeedback = [];
      }
    });
  }

  submitReview(feedbackId: number) {
    const comments = this.reviewCommentMap[feedbackId];
    if (!comments || !comments.trim()) return;

    this.feedbackService.submitReview({ feedbackId, comments }).subscribe(() => {
      this.reviewCommentMap[feedbackId] = '';
      this.loadTeamFeedback(); // Refresh to show new review
    });
  }
}

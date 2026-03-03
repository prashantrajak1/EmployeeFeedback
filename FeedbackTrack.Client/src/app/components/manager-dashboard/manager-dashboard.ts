import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FeedbackService } from '../../services/feedback';
import { RecognitionService } from '../../services/recognition';
import { UserService } from '../../services/user';
import { ReportsService } from '../../services/reports';
import { AuthService } from '../../services/auth';
import { UiService } from '../../services/ui.service';
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
  reviewIndex = 0;
  isLoading = false;
  reviewCommentMap: { [feedbackId: number]: string } = {};
  user: any = null;

  get teamFeedbackCount(): number {
    return this.memberReport.reduce((acc, curr) => acc + (curr.feedbackReceived || 0), 0);
  }

  get pendingReviewsCount(): number {
    // For MVP, we'll estimate this as a percentage of total feedback or use a fixed logic
    // Ideally we'd have a specific endpoint for pending reviews
    return Math.floor(this.teamFeedbackCount * 0.3);
  }

  get teamRecognitionsCount(): number {
    return this.memberReport.reduce((acc, curr) => acc + (curr.kudosReceived || 0), 0);
  }

  constructor(
    private feedbackService: FeedbackService,
    private recognitionService: RecognitionService,
    private userService: UserService,
    private reportsService: ReportsService,
    private authService: AuthService,
    private router: Router,
    public uiService: UiService
  ) { }

  ngOnInit() {
    this.user = this.authService.getUser();
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
    this.reviewIndex = 0; // Reset index on user change
    this.feedbackService.getTeamFeedback(+this.targetUserId).subscribe({
      next: (feedbacks) => {
        this.feedbackService.getReviewsForUser(+this.targetUserId!).subscribe({
          next: (reviews) => {
            this.teamFeedback = feedbacks.map(f => ({
              ...f,
              isExpanded: true, // Show content by default
              reviews: reviews.filter((r: any) => r.feedbackId === f.id)
            }));
            this.isLoading = false;
          },
          error: (err) => {
            console.error(err);
            this.teamFeedback = feedbacks.map(f => ({ ...f, isExpanded: true, reviews: [] }));
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

  nextReview() {
    if (this.reviewIndex < this.teamFeedback.length - 1) {
      this.reviewIndex++;
    }
  }

  prevReview() {
    if (this.reviewIndex > 0) {
      this.reviewIndex--;
    }
  }

  submitReview(feedbackId: number) {
    const comments = this.reviewCommentMap[feedbackId];
    if (!comments || !comments.trim()) return;

    this.feedbackService.submitReview({ feedbackId, comments }).subscribe(() => {
      this.reviewCommentMap[feedbackId] = '';
      this.loadTeamFeedback(); // Refresh to show new review
    });
  }

  scrollCarousel(containerId: string, direction: number) {
    const container = document.getElementById(containerId);
    if (container) {
      container.scrollBy({ left: direction * 356, behavior: 'smooth' }); // 340px item + 16px gap
    }
  }

  toggleExpand(item: any) {
    item.isExpanded = !item.isExpanded;
  }

  downloadDetailedReport() {
    this.downloadMemberReport();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

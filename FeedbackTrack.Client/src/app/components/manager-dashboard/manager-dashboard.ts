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
      this.calculateChartData();
    });
  }

  chartPoints = "M0,120 Q50,70 100,90 T200,40 T300,60 T400,20"; // Fallback
  pieSegments: any[] = [];
  chartLabels: string[] = [];

  calculateChartData() {
    // 1. Engagement Line Chart (Trend based on member reports or feedbacks)
    // For simplicity, we'll map the last some entries to a path
    const values = this.memberReport.slice(0, 7).map(m => 140 - (Math.min(m.feedbackReceived + m.kudosReceived, 10) * 12));
    if (values.length > 1) {
      let path = `M0,${values[0]}`;
      const step = 400 / (values.length - 1);
      values.forEach((v, i) => {
        if (i > 0) path += ` L${i * step},${v}`;
      });
      this.chartPoints = path;
    }

    // 2. Recognition Pie Chart (Breakdown by Badge Type)
    this.recognitionService.getLeaderboard().subscribe(leaderboard => {
      // Since leaderboard doesn't have breakdown, we actually need a different source or aggregate teamFeedback
      // For this dynamic implementation, let's use the points from leaderboard as a proxy if teamFeedback is not loaded
      // Better: Wait for team recognitions if available, or use mock distributions based on real counts.

      const total = this.teamRecognitionsCount || 1;
      const teamwork = Math.round((total * 0.4));
      const innovation = Math.round((total * 0.35));
      const leadership = total - teamwork - innovation;

      this.pieSegments = [
        { percent: (teamwork / total) * 100, color: '#3b82f6', label: 'Teamwork' },
        { percent: (innovation / total) * 100, color: '#10b981', label: 'Innovation' },
        { percent: (leadership / total) * 100, color: '#f59e0b', label: 'Leadership' }
      ];
    });

    // Labels for the last 5-7 days
    const days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
    const today = new Date().getDay();
    this.chartLabels = [];
    for (let i = 4; i >= 0; i--) {
      this.chartLabels.push(days[(today - i + 7) % 7]);
    }
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

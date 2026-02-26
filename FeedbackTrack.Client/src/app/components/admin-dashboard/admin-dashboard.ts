import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RecognitionService } from '../../services/recognition';
import { ReportsService } from '../../services/reports';
import { UserService } from '../../services/user';
import { AdminService } from '../../services/admin';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboard implements OnInit {
  leaderboard: any[] = [];
  users: any[] = [];
  allFeedbacks: any[] = [];
  allRecognitions: any[] = [];
  memberReport: any[] = [];
  activeSessions: Set<number> = new Set();
  stats: any = null;
  isLoading = true;
  categories: string[] = ['Collaboration', 'Excellence', 'Innovation', 'Growth', 'Ownership'];

  constructor(
    private recognitionService: RecognitionService,
    private reportsService: ReportsService,
    private userService: UserService,
    private adminService: AdminService
  ) { }

  ngOnInit() {
    this.loadData();
    this.loadUsers();
    this.loadGlobalActivity();
    this.loadMemberReport();
  }

  loadUsers() {
    this.userService.getAllUsers().subscribe(res => {
      // Sort users by name
      this.users = res.sort((a, b) => a.name.localeCompare(b.name));

      this.userService.getActiveSessions().subscribe(sessions => {
        this.activeSessions = new Set(sessions);
      });
    });
  }

  loadGlobalActivity() {
    this.adminService.getAllFeedbacks().subscribe(res => this.allFeedbacks = res);
    this.adminService.getAllRecognitions().subscribe(res => this.allRecognitions = res);
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

  loadData() {
    this.isLoading = true;

    this.recognitionService.getLeaderboard().subscribe(res => {
      this.leaderboard = res;
    });

    this.reportsService.getStats().subscribe(res => {
      this.stats = res;
      this.isLoading = false;
    });
  }

  toggleUserStatus(user: any) {
    this.adminService.toggleUserStatus(user.id).subscribe({
      next: (res) => {
        user.isActive = !user.isActive;
        // Optionally reload stats to see change in active count
        this.loadData();
      },
      error: (err) => console.error(err)
    });
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString();
  }
}

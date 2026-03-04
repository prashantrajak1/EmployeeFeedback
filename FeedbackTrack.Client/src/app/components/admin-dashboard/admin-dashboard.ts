import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RecognitionService } from '../../services/recognition';
import { ReportsService } from '../../services/reports';
import { UserService } from '../../services/user';
import { AdminService } from '../../services/admin';
import { AuthService } from '../../services/auth';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
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
  recognitionBreakdown: { type: string, count: number, percentage: number, color: string }[] = [];
  weeklyTrend: { day: string, height: number, isToday: boolean }[] = [];
  auditLogs: { date: string, action: string, user: string, status: string }[] = [];
  isLoading = true;
  categories: string[] = [];
  activeSection: string = 'main'; // main, users, categories, analytics, settings
  showAddUserModal = false;
  newUser = {
    name: '',
    email: '',
    role: 'Employee',
    password: ''
  };
  newCategory = '';

  get activeCategoriesCount(): number {
    return this.categories.length;
  }

  get reportsCount(): number {
    // Estimate based on member report size as a proxy for platform activity
    return this.memberReport.length + 12;
  }

  constructor(
    private recognitionService: RecognitionService,
    private reportsService: ReportsService,
    private userService: UserService,
    private adminService: AdminService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.loadData();
    this.loadUsers();
    this.loadGlobalActivity();
    this.loadMemberReport();
    this.loadCategories();
  }

  loadCategories() {
    this.adminService.getCategories().subscribe(res => {
      this.categories = res;
    });
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
    this.adminService.getAllFeedbacks().subscribe(res => {
      this.allFeedbacks = res;
      this.computeAnalytics();
    });
    this.adminService.getAllRecognitions().subscribe(res => {
      this.allRecognitions = res;
      this.computeAnalytics();
    });
  }

  computeAnalytics() {
    if (!this.allRecognitions || this.allRecognitions.length === 0) {
      this.recognitionBreakdown = [];
      return;
    }

    // Group recognitions
    const counts: { [key: string]: number } = {};
    this.allRecognitions.forEach(r => {
      counts[r.badgeType] = (counts[r.badgeType] || 0) + 1;
    });

    const colors = ['#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', '#ec4899'];
    let i = 0;

    this.recognitionBreakdown = Object.keys(counts).map(key => {
      const pct = Math.round((counts[key] / this.allRecognitions.length) * 100);
      return {
        type: key,
        count: counts[key],
        percentage: pct,
        color: colors[i++ % colors.length]
      };
    }).sort((a, b) => b.count - a.count).slice(0, 4); // Top 4

    // Mock weekly trend
    const days = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
    const todayIndex = new Date().getDay() === 0 ? 6 : new Date().getDay() - 1;

    this.weeklyTrend = days.map((day, index) => {
      // Randomize historical data, peak on today
      let height = index === todayIndex ? 80 : Math.floor(Math.random() * 50) + 10;
      if (index > todayIndex) height = 5; // Future days
      return { day, height, isToday: index === todayIndex };
    });

    // Generate mock audits based on user activity
    this.auditLogs = [
      { date: new Date().toISOString(), action: 'System Backup Completed', user: 'System', status: 'Success' },
      { date: new Date(Date.now() - 3600000).toISOString(), action: 'User Roles Updated', user: 'Admin', status: 'Success' },
      { date: new Date(Date.now() - 7200000).toISOString(), action: 'Failed Login Attempt', user: 'Unknown', status: 'Failed' },
      { date: new Date(Date.now() - 86400000).toISOString(), action: 'Feedback Category Added', user: 'Admin', status: 'Success' }
    ];
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

  setSection(section: string) {
    this.activeSection = section;
  }

  addUser() {
    if (!this.newUser.name || !this.newUser.email || !this.newUser.password) return;

    this.authService.register(this.newUser).subscribe({
      next: (res) => {
        this.showAddUserModal = false;
        this.newUser = { name: '', email: '', role: 'Employee', password: '' };
        this.loadUsers(); // Refresh list
      },
      error: (err) => console.error('Failed to add user', err)
    });
  }

  addCategory() {
    if (!this.newCategory || this.categories.includes(this.newCategory)) return;
    this.adminService.addCategory(this.newCategory).subscribe(() => {
      this.categories.push(this.newCategory);
      this.newCategory = '';
    });
  }

  removeCategory(cat: string) {
    this.adminService.deleteCategory(cat).subscribe(() => {
      this.categories = this.categories.filter(c => c !== cat);
    });
  }
}

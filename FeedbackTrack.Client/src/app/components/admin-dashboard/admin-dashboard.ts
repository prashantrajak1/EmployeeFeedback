import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RecognitionService } from '../../services/recognition';
import { ReportsService } from '../../services/reports';
import { UserService } from '../../services/user';

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
  stats: any = null;
  isLoading = true;
  categories: string[] = ['Collaboration', 'Excellence', 'Innovation', 'Growth', 'Ownership'];

  constructor(
    private recognitionService: RecognitionService,
    private reportsService: ReportsService,
    private userService: UserService
  ) { }

  ngOnInit() {
    this.loadData();
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getAllUsers().subscribe(res => this.users = res);
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
    // Logic for toggle (backend toggle endpoint needed ideally)
    alert(`User ${user.name} status toggled (UI Only for now).`);
  }
}

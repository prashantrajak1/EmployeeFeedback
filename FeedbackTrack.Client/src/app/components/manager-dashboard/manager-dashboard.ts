import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FeedbackService } from '../../services/feedback';
import { RecognitionService } from '../../services/recognition';
import { UserService } from '../../services/user';
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
  targetUserId: number | null = null;
  isLoading = false;

  constructor(
    private feedbackService: FeedbackService,
    private recognitionService: RecognitionService,
    private userService: UserService
  ) { }

  ngOnInit() {
    this.loadUsers();
    this.loadLeaderboard();
  }

  loadUsers() {
    this.userService.getAllUsers().subscribe(res => this.users = res);
  }

  loadLeaderboard() {
    this.recognitionService.getLeaderboard().subscribe(res => this.leaderboard = res);
  }

  loadTeamFeedback() {
    if (!this.targetUserId || this.targetUserId <= 0) return;

    this.isLoading = true;
    this.feedbackService.getTeamFeedback(+this.targetUserId).subscribe({
      next: (res) => {
        this.teamFeedback = res;
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
      }
    });
  }
}

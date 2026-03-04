import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RecognitionService } from '../../services/recognition';
import { UserService } from '../../services/user';
import { UiService } from '../../services/ui.service';
import { NotificationService } from '../../services/notification';
import { Location } from '@angular/common';
import { AdminService } from '../../services/admin';

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

  badges: string[] = [];

  constructor(
    private recognitionService: RecognitionService,
    private userService: UserService,
    private router: Router,
    private uiService: UiService,
    private notificationService: NotificationService,
    private location: Location,
    private adminService: AdminService
  ) { }

  ngOnInit() {
    this.loadUsers();
    this.loadBadges();
  }

  loadBadges() {
    this.adminService.getCategories().subscribe({
      next: (res) => {
        this.badges = res;
        if (this.badges.length > 0) {
          this.recognition.badgeType = this.badges[0];
        } else {
          this.recognition.badgeType = 'Custom Badge';
        }
      },
      error: (err) => console.error(err)
    });
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
        this.uiService.showToast('Kudos sent successfully!', 'success');

        setTimeout(() => {
          this.router.navigate(['/employee-dashboard']);
        }, 2000);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to send recognition.';
        console.error(err);
      }
    });
  }

  goBack() {
    this.location.back();
  }
}

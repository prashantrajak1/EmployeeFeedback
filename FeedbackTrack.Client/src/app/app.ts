import { Component, OnInit } from '@angular/core';
import { RouterOutlet, RouterModule, Router } from '@angular/router';
import { CommonModule, Location } from '@angular/common';
import { AuthService } from './services/auth';
import { NotificationService } from './services/notification';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterModule, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  notifications: any[] = [];
  showNotifications = false;

  constructor(
    public authService: AuthService,
    private router: Router,
    private notificationService: NotificationService,
    private location: Location
  ) { }

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.loadNotifications();
    }
  }

  loadNotifications() {
    this.notificationService.getNotifications().subscribe(res => {
      this.notifications = res;
    });
  }

  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
    if (this.showNotifications) {
      this.loadNotifications();
    }
  }

  markAsRead(notification: any) {
    this.notificationService.markAsRead(notification.id).subscribe(() => {
      notification.isRead = true;
    });
  }

  get unreadCount(): number {
    return this.notifications.filter(n => !n.isRead).length;
  }

  getUserRole(): string {
    const user = this.authService.getUser();
    return user?.role || '';
  }

  get currentUser(): any {
    return this.authService.getUser();
  }

  goBack() {
    this.location.back();
  }

  logout() {
    if (confirm('Are you sure you want to log out?')) {
      this.authService.logout();
      this.router.navigate(['/login']);
    }
  }
}

import { Component, OnInit } from '@angular/core';
import { RouterOutlet, RouterModule, Router } from '@angular/router';
import { CommonModule, Location } from '@angular/common';
import { AuthService } from './services/auth';
import { NotificationService } from './services/notification';
import { UiService } from './services/ui.service';

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
  showProfileMenu = false;
  currentToast: { message: string, type: 'success' | 'error' } | null = null;

  constructor(
    public authService: AuthService,
    private router: Router,
    private notificationService: NotificationService,
    private location: Location,
    public uiService: UiService
  ) { }

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.loadNotifications();
    }
    this.uiService.toast$.subscribe(toast => {
      this.currentToast = toast;
    });

    this.notificationService.newNotification$.subscribe(notif => {
      if (notif) {
        const user = this.authService.getUser();
        if (!user) return;

        // Criteria for showing notification:
        // 1. It is specifically for this user (recipient)
        // 2. It is an Admin-only system alert and current user is Admin
        // 3. (Optional) It has no userId and is broadcast (not used much here)

        const isRecipient = notif.userId === user.id;
        const isAdminResub = notif.isAdminOnly && (user.role === 'Admin' || user.role?.roleName === 'Admin');

        if (isRecipient || isAdminResub) {
          this.notifications.unshift(notif);
        }
      }
    });
  }

  loadNotifications() {
    this.notificationService.getNotifications().subscribe(res => {
      this.notifications = res;
    });
  }

  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
    this.showProfileMenu = false;
    if (this.showNotifications) {
      this.loadNotifications();
    }
  }

  toggleProfileMenu() {
    this.showProfileMenu = !this.showProfileMenu;
    this.showNotifications = false;
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
    if (window.confirm('Are you sure you want to log out?')) {
      this.authService.logout();
      this.router.navigate(['/login']);
    }
  }

  toggleProfile() {
    this.uiService.toggleProfileSidebar();
  }
}

import { Component, OnInit } from '@angular/core';
import { RouterOutlet, RouterModule, Router, NavigationEnd } from '@angular/router';
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
      // Poll every 10 seconds for new notifications
      setInterval(() => {
        if (this.authService.isLoggedIn()) {
          this.loadNotifications();
        }
      }, 10000);
    }

    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.showProfileMenu = false;
        this.showNotifications = false;
        this.uiService.setProfileSidebarVisible(false);
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
    if (notification.isRead) return;
    this.notificationService.markAsRead(notification.id).subscribe(() => {
      notification.isRead = true;
    });
  }

  markAllRead() {
    this.notificationService.markAllAsRead().subscribe(() => {
      this.notifications.forEach(n => n.isRead = true);
    });
  }

  deleteNotification(event: Event, notification: any) {
    event.stopPropagation();
    if (confirm('Delete this notification?')) {
      this.notificationService.deleteNotification(notification.id).subscribe(() => {
        this.notifications = this.notifications.filter(n => n.id !== notification.id);
      });
    }
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

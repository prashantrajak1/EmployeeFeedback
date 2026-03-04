import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class UiService {
    private sidebarVisible = new BehaviorSubject<boolean>(false);
    isProfileSidebarVisible$ = this.sidebarVisible.asObservable();

    private toastSubject = new BehaviorSubject<{ message: string, type: 'success' | 'error' } | null>(null);
    toast$ = this.toastSubject.asObservable();

    showToast(message: string, type: 'success' | 'error' = 'success') {
        this.toastSubject.next({ message, type });
        setTimeout(() => this.toastSubject.next(null), 3000);
    }

    clearToast() {
        this.toastSubject.next(null);
    }

    toggleProfileSidebar() {
        this.sidebarVisible.next(!this.sidebarVisible.value);
    }

    setProfileSidebarVisible(isVisible: boolean) {
        this.sidebarVisible.next(isVisible);
    }
}

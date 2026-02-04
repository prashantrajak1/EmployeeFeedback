import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const authGuard = () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (authService.isLoggedIn()) {
        return true;
    }

    return router.parseUrl('/login');
};

export const roleGuard = (allowedRoles: string[]) => {
    return () => {
        const authService = inject(AuthService);
        const router = inject(Router);
        const token = authService.getToken();

        if (!token) return router.parseUrl('/login');

        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const userRole = payload['role'] || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

            if (allowedRoles.includes(userRole)) {
                return true;
            }
        } catch (e) {
            console.error('Error parsing token', e);
        }

        // If not authorized, maybe go to a "Access Denied" or just their own dashboard?
        // For simplicity, back to login or stay put (but return false)
        alert('Access Denied');
        return false;
    };
};

import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { Register } from './components/register/register';
import { EmployeeDashboard } from './components/employee-dashboard/employee-dashboard';
import { ManagerDashboard } from './components/manager-dashboard/manager-dashboard';
import { AdminDashboard } from './components/admin-dashboard/admin-dashboard';
import { FeedbackForm } from './components/feedback-form/feedback-form';
import { RecognitionForm } from './components/recognition-form/recognition-form';
import { Reports } from './components/reports/reports';

import { authGuard, roleGuard } from './guards/auth.guard';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: Login },
    { path: 'register', component: Register },
    { path: 'employee-dashboard', component: EmployeeDashboard, canActivate: [authGuard, roleGuard(['Employee', 'Manager', 'Admin'])] },
    { path: 'manager-dashboard', component: ManagerDashboard, canActivate: [authGuard, roleGuard(['Manager', 'Admin'])] },
    { path: 'admin-dashboard', component: AdminDashboard, canActivate: [authGuard, roleGuard(['Admin'])] },
    { path: 'give-feedback', component: FeedbackForm, canActivate: [authGuard] },
    { path: 'give-recognition', component: RecognitionForm, canActivate: [authGuard] },
    { path: 'reports', component: Reports, canActivate: [authGuard, roleGuard(['Manager', 'Admin'])] },
];

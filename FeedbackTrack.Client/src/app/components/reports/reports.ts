import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportsService } from '../../services/reports';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.html',
  styleUrl: './reports.css'
})
export class Reports implements OnInit {
  stats: any = null;
  trends: any = null;
  isLoading = true;
  errorMessage = '';

  constructor(private reportsService: ReportsService) { }

  ngOnInit() {
    this.reportsService.getStats().subscribe({
      next: (res) => {
        this.stats = res;
        this.loadTrends();
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Failed to load report stats. Please try again later.';
        this.isLoading = false;
      }
    });
  }

  loadTrends() {
    this.reportsService.getTrends().subscribe({
      next: (res) => {
        this.trends = res;
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        // Stats loaded, but trends failed.
        this.isLoading = false;
      }
    });
  }

  getMax(data: any[]): number {
    return Math.max(...data.map(d => d.count), 1);
  }

  downloadReport() {
    this.reportsService.exportCsv().subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `FeedbackTrack_Report_${new Date().toISOString().split('T')[0]}.csv`;
      a.click();
      window.URL.revokeObjectURL(url);
    });
  }
}

import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UrlService } from '../../services/url';
import { TinyUrl } from '../../models/tiny-url';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class DashboardComponent implements OnInit {
  // Signal to store the list of URLs for reactive UI updates
  urls = signal<TinyUrl[]>([]);
  searchText: string = '';

  constructor(private urlService: UrlService) {}

  ngOnInit(): void {
    this.refreshList();
  }

  /**
   * Fetches the latest data from the public API
   */
  refreshList(): void {
    this.urlService.getPublicUrls().subscribe({
      next: (data) => {
        console.log('Dashboard Data Received:', data);
        // We set the signal with the raw data from your JSON response
        this.urls.set(data || []);
      },
      error: (err) => {
        console.error('Failed to load dashboard data', err);
      }
    });
  }

  /**
   * Handles the deletion of a URL record
   * @param code The unique identifier for the URL
   */
  onDelete(code: string): void {
    if (!code) return;
    
    if (confirm('Are you sure you want to delete this analytics record?')) {
      this.urlService.deleteUrl(code).subscribe({
        next: () => {
          this.refreshList(); // Reload data after successful deletion
        },
        error: (err) => console.error('Delete failed', err)
      });
    }
  }

  /**
   * Copies the short URL to the system clipboard
   * @param url The shortURL string
   */
  copyToClipboard(url: string): void {
    if (!url) return;
    
    navigator.clipboard.writeText(url).then(() => {
      alert('Link copied to clipboard!');
    }).catch(err => {
      console.error('Could not copy text: ', err);
    });
  }

  /**
   * Computed property to filter URLs based on the search input.
   * Matches against originalURL and shortURL.
   */
  get filteredUrls(): TinyUrl[] {
    const list = this.urls();
    const search = this.searchText.toLowerCase().trim();

    if (!search) {
      return list;
    }

    return list.filter(u => {
      // Using the exact keys found in your JSON response
      const original = (u.originalURL || '').toLowerCase();
      const short = (u.shortURL || '').toLowerCase();
      
      return original.includes(search) || short.includes(search);
    });
  }
}
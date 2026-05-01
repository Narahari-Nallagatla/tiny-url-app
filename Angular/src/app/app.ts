import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UrlService } from './services/url';
import { TinyUrl, TinyUrlAddDto } from './models/tiny-url';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent implements OnInit {
  inputUrl: string = '';
  isPrivate: boolean = false;
  searchText: string = '';
  urls = signal<TinyUrl[]>([]);

  // NEW: Stores the newly created URL to show in the success box
  generatedUrl: string | null = null;

  constructor(private urlService: UrlService) {}

  ngOnInit(): void {
    this.refreshList();
  }

  refreshList(): void {
    this.urlService.getPublicUrls().subscribe({
      next: (data) => this.urls.set(data || []),
      error: (err) => console.error('Fetch error:', err)
    });
  }

  onGenerate(): void {
    if (!this.inputUrl.trim()) return;

    const payload: TinyUrlAddDto = {
      OriginalURL: this.inputUrl.trim(),
      IsPrivate: this.isPrivate
    };

    this.urlService.addUrl(payload).subscribe({
      next: (response: any) => {
        // Capture the shortURL from response (noting uppercase URL as per your JSON)
        this.generatedUrl = response.shortURL || response.shortUrl;
        
        this.inputUrl = '';
        this.isPrivate = false;
        
        // Refresh list
        setTimeout(() => this.refreshList(), 1000);
      },
      error: (err) => alert('API Error. Check URL format.')
    });
  }

  onDelete(code: string): void {
    if (confirm('Delete this link?')) {
      this.urlService.deleteUrl(code).subscribe(() => {
        // If we deleted the one currently being shown in the green box, hide it
        this.generatedUrl = null;
        this.refreshList();
      });
    }
  }

  copyToClipboard(url: string): void {
    if (!url) return;
    navigator.clipboard.writeText(url);
  }

  get filteredUrls(): TinyUrl[] {
    const list = this.urls();
    const search = this.searchText.toLowerCase().trim();
    if (!search) return list;
    return list.filter(u => 
      (u.originalURL || '').toLowerCase().includes(search) || 
      (u.shortURL || '').toLowerCase().includes(search)
    );
  }
}
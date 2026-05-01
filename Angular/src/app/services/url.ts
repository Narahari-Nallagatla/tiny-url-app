import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { TinyUrl, TinyUrlAddDto } from '../models/tiny-url';

@Injectable({
  providedIn: 'root'
})
export class UrlService {
  // Use the variable from environment.ts
  private readonly baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getPublicUrls(): Observable<TinyUrl[]> {
    return this.http.get<TinyUrl[]>(`${this.baseUrl}/public`);
  }

addUrl(payload: TinyUrlAddDto): Observable<TinyUrl> {
  return this.http.post<TinyUrl>(`${this.baseUrl}/add`, payload);
}

  deleteUrl(code: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/delete/${code}`);
  }
}
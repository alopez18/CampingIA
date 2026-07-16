import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  AiCompareRequest,
  AiCompareResponse,
  AiRecommendationResponse,
  AiSearchRequest,
  AiSearchResponse
} from '../models/ai.model';

@Injectable({ providedIn: 'root' })
export class AiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/ai`;

  search(query: string, page = 1, pageSize = 10): Observable<AiSearchResponse> {
    const body: AiSearchRequest = { query, page, pageSize };
    return this.http.post<AiSearchResponse>(`${this.base}/search`, body);
  }

  getRecommendations(maxResults = 5): Observable<AiRecommendationResponse> {
    const params = new HttpParams().set('maxResults', maxResults);
    return this.http.get<AiRecommendationResponse>(`${this.base}/recommendations`, { params });
  }

  compare(campingIds: string[]): Observable<AiCompareResponse> {
    const body: AiCompareRequest = { campingIds };
    return this.http.post<AiCompareResponse>(`${this.base}/compare`, body);
  }
}

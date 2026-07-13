import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Camping, CampingSearchParams, PagedCampingsResponse } from '../models/camping.model';

@Injectable({ providedIn: 'root' })
export class CampingsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/campings`;

  getAll(page = 1, pageSize = 10): Observable<PagedCampingsResponse> {
    const params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);
    return this.http.get<PagedCampingsResponse>(this.base, { params });
  }

  search(filters: CampingSearchParams): Observable<PagedCampingsResponse> {
    let params = new HttpParams()
      .set('page', filters.page ?? 1)
      .set('pageSize', filters.pageSize ?? 10);

    if (filters.name) params = params.set('name', filters.name);
    if (filters.provinciaId) params = params.set('provinciaId', filters.provinciaId);
    if (filters.provinciaCode) params = params.set('provinciaCode', filters.provinciaCode);
    if (filters.categoryId != null) params = params.set('categoryId', filters.categoryId);
    if (filters.minPrice != null) params = params.set('minPrice', filters.minPrice);
    if (filters.maxPrice != null) params = params.set('maxPrice', filters.maxPrice);

    return this.http.get<PagedCampingsResponse>(`${this.base}/search`, { params });
  }

  getById(id: string): Observable<Camping> {
    return this.http.get<Camping>(`${this.base}/${id}`);
  }
}

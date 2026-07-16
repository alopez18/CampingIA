import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { expand, map, reduce, takeWhile } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Camping, CampingSearchParams, PagedCampingsResponse } from '../models/camping.model';

const MAP_PAGE_SIZE = 100;
const MAP_MAX_PAGES = 20;

export interface MapBounds {
  minLat: number;
  maxLat: number;
  minLng: number;
  maxLng: number;
}

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
    if (filters.minLat != null) params = params.set('minLat', filters.minLat);
    if (filters.maxLat != null) params = params.set('maxLat', filters.maxLat);
    if (filters.minLng != null) params = params.set('minLng', filters.minLng);
    if (filters.maxLng != null) params = params.set('maxLng', filters.maxLng);

    return this.http.get<PagedCampingsResponse>(`${this.base}/search`, { params });
  }

  getById(id: string): Observable<Camping> {
    return this.http.get<Camping>(`${this.base}/${id}`);
  }

  /**
   * Carga los campings para el mapa recorriendo el paginado.
   * Si se indica un área (`bounds`), delega en el endpoint de búsqueda para que el backend
   * filtre por bounding box; en caso contrario trae todos los campings.
   * Devuelve solo los campings con coordenadas válidas.
   */
  getForMap(bounds?: MapBounds): Observable<Camping[]> {
    const fetchPage = bounds
      ? (page: number) => this.search({
          minLat: bounds.minLat,
          maxLat: bounds.maxLat,
          minLng: bounds.minLng,
          maxLng: bounds.maxLng,
          page,
          pageSize: MAP_PAGE_SIZE
        })
      : (page: number) => this.getAll(page, MAP_PAGE_SIZE);

    return fetchPage(1).pipe(
      expand((response, index) => {
        const currentPage = index + 1;
        const loaded = currentPage * MAP_PAGE_SIZE;
        const hasMore = loaded < response.totalCount && currentPage < MAP_MAX_PAGES;
        return hasMore ? fetchPage(currentPage + 1) : [];
      }),
      takeWhile(response => response.items.length > 0, true),
      reduce((acc, response) => acc.concat(response.items), [] as Camping[]),
      map(campings => campings.filter(c => this.hasValidCoordinates(c)))
    );
  }

  private hasValidCoordinates(camping: Camping): boolean {
    return (
      camping.latitude != null &&
      camping.longitude != null &&
      !(camping.latitude === 0 && camping.longitude === 0) &&
      Math.abs(camping.latitude) <= 90 &&
      Math.abs(camping.longitude) <= 180
    );
  }
}

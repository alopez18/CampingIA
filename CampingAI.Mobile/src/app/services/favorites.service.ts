import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { Favorite } from '../models/favorite.model';

@Injectable({ providedIn: 'root' })
export class FavoritesService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/favorites`;

  readonly favoriteIds = signal<Set<string>>(new Set());

  getAll(): Observable<Favorite[]> {
    return this.http.get<Favorite[]>(this.base).pipe(
      tap(favs => this.favoriteIds.set(new Set(favs.map(f => f.campingId))))
    );
  }

  add(campingId: string): Observable<Favorite> {
    return this.http.post<Favorite>(this.base, { campingId }).pipe(
      tap(() => this.favoriteIds.update(ids => new Set([...ids, campingId])))
    );
  }

  remove(campingId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${campingId}`).pipe(
      tap(() => this.favoriteIds.update(ids => { ids.delete(campingId); return new Set(ids); }))
    );
  }

  isFavorite(campingId: string): boolean {
    return this.favoriteIds().has(campingId);
  }

  toggle(campingId: string): Observable<Favorite | void> {
    return this.isFavorite(campingId) ? this.remove(campingId) : this.add(campingId);
  }
}

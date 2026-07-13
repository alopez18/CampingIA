import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { from, Observable, switchMap, tap } from 'rxjs';
import { Preferences } from '@capacitor/preferences';
import { environment } from '../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest, User } from '../models/user.model';
import { TOKEN_KEY } from '../core/interceptors/jwt.interceptor';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/register`, request)
      .pipe(tap(res => this.saveToken(res.token)));
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/login`, request)
      .pipe(tap(res => this.saveToken(res.token)));
  }

  logout(): void {
    Preferences.remove({ key: TOKEN_KEY });
    this.router.navigate(['/auth/login']);
  }

  getToken(): Observable<string | null> {
    return from(Preferences.get({ key: TOKEN_KEY })).pipe(
      switchMap(({ value }) => [value])
    );
  }

  private saveToken(token: string): void {
    Preferences.set({ key: TOKEN_KEY, value: token });
  }
}

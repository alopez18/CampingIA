import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { from, Observable, switchMap } from 'rxjs';
import { Preferences } from '@capacitor/preferences';
import { Capacitor } from '@capacitor/core';
import { SocialLogin } from '@capgo/capacitor-social-login';
import { environment } from '../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest, User } from '../models/user.model';
import { TOKEN_KEY } from '../core/interceptors/jwt.interceptor';

declare const google: {
  accounts: {
    id: {
      initialize(config: { client_id: string; callback: (response: { credential: string }) => void; auto_select?: boolean; cancel_on_tap_outside?: boolean; use_fedcm_for_prompt?: boolean }): void;
      prompt(momentListener?: (notification: {
        isDisplayed(): boolean;
        isNotDisplayed(): boolean;
        isSkippedMoment(): boolean;
        isDismissedMoment(): boolean;
      }) => void): void;
      cancel(): void;
    };
  };
};

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private googleNativeInitialized = false;

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/register`, request)
      .pipe(switchMap(res => from(this.handleAuthSuccess(res))));
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/login`, request)
      .pipe(switchMap(res => from(this.handleAuthSuccess(res))));
  }

  loginWithGoogle(): Promise<void> {
    const idTokenPromise = Capacitor.isNativePlatform()
      ? this.loginWithGoogleNative()
      : this.tryOneTap().catch(() => this.loginWithGooglePopup());

    return idTokenPromise.then(idToken =>
      new Promise<void>((resolve, reject) =>
        this.http
          .post<AuthResponse>(`${environment.apiUrl}/auth/google`, { idToken })
          .subscribe({
            next: res => this.handleAuthSuccess(res).then(() => resolve()).catch(reject),
            error: reject
          })
      )
    );
  }

  private async loginWithGoogleNative(): Promise<string> {
    if (!this.googleNativeInitialized) {
      await SocialLogin.initialize({
        google: { webClientId: environment.googleClientId }
      });
      this.googleNativeInitialized = true;
    }

    const { result } = await SocialLogin.login({
      provider: 'google',
      options: {
        filterByAuthorizedAccounts: false,
        autoSelectEnabled: false
      }
    });

    const idToken = (result as { idToken?: string | null }).idToken;
    if (!idToken) {
      throw new Error('Google no devolvió un idToken.');
    }
    return idToken;
  }

  private tryOneTap(): Promise<string> {
    return new Promise((resolve, reject) => {
      let resolved = false;

      google.accounts.id.initialize({
        client_id: environment.googleClientId,
        use_fedcm_for_prompt: true,
        callback: (response) => {
          resolved = true;
          resolve(response.credential);
        },
        cancel_on_tap_outside: true
      });

      google.accounts.id.prompt((notification) => {
        if (!resolved && (notification.isNotDisplayed() || notification.isSkippedMoment())) {
          reject(new Error('One Tap no disponible'));
        }
      });
    });
  }

  private loginWithGooglePopup(): Promise<string> {
    return new Promise((resolve, reject) => {
      const nonce = crypto.randomUUID ? crypto.randomUUID() : Math.random().toString(36).substring(2);

      const params = new URLSearchParams({
        client_id: environment.googleClientId,
        redirect_uri: `${window.location.origin}/oauth-callback.html`,
        response_type: 'id_token',
        scope: 'openid email profile',
        nonce,
        prompt: 'select_account'
      });

      const popup = window.open(
        `https://accounts.google.com/o/oauth2/v2/auth?${params}`,
        'google-oauth',
        'width=500,height=600,popup=yes,left=200,top=100'
      );

      if (!popup) {
        reject(new Error('El navegador bloqueó la ventana emergente. Permite popups para este sitio.'));
        return;
      }

      let checkClosed: ReturnType<typeof setInterval>;

      const onMessage = (event: MessageEvent) => {
        if (event.origin !== window.location.origin) return;
        if (event.data?.google_id_token) {
          window.removeEventListener('message', onMessage);
          clearInterval(checkClosed);
          resolve(event.data.google_id_token as string);
        } else if (event.data?.google_error) {
          window.removeEventListener('message', onMessage);
          clearInterval(checkClosed);
          reject(new Error(`Google error: ${event.data.google_error as string}`));
        }
      };
      window.addEventListener('message', onMessage);

      checkClosed = setInterval(() => {
        if (popup.closed) {
          clearInterval(checkClosed);
          window.removeEventListener('message', onMessage);
          reject(new Error('Login cancelado'));
        }
      }, 500);
    });
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

  private async handleAuthSuccess(res: AuthResponse): Promise<AuthResponse> {
    await this.saveToken(res.token);
    await this.router.navigate(['/tabs/home']);
    return res;
  }

  private saveToken(token: string): Promise<void> {
    return Preferences.set({ key: TOKEN_KEY, value: token });
  }
}

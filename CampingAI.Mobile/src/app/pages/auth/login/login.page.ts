import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonItem, IonLabel,
  IonInput, IonButton, IonText, IonIcon, ToastController, LoadingController
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { logoGoogle } from 'ionicons/icons';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule, RouterLink,
    IonHeader, IonToolbar, IonTitle, IonContent, IonItem, IonLabel,
    IonInput, IonButton, IonText, IonIcon
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Iniciar sesión</ion-title>
      </ion-toolbar>
    </ion-header>
    <ion-content class="ion-padding">
      <div class="auth-hero">
        <span class="auth-hero-icon">🏕️</span>
        <h1 class="auth-app-name">CampingAI</h1>
        <p class="auth-tagline">Tu aventura al aire libre comienza aquí</p>
      </div>
      <form [formGroup]="form" (ngSubmit)="onSubmit()">
        <ion-item>
          <ion-label position="floating">Email</ion-label>
          <ion-input type="email" formControlName="email" autocomplete="email"></ion-input>
        </ion-item>
        @if (form.get('email')?.invalid && form.get('email')?.touched) {
          <p class="error-msg">Email requerido y válido.</p>
        }

        <ion-item class="ion-margin-top">
          <ion-label position="floating">Contraseña</ion-label>
          <ion-input type="password" formControlName="password" autocomplete="current-password"></ion-input>
        </ion-item>
        @if (form.get('password')?.invalid && form.get('password')?.touched) {
          <p class="error-msg">Contraseña requerida (mín. 6 caracteres).</p>
        }

        <ion-button
          expand="block"
          class="ion-margin-top"
          type="submit"
          [disabled]="form.invalid">
          Entrar
        </ion-button>
      </form>

      <div class="separator ion-text-center ion-padding-vertical">
        <span>o continúa con</span>
      </div>

      <ion-button
        expand="block"
        fill="outline"
        color="medium"
        (click)="onGoogleLogin()">
        <ion-icon slot="start" name="logo-google"></ion-icon>
        Iniciar sesión con Google
      </ion-button>

      <div class="ion-text-center ion-padding-top">
        <ion-text>¿No tienes cuenta? <a routerLink="/auth/register">Regístrate</a></ion-text>
      </div>
    </ion-content>
  `,
  styles: [`
    .auth-hero {
      background: linear-gradient(145deg, #1b5e20 0%, #2e7d32 60%, #43a047 100%);
      margin: -16px -16px 28px;
      width: calc(100% + 32px);
      padding: 52px 24px 44px;
      text-align: center;
      border-radius: 0 0 32px 32px;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 4px;
    }
    .auth-hero-icon { font-size: 3.5em; filter: drop-shadow(0 2px 12px rgba(0,0,0,0.3)); }
    .auth-app-name { color: white; font-size: 2em; font-weight: 800; margin: 8px 0 4px; letter-spacing: -0.5px; }
    .auth-tagline { color: rgba(255,255,255,0.8); font-size: 0.9em; margin: 0; }
    .error-msg { color: var(--ion-color-danger); font-size: 0.8em; padding: 4px 16px; }
    .separator { color: var(--ion-color-medium); font-size: 0.85em; }
    .separator span { background: var(--ion-background-color); padding: 0 8px; }
  `]
})
export class LoginPage {
  private readonly auth = inject(AuthService);
  private readonly fb = inject(FormBuilder);
  private readonly toast = inject(ToastController);
  private readonly loading = inject(LoadingController);

  constructor() {
    addIcons({ logoGoogle });
  }

  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  async onSubmit(): Promise<void> {
    if (this.form.invalid) return;
    const loader = await this.loading.create({ message: 'Iniciando sesión...' });
    await loader.present();

    const { email, password } = this.form.value;
    this.auth.login({ email: email!, password: password! }).subscribe({
      next: async () => {
        await loader.dismiss();
      },
      error: async () => {
        await loader.dismiss();
        const t = await this.toast.create({
          message: 'Credenciales incorrectas. Inténtalo de nuevo.',
          duration: 3000, color: 'danger', position: 'bottom'
        });
        await t.present();
      }
    });
  }

  async onGoogleLogin(): Promise<void> {
    const loader = await this.loading.create({ message: 'Conectando con Google...' });
    await loader.present();
    try {
      await this.auth.loginWithGoogle();
      await loader.dismiss();
    } catch {
      await loader.dismiss();
      const t = await this.toast.create({
        message: 'No se pudo iniciar sesión con Google. Inténtalo de nuevo.',
        duration: 3000, color: 'danger', position: 'bottom'
      });
      await t.present();
    }
  }
}

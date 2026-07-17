import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonItem, IonLabel,
  IonInput, IonButton, IonText, ToastController, LoadingController
} from '@ionic/angular/standalone';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    ReactiveFormsModule, RouterLink,
    IonHeader, IonToolbar, IonTitle, IonContent, IonItem, IonLabel,
    IonInput, IonButton, IonText
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Crear cuenta</ion-title>
      </ion-toolbar>
    </ion-header>
    <ion-content class="ion-padding">
      <div class="auth-shell">
        <div class="auth-card">
          <div class="auth-hero">
            <span class="auth-hero-icon">🏕️</span>
            <h1 class="auth-app-name">CampingAI</h1>
            <p class="auth-tagline">Crea tu cuenta</p>
          </div>
          <div class="auth-body">
            <form [formGroup]="form" (ngSubmit)="onSubmit()">
              <ion-item>
                <ion-label position="floating">Nombre</ion-label>
                <ion-input type="text" formControlName="name" autocomplete="name"></ion-input>
              </ion-item>

              <ion-item class="ion-margin-top">
                <ion-label position="floating">Email</ion-label>
                <ion-input type="email" formControlName="email" autocomplete="email"></ion-input>
              </ion-item>

              <ion-item class="ion-margin-top">
                <ion-label position="floating">Contraseña</ion-label>
                <ion-input type="password" formControlName="password" autocomplete="new-password"></ion-input>
              </ion-item>
              @if (form.get('password')?.invalid && form.get('password')?.touched) {
                <p class="error-msg">Mínimo 6 caracteres.</p>
              }

              <ion-item class="ion-margin-top">
                <ion-label position="floating">Repetir contraseña</ion-label>
                <ion-input type="password" formControlName="confirmPassword" autocomplete="new-password"></ion-input>
              </ion-item>
              @if (form.get('confirmPassword')?.touched && form.hasError('passwordMismatch')) {
                <p class="error-msg">Las contraseñas no coinciden.</p>
              }

              <ion-button
                expand="block"
                class="ion-margin-top"
                type="submit"
                [disabled]="form.invalid">
                Registrarse
              </ion-button>
            </form>
            <div class="ion-text-center ion-padding-top">
              <ion-text>¿Ya tienes cuenta? <a routerLink="/auth/login">Inicia sesión</a></ion-text>
            </div>
          </div>
        </div>
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

    /* ── Apariencia Web / Escritorio ─────────────────────────────────────────
       Solo se aplica cuando la app corre en navegador (body.platform-web).
       En app nativa el layout móvil original permanece intacto. */
    :host-context(body.platform-web) ion-header {
      display: none;
    }

    :host-context(body.platform-web) .auth-shell {
      min-height: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 24px;
    }

    :host-context(body.platform-web) .auth-card {
      width: 100%;
      max-width: 900px;
      display: flex;
      background: var(--ion-background-color, #fff);
      border-radius: 24px;
      overflow: hidden;
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.18);
    }

    :host-context(body.platform-web) .auth-hero {
      /* Reinicia el sangrado móvil (márgenes negativos) y ocupa la columna izquierda. */
      margin: 0;
      width: 42%;
      border-radius: 0;
      justify-content: center;
      padding: 48px 32px;
    }

    :host-context(body.platform-web) .auth-hero-icon { font-size: 4.5em; }
    :host-context(body.platform-web) .auth-app-name { font-size: 2.4em; }
    :host-context(body.platform-web) .auth-tagline { font-size: 1em; }

    :host-context(body.platform-web) .auth-body {
      flex: 1;
      display: flex;
      flex-direction: column;
      justify-content: center;
      padding: 48px 40px;
    }

    /* En pantallas estrechas (web en móvil o ventana pequeña) apila en una columna. */
    @media (max-width: 720px) {
      :host-context(body.platform-web) .auth-card {
        flex-direction: column;
        max-width: 440px;
      }
      :host-context(body.platform-web) .auth-hero {
        width: 100%;
        padding: 40px 24px 32px;
      }
      :host-context(body.platform-web) .auth-body {
        padding: 32px 28px;
      }
    }
  `]
})
export class RegisterPage {
  private readonly auth = inject(AuthService);
  private readonly fb = inject(FormBuilder);
  private readonly toast = inject(ToastController);
  private readonly loading = inject(LoadingController);

  readonly form = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: RegisterPage.passwordsMatchValidator });

  private static passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) return;
    const loader = await this.loading.create({ message: 'Creando cuenta...' });
    await loader.present();

    const { name, email, password } = this.form.value;
    this.auth.register({ name: name!, email: email!, password: password! }).subscribe({
      next: async () => {
        await loader.dismiss();
      },
      error: async () => {
        await loader.dismiss();
        const t = await this.toast.create({
          message: 'Error al registrarse. El email puede estar en uso.',
          duration: 3000, color: 'danger', position: 'bottom'
        });
        await t.present();
      }
    });
  }
}

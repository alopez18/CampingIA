import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonItem, IonLabel,
  IonInput, IonButton, IonText, ToastController, LoadingController
} from '@ionic/angular/standalone';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule, RouterLink,
    IonHeader, IonToolbar, IonTitle, IonContent, IonItem, IonLabel,
    IonInput, IonButton, IonText
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Iniciar sesión</ion-title>
      </ion-toolbar>
    </ion-header>
    <ion-content class="ion-padding">
      <div class="logo-section ion-text-center ion-padding-vertical">
        <h1>🏕️ CampingAI</h1>
        <p>Tu asistente de campings</p>
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
      <div class="ion-text-center ion-padding-top">
        <ion-text>¿No tienes cuenta? <a routerLink="/auth/register">Regístrate</a></ion-text>
      </div>
    </ion-content>
  `,
  styles: [`.error-msg { color: var(--ion-color-danger); font-size: 0.8em; padding: 4px 16px; }`]
})
export class LoginPage {
  private readonly auth = inject(AuthService);
  private readonly fb = inject(FormBuilder);
  private readonly toast = inject(ToastController);
  private readonly loading = inject(LoadingController);

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
}

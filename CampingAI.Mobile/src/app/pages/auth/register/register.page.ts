import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
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
    </ion-content>
  `,
  styles: [`.error-msg { color: var(--ion-color-danger); font-size: 0.8em; padding: 4px 16px; }`]
})
export class RegisterPage {
  private readonly auth = inject(AuthService);
  private readonly fb = inject(FormBuilder);
  private readonly toast = inject(ToastController);
  private readonly loading = inject(LoadingController);

  readonly form = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

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

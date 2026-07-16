import { Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonItem, IonLabel, IonInput,
  IonButton, IonAvatar, IonText, IonSpinner, IonList, IonIcon, ToastController,
  LoadingController, AlertController
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { calendarOutline, chevronForwardOutline } from 'ionicons/icons';
import { UsersService } from '../../services/users.service';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    IonHeader, IonToolbar, IonTitle, IonContent, IonItem, IonLabel, IonInput,
    IonButton, IonAvatar, IonText, IonSpinner, IonList, IonIcon
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Mi perfil</ion-title>
      </ion-toolbar>
    </ion-header>
    <ion-content class="ion-padding">
      @if (loading()) {
        <div class="ion-text-center ion-padding"><ion-spinner></ion-spinner></div>
      } @else {
        <div class="ion-text-center ion-padding-vertical">
          <ion-avatar style="margin:auto;width:80px;height:80px;background:var(--ion-color-primary);display:flex;align-items:center;justify-content:center;">
            <span style="font-size:2em;color:white">{{ initials() }}</span>
          </ion-avatar>
          <ion-text><p>{{ user()?.email }}</p></ion-text>
        </div>

        <form [formGroup]="form" (ngSubmit)="onSave()">
          <ion-item>
            <ion-label position="floating">Nombre</ion-label>
            <ion-input type="text" formControlName="name"></ion-input>
          </ion-item>
          <ion-item class="ion-margin-top">
            <ion-label position="floating">Email</ion-label>
            <ion-input type="email" formControlName="email"></ion-input>
          </ion-item>
          <ion-button expand="block" class="ion-margin-top" type="submit" [disabled]="form.invalid">
            Guardar cambios
          </ion-button>
        </form>

        <ion-list class="ion-margin-top">
          <ion-item button detail="false" (click)="goToReservations()">
            <ion-icon name="calendar-outline" slot="start" color="primary"></ion-icon>
            <ion-label>Mis reservas</ion-label>
            <ion-icon name="chevron-forward-outline" slot="end" color="medium"></ion-icon>
          </ion-item>
        </ion-list>

        <ion-button expand="block" fill="outline" color="danger" class="ion-margin-top" (click)="logout()">
          Cerrar sesión
        </ion-button>
      }
    </ion-content>
  `
})
export class ProfilePage implements OnInit {
  private readonly usersService = inject(UsersService);
  private readonly authService = inject(AuthService);
  private readonly fb = inject(FormBuilder);
  private readonly toast = inject(ToastController);
  private readonly loading2 = inject(LoadingController);
  private readonly router = inject(Router);

  readonly user = signal<User | null>(null);
  readonly loading = signal(true);

  readonly form = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]]
  });

  constructor() { addIcons({ calendarOutline, chevronForwardOutline }); }

  ngOnInit(): void {
    this.usersService.getMe().subscribe({
      next: u => {
        this.user.set(u);
        this.form.patchValue({ name: u.name, email: u.email });
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  initials(): string {
    return this.user()?.name?.charAt(0).toUpperCase() ?? '?';
  }

  async onSave(): Promise<void> {
    if (this.form.invalid || !this.user()) return;
    const loader = await this.loading2.create({ message: 'Guardando...' });
    await loader.present();
    const { name, email } = this.form.value;
    this.usersService.update(this.user()!.id, { name: name!, email: email! }).subscribe({
      next: async u => {
        this.user.set(u);
        await loader.dismiss();
        const t = await this.toast.create({ message: 'Perfil actualizado ✅', duration: 2000, color: 'success' });
        await t.present();
      },
      error: async () => {
        await loader.dismiss();
        const t = await this.toast.create({ message: 'Error al guardar.', duration: 2000, color: 'danger' });
        await t.present();
      }
    });
  }

  goToReservations(): void { this.router.navigate(['/tabs/reservations']); }

  logout(): void { this.authService.logout(); }
}

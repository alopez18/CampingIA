import { Component, OnInit, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonItem, IonLabel,
  IonDatetime, IonButton, IonBackButton, IonButtons,
  ToastController, LoadingController
} from '@ionic/angular/standalone';
import { ReservationsService } from '../../../services/reservations.service';

@Component({
  selector: 'app-new-reservation',
  standalone: true,
  imports: [
    ReactiveFormsModule, DecimalPipe,
    IonHeader, IonToolbar, IonTitle, IonContent, IonItem, IonLabel,
    IonDatetime, IonButton, IonBackButton, IonButtons
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-buttons slot="start">
          <ion-back-button defaultHref="/tabs/reservations"></ion-back-button>
        </ion-buttons>
        <ion-title>Nueva reserva</ion-title>
      </ion-toolbar>
    </ion-header>
    <ion-content class="ion-padding">
      <form [formGroup]="form" (ngSubmit)="onSubmit()">
        <ion-item>
          <ion-label>Check-in</ion-label>
          <ion-datetime
            formControlName="checkIn"
            presentation="date"
            [min]="today">
          </ion-datetime>
        </ion-item>

        <ion-item class="ion-margin-top">
          <ion-label>Check-out</ion-label>
          <ion-datetime
            formControlName="checkOut"
            presentation="date"
            [min]="form.value.checkIn ?? today">
          </ion-datetime>
        </ion-item>

        @if (nights() > 0) {
          <p class="ion-padding summary">
            {{ nights() }} noche(s) · Total: {{ totalPrice() | number:'1.2-2' }} €
          </p>
        }

        <ion-button
          expand="block"
          class="ion-margin-top"
          type="submit"
          [disabled]="form.invalid || nights() < 1">
          Confirmar reserva
        </ion-button>
      </form>
    </ion-content>
  `,
  styles: [`.summary { font-weight: 600; color: var(--ion-color-primary); }`]
})
export class NewReservationPage implements OnInit {
  private readonly reservationsService = inject(ReservationsService);
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly loading = inject(LoadingController);
  private readonly toast = inject(ToastController);

  campingId = '';
  pricePerNight = 0;
  readonly today = new Date().toISOString();

  readonly form = this.fb.group({
    checkIn: ['', Validators.required],
    checkOut: ['', Validators.required]
  });

  ngOnInit(): void {
    this.campingId = this.route.snapshot.queryParamMap.get('campingId') ?? '';
    this.pricePerNight = Number(this.route.snapshot.queryParamMap.get('pricePerNight') ?? 0);
  }

  nights(): number {
    const ci = this.form.value.checkIn;
    const co = this.form.value.checkOut;
    if (!ci || !co) return 0;
    const diff = new Date(co).getTime() - new Date(ci).getTime();
    return Math.max(0, Math.round(diff / (1000 * 60 * 60 * 24)));
  }

  totalPrice(): number { return this.nights() * this.pricePerNight; }

  async onSubmit(): Promise<void> {
    if (this.form.invalid || this.nights() < 1) return;
    const loader = await this.loading.create({ message: 'Reservando...' });
    await loader.present();

    const { checkIn, checkOut } = this.form.value;
    this.reservationsService.create({
      campingId: this.campingId,
      checkIn: checkIn!,
      checkOut: checkOut!,
      totalPrice: this.totalPrice()
    }).subscribe({
      next: async () => {
        await loader.dismiss();
        const t = await this.toast.create({ message: '✅ Reserva creada', duration: 2000, color: 'success' });
        await t.present();
        this.router.navigate(['/tabs/reservations']);
      },
      error: async () => {
        await loader.dismiss();
        const t = await this.toast.create({ message: 'Error al crear la reserva.', duration: 3000, color: 'danger' });
        await t.present();
      }
    });
  }
}

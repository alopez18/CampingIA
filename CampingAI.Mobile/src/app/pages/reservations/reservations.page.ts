import { Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonList, IonItem, IonLabel,
  IonBadge, IonIcon, IonItemSliding, IonItemOptions, IonItemOption,
  IonSpinner, IonText, IonRefresher, IonRefresherContent, IonFab, IonFabButton,
  AlertController, ToastController
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { addOutline, closeCircleOutline } from 'ionicons/icons';
import { ReservationsService } from '../../services/reservations.service';
import { Reservation } from '../../models/reservation.model';

const STATUS_COLOR: Record<string, string> = {
  Pending: 'warning',
  Confirmed: 'success',
  Cancelled: 'medium'
};

@Component({
  selector: 'app-reservations',
  standalone: true,
  imports: [
    CurrencyPipe, DatePipe,
    IonHeader, IonToolbar, IonTitle, IonContent, IonList, IonItem, IonLabel,
    IonBadge, IonIcon, IonItemSliding, IonItemOptions, IonItemOption,
    IonSpinner, IonText, IonRefresher, IonRefresherContent, IonFab, IonFabButton
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Reservas</ion-title>
      </ion-toolbar>
    </ion-header>
    <ion-content>
      <ion-refresher slot="fixed" (ionRefresh)="onRefresh($event)">
        <ion-refresher-content></ion-refresher-content>
      </ion-refresher>

      @if (loading()) {
        <div class="ion-text-center ion-padding"><ion-spinner></ion-spinner></div>
      } @else if (reservations().length === 0) {
        <div class="ion-text-center ion-padding">
          <p style="font-size:3em">🏕️</p>
          <ion-text color="medium">No tienes reservas todavía.</ion-text>
        </div>
      } @else {
        <ion-list>
          @for (res of reservations(); track res.id) {
            <ion-item-sliding>
              <ion-item>
                <ion-label>
                  <h2>Reserva #{{ res.id.substring(0,8) }}</h2>
                  <p>{{ res.checkIn | date:'dd/MM/yyyy' }} → {{ res.checkOut | date:'dd/MM/yyyy' }}</p>
                  <p>{{ res.nights }} noches · {{ res.totalPrice | currency:'EUR' }}</p>
                </ion-label>
                <ion-badge slot="end" [color]="statusColor(res.status)">{{ res.status }}</ion-badge>
              </ion-item>
              @if (res.status !== 'Cancelled') {
                <ion-item-options side="end">
                  <ion-item-option color="danger" (click)="confirmCancel(res.id)">
                    <ion-icon name="close-circle-outline" slot="icon-only"></ion-icon>
                  </ion-item-option>
                </ion-item-options>
              }
            </ion-item-sliding>
          }
        </ion-list>
      }

      <ion-fab slot="fixed" vertical="bottom" horizontal="end">
        <ion-fab-button (click)="router.navigate(['/tabs/reservations/new'])">
          <ion-icon name="add-outline"></ion-icon>
        </ion-fab-button>
      </ion-fab>
    </ion-content>
  `
})
export class ReservationsPage implements OnInit {
  private readonly reservationsService = inject(ReservationsService);
  private readonly alert = inject(AlertController);
  private readonly toast = inject(ToastController);
  readonly router = inject(Router);

  readonly reservations = signal<Reservation[]>([]);
  readonly loading = signal(true);

  constructor() { addIcons({ addOutline, closeCircleOutline }); }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.reservationsService.getAll().subscribe({
      next: r => { this.reservations.set(r); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  statusColor(status: string): string { return STATUS_COLOR[status] ?? 'medium'; }

  async confirmCancel(id: string): Promise<void> {
    const a = await this.alert.create({
      header: 'Cancelar reserva',
      message: '¿Seguro que deseas cancelar esta reserva?',
      buttons: [
        { text: 'No', role: 'cancel' },
        { text: 'Sí, cancelar', role: 'destructive', handler: () => this.cancelReservation(id) }
      ]
    });
    await a.present();
  }

  cancelReservation(id: string): void {
    this.reservationsService.cancel(id).subscribe({
      next: () => {
        this.reservations.update(list =>
          list.map(r => r.id === id ? { ...r, status: 'Cancelled' } : r) as Reservation[]
        );
      }
    });
  }

  onRefresh(event: CustomEvent): void {
    this.load();
    (event.target as HTMLIonRefresherElement).complete();
  }
}

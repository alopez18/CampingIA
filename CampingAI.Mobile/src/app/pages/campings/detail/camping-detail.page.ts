import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonBackButton, IonButtons,
  IonButton, IonIcon, IonSpinner, IonCard,
  IonCardHeader, IonCardTitle, IonCardContent, ToastController
} from '@ionic/angular/standalone';
import { CurrencyPipe } from '@angular/common';
import { addIcons } from 'ionicons';
import { heart, heartOutline, locationOutline, cashOutline, calendarOutline } from 'ionicons/icons';
import { CampingsService } from '../../../services/campings.service';
import { FavoritesService } from '../../../services/favorites.service';
import { Camping } from '../../../models/camping.model';

@Component({
  selector: 'app-camping-detail',
  standalone: true,
  imports: [
    CurrencyPipe,
    IonHeader, IonToolbar, IonTitle, IonContent, IonBackButton, IonButtons,
    IonButton, IonIcon, IonSpinner, IonCard,
    IonCardHeader, IonCardTitle, IonCardContent
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-buttons slot="start">
          <ion-back-button defaultHref="/tabs/campings"></ion-back-button>
        </ion-buttons>
        <ion-title>{{ camping()?.name ?? 'Camping' }}</ion-title>
        <ion-buttons slot="end">
          <ion-button (click)="toggleFavorite()">
            <ion-icon
              slot="icon-only"
              [name]="favoritesService.isFavorite(camping()?.id ?? '') ? 'heart' : 'heart-outline'"
              [color]="favoritesService.isFavorite(camping()?.id ?? '') ? 'danger' : 'light'">
            </ion-icon>
          </ion-button>
        </ion-buttons>
      </ion-toolbar>
    </ion-header>
    <ion-content>
      @if (loading()) {
        <div class="ion-text-center ion-padding"><ion-spinner></ion-spinner></div>
      } @else if (camping()) {
        <ion-card>
          <ion-card-header>
            <ion-card-title>{{ camping()!.name }}</ion-card-title>
          </ion-card-header>
          <ion-card-content>
            <p>{{ camping()!.description }}</p>
            <div class="detail-row">
              <ion-icon name="cash-outline" color="primary"></ion-icon>
              <strong>{{ camping()!.pricePerNight | currency:'EUR' }} / noche</strong>
            </div>
            <div class="detail-row">
              <ion-icon name="location-outline" color="primary"></ion-icon>
              <span>Lat: {{ camping()!.latitude }}, Lng: {{ camping()!.longitude }}</span>
            </div>
            <div class="map-placeholder ion-margin-vertical">
              <p class="ion-text-center">🗺️ Mapa — disponible en Fase 10</p>
            </div>
          </ion-card-content>
        </ion-card>
        <div class="ion-padding">
          <ion-button expand="block" (click)="goToReserve()">
            <ion-icon name="calendar-outline" slot="start"></ion-icon>
            Reservar
          </ion-button>
        </div>
      }
    </ion-content>
  `,
  styles: [`
    .detail-row { display: flex; align-items: center; gap: 8px; margin: 8px 0; }
    .map-placeholder { background: var(--ion-color-light); border-radius: 8px; padding: 40px; }
  `]
})
export class CampingDetailPage implements OnInit {
  private readonly campingsService = inject(CampingsService);
  readonly favoritesService = inject(FavoritesService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastController);

  readonly camping = signal<Camping | null>(null);
  readonly loading = signal(true);

  constructor() { addIcons({ heart, heartOutline, locationOutline, cashOutline, calendarOutline }); }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.campingsService.getById(id).subscribe({
      next: c => { this.camping.set(c); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  toggleFavorite(): void {
    if (!this.camping()) return;
    this.favoritesService.toggle(this.camping()!.id).subscribe();
  }

  goToReserve(): void {
    this.router.navigate(['/tabs/reservations/new'], {
      queryParams: { campingId: this.camping()?.id, pricePerNight: this.camping()?.pricePerNight }
    });
  }
}

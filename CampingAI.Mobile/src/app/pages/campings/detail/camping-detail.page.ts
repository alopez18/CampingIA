import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonBackButton, IonButtons,
  IonButton, IonIcon, IonSpinner, IonCard,
  IonCardHeader, IonCardTitle, IonCardContent, ToastController
} from '@ionic/angular/standalone';
import { CurrencyPipe } from '@angular/common';
import { addIcons } from 'ionicons';
import { heart, heartOutline, locationOutline, cashOutline, calendarOutline } from 'ionicons/icons';
import * as L from 'leaflet';
import { CampingsService } from '../../../services/campings.service';
import { FavoritesService } from '../../../services/favorites.service';
import { Camping } from '../../../models/camping.model';

const DETAIL_ZOOM = 13;

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
        <div class="detail-hero">
          <span class="detail-hero-icon" aria-hidden="true">🏕️</span>
        </div>
        <ion-card class="detail-card">
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
            @if (hasValidCoordinates()) {
              <div id="camping-detail-map" class="detail-map ion-margin-vertical"></div>
            } @else {
              <div class="map-placeholder ion-margin-vertical">
                <p class="ion-text-center">📍 Ubicación no disponible</p>
              </div>
            }
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
    .detail-hero {
      height: 220px;
      background: linear-gradient(145deg, #1b5e20 0%, #2e7d32 50%, #0277bd 100%);
      display: flex;
      align-items: center;
      justify-content: center;
    }
    .detail-hero-icon { font-size: 5em; filter: drop-shadow(0 4px 16px rgba(0,0,0,0.3)); }
    .detail-card { --border-radius: 20px; margin-top: -24px; position: relative; z-index: 2; }
    .detail-row { display: flex; align-items: center; gap: 8px; margin: 8px 0; }
    .map-placeholder { background: var(--ion-color-light); border-radius: 8px; padding: 40px; }
    .detail-map { height: 220px; border-radius: 8px; overflow: hidden; z-index: 0; }
  `]
})
export class CampingDetailPage implements OnInit, OnDestroy {
  private readonly campingsService = inject(CampingsService);
  readonly favoritesService = inject(FavoritesService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastController);

  readonly camping = signal<Camping | null>(null);
  readonly loading = signal(true);

  private map?: L.Map;

  constructor() {
    addIcons({ heart, heartOutline, locationOutline, cashOutline, calendarOutline });
    this.configureDefaultIcon();
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.campingsService.getById(id).subscribe({
      next: c => {
        this.camping.set(c);
        this.loading.set(false);
        if (this.hasValidCoordinates()) {
          // Espera a que Angular renderice el contenedor del mapa.
          setTimeout(() => this.initMap(), 0);
        }
      },
      error: () => this.loading.set(false)
    });
  }

  ngOnDestroy(): void {
    this.map?.remove();
    this.map = undefined;
  }

  hasValidCoordinates(): boolean {
    const c = this.camping();
    return !!c && c.latitude != null && c.longitude != null && !(c.latitude === 0 && c.longitude === 0);
  }

  private configureDefaultIcon(): void {
    // Evita que Leaflet anteponga el imagePath autodetectado (p. ej. "media/") a las rutas.
    delete (L.Icon.Default.prototype as unknown as { _getIconUrl?: unknown })._getIconUrl;
    L.Icon.Default.mergeOptions({
      iconRetinaUrl: 'assets/leaflet/marker-icon-2x.png',
      iconUrl: 'assets/leaflet/marker-icon.png',
      shadowUrl: 'assets/leaflet/marker-shadow.png'
    });
  }

  private initMap(): void {
    const c = this.camping();
    if (!c || this.map) {
      return;
    }

    const position: L.LatLngTuple = [c.latitude, c.longitude];
    this.map = L.map('camping-detail-map', {
      center: position,
      zoom: DETAIL_ZOOM
    });

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(this.map);

    L.marker(position).addTo(this.map).bindPopup(c.name);

    setTimeout(() => this.map?.invalidateSize(), 100);
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

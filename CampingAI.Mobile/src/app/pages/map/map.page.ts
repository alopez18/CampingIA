import { Component, NgZone, OnDestroy, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonSpinner,
  IonButton, IonIcon, ToastController
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { locateOutline, searchOutline } from 'ionicons/icons';
import * as L from 'leaflet';
import 'leaflet.markercluster';
import { Geolocation } from '@capacitor/geolocation';
import { CampingsService, MapBounds } from '../../services/campings.service';
import { Camping } from '../../models/camping.model';

const SPAIN_CENTER: L.LatLngTuple = [40.4168, -3.7038];
const SPAIN_ZOOM = 6;
const USER_ZOOM = 11;

@Component({
  selector: 'app-map',
  standalone: true,
  imports: [
    IonHeader, IonToolbar, IonTitle, IonContent, IonSpinner, IonButton, IonIcon
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Mapa</ion-title>
        <ion-button slot="end" fill="clear" color="light" (click)="centerOnUser()" aria-label="Mi ubicación">
          <ion-icon name="locate-outline"></ion-icon>
        </ion-button>
      </ion-toolbar>
    </ion-header>
    <ion-content>
      @if (loading()) {
        <div class="map-overlay">
          <ion-spinner></ion-spinner>
        </div>
      }
      <div id="map"></div>
      <ion-button
        class="search-area-btn"
        size="small"
        (click)="searchThisArea()"
        [disabled]="loading()">
        <ion-icon slot="start" name="search-outline"></ion-icon>
        Buscar en esta zona
      </ion-button>
    </ion-content>
  `,
  styles: [`
    ion-content { --background: transparent; }
    #map { position: absolute; inset: 0; width: 100%; height: 100%; z-index: 0; }
    .map-overlay {
      position: absolute; inset: 0; z-index: 1000;
      display: flex; align-items: center; justify-content: center;
      background: rgba(255, 255, 255, 0.5);
    }
    .search-area-btn {
      position: absolute; z-index: 1000;
      left: 50%; transform: translateX(-50%);
      bottom: 16px;
    }
  `]
})
export class MapPage implements OnDestroy {
  private readonly campingsService = inject(CampingsService);
  private readonly router = inject(Router);
  private readonly zone = inject(NgZone);
  private readonly toastController = inject(ToastController);

  readonly loading = signal(false);

  private map?: L.Map;
  private markersLayer?: L.MarkerClusterGroup;

  constructor() {
    addIcons({ locateOutline, searchOutline });
    this.configureDefaultIcon();
  }

  ionViewDidEnter(): void {
    this.initMap();
    this.loadCampings();
    this.tryCenterOnUser();
  }

  ionViewWillLeave(): void {
    this.destroyMap();
  }

  ngOnDestroy(): void {
    this.destroyMap();
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
    if (this.map) {
      this.map.invalidateSize();
      return;
    }

    this.map = L.map('map', {
      center: SPAIN_CENTER,
      zoom: SPAIN_ZOOM
    });

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(this.map);

    this.markersLayer = L.markerClusterGroup();
    this.map.addLayer(this.markersLayer);

    // Asegura el render correcto cuando el contenedor termina de dimensionarse.
    setTimeout(() => this.map?.invalidateSize(), 200);
  }

  private loadCampings(bounds?: MapBounds): void {
    this.loading.set(true);
    this.campingsService.getForMap(bounds).subscribe({
      next: campings => {
        // En la carga inicial (sin área) encuadramos a los resultados; al "buscar en esta zona"
        // mantenemos la vista actual del usuario.
        this.renderMarkers(campings, bounds === undefined);
        this.loading.set(false);
      },
      error: async () => {
        this.loading.set(false);
        await this.showToast('No se pudieron cargar los campings.');
      }
    });
  }

  /** Recarga los campings limitándose al área visible del mapa (bounding box). */
  searchThisArea(): void {
    if (!this.map) {
      return;
    }
    const bounds = this.map.getBounds();
    this.loadCampings({
      minLat: bounds.getSouth(),
      maxLat: bounds.getNorth(),
      minLng: bounds.getWest(),
      maxLng: bounds.getEast()
    });
  }

  private renderMarkers(campings: Camping[], fitToBounds = true): void {
    if (!this.map || !this.markersLayer) {
      return;
    }

    this.markersLayer.clearLayers();

    const markers: L.Marker[] = campings.map(camping => {
      const marker = L.marker([camping.latitude, camping.longitude]);
      marker.bindPopup(this.buildPopup(camping));
      marker.on('popupopen', () => this.wirePopupButton(camping.id));
      return marker;
    });

    this.markersLayer.addLayers(markers);

    if (fitToBounds && markers.length > 0) {
      this.map.fitBounds(this.markersLayer.getBounds().pad(0.1));
    }
  }

  private buildPopup(camping: Camping): string {
    const price = camping.pricePerNight != null
      ? `${camping.pricePerNight.toFixed(2)} €/noche`
      : '';
    return `
      <div style="min-width:160px">
        <strong>${this.escapeHtml(camping.name)}</strong>
        ${price ? `<div style="color:#666;margin:4px 0">${price}</div>` : ''}
        <button id="detail-${camping.id}" style="margin-top:6px;padding:6px 10px;border:none;border-radius:6px;background:var(--ion-color-primary,#3880ff);color:#fff;cursor:pointer">Ver detalle</button>
      </div>`;
  }

  private wirePopupButton(campingId: string): void {
    const button = document.getElementById(`detail-${campingId}`);
    button?.addEventListener('click', () => {
      this.zone.run(() => this.router.navigate(['/tabs/campings', campingId]));
    });
  }

  private async tryCenterOnUser(): Promise<void> {
    try {
      const permission = await Geolocation.checkPermissions();
      if (permission.location === 'denied') {
        return;
      }
      const position = await Geolocation.getCurrentPosition({ timeout: 8000 });
      this.map?.setView(
        [position.coords.latitude, position.coords.longitude],
        USER_ZOOM
      );
    } catch {
      // Sin permiso o sin señal: se mantiene el encuadre por defecto (España).
    }
  }

  async centerOnUser(): Promise<void> {
    try {
      const permission = await Geolocation.requestPermissions();
      if (permission.location === 'denied') {
        await this.showToast('Permiso de ubicación denegado.');
        return;
      }
      const position = await Geolocation.getCurrentPosition({ timeout: 8000 });
      this.map?.setView(
        [position.coords.latitude, position.coords.longitude],
        USER_ZOOM
      );
    } catch {
      await this.showToast('No se pudo obtener tu ubicación.');
    }
  }

  private destroyMap(): void {
    this.map?.remove();
    this.map = undefined;
    this.markersLayer = undefined;
  }

  private async showToast(message: string): Promise<void> {
    const toast = await this.toastController.create({
      message,
      duration: 2500,
      color: 'danger',
      position: 'bottom'
    });
    await toast.present();
  }

  private escapeHtml(value: string): string {
    const div = document.createElement('div');
    div.textContent = value;
    return div.innerHTML;
  }
}

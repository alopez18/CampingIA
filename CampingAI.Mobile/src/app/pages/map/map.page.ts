import { Component } from '@angular/core';
import {
  IonHeader, IonToolbar, IonTitle, IonContent
} from '@ionic/angular/standalone';

@Component({
  selector: 'app-map',
  standalone: true,
  imports: [IonHeader, IonToolbar, IonTitle, IonContent],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Mapa</ion-title>
      </ion-toolbar>
    </ion-header>
    <ion-content>
      <div id="map" style="width:100%;height:100%;display:flex;align-items:center;justify-content:center;flex-direction:column;">
        <p style="font-size:4em">🗺️</p>
        <p>Mapa interactivo — Fase 10</p>
        <p style="color:var(--ion-color-medium);font-size:0.85em">Se integrará Leaflet + OpenStreetMap</p>
      </div>
    </ion-content>
  `
})
export class MapPage {}

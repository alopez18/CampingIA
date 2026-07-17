import { Component, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe, SlicePipe } from '@angular/common';
import { IonCard, IonCardContent, IonButton, IonIcon } from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { heart, heartOutline, locationOutline } from 'ionicons/icons';
import { Camping } from '../../../models/camping.model';

@Component({
  selector: 'app-camping-card',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, SlicePipe, IonCard, IonCardContent, IonButton, IonIcon],
  template: `
    <ion-card class="camping-card" [routerLink]="['/tabs/campings', camping().id]">
      <div class="card-hero">
        <span class="hero-emoji" aria-hidden="true">🏕️</span>
        @if (categoryName()) {
          <span class="hero-category">{{ categoryName() }}</span>
        }
        <ion-button fill="clear" class="hero-fav" size="small" (click)="onFavoriteToggle($event)">
          <ion-icon
            slot="icon-only"
            [name]="isFavorite() ? 'heart' : 'heart-outline'"
            [color]="isFavorite() ? 'danger' : 'light'">
          </ion-icon>
        </ion-button>
      </div>
      <ion-card-content class="card-body">
        <p class="card-location">
          <ion-icon name="location-outline"></ion-icon>
          {{ provinceName() || 'Sin ubicación' }}
        </p>
        <h3 class="card-title">{{ camping().name }}</h3>
        <p class="card-desc">{{ camping().description | slice:0:80 }}...</p>
        <div class="price-pill">
          <span class="price-amount">{{ camping().pricePerNight | currency:'EUR':'symbol':'1.0-0' }}</span>
          <span class="price-unit">/noche</span>
        </div>
      </ion-card-content>
    </ion-card>
  `,
  styles: [`
    :host { display: block; }
    .camping-card { margin: 8px 16px; overflow: hidden; }
    .card-hero {
      height: 90px;
      background: linear-gradient(135deg, #2e7d32 0%, #0277bd 100%);
      position: relative;
      display: flex;
      align-items: center;
      justify-content: center;
      overflow: hidden;
    }
    .hero-emoji { font-size: 2.5em; filter: drop-shadow(0 2px 8px rgba(0,0,0,0.25)); pointer-events: none; }
    .hero-category {
      position: absolute;
      top: 8px;
      left: 8px;
      background: rgba(0,0,0,0.35);
      color: white;
      font-size: 0.72em;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      padding: 3px 10px;
      border-radius: 12px;
      backdrop-filter: blur(4px);
      pointer-events: none;
    }
    .hero-fav {
      position: absolute;
      top: 8px;
      right: 8px;
      --background: rgba(0,0,0,0.3);
      --border-radius: 50%;
      --padding-start: 8px;
      --padding-end: 8px;
      backdrop-filter: blur(4px);
      width: 36px;
      height: 36px;
    }
    .card-body { padding: 12px 14px 16px; }
    .card-location {
      display: flex;
      align-items: center;
      gap: 4px;
      font-size: 0.78em;
      text-transform: uppercase;
      letter-spacing: 0.8px;
      color: var(--ion-color-medium);
      margin: 0 0 4px;
    }
    .card-title { font-size: 1.05em; font-weight: 700; color: var(--ion-text-color); margin: 0 0 6px; line-height: 1.3; }
    .card-desc { font-size: 0.85em; color: var(--ion-color-medium); line-height: 1.4; margin: 0 0 12px; }
    .price-pill {
      background: var(--ion-color-primary, #2e7d32);
      color: white;
      border-radius: 20px;
      padding: 5px 14px;
      display: inline-flex;
      align-items: baseline;
      gap: 2px;
    }
    .price-amount { font-size: 1.05em; font-weight: 700; }
    .price-unit { font-size: 0.75em; opacity: 0.85; margin-left: 1px; }
  `]
})
export class CampingCardComponent {
  readonly camping = input.required<Camping>();
  readonly isFavorite = input<boolean>(false);
  readonly categoryName = input<string>('');
  readonly provinceName = input<string>('');
  readonly favoriteToggle = output<string>();

  constructor() {
    addIcons({ heart, heartOutline, locationOutline });
  }

  onFavoriteToggle(event: Event): void {
    event.stopPropagation();
    event.preventDefault();
    this.favoriteToggle.emit(this.camping().id);
  }
}

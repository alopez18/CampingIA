import { Component, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe, SlicePipe } from '@angular/common';
import {
  IonCard, IonCardHeader, IonCardTitle, IonCardSubtitle,
  IonCardContent, IonButton, IonIcon
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { heart, heartOutline, locationOutline, cashOutline } from 'ionicons/icons';
import { Camping } from '../../../models/camping.model';

@Component({
  selector: 'app-camping-card',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, SlicePipe, IonCard, IonCardHeader, IonCardTitle,
            IonCardSubtitle, IonCardContent, IonButton, IonIcon],
  template: `
    <ion-card [routerLink]="['/tabs/campings', camping().id]">
      <ion-card-header>
        <ion-card-title>{{ camping().name }}</ion-card-title>
        <ion-card-subtitle>
          <ion-icon name="location-outline"></ion-icon>
          {{ camping().provinciaId ?? 'Sin ubicación' }}
        </ion-card-subtitle>
      </ion-card-header>
      <ion-card-content>
        <p class="description">{{ camping().description | slice:0:100 }}...</p>
        <div class="card-footer">
          <span class="price">
            <ion-icon name="cash-outline"></ion-icon>
            {{ camping().pricePerNight | currency:'EUR':'symbol':'1.0-0' }}/noche
          </span>
          <ion-button fill="clear" size="small" (click)="onFavoriteToggle($event)">
            <ion-icon
              slot="icon-only"
              [name]="isFavorite() ? 'heart' : 'heart-outline'"
              [color]="isFavorite() ? 'danger' : 'medium'">
            </ion-icon>
          </ion-button>
        </div>
      </ion-card-content>
    </ion-card>
  `,
  styles: [`
    .description { color: var(--ion-color-medium); font-size: 0.9em; margin-bottom: 8px; }
    .card-footer { display: flex; justify-content: space-between; align-items: center; }
    .price { display: flex; align-items: center; gap: 4px; font-weight: 600; color: var(--ion-color-primary); }
  `]
})
export class CampingCardComponent {
  readonly camping = input.required<Camping>();
  readonly isFavorite = input<boolean>(false);
  readonly favoriteToggle = output<string>();

  constructor() {
    addIcons({ heart, heartOutline, locationOutline, cashOutline });
  }

  onFavoriteToggle(event: Event): void {
    event.stopPropagation();
    event.preventDefault();
    this.favoriteToggle.emit(this.camping().id);
  }
}

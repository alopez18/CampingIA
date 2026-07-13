import { Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonList, IonItemSliding,
  IonItem, IonLabel, IonItemOptions, IonItemOption, IonIcon,
  IonSpinner, IonText, IonRefresher, IonRefresherContent, ToastController
} from '@ionic/angular/standalone';
import { RouterLink } from '@angular/router';
import { addIcons } from 'ionicons';
import { trashOutline, locationOutline } from 'ionicons/icons';
import { FavoritesService } from '../../services/favorites.service';
import { CampingsService } from '../../services/campings.service';
import { Camping } from '../../models/camping.model';

@Component({
  selector: 'app-favorites',
  standalone: true,
  imports: [
    RouterLink, CurrencyPipe,
    IonHeader, IonToolbar, IonTitle, IonContent, IonList, IonItemSliding,
    IonItem, IonLabel, IonItemOptions, IonItemOption, IonIcon,
    IonSpinner, IonText, IonRefresher, IonRefresherContent
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Favoritos</ion-title>
      </ion-toolbar>
    </ion-header>
    <ion-content>
      <ion-refresher slot="fixed" (ionRefresh)="onRefresh($event)">
        <ion-refresher-content></ion-refresher-content>
      </ion-refresher>

      @if (loading()) {
        <div class="ion-text-center ion-padding"><ion-spinner></ion-spinner></div>
      } @else if (favCampings().length === 0) {
        <div class="ion-text-center ion-padding">
          <p style="font-size:3em">❤️</p>
          <ion-text color="medium">Aún no tienes favoritos.</ion-text>
        </div>
      } @else {
        <ion-list>
          @for (camping of favCampings(); track camping.id) {
            <ion-item-sliding>
              <ion-item [routerLink]="['/tabs/campings', camping.id]">
                <ion-icon name="location-outline" slot="start" color="primary"></ion-icon>
                <ion-label>
                  <h2>{{ camping.name }}</h2>
                  <p>{{ camping.pricePerNight | currency:'EUR' }} / noche</p>
                </ion-label>
              </ion-item>
              <ion-item-options side="end">
                <ion-item-option color="danger" (click)="removeFavorite(camping.id)">
                  <ion-icon name="trash-outline" slot="icon-only"></ion-icon>
                </ion-item-option>
              </ion-item-options>
            </ion-item-sliding>
          }
        </ion-list>
      }
    </ion-content>
  `
})
export class FavoritesPage implements OnInit {
  private readonly favoritesService = inject(FavoritesService);
  private readonly campingsService = inject(CampingsService);
  private readonly toast = inject(ToastController);

  readonly loading = signal(true);
  readonly favCampings = signal<Camping[]>([]);

  constructor() { addIcons({ trashOutline, locationOutline }); }

  ngOnInit(): void { this.loadFavorites(); }

  loadFavorites(): void {
    this.loading.set(true);
    this.favoritesService.getAll().subscribe({
      next: favs => {
        const ids = favs.map(f => f.campingId);
        if (ids.length === 0) { this.favCampings.set([]); this.loading.set(false); return; }
        // Carga los detalles de cada camping favorito
        Promise.all(
          ids.map(id => new Promise<Camping>(resolve =>
            this.campingsService.getById(id).subscribe({ next: resolve, error: () => resolve(null as unknown as Camping) })
          ))
        ).then(campings => {
          this.favCampings.set(campings.filter(Boolean));
          this.loading.set(false);
        });
      },
      error: () => this.loading.set(false)
    });
  }

  removeFavorite(campingId: string): void {
    this.favoritesService.remove(campingId).subscribe({
      next: () => this.favCampings.update(list => list.filter(c => c.id !== campingId))
    });
  }

  onRefresh(event: CustomEvent): void {
    this.loadFavorites();
    (event.target as HTMLIonRefresherElement).complete();
  }
}

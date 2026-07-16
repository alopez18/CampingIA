import { Component, inject, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonList, IonItemSliding,
  IonItem, IonLabel, IonItemOptions, IonItemOption, IonIcon,
  IonSpinner, IonText, IonRefresher, IonRefresherContent, ToastController,
  IonButtons, IonButton, IonCheckbox, IonFab, IonFabButton
} from '@ionic/angular/standalone';
import { Router, RouterLink } from '@angular/router';
import { addIcons } from 'ionicons';
import { trashOutline, locationOutline, gitCompareOutline, closeOutline } from 'ionicons/icons';
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
    IonSpinner, IonText, IonRefresher, IonRefresherContent,
    IonButtons, IonButton, IonCheckbox, IonFab, IonFabButton
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Favoritos</ion-title>
        @if (favCampings().length >= 2) {
          <ion-buttons slot="end">
            <ion-button (click)="toggleCompareMode()">
              <ion-icon slot="icon-only" [name]="compareMode() ? 'close-outline' : 'git-compare-outline'"></ion-icon>
            </ion-button>
          </ion-buttons>
        }
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
            @if (compareMode()) {
              <ion-item>
                <ion-checkbox
                  slot="start"
                  [checked]="selected().has(camping.id)"
                  (ionChange)="toggleSelection(camping.id)">
                </ion-checkbox>
                <ion-label>
                  <h2>{{ camping.name }}</h2>
                  <p>{{ camping.pricePerNight | currency:'EUR' }} / noche</p>
                </ion-label>
              </ion-item>
            } @else {
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
          }
        </ion-list>

        @if (compareMode()) {
          <ion-fab slot="fixed" vertical="bottom" horizontal="end">
            <ion-fab-button [disabled]="selected().size < 2" (click)="compareSelected()">
              <ion-icon name="git-compare-outline"></ion-icon>
            </ion-fab-button>
          </ion-fab>
        }
      }
    </ion-content>
  `
})
export class FavoritesPage {
  private readonly favoritesService = inject(FavoritesService);
  private readonly campingsService = inject(CampingsService);
  private readonly toast = inject(ToastController);
  private readonly router = inject(Router);

  readonly loading = signal(true);
  readonly favCampings = signal<Camping[]>([]);
  readonly compareMode = signal(false);
  readonly selected = signal<Set<string>>(new Set());

  constructor() { addIcons({ trashOutline, locationOutline, gitCompareOutline, closeOutline }); }

  ionViewWillEnter(): void { this.loadFavorites(); }

  toggleCompareMode(): void {
    this.compareMode.update(v => !v);
    this.selected.set(new Set());
  }

  toggleSelection(campingId: string): void {
    this.selected.update(set => {
      const next = new Set(set);
      if (next.has(campingId)) next.delete(campingId);
      else next.add(campingId);
      return next;
    });
  }

  compareSelected(): void {
    const ids = [...this.selected()];
    if (ids.length < 2) return;
    this.router.navigate(['/tabs/compare'], { queryParams: { ids: ids.join(',') } });
  }

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

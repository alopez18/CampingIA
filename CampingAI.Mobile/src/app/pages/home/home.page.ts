import { Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonSearchbar,
  IonButton, IonIcon, IonGrid, IonRow, IonCol, IonSpinner,
  IonRefresher, IonRefresherContent
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { optionsOutline } from 'ionicons/icons';
import { CampingsService } from '../../services/campings.service';
import { FavoritesService } from '../../services/favorites.service';
import { CampingCardComponent } from '../../shared/components/camping-card/camping-card.component';
import { Camping } from '../../models/camping.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    FormsModule, CampingCardComponent,
    IonHeader, IonToolbar, IonTitle, IonContent, IonSearchbar,
    IonButton, IonIcon, IonGrid, IonRow, IonCol, IonSpinner,
    IonRefresher, IonRefresherContent
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>🏕️ CampingAI</ion-title>
      </ion-toolbar>
      <ion-toolbar>
        <ion-searchbar
          [(ngModel)]="searchText"
          (ionInput)="onSearch()"
          placeholder="Buscar campings..."
          debounce="400">
        </ion-searchbar>
        <ion-button slot="end" fill="clear" (click)="openFilters()">
          <ion-icon name="options-outline"></ion-icon>
        </ion-button>
      </ion-toolbar>
    </ion-header>
    <ion-content>
      <ion-refresher slot="fixed" (ionRefresh)="onRefresh($event)">
        <ion-refresher-content></ion-refresher-content>
      </ion-refresher>

      @if (loading()) {
        <div class="ion-text-center ion-padding"><ion-spinner></ion-spinner></div>
      } @else {
        <section class="ion-padding-horizontal">
          <h2>Destacados</h2>
          <ion-grid>
            <ion-row>
              @for (camping of featured(); track camping.id) {
                <ion-col size="12" size-md="6">
                  <app-camping-card
                    [camping]="camping"
                    [isFavorite]="favoritesService.isFavorite(camping.id)"
                    (favoriteToggle)="onFavoriteToggle($event)">
                  </app-camping-card>
                </ion-col>
              }
            </ion-row>
          </ion-grid>
        </section>
      }
    </ion-content>
  `
})
export class HomePage implements OnInit {
  private readonly campings = inject(CampingsService);
  readonly favoritesService = inject(FavoritesService);
  private readonly router = inject(Router);

  readonly featured = signal<Camping[]>([]);
  readonly loading = signal(true);
  searchText = '';

  constructor() { addIcons({ optionsOutline }); }

  ngOnInit(): void { this.loadFeatured(); }

  loadFeatured(): void {
    this.loading.set(true);
    this.campings.getAll(1, 6).subscribe({
      next: res => { this.featured.set(res.items); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  onSearch(): void {
    this.router.navigate(['/tabs/campings'], {
      queryParams: { name: this.searchText }
    });
  }

  openFilters(): void {
    this.router.navigate(['/tabs/campings']);
  }

  onFavoriteToggle(campingId: string): void {
    this.favoritesService.toggle(campingId).subscribe();
  }

  onRefresh(event: CustomEvent): void {
    this.loadFeatured();
    (event.target as HTMLIonRefresherElement).complete();
  }
}

import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonSearchbar,
  IonInfiniteScroll, IonInfiniteScrollContent, IonSpinner,
  IonButton, IonIcon, IonRefresher, IonRefresherContent, IonText
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { filterOutline } from 'ionicons/icons';
import { CampingsService } from '../../../services/campings.service';
import { FavoritesService } from '../../../services/favorites.service';
import { CampingCardComponent } from '../../../shared/components/camping-card/camping-card.component';
import { Camping, CampingSearchParams } from '../../../models/camping.model';

@Component({
  selector: 'app-campings-list',
  standalone: true,
  imports: [
    FormsModule, CampingCardComponent,
    IonHeader, IonToolbar, IonTitle, IonContent, IonSearchbar,
    IonInfiniteScroll, IonInfiniteScrollContent, IonSpinner,
    IonButton, IonIcon, IonRefresher, IonRefresherContent, IonText
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Campings</ion-title>
      </ion-toolbar>
      <ion-toolbar>
        <ion-searchbar
          [(ngModel)]="filters.name"
          (ionInput)="onFilterChange()"
          placeholder="Nombre del camping..."
          debounce="400">
        </ion-searchbar>
        <ion-button slot="end" fill="clear">
          <ion-icon name="filter-outline"></ion-icon>
        </ion-button>
      </ion-toolbar>
    </ion-header>
    <ion-content>
      <ion-refresher slot="fixed" (ionRefresh)="onRefresh($event)">
        <ion-refresher-content></ion-refresher-content>
      </ion-refresher>

      @if (loading() && campings().length === 0) {
        <div class="ion-text-center ion-padding"><ion-spinner></ion-spinner></div>
      } @else if (campings().length === 0) {
        <div class="ion-text-center ion-padding">
          <ion-text color="medium">No se encontraron campings.</ion-text>
        </div>
      } @else {
        @for (camping of campings(); track camping.id) {
          <app-camping-card
            [camping]="camping"
            [isFavorite]="favoritesService.isFavorite(camping.id)"
            (favoriteToggle)="onFavoriteToggle($event)">
          </app-camping-card>
        }
      }

      <ion-infinite-scroll (ionInfinite)="loadMore($event)" [disabled]="allLoaded()">
        <ion-infinite-scroll-content></ion-infinite-scroll-content>
      </ion-infinite-scroll>
    </ion-content>
  `
})
export class CampingsListPage implements OnInit {
  private readonly campingsService = inject(CampingsService);
  readonly favoritesService = inject(FavoritesService);
  private readonly route = inject(ActivatedRoute);

  readonly campings = signal<Camping[]>([]);
  readonly loading = signal(false);
  readonly allLoaded = signal(false);

  filters: CampingSearchParams = { page: 1, pageSize: 10 };

  constructor() { addIcons({ filterOutline }); }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      if (params['name']) this.filters.name = params['name'];
      this.resetAndLoad();
    });
  }

  resetAndLoad(): void {
    this.filters.page = 1;
    this.campings.set([]);
    this.allLoaded.set(false);
    this.loadCampings();
  }

  loadCampings(): void {
    this.loading.set(true);
    this.campingsService.search(this.filters).subscribe({
      next: res => {
        this.campings.update(prev => [...prev, ...res.items]);
        this.allLoaded.set(res.items.length < (this.filters.pageSize ?? 10));
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  loadMore(event: CustomEvent): void {
    this.filters.page = (this.filters.page ?? 1) + 1;
    this.loadCampings();
    setTimeout(() => (event.target as HTMLIonInfiniteScrollElement).complete(), 500);
  }

  onFilterChange(): void { this.resetAndLoad(); }

  onFavoriteToggle(campingId: string): void {
    this.favoritesService.toggle(campingId).subscribe();
  }

  onRefresh(event: CustomEvent): void {
    this.resetAndLoad();
    (event.target as HTMLIonRefresherElement).complete();
  }
}

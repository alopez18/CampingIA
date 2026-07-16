import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonSearchbar,
  IonInfiniteScroll, IonInfiniteScrollContent, IonSpinner,
  IonButton, IonIcon, IonRefresher, IonRefresherContent, IonText,
  IonModal, IonButtons, IonList, IonItem, IonSelect,
  IonSelectOption, IonInput
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { filterOutline } from 'ionicons/icons';
import { CampingsService } from '../../../services/campings.service';
import { FavoritesService } from '../../../services/favorites.service';
import { LocationService } from '../../../services/location.service';
import { CategoriesService } from '../../../services/categories.service';
import { FacilitiesService } from '../../../services/facilities.service';
import { CampingCardComponent } from '../../../shared/components/camping-card/camping-card.component';
import { Camping, CampingSearchParams } from '../../../models/camping.model';
import { Province } from '../../../models/location.model';
import { Category } from '../../../models/category.model';
import { Facility } from '../../../models/facility.model';

@Component({
  selector: 'app-campings-list',
  standalone: true,
  imports: [
    FormsModule, CampingCardComponent,
    IonHeader, IonToolbar, IonTitle, IonContent, IonSearchbar,
    IonInfiniteScroll, IonInfiniteScrollContent, IonSpinner,
    IonButton, IonIcon, IonRefresher, IonRefresherContent, IonText,
    IonModal, IonButtons, IonList, IonItem, IonSelect,
    IonSelectOption, IonInput
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
        <ion-button slot="end" fill="clear" (click)="openFilters()">
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

    <ion-modal [isOpen]="showFilters()" (didDismiss)="closeFilters()">
      <ng-template>
        <ion-header>
          <ion-toolbar color="primary">
            <ion-buttons slot="start">
              <ion-button (click)="closeFilters()">Cancelar</ion-button>
            </ion-buttons>
            <ion-title>Búsqueda avanzada</ion-title>
            <ion-buttons slot="end">
              <ion-button (click)="clearFilters()">Limpiar</ion-button>
            </ion-buttons>
          </ion-toolbar>
        </ion-header>
        <ion-content class="ion-padding">
          <ion-list>
            <ion-item>
              <ion-select
                label="Provincia"
                labelPlacement="stacked"
                interface="popover"
                placeholder="Todas"
                [(ngModel)]="draft.provinciaId">
                <ion-select-option [value]="undefined">Todas</ion-select-option>
                @for (province of provinces(); track province.id) {
                  <ion-select-option [value]="province.id">{{ province.name }}</ion-select-option>
                }
              </ion-select>
            </ion-item>
            <ion-item>
              <ion-select
                label="Categoría"
                labelPlacement="stacked"
                interface="popover"
                placeholder="Todas"
                [(ngModel)]="draft.categoryId">
                <ion-select-option [value]="undefined">Todas</ion-select-option>
                @for (category of categories(); track category.id) {
                  <ion-select-option [value]="category.id">{{ category.name }}</ion-select-option>
                }
              </ion-select>
            </ion-item>
            <ion-item>
              <ion-input
                label="Precio mínimo (€)"
                labelPlacement="stacked"
                type="number"
                inputmode="numeric"
                min="0"
                placeholder="Sin mínimo"
                [(ngModel)]="draft.minPrice">
              </ion-input>
            </ion-item>
            <ion-item>
              <ion-input
                label="Precio máximo (€)"
                labelPlacement="stacked"
                type="number"
                inputmode="numeric"
                min="0"
                placeholder="Sin máximo"
                [(ngModel)]="draft.maxPrice">
              </ion-input>
            </ion-item>
            <ion-item>
              <ion-select
                label="Instalaciones"
                labelPlacement="stacked"
                [multiple]="true"
                placeholder="Cualquiera"
                [(ngModel)]="draft.facilityIds">
                @for (facility of facilities(); track facility.id) {
                  <ion-select-option [value]="facility.id">{{ facility.name }}</ion-select-option>
                }
              </ion-select>
            </ion-item>
          </ion-list>
          <ion-button expand="block" class="ion-margin-top" (click)="applyFilters()">
            Aplicar filtros
          </ion-button>
        </ion-content>
      </ng-template>
    </ion-modal>
  `
})
export class CampingsListPage implements OnInit {
  private readonly campingsService = inject(CampingsService);
  readonly favoritesService = inject(FavoritesService);
  private readonly locationService = inject(LocationService);
  private readonly categoriesService = inject(CategoriesService);
  private readonly facilitiesService = inject(FacilitiesService);
  private readonly route = inject(ActivatedRoute);

  readonly campings = signal<Camping[]>([]);
  readonly loading = signal(false);
  readonly allLoaded = signal(false);
  readonly showFilters = signal(false);
  readonly provinces = signal<Province[]>([]);
  readonly categories = signal<Category[]>([]);
  readonly facilities = signal<Facility[]>([]);

  filters: CampingSearchParams = { page: 1, pageSize: 10 };
  draft: { provinciaId?: string; categoryId?: string; minPrice?: number; maxPrice?: number; facilityIds?: string[] } = {};

  constructor() { addIcons({ filterOutline }); }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      if (params['name']) this.filters.name = params['name'];
      this.resetAndLoad();
    });
  }

  openFilters(): void {
    if (this.provinces().length === 0) {
      this.locationService.getProvinces().subscribe(p => this.provinces.set(p));
    }
    if (this.categories().length === 0) {
      this.categoriesService.getCategories().subscribe(c => this.categories.set(c));
    }
    if (this.facilities().length === 0) {
      this.facilitiesService.getFacilities().subscribe(f => this.facilities.set(f));
    }
    this.draft = {
      provinciaId: this.filters.provinciaId,
      categoryId: this.filters.categoryIds?.[0],
      minPrice: this.filters.minPrice,
      maxPrice: this.filters.maxPrice,
      facilityIds: this.filters.facilityIds ? [...this.filters.facilityIds] : undefined
    };
    this.showFilters.set(true);
  }

  closeFilters(): void { this.showFilters.set(false); }

  applyFilters(): void {
    this.filters.provinciaId = this.draft.provinciaId || undefined;
    this.filters.categoryIds = this.draft.categoryId ? [this.draft.categoryId] : undefined;
    this.filters.minPrice = this.draft.minPrice != null ? Number(this.draft.minPrice) : undefined;
    this.filters.maxPrice = this.draft.maxPrice != null ? Number(this.draft.maxPrice) : undefined;
    this.filters.facilityIds = this.draft.facilityIds?.length ? [...this.draft.facilityIds] : undefined;
    this.closeFilters();
    this.resetAndLoad();
  }

  clearFilters(): void {
    this.draft = {};
    this.filters = { page: 1, pageSize: 10, name: this.filters.name };
    this.closeFilters();
    this.resetAndLoad();
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

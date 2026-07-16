import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonSearchbar,
  IonInfiniteScroll, IonInfiniteScrollContent, IonSpinner,
  IonText, IonButtons, IonBackButton, IonChip, IonIcon, IonLabel
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { sparklesOutline } from 'ionicons/icons';
import { AiService } from '../../../services/ai.service';
import { FavoritesService } from '../../../services/favorites.service';
import { CampingCardComponent } from '../../../shared/components/camping-card/camping-card.component';
import { Camping } from '../../../models/camping.model';

@Component({
  selector: 'app-ai-search',
  standalone: true,
  imports: [
    FormsModule, CampingCardComponent,
    IonHeader, IonToolbar, IonTitle, IonContent, IonSearchbar,
    IonInfiniteScroll, IonInfiniteScrollContent, IonSpinner,
    IonText, IonButtons, IonBackButton, IonChip, IonIcon, IonLabel
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-buttons slot="start">
          <ion-back-button defaultHref="/tabs/home"></ion-back-button>
        </ion-buttons>
        <ion-title>✨ Búsqueda inteligente</ion-title>
      </ion-toolbar>
      <ion-toolbar>
        <ion-searchbar
          [(ngModel)]="query"
          (ionChange)="onSearch()"
          (keyup.enter)="onSearch()"
          placeholder="Ej: camping familiar con piscina cerca de la playa"
          search-icon="sparkles-outline"
          debounce="0">
        </ion-searchbar>
      </ion-toolbar>
    </ion-header>
    <ion-content>
      @if (!searched()) {
        <div class="ai-hint ion-padding ion-text-center">
          <ion-icon name="sparkles-outline" class="ai-hint-icon"></ion-icon>
          <p>Describe con tus palabras el camping que buscas y la IA encontrará las mejores opciones.</p>
          <div class="ai-suggestions">
            @for (example of examples; track example) {
              <ion-chip (click)="useExample(example)">
                <ion-label>{{ example }}</ion-label>
              </ion-chip>
            }
          </div>
        </div>
      }

      @if (loading() && campings().length === 0) {
        <div class="ion-text-center ion-padding"><ion-spinner></ion-spinner></div>
      } @else if (searched() && campings().length === 0 && !loading()) {
        <div class="ion-text-center ion-padding">
          <ion-text color="medium">No se encontraron campings para tu búsqueda.</ion-text>
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
  `,
  styles: [`
    .ai-hint { color: var(--ion-color-medium); }
    .ai-hint-icon { font-size: 3em; color: var(--ion-color-primary); margin-bottom: 8px; }
    .ai-suggestions { display: flex; flex-wrap: wrap; gap: 6px; justify-content: center; margin-top: 12px; }
  `]
})
export class AiSearchPage {
  private readonly aiService = inject(AiService);
  readonly favoritesService = inject(FavoritesService);

  readonly campings = signal<Camping[]>([]);
  readonly loading = signal(false);
  readonly allLoaded = signal(false);
  readonly searched = signal(false);

  query = '';
  private page = 1;
  private readonly pageSize = 10;

  readonly examples = [
    'Camping familiar con piscina cerca de la playa',
    'Camping tranquilo en la montaña que admita mascotas',
    'Camping barato en Tarragona con wifi'
  ];

  constructor() { addIcons({ sparklesOutline }); }

  useExample(example: string): void {
    this.query = example;
    this.onSearch();
  }

  onSearch(): void {
    const trimmed = this.query.trim();
    if (!trimmed) return;

    this.page = 1;
    this.campings.set([]);
    this.allLoaded.set(false);
    this.searched.set(true);
    this.loadResults();
  }

  private loadResults(): void {
    this.loading.set(true);
    this.aiService.search(this.query.trim(), this.page, this.pageSize).subscribe({
      next: res => {
        this.campings.update(prev => [...prev, ...res.items]);
        this.allLoaded.set(res.items.length < this.pageSize);
        this.loading.set(false);
      },
      error: () => {
        this.allLoaded.set(true);
        this.loading.set(false);
      }
    });
  }

  loadMore(event: CustomEvent): void {
    this.page += 1;
    this.loadResults();
    setTimeout(() => (event.target as HTMLIonInfiniteScrollElement).complete(), 500);
  }

  onFavoriteToggle(campingId: string): void {
    this.favoritesService.toggle(campingId).subscribe();
  }
}

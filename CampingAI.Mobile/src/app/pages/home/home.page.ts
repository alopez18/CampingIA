import { Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonSearchbar,
  IonButton, IonIcon, IonGrid, IonRow, IonCol, IonSpinner,
  IonRefresher, IonRefresherContent
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { optionsOutline, sparklesOutline } from 'ionicons/icons';
import { CampingsService } from '../../services/campings.service';
import { FavoritesService } from '../../services/favorites.service';
import { AiService } from '../../services/ai.service';
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

      <div class="home-hero">
        <div class="home-hero-text">
          <p class="hero-eyebrow">🌿 Explora España</p>
          <h2 class="hero-heading">Los mejores campings te esperan</h2>
        </div>
        <span class="hero-art" aria-hidden="true">🏕️</span>
      </div>

      <ion-button class="ai-search-cta" expand="block" (click)="openAiSearch()">
        <ion-icon name="sparkles-outline" slot="start"></ion-icon>
        Búsqueda inteligente con IA
      </ion-button>

      @if (loading()) {
        <div class="ion-text-center ion-padding"><ion-spinner></ion-spinner></div>
      } @else {
        @if (recommendations().length > 0) {
          <section>
            <div class="section-header">
              <h3 class="section-title">✨ Recomendado para ti</h3>
            </div>
            @if (reasoning()) {
              <p class="ai-reasoning">{{ reasoning() }}</p>
            }
            <ion-grid>
              <ion-row>
                @for (camping of recommendations(); track camping.id) {
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

        <section>
          <div class="section-header">
            <h3 class="section-title">⭐ Destacados</h3>
          </div>
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
  `,
  styles: [`
    .home-hero {
      background: linear-gradient(145deg, #1b5e20 0%, #2e7d32 60%, #43a047 100%);
      padding: 28px 20px 32px;
      display: flex;
      justify-content: space-between;
      align-items: flex-end;
      overflow: hidden;
    }
    .home-hero-text { flex: 1; }
    .hero-eyebrow { color: rgba(255,255,255,0.75); font-size: 0.82em; text-transform: uppercase; letter-spacing: 1.5px; margin: 0 0 6px; }
    .hero-heading { color: white; font-size: 1.45em; font-weight: 800; margin: 0; line-height: 1.25; letter-spacing: -0.3px; }
    .hero-art { font-size: 3.8em; opacity: 0.4; line-height: 1; margin-left: 8px; }
    .section-header { padding: 20px 16px 4px; }
    .section-title { font-size: 1.05em; font-weight: 700; color: var(--ion-text-color); margin: 0; }
    .ai-search-cta { margin: 16px; --border-radius: 12px; }
    .ai-reasoning { padding: 4px 16px 8px; margin: 0; font-size: 0.85em; font-style: italic; color: var(--ion-color-medium); }
  `]
})
export class HomePage implements OnInit {
  private readonly campings = inject(CampingsService);
  readonly favoritesService = inject(FavoritesService);
  private readonly aiService = inject(AiService);
  private readonly router = inject(Router);

  readonly featured = signal<Camping[]>([]);
  readonly recommendations = signal<Camping[]>([]);
  readonly reasoning = signal<string>('');
  readonly loading = signal(true);
  searchText = '';

  constructor() { addIcons({ optionsOutline, sparklesOutline }); }

  ngOnInit(): void {
    this.loadFeatured();
    this.loadRecommendations();
  }

  loadFeatured(): void {
    this.loading.set(true);
    this.campings.getAll(1, 6).subscribe({
      next: res => { this.featured.set(res.items); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  loadRecommendations(): void {
    this.aiService.getRecommendations(4).subscribe({
      next: res => { this.recommendations.set(res.items); this.reasoning.set(res.reasoning); },
      error: () => { this.recommendations.set([]); this.reasoning.set(''); }
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

  openAiSearch(): void {
    this.router.navigate(['/tabs/ai-search']);
  }

  onFavoriteToggle(campingId: string): void {
    this.favoritesService.toggle(campingId).subscribe();
  }

  onRefresh(event: CustomEvent): void {
    this.loadFeatured();
    this.loadRecommendations();
    (event.target as HTMLIonRefresherElement).complete();
  }
}

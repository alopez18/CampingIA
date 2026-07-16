import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonSpinner, IonText,
  IonButtons, IonBackButton, IonCard, IonCardHeader, IonCardTitle,
  IonCardContent, IonIcon, IonChip, IonLabel
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { trophyOutline, cashOutline } from 'ionicons/icons';
import { AiService } from '../../../services/ai.service';
import { CampingsService } from '../../../services/campings.service';
import { AiCompareResponse } from '../../../models/ai.model';
import { Camping } from '../../../models/camping.model';

@Component({
  selector: 'app-camping-compare',
  standalone: true,
  imports: [
    IonHeader, IonToolbar, IonTitle, IonContent, IonSpinner, IonText,
    IonButtons, IonBackButton, IonCard, IonCardHeader, IonCardTitle,
    IonCardContent, IonIcon, IonChip, IonLabel
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-buttons slot="start">
          <ion-back-button defaultHref="/tabs/favorites"></ion-back-button>
        </ion-buttons>
        <ion-title>⚖️ Comparador IA</ion-title>
      </ion-toolbar>
    </ion-header>
    <ion-content>
      @if (loading()) {
        <div class="ion-text-center ion-padding"><ion-spinner></ion-spinner></div>
      } @else if (error()) {
        <div class="ion-text-center ion-padding">
          <ion-text color="medium">{{ error() }}</ion-text>
        </div>
      } @else if (result()) {
        <ion-card>
          <ion-card-header>
            <ion-card-title>Resumen</ion-card-title>
          </ion-card-header>
          <ion-card-content>
            <p class="compare-summary">{{ result()!.summary }}</p>
            <div class="compare-badges">
              @if (bestOverall()) {
                <ion-chip color="warning">
                  <ion-icon name="trophy-outline"></ion-icon>
                  <ion-label>Mejor opción: {{ bestOverall()!.name }}</ion-label>
                </ion-chip>
              }
              @if (bestForBudget()) {
                <ion-chip color="success">
                  <ion-icon name="cash-outline"></ion-icon>
                  <ion-label>Mejor precio: {{ bestForBudget()!.name }}</ion-label>
                </ion-chip>
              }
            </div>
          </ion-card-content>
        </ion-card>

        @for (camping of campings(); track camping.id) {
          <ion-card>
            <ion-card-header>
              <ion-card-title>{{ camping.name }}</ion-card-title>
            </ion-card-header>
            <ion-card-content>
              <p>{{ camping.pricePerNight }} € / noche</p>
              <p>{{ camping.description }}</p>
            </ion-card-content>
          </ion-card>
        }
      }
    </ion-content>
  `,
  styles: [`
    .compare-summary { margin: 0 0 12px; line-height: 1.5; }
    .compare-badges { display: flex; flex-wrap: wrap; gap: 6px; }
  `]
})
export class CampingComparePage {
  private readonly aiService = inject(AiService);
  private readonly campingsService = inject(CampingsService);
  private readonly route = inject(ActivatedRoute);

  readonly loading = signal(true);
  readonly error = signal<string>('');
  readonly result = signal<AiCompareResponse | null>(null);
  readonly campings = signal<Camping[]>([]);

  constructor() {
    addIcons({ trophyOutline, cashOutline });
    const ids = (this.route.snapshot.queryParamMap.get('ids') ?? '')
      .split(',')
      .map(id => id.trim())
      .filter(Boolean);
    this.load(ids);
  }

  bestOverall(): Camping | undefined {
    const id = this.result()?.bestOverall;
    return id ? this.campings().find(c => c.id === id) : undefined;
  }

  bestForBudget(): Camping | undefined {
    const id = this.result()?.bestForBudget;
    return id ? this.campings().find(c => c.id === id) : undefined;
  }

  private load(ids: string[]): void {
    if (ids.length < 2) {
      this.error.set('Selecciona al menos 2 campings para comparar.');
      this.loading.set(false);
      return;
    }

    Promise.all(
      ids.map(id => new Promise<Camping>(resolve =>
        this.campingsService.getById(id).subscribe({ next: resolve, error: () => resolve(null as unknown as Camping) })
      ))
    ).then(campings => this.campings.set(campings.filter(Boolean)));

    this.aiService.compare(ids).subscribe({
      next: res => { this.result.set(res); this.loading.set(false); },
      error: () => {
        this.error.set('No se pudo generar la comparación.');
        this.loading.set(false);
      }
    });
  }
}

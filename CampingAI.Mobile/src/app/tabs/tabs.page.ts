import { Component } from '@angular/core';
import { IonTabs, IonTabBar, IonTabButton, IonIcon, IonLabel } from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { homeOutline, searchOutline, mapOutline, heartOutline, personOutline } from 'ionicons/icons';
import { Capacitor } from '@capacitor/core';

@Component({
  selector: 'app-tabs',
  templateUrl: 'tabs.page.html',
  styleUrls: ['tabs.page.scss'],
  imports: [IonTabs, IonTabBar, IonTabButton, IonIcon, IonLabel],
})
export class TabsPage {
  // En web (navegador) la barra de navegación se coloca arriba (aspecto escritorio);
  // en la app nativa se mantiene abajo (aspecto móvil).
  readonly isWeb = !Capacitor.isNativePlatform();
  readonly tabBarSlot = this.isWeb ? 'top' : 'bottom';

  constructor() {
    addIcons({ homeOutline, searchOutline, mapOutline, heartOutline, personOutline });
  }
}

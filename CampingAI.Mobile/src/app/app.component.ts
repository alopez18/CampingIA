import { Component } from '@angular/core';
import { IonApp, IonRouterOutlet } from '@ionic/angular/standalone';
import { Capacitor } from '@capacitor/core';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  imports: [IonApp, IonRouterOutlet],
})
export class AppComponent {
  constructor() {
    // Aplica una apariencia de escritorio cuando la app corre en navegador (build web)
    // y mantiene la apariencia móvil cuando corre como app nativa (Android/iOS).
    const isNative = Capacitor.isNativePlatform();
    document.body.classList.add(isNative ? 'platform-native' : 'platform-web');
  }
}

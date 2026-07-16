import { bootstrapApplication } from '@angular/platform-browser';
import { RouteReuseStrategy, provideRouter, withPreloading, PreloadAllModules } from '@angular/router';
import { IonicRouteStrategy, provideIonicAngular } from '@ionic/angular/standalone';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { isDevMode } from '@angular/core';
import { provideServiceWorker } from '@angular/service-worker';

import { routes } from './app/app.routes';
import { AppComponent } from './app/app.component';
import { jwtInterceptor } from './app/core/interceptors/jwt.interceptor';

// Google OAuth popup callback: el servidor de desarrollo sirve index.html para cualquier ruta,
// por lo que el popup carga la app. Detectamos el id_token antes de arrancar Angular.
const _oauthHash = window.location.hash.startsWith('#')
  ? new URLSearchParams(window.location.hash.slice(1))
  : null;

if (window.opener && _oauthHash && (_oauthHash.has('id_token') || _oauthHash.has('error'))) {
  const idToken = _oauthHash.get('id_token');
  const error   = _oauthHash.get('error');
  window.opener.postMessage(
    idToken ? { google_id_token: idToken } : { google_error: error ?? 'unknown' },
    window.location.origin
  );
  window.close();
} else {
  bootstrapApplication(AppComponent, {
    providers: [
      { provide: RouteReuseStrategy, useClass: IonicRouteStrategy },
      provideIonicAngular(),
      provideRouter(routes, withPreloading(PreloadAllModules)),
      provideHttpClient(withInterceptors([jwtInterceptor])),
      provideServiceWorker('ngsw-worker.js', {
        enabled: !isDevMode(),
        registrationStrategy: 'registerWhenStable:30000'
      })
    ],
  });
}

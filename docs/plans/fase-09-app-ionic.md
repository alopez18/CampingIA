# Fase 9 — Aplicación Ionic

> Objetivo: app multiplataforma (Android, iOS, Web PWA) con Ionic + Angular + Capacitor.

## Setup del proyecto
- [x] Crear proyecto `CampingAI.Mobile` (Ionic + Angular standalone).
- [x] Configurar Capacitor (Android + iOS) y PWA.
- [x] Configurar cliente HTTP + interceptor JWT (token del login).
- [x] Servicios Angular por dominio (auth, campings, favorites, reservations).

## Pantallas
### Autenticación
- [x] Login
- [x] Registro

### Home
- [x] Buscador (texto + filtros)
- [x] Destacados

### Campings
- [x] Listado (paginado/scroll infinito)
- [x] Detalle (categorías, servicios, precio, mapa)

### Mapa
- [ ] Vista geográfica (integra Fase 10)

### Favoritos
- [x] Listado y gestión

### Reservas
- [x] Historial y creación

### Perfil
- [x] Información del usuario

## Integración
- [x] Consumir endpoints de Fases 4–8.
- [x] Guardado seguro del token.

## Criterio de aceptación
- App navegable en Web PWA y compilable a Android/iOS consumiendo la API real.

## Registro de implementación

- Fecha: 2026-07-12. Autor: Copilot.

### Verificación y completado de pendientes

**Ficheros creados:**
- `CampingAI.Mobile/src/manifest.webmanifest` — Manifiesto PWA (nombre, iconos, colores, display standalone).
- `CampingAI.Mobile/ngsw-config.json` — Configuración del Angular Service Worker (caché de assets y API de campings).

**Ficheros modificados:**
- `CampingAI.Mobile/angular.json` — Añadido `manifest.webmanifest` a assets; activado `serviceWorker: "ngsw-config.json"` en la configuración de producción.
- `CampingAI.Mobile/src/index.html` — Añadidos `<link rel="manifest">` y `<meta name="theme-color">`.
- `CampingAI.Mobile/src/main.ts` — Registrado `provideServiceWorker('ngsw-worker.js')` (habilitado solo en producción).
- `CampingAI.Mobile/package.json` / `package-lock.json` — Añadidos `@angular/pwa` y `@angular/service-worker`.

**Comandos ejecutados:**
- `npm install @angular/pwa @angular/service-worker --save --legacy-peer-deps` — Instalación de paquetes PWA.
- `npx cap add android` — Generación del proyecto nativo Android (carpeta `android/` con Gradle, `MainActivity.java`, `AndroidManifest.xml`).
- `npx ng build --configuration=production` ✅ — Build de producción que genera `ngsw-worker.js`, `ngsw.json`, `manifest.webmanifest`.

**Nota sobre iOS:** La generación del proyecto nativo iOS (`cap add ios`) requiere macOS + Xcode y no se puede ejecutar en Windows. Pendiente de ejecutar en entorno macOS.

**Resultado:** `ng build --configuration=production` ✅ — Service Worker y PWA operativos en Web.


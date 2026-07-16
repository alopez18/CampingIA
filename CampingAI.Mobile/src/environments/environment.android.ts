// Android production environment.
// Used when building the APK (project: MÓVIL / Android).
// Swapped in via angular.json fileReplacements for the `android` configuration.
export const environment = {
  production: true,
  platform: 'android',
  apiUrl: 'https://campingaibackoffice.runasp.net/api',
  // Web Client ID del proyecto MÓVIL de Google Cloud.
  // Es el `webClientId` que recibe el plugin nativo y el `aud` del idToken.
  googleClientId: '372900543223-9dgig7pl0bhb684vnkqhct1j2u0h71e6.apps.googleusercontent.com'
};

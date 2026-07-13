import type { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'es.campingai.app',
  appName: 'CampingAI',
  webDir: 'www',
  plugins: {
    SplashScreen: {
      launchShowDuration: 0
    }
  },
  server: {
    // Solo para desarrollo local con dispositivo físico: reemplaza con la IP de tu máquina
    // androidScheme: 'https'
  }
};

export default config;

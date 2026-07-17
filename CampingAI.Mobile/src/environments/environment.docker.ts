// Docker environment.
// Used when building for the Docker container (project: DOCKER).
// Swapped in via angular.json fileReplacements for the `docker` configuration.
// nginx proxies /api → campingai-webapi:8080/api, so no CORS needed.
export const environment = {
  production: true,
  platform: 'web',
  apiUrl: '/api',
  googleClientId: 'REPLACE_WITH_WEB_CLIENT_ID'
};

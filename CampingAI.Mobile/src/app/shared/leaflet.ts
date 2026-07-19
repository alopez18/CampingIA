// Configuración global de Leaflet.
//
// El plugin `leaflet.markercluster`, una vez empaquetado por esbuild (el bundler
// de Angular), busca un objeto `L` global (`window.L`) para aumentarlo con
// `markerClusterGroup`. Además, `import * as L from 'leaflet'` genera en cada
// módulo una copia del namespace cuyo snapshot se toma ANTES de que el plugin
// aumente Leaflet, por lo que esa copia nunca expone `markerClusterGroup` en
// builds optimizados (producción).
//
// Este módulo expone `window.L` (para que el plugin lo encuentre) y exporta el
// mismo objeto vivo, que sí queda aumentado tras importar el plugin. Debe
// importarse ANTES de `import 'leaflet.markercluster'`.
import * as L from 'leaflet';

(window as unknown as { L: typeof L }).L = L;

export default L;

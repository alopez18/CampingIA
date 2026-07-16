/* eslint-disable */
// Genera las imágenes fuente (icon/splash) para @capacitor/assets a partir de
// diseños SVG con la marca CampingAI (tienda de campaña blanca sobre verde bosque).
// Uso: node scripts/generate-source-assets.js
const fs = require('fs');
const path = require('path');
const sharp = require('sharp');

const GREEN = '#2e7d32';
const GREEN_DARK = '#14361a';
const WHITE = '#ffffff';

const outDir = path.resolve(__dirname, '..', 'assets');
fs.mkdirSync(outDir, { recursive: true });

// Path de la tienda de campaña dentro de un lienzo 1024x1024.
// scale: factor de tamaño (1 = grande a sangre, <1 = reducido para el área segura).
function tentPath(scale = 1) {
  const cx = 512;
  const cy = 512;
  // Vértices base (a sangre) de la tienda.
  const apex = [512, 300];
  const left = [200, 724];
  const right = [824, 724];
  // Puerta (triángulo recortado en el centro inferior).
  const dLeft = [452, 724];
  const dTop = [512, 512];
  const dRight = [572, 724];

  const s = (p) => [cx + (p[0] - cx) * scale, cy + (p[1] - cy) * scale];
  const f = (p) => `${s(p)[0].toFixed(1)} ${s(p)[1].toFixed(1)}`;

  return (
    `M${f(apex)} L${f(right)} L${f(left)} Z ` +
    `M${f(dLeft)} L${f(dTop)} L${f(dRight)} Z`
  );
}

function iconOnlySvg() {
  return `<svg xmlns="http://www.w3.org/2000/svg" width="1024" height="1024" viewBox="0 0 1024 1024">
  <rect width="1024" height="1024" fill="${GREEN}"/>
  <path fill="${WHITE}" fill-rule="evenodd" d="${tentPath(1)}"/>
</svg>`;
}

function iconForegroundSvg() {
  // El primer plano del icono adaptativo lo recorta Android (inset ~16.7%),
  // por eso la tienda se dibuja grande para que quede bien proporcionada.
  return `<svg xmlns="http://www.w3.org/2000/svg" width="1024" height="1024" viewBox="0 0 1024 1024">
  <path fill="${WHITE}" fill-rule="evenodd" d="${tentPath(1.25)}"/>
</svg>`;
}

function iconBackgroundSvg() {
  return `<svg xmlns="http://www.w3.org/2000/svg" width="1024" height="1024" viewBox="0 0 1024 1024">
  <rect width="1024" height="1024" fill="${GREEN}"/>
</svg>`;
}

function splashSvg(bg) {
  // Lienzo 2732x2732; tienda centrada (~26% del ancho) + nombre de la app.
  const tentSize = 720;
  const offset = (2732 - tentSize) / 2;
  return `<svg xmlns="http://www.w3.org/2000/svg" width="2732" height="2732" viewBox="0 0 2732 2732">
  <rect width="2732" height="2732" fill="${bg}"/>
  <g transform="translate(${offset}, ${offset - 120}) scale(${tentSize / 1024})">
    <path fill="${WHITE}" fill-rule="evenodd" d="${tentPath(0.9)}"/>
  </g>
  <text x="1366" y="1720" text-anchor="middle" fill="${WHITE}"
    font-family="Arial, Helvetica, sans-serif" font-size="180" font-weight="700"
    letter-spacing="4">CampingAI</text>
</svg>`;
}

async function render(name, svg) {
  const file = path.join(outDir, name);
  await sharp(Buffer.from(svg)).png().toFile(file);
  console.log('  ✓', name);
}

(async () => {
  console.log('Generando imágenes fuente en /assets ...');
  await render('icon-only.png', iconOnlySvg());
  await render('icon-foreground.png', iconForegroundSvg());
  await render('icon-background.png', iconBackgroundSvg());
  await render('splash.png', splashSvg(GREEN));
  await render('splash-dark.png', splashSvg(GREEN_DARK));
  console.log('Listo.');
})().catch((e) => {
  console.error(e);
  process.exit(1);
});

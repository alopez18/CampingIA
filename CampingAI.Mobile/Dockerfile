# ─── Stage 1: build ───────────────────────────────────────────────────────────
FROM node:22-alpine AS build
WORKDIR /app

# Install dependencies first (layer cache)
COPY package*.json ./
RUN npm ci

# Copy source and build for Docker (uses environment.docker.ts + nginx proxy)
COPY . .
RUN npm run build:docker

# ─── Stage 2: serve ───────────────────────────────────────────────────────────
FROM nginx:alpine AS final

# Copy compiled SPA
COPY --from=build /app/www /usr/share/nginx/html

# Copy custom nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]

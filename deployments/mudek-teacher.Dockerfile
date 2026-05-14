# ── Stage 1: Build ──────────────────────────────────────────────────────────
FROM node:22-alpine AS builder

WORKDIR /app

COPY mudek-teacher/package*.json ./
RUN npm ci --prefer-offline

COPY mudek-teacher/ ./

ARG VITE_API_BASE_URL=https://mude.ktun.edu.tr
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

RUN npm run build

# ── Stage 2: Serve ───────────────────────────────────────────────────────────
FROM nginx:1.27-alpine

COPY --from=builder /app/dist /usr/share/nginx/html
COPY deployments/spa.nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

FROM nginx:1.27-alpine

COPY deployments/portal/ /usr/share/nginx/html/

# Sade bir static-file server — SPA değil, her istek doğrudan dosyayı döndürür
RUN printf 'server {\n\
    listen 80;\n\
    root /usr/share/nginx/html;\n\
    index index.html;\n\
    location / { try_files $uri $uri/ =404; }\n\
    location ~* \\.(jpg|png|ico|gif)$ { expires 30d; add_header Cache-Control "public"; }\n\
}\n' > /etc/nginx/conf.d/default.conf

EXPOSE 80

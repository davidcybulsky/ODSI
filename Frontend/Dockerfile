FROM node:18.18-alpine3.18 as build

WORKDIR /app

COPY . .

RUN npm ci

RUN npm run build

FROM nginx:latest

COPY nginx.conf /etc/nginx/nginx.conf

COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

COPY ssl/www.bank.com.crt /etc/nginx/ssl/server.crt

COPY ssl/www.bank.com.key /etc/nginx/ssl/server.key

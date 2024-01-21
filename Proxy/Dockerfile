FROM nginx:alpine

COPY nginx.conf /etc/nginx/nginx.conf

COPY ssl/www.bank.com.crt /etc/nginx/ssl/server.crt

COPY ssl/www.bank.com.key /etc/nginx/ssl/server.key

EXPOSE 443
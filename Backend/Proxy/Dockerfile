FROM nginx:1.13-alpine
EXPOSE 80
EXPOSE 443
COPY ["./conf", "/etc/nginx/conf.d/default.conf"]

COPY ["./Certs/ssl/openssl.crt", "/etc/nginx/conf/bank.crt"]
COPY ["./Certs/ssl/openssl.key", "/etc/nginx/conf/bank.key"]
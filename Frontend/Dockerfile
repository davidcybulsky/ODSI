FROM node:21.5.0-bullseye-slim AS base
WORKDIR /client
EXPOSE 4200

RUN npm install -g @angular/cli@17

COPY package.json package-lock.json ./
RUN npm ci

COPY . . 
RUN ng serve 
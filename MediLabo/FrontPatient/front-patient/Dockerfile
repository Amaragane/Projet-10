# Utilise l'image node pour builder le projet
FROM node:18-alpine AS build
WORKDIR /app
ENV LANG en_US.UTF-8
ENV LC_ALL en_US.UTF-8
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# Stage pour servir les fichiers statiques
FROM nginx:alpine
COPY --from=build /app/build /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

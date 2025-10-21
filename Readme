Projet P10 – Application de Détection du Risque de Diabète

Description générale
Cette application permet aux professionnels de santé d’évaluer le risque de diabète de type 2 d’un patient à partir de ses données personnelles et de ses notes médicales.
L’architecture repose sur une approche microservices en .NET 8, orchestrée avec Docker Compose, et un frontend React (TypeScript).

PRÉREQUIS
Docker Desktop

.NET SDK 8

Node.js 18+

Génération des clés JWT RSA
Pour l’authentification JWT, générez la paire de clés RSA à la racine du projet :

bash
openssl genpkey -algorithm RSA -out private_key.pem -pkeyopt rsa_keygen_bits:2048
openssl rsa -pubout -in private_key.pem -out public_key.pem
Les fichiers private_key.pem et public_key.pem doivent être présents à la racine car ils sont référencés ainsi dans docker-compose.yml :

text
secrets:
  jwt_private_key:
    file: ./private_key.pem
  jwt_public_key:
    file: ./public_key.pem
ARCHITECTURE DU PROJET
PatientService (.NET + SQL Server)

Gère les informations des patients (identité, âge, genre, etc.).

Données stockées dans SQL Server.

Expose une API : /api/patients.

PatientNotesService (.NET + MongoDB)

Gère les notes médicales associées aux patients.

Stocke les notes textuelles dans MongoDB.

Expose une API : /api/notes.

Contient un seed automatique UTF-8 insérant des données au démarrage.

RiskLevelService (.NET)

Calcule le niveau de risque diabétique.

Appelle PatientService et PatientNotesService via la Gateway.

Gateway (Ocelot - .NET)

Point d’entrée unique pour les appels backend.

Joue le rôle de reverse proxy et couche de sécurité JWT.

Communique avec les services internes via HTTP.

Seule la Gateway expose un port externe.

Frontend (React + TypeScript)

Interface utilisateur affichant patients, notes et risques.

Bases de données

SQL Server : données patients.

MongoDB : notes médicales.

Les deux sont isolées dans le réseau Docker interne.

MISE EN PLACE ET EXÉCUTION
Prérequis détaillés ci-dessus.

Étapes :

Cloner le dépôt :

bash
git clone <url-du-repository>
cd <dossier-du-projet>
Vérifier la présence de certificats dans ./certs/localhost.pfx
Mot de passe : MyPassword123

Lancer la stack :

bash
docker-compose up --build
Accès :

Frontend : http://localhost:3000



TECHNOLOGIES
.NET 8

Entity Framework Core 8

MongoDB .NET Driver

Ocelot Gateway

React + TypeScript

JWT Authentication

Docker & Docker Compose

AUTEURS
Projet OpenClassrooms P10 - Microservices .NET / Docker / React
Stack principale : C#, .NET, Docker, React & MongoDB
Réalisation : Architecture microservices & intégration DevOps
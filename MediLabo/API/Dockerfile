FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV LANG C.UTF-8
ENV LC_ALL C.UTF-8
EXPOSE 5001
RUN apt-get update && apt-get install -y libgssapi-krb5-2

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["API/API.csproj", "API/"]
RUN dotnet restore "API/API.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "API/API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API/API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV LANG C.UTF-8
ENV LC_ALL C.UTF-8
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]

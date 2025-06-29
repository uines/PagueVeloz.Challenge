FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

ENV NUGET_PACKAGES="/nuget_packages"
ENV NUGET_FALLBACK_PACKAGES=""

RUN dotnet restore PagueVeloz.Challenge.sln

RUN dotnet build PagueVeloz.Challenge.sln -c Release --no-restore

WORKDIR /src/PagueVeloz.Challenge.Api

RUN dotnet publish -c Release -o /app/publish --no-restore

# Estágio 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /src/PagueVeloz.Challenge.Api/appsettings.json .
COPY --from=build /src/PagueVeloz.Challenge.Api/appsettings.Development.json .
COPY --from=build /app/publish .

EXPOSE 80
EXPOSE 443

ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/PagueVelozChallenge.db"

ENTRYPOINT ["dotnet", "PagueVeloz.Challenge.Api.dll"]
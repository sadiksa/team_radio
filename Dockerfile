FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5100

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TeamRadio/TeamRadio.csproj", "TeamRadio/"]
RUN dotnet restore "TeamRadio/TeamRadio.csproj"
COPY . .
WORKDIR "/src/TeamRadio"
RUN dotnet build "TeamRadio.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TeamRadio.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV ASPNETCORE_URLS=http://0.0.0.0:5100
WORKDIR /app
COPY --from=publish /app/publish .
COPY envvarcheckandstartapp.sh /app/envvarcheckandstartapp.sh
USER root
RUN chmod +x /app/envvarcheckandstartapp.sh
USER $APP_UID
ENTRYPOINT ["/app/envvarcheckandstartapp.sh"]

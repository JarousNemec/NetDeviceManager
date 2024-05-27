FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NetDeviceManager.ScheduledDatabaseReporter/NetDeviceManager.ScheduledDatabaseReporter.csproj", "NetDeviceManager.ScheduledDatabaseReporter/"]
RUN dotnet restore "NetDeviceManager.ScheduledDatabaseReporter/NetDeviceManager.ScheduledDatabaseReporter.csproj"
COPY . .
WORKDIR "/src/NetDeviceManager.ScheduledDatabaseReporter"
RUN dotnet build "NetDeviceManager.ScheduledDatabaseReporter.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NetDeviceManager.ScheduledDatabaseReporter.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetDeviceManager.ScheduledDatabaseReporter.dll"]

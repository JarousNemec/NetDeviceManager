FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NetDeviceManager.ScheduledSnmpAgent/NetDeviceManager.ScheduledSnmpAgent.csproj", "NetDeviceManager.ScheduledSnmpAgent/"]
RUN dotnet restore "NetDeviceManager.ScheduledSnmpAgent/NetDeviceManager.ScheduledSnmpAgent.csproj"
COPY . .
WORKDIR "/src/NetDeviceManager.ScheduledSnmpAgent"
RUN dotnet build "NetDeviceManager.ScheduledSnmpAgent.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NetDeviceManager.ScheduledSnmpAgent.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetDeviceManager.ScheduledSnmpAgent.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 514

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NetDeviceManager.SyslogServer/NetDeviceManager.SyslogServer.csproj", "NetDeviceManager.SyslogServer/"]
RUN dotnet restore "NetDeviceManager.SyslogServer/NetDeviceManager.SyslogServer.csproj"
COPY . .
WORKDIR "/src/NetDeviceManager.SyslogServer"
RUN dotnet build "NetDeviceManager.SyslogServer.csproj" --no-cache -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NetDeviceManager.SyslogServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetDeviceManager.SyslogServer.dll"]

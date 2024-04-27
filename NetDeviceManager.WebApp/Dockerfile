FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NetDeviceManager.WebApp/NetDeviceManager.WebApp.csproj", "NetDeviceManager.WebApp/"]
RUN dotnet restore "NetDeviceManager.WebApp/NetDeviceManager.WebApp.csproj"
COPY . .
WORKDIR "/src/NetDeviceManager.WebApp"
RUN dotnet build "NetDeviceManager.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NetDeviceManager.WebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetDeviceManager.WebApp.dll"]

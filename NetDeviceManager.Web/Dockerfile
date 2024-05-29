FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NetDeviceManager.Web/NetDeviceManager.Web.csproj", "NetDeviceManager.Web/"]
RUN dotnet restore "NetDeviceManager.Web/NetDeviceManager.Web.csproj"
COPY . .
WORKDIR "/src/NetDeviceManager.Web"

RUN dotnet build "NetDeviceManager.Web.csproj" --no-cache -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NetDeviceManager.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetDeviceManager.Web.dll"]

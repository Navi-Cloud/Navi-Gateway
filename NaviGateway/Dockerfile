FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["NaviGateway/NaviGateway.csproj", "NaviGateway/"]
RUN dotnet restore "NaviGateway/NaviGateway.csproj"
COPY . .
WORKDIR "/src/NaviGateway"
RUN dotnet build "NaviGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NaviGateway.csproj" -c Release -o /app/publish

FROM base AS final

ENV ASPNETCORE_URLS 4040

WORKDIR /app
RUN find . -name "appsettings.json" -exec rm {} \;
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NaviGateway.dll"]
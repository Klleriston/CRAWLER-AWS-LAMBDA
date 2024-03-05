FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DOTNET-CRAWLER-AWS.csproj", "."]
RUN dotnet restore "./././DOTNET-CRAWLER-AWS.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./DOTNET-CRAWLER-AWS.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DOTNET-CRAWLER-AWS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DOTNET-CRAWLER-AWS.dll"]

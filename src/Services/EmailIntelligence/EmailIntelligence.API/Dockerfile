FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
# Install debugging tools
RUN apt-get update && apt-get install -y curl unzip \
    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg \
    && chmod +x /vsdbg/vsdbg
USER app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/EmailIntelligence/EmailIntelligence.API/EmailIntelligence.API.csproj", "Services/EmailIntelligence/EmailIntelligence.API/"]
COPY ["Services/EmailIntelligence/EmailIntelligence.Application/EmailIntelligence.Application.csproj", "Services/EmailIntelligence/EmailIntelligence.Application/"]
COPY ["Services/EmailIntelligence/EmailIntelligence.Domain/EmailIntelligence.Domain.csproj", "Services/EmailIntelligence/EmailIntelligence.Domain/"]
COPY ["Services/EmailIntelligence/EmailIntelligence.Infrastructure/EmailIntelligence.Infrastructure.csproj", "Services/EmailIntelligence/EmailIntelligence.Infrastructure/"]
COPY ["BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/BuildingBlocks/"]
COPY ["BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "BuildingBlocks/BuildingBlocks.Messaging/"]
RUN dotnet restore "./Services/EmailIntelligence/EmailIntelligence.API/EmailIntelligence.API.csproj"
COPY . .
WORKDIR "/src/Services/EmailIntelligence/EmailIntelligence.API"
RUN dotnet build "./EmailIntelligence.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EmailIntelligence.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmailIntelligence.API.dll"]

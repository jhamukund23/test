FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["dss.api/dss.api.csproj", "dss.api/"]
RUN dotnet restore "dss.api/dss.api.csproj"
COPY . .
WORKDIR "/src/dss.api"
RUN dotnet build "dss.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dss.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dss.api.dll"]

# Setup runner
FROM ndusw3dcr.azurecr.io/ndaspnet:7.0 AS base
WORKDIR /app

# Expose ports for external consumption
EXPOSE 8080

# Build it
FROM ndusw3dcr.azurecr.io/ndsdk:7.0 AS build
WORKDIR /src
COPY ["sampleapp.csproj", "appsettings.json", "appsettings.*.json", "Service/"]
RUN dotnet restore "Service/sampleapp.csproj"
COPY src Service
WORKDIR "/src/Service"
RUN dotnet build "sampleapp.csproj" -c Release -o /app/build

# Publish it
FROM build AS publish
RUN dotnet publish "sampleapp.csproj" -c Release -o /app/publish


# Copy published
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#COPY ["version.json", "./"]
RUN find /app -type d -exec chmod 755 {} \;
RUN find /app -type f -exec chmod 644 {} \;

# Run as svcuser
USER svcuser

ENTRYPOINT ["dotnet", "sampleapp.dll", "--urls", "http://*:8080"]

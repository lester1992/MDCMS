# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["MDCMS.Server.csproj", "./"]
RUN dotnet restore "MDCMS.Server.csproj"

# Copy all source code
COPY . .

# Publish the app
RUN dotnet publish "MDCMS.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port (adjust if using another)
EXPOSE 80

# Entry point
ENTRYPOINT ["dotnet", "MDCMS.Server.dll"]

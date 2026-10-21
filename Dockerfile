# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AssetManagementAPI.csproj", "./"]
RUN dotnet restore "AssetManagementAPI.csproj"

COPY . .
RUN dotnet build "AssetManagementAPI.csproj" -c Release -o /app/build
RUN dotnet publish "AssetManagementAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AssetManagementAPI.dll"]

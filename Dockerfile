# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AssetManagementAPI/AssetManagementAPI.csproj", "AssetManagementAPI/"]
RUN dotnet restore "AssetManagementAPI/AssetManagementAPI.csproj"

COPY . .
WORKDIR "/src/AssetManagementAPI"
RUN dotnet build "AssetManagementAPI.csproj" -c Release -o /app/build
RUN dotnet publish "AssetManagementAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AssetManagementAPI.dll"]

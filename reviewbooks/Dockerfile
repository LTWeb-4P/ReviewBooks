# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ReviewBooks.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "ReviewBooks.dll"]

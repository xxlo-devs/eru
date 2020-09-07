FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-env
WORKDIR /app

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "eru.WebApp.dll"]
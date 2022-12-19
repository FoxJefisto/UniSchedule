# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine AS build-env

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:3.1-alpine
RUN apk add --no-cache --update \
        icu-libs \
        ca-certificates \
        tzdata \
    && ln -fs /usr/share/zoneinfo/Europe/Moscow /etc/localtime
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "UniShedule.dll"]
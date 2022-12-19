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
WORKDIR /app
RUN apk update
RUN apk upgrade
RUN apk add ca-certificates && update-ca-certificates
RUN apk add --no-cache icu-libs tzdata
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV TZ="Europe/Moscow"
ENV DEBIAN_FRONTEND="noninteractive"
ENV DOTNET_RUNNING_IN_CONTAINER=true
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "UniShedule.dll"]
# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine AS build-env
ENV DOTNET_RUNNING_IN_CONTAINER=true
RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:3.1-alpine
ENV TZ="Europe/Moscow"
ENV DEBIAN_FRONTEND="noninteractive"
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "UniShedule.dll"]
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
ENV TZ="Europe/Moscow"
ENV DEBIAN_FRONTEND="noninteractive"
ENV DOTNET_RUNNING_IN_CONTAINER=true
RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib
RUN apk add --no-cache alpine-conf && \
    setup-timezone -z Europe/Moscow
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "UniShedule.dll"]
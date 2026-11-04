# syntax=docker/dockerfile:1

# This stage is used when running from VS in fast mode (Default for Debug configuration).
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

################################################################################

# This stage is used to build and publish the project for the final stage.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

# Required for AspNet.SassCompiler
RUN apk add --no-cache gcompat libstdc++

ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /app

COPY Directory.Build.props ./
COPY src/ src/

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish src/Distags.WebApp/Distags.WebApp.csproj \
      -c $BUILD_CONFIGURATION \
      -p:UseAppHost=false \
      -o /app/publish

################################################################################

# This stage is used in production or when running from VS in regular mode.
FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Distags.WebApp.dll"]

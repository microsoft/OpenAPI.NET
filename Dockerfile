# syntax=docker/dockerfile:1.2
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
ARG VSS_NUGET_URI_PREFIXES
ARG VSS_NUGET_EXTERNAL_FEED_ENDPOINTS

COPY ./src ./hidi/src
COPY ./Directory.Build.props ./hidi/Directory.Build.props
COPY ./nuget.config ./hidi/nuget.config
COPY ./README.md ./hidi/README.md
WORKDIR /app/hidi
RUN wget -qO- https://aka.ms/install-artifacts-credprovider.sh | bash
RUN --mount=type=secret,id=vss_nuget_accesstoken,target=/run/secrets/vss_nuget_accesstoken \
    VSS_NUGET_ACCESSTOKEN="$(cat /run/secrets/vss_nuget_accesstoken)" \
    dotnet publish ./src/Microsoft.OpenApi.Hidi/Microsoft.OpenApi.Hidi.csproj -c Release -p:RestoreConfigFile=/app/hidi/nuget.config

FROM mcr.microsoft.com/dotnet/runtime:8.0-jammy-chiseled AS runtime
WORKDIR /app

COPY --from=build-env /app/hidi/src/Microsoft.OpenApi.Hidi/bin/Release/net8.0 ./

VOLUME /app/output
VOLUME /app/openapi.yml
VOLUME /app/api.csdl
VOLUME /app/collection.json
ENV HIDI_CONTAINER=true DOTNET_TieredPGO=1 DOTNET_TC_QuickJitForLoops=1
ENTRYPOINT ["dotnet", "Microsoft.OpenApi.Hidi.dll"]
LABEL description="# Welcome to Hidi \
To start transforming OpenAPI documents checkout [the getting started documentation](https://github.com/microsoft/OpenAPI.NET/tree/main/src/Microsoft.OpenApi.Hidi)  \
[Source dockerfile](https://github.com/microsoft/OpenAPI.NET/blob/main/Dockerfile)"

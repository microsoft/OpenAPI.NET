FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY ./src ./hidi/src
WORKDIR /app/hidi
RUN dotnet publish ./src/Microsoft.OpenApi.Hidi/Microsoft.OpenApi.Hidi.csproj -c Release

FROM mcr.microsoft.com/dotnet/runtime:6.0 as runtime
WORKDIR /app

COPY --from=build-env /app/hidi/src/Microsoft.OpenApi.Hidi/bin/Release/net6.0 ./

VOLUME /app/output
VOLUME /app/openapi.yml
VOLUME /app/api.csdl
VOLUME /app/collection.json
ENV HIDI_CONTAINER=true DOTNET_TieredPGO=1 DOTNET_TC_QuickJitForLoops=1
ENTRYPOINT ["dotnet", "Microsoft.OpenApi.Hidi.dll"]
LABEL description="# Welcome to Hidi \
To start transforming OpenAPI documents checkout [the getting started documentation](https://github.com/microsoft/OpenAPI.NET/tree/vnext/src/Microsoft.OpenApi.Hidi)  \
[Source dockerfile](https://github.com/microsoft/OpenAPI.NET/blob/vnext/Dockerfile)"

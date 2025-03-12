#!/bin/bash

echo "Building Microsoft.OpenApi"

PROJ="$(dirname "$0")/src/Microsoft.OpenApi/Microsoft.OpenApi.csproj"
dotnet msbuild "$PROJ" /t:restore /p:Configuration=Release
dotnet msbuild "$PROJ" /t:build /p:Configuration=Release
dotnet msbuild "$PROJ" /t:pack "/p:Configuration=Release;PackageOutputPath=$(dirname "$0")/artifacts"

echo "Building Microsoft.OpenApi.Readers"

PROJ="$(dirname "$0")/src/Microsoft.OpenApi.Readers/Microsoft.OpenApi.Readers.csproj"
dotnet msbuild "$PROJ" /t:restore /p:Configuration=Release
dotnet msbuild "$PROJ" /t:build /p:Configuration=Release
dotnet msbuild "$PROJ" /t:pack "/p:Configuration=Release;PackageOutputPath=$(dirname "$0")/artifacts"

echo "Building Microsoft.OpenApi.Hidi"

PROJ="$(dirname "$0")/src/Microsoft.OpenApi.Hidi/Microsoft.OpenApi.Hidi.csproj"
dotnet msbuild "$PROJ" /t:restore /p:Configuration=Release
dotnet msbuild "$PROJ" /t:build /p:Configuration=Release
dotnet msbuild "$PROJ" /t:pack "/p:Configuration=Release;PackageOutputPath=$(dirname "$0")/artifacts"
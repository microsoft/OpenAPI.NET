SET VERSION=0.9.1
SET PROJ=%~dp0src\OpenApi\OpenApi.csproj 
msbuild %PROJ% /t:restore /p:Configuration=Release
msbuild %PROJ% /t:build /p:Configuration=Release
msbuild %PROJ% /t:pack /p:Configuration=Release;PackageOutputPath=%~dp0artifacts;Version=%VERSION%

SET PROJ=%~dp0src\OpenApi.Readers\OpenApi.Readers.csproj 
msbuild %PROJ% /t:restore /p:Configuration=Release
msbuild %PROJ% /t:build /p:Configuration=Release
msbuild %PROJ% /t:pack /p:Configuration=Release;PackageOutputPath=%~dp0artifacts;Version=%VERSION%
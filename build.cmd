SET PROJ=%~dp0src\OpenApi\OpenApi.csproj 
msbuild %PROJ% /t:restore /p:Configuration=Release
msbuild %PROJ% /t:build /p:Configuration=Release
msbuild %PROJ% /t:pack /p:Configuration=Release;PackageOutputPath=%~dp0artifacts
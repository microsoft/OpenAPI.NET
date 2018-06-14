@echo off
if "%~1"=="" goto :error

SET VERSION=%~1

Echo Building Microsoft.OpenApi

SET PROJ=%~dp0src\Microsoft.OpenApi\Microsoft.OpenApi.csproj 
msbuild %PROJ% /t:restore /p:Configuration=Release
msbuild %PROJ% /t:build /p:Configuration=Release
msbuild %PROJ% /t:pack /p:Configuration=Release;PackageOutputPath=%~dp0artifacts;Version=%VERSION%

Echo Building Microsoft.OpenApi.Readers

SET PROJ=%~dp0src\Microsoft.OpenApi.Readers\Microsoft.OpenApi.Readers.csproj 
msbuild %PROJ% /t:restore /p:Configuration=Release
msbuild %PROJ% /t:build /p:Configuration=Release
msbuild %PROJ% /t:pack /p:Configuration=Release;PackageOutputPath=%~dp0artifacts;Version=%VERSION%

goto :end
:error
echo Version parameter missing e.g. build.cmd 1.0.0-beta0008

:end

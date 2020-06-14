@echo off
Echo Building Microsoft.OpenApi 

SET PROJ=%~dp0src\Microsoft.OpenApi\Microsoft.OpenApi.csproj 
dotnet msbuild %PROJ% /t:restore /p:Configuration=Release
dotnet msbuild %PROJ% /t:build /p:Configuration=Release
dotnet msbuild %PROJ% /t:pack /p:Configuration=Release;PackageOutputPath=%~dp0artifacts

Echo Building Microsoft.OpenApi.Readers

SET PROJ=%~dp0src\Microsoft.OpenApi.Readers\Microsoft.OpenApi.Readers.csproj 
dotnet msbuild %PROJ% /t:restore /p:Configuration=Release
dotnet msbuild %PROJ% /t:build /p:Configuration=Release
dotnet msbuild %PROJ% /t:pack /p:Configuration=Release;PackageOutputPath=%~dp0artifacts

goto :end
:error
echo Version parameter missing e.g. build.cmd 1.0.0-beta0008

:end

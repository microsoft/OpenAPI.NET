$latest = Get-ChildItem .\artifacts\ Microsoft.OpenApi.Tool* | select-object -Last 1
$version = $latest.Name.Split(".")[3..5] | join-string -Separator "."

if (Test-Path -Path ./artifacts/openapi-parser.exe) {
  dotnet tool uninstall --tool-path artifacts Microsoft.OpenApi.Tool
}
dotnet tool install --tool-path artifacts --add-source .\artifacts\ --version $version Microsoft.OpenApi.Tool
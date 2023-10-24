$latest = Get-ChildItem .\artifacts\Microsoft.OpenApi.Hidi* | select-object -Last 1
$version = $latest.Name.Split(".")[3..5] | join-string -Separator "."

if (Test-Path -Path ./artifacts/hidi.exe) {
  dotnet tool uninstall --tool-path artifacts Microsoft.OpenApi.Hidi
}
dotnet tool install --tool-path artifacts --add-source .\artifacts\ --version $version Microsoft.OpenApi.Hidi
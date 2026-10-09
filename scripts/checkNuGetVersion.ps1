# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

param(
    [Parameter(Mandatory = $true)]
    [string]$PackageId,
    [Parameter(Mandatory = $true)]
    [string]$PackageDirectory,
    [Parameter(Mandatory = $true)]
    [string]$NuGetServiceIndexUrl
)

$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($env:FEED_ACCESS_TOKEN)) {
    throw 'No Azure Artifacts access token available for the version check.'
}

$packagePattern = '^' + [regex]::Escape($PackageId) + '\.(\d[\w\.\-]*)\.nupkg$'
$packages = @(Get-ChildItem -Path $PackageDirectory -Filter "$PackageId.*.nupkg" |
    Where-Object { $_.Name -match $packagePattern })
if ($packages.Count -ne 1) {
    throw "Expected exactly one $PackageId nupkg to publish; found $($packages.Count)."
}
$version = [regex]::Match($packages[0].Name, $packagePattern, 'IgnoreCase').Groups[1].Value
$id = $PackageId.ToLowerInvariant()
$credentials = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes("AzureDevOps:$($env:FEED_ACCESS_TOKEN)"))
$headers = @{
    'User-Agent' = 'openapinet-azdo-pipeline'
    'Authorization' = "Basic $credentials"
}

$serviceIndex = Invoke-RestMethod -Uri $NuGetServiceIndexUrl -Headers $headers
$packageBaseAddress = $serviceIndex.resources |
    Where-Object { $_.'@type' -eq 'PackageBaseAddress/3.0.0' } |
    Select-Object -First 1 -ExpandProperty '@id'
if ([string]::IsNullOrWhiteSpace($packageBaseAddress)) {
    throw 'The private NuGet feed does not advertise a package base address.'
}

$uri = "$($packageBaseAddress.TrimEnd('/'))/$id/index.json"
try {
    $response = Invoke-RestMethod -Uri $uri -Headers $headers
    if ($null -eq $response.versions) {
        throw 'The private NuGet feed returned an invalid package version response.'
    }
    $alreadyPublished = $response.versions -contains $version
} catch {
    if ([int]$_.Exception.Response.StatusCode -eq 404) {
        $alreadyPublished = $false
    } else {
        throw
    }
}

if ($alreadyPublished) {
    Write-Host "NuGet $id $version already present in the private feed; skipping ESRP release (idempotent re-run)."
} else {
    Write-Host "NuGet $id $version not found in the private feed; will publish via ESRP."
}
Write-Host "##vso[task.setvariable variable=nugetAlreadyPublished]$($alreadyPublished.ToString().ToLowerInvariant())"

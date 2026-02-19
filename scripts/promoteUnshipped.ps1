$nullableConstant = "#nullable enable"
$unshippedDocuments = ls *.Unshipped* -R | Select-Object -ExpandProperty FullName
foreach ($unshippedDocumentPath in $unshippedDocuments) {
    $shippedDocumentPath = $unshippedDocumentPath -replace '\.Unshipped', '.Shipped'
    $unshippedDocumentContent = Get-Content $unshippedDocumentPath -Raw
    $unshippedDocumentContent = ($unshippedDocumentContent -replace [regex]::Escape($nullableConstant), '').Trim()
    if ([string]::IsNullOrWhiteSpace($unshippedDocumentContent)) {
        Write-Host "No content to promote for $unshippedDocumentPath, skipping." -ForegroundColor Yellow
        continue
    }
    Add-Content -Path $shippedDocumentPath -Value $unshippedDocumentContent -Verbose
    Set-Content -Path $unshippedDocumentPath -Value $nullableConstant -Verbose
}
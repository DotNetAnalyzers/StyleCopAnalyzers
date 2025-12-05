param (
    [switch]$Debug,
    [switch]$NoBuild,
    [switch]$NoReport,
    [switch]$AppVeyor,
    [switch]$Azure
)

Write-Warning "build/opencover-report.ps1 is deprecated; forwarding to build/coverage-report.ps1."
& "$PSScriptRoot\coverage-report.ps1" @PSBoundParameters
exit $LASTEXITCODE

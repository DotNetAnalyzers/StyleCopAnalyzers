[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $Runner,
    [int[]] $LanguageVersions = @(6, 15),
    [ValidateRange(1, 10)]
    [int] $MaxProcesses = 2,
    [int] $Repetitions = 3,
    [Parameter(Mandatory)]
    [string] $OutputDirectory
)

$ErrorActionPreference = 'Stop'
$Runner = (Resolve-Path $Runner).Path
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Force $OutputDirectory | Out-Null
$script = Join-Path $PSScriptRoot 'test-performance.ps1'
$rows = @()
for ($iteration = 1; $iteration -le $Repetitions; $iteration++) {
    $destination = Join-Path $OutputDirectory "repeat-$iteration"
    $timer = [Diagnostics.Stopwatch]::StartNew()
    $LanguageVersions | ForEach-Object -Parallel {
        $ErrorActionPreference = 'Stop'
        $directory = Join-Path $using:destination "cs$_"
        & $using:script -Runner $using:Runner -LanguageVersions $_ -ThreadCounts default `
            -Repetitions 1 -OutputDirectory $directory
    } -ThrottleLimit $MaxProcesses -ErrorAction Stop
    $timer.Stop()
    $measurements = @(foreach ($version in $LanguageVersions) {
        Import-Csv (Join-Path $destination "cs$version\measurements.csv")
    })
    $rows += [pscustomobject]@{
        iteration = $iteration
        maxProcesses = $MaxProcesses
        wallSeconds = [Math]::Round($timer.Elapsed.TotalSeconds, 3)
        cpuSeconds = [Math]::Round(($measurements | Measure-Object cpuSeconds -Sum).Sum, 3)
        total = ($measurements | Measure-Object total -Sum).Sum
        passed = ($measurements | Measure-Object passed -Sum).Sum
    }
    $rows | Export-Csv (Join-Path $OutputDirectory 'measurements.csv') -NoTypeInformation
    $rows[-1] | Format-Table | Out-Host
}

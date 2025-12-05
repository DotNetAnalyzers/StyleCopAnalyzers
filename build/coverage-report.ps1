param (
    [switch]$Debug,
    [switch]$NoBuild,
    [switch]$NoReport,
    [switch]$AppVeyor,
    [switch]$Azure
)

$ErrorActionPreference = 'Stop'

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $scriptDir

try {
    if (-not $NoBuild) {
        if ($Debug) {
            .\build.ps1 -Debug -Incremental
        } else {
            .\build.ps1 -Incremental
        }
    }

    $configuration = if ($Debug) { 'Debug' } else { 'Release' }
    $resultsRoot = Join-Path $scriptDir 'TestResults'
    $coverageRoot = Join-Path $scriptDir 'coverage'

    if (Test-Path $resultsRoot) {
        Remove-Item -Recurse -Force $resultsRoot
    }

    if (Test-Path $coverageRoot) {
        Remove-Item -Recurse -Force $coverageRoot
    }

    New-Item -ItemType Directory -Force -Path $resultsRoot | Out-Null
    New-Item -ItemType Directory -Force -Path $coverageRoot | Out-Null

    $runs = @(
        @{ Lang = '6'; Project = 'StyleCop.Analyzers.Test'; Framework = 'net452' }
        @{ Lang = '7'; Project = 'StyleCop.Analyzers.Test.CSharp7'; Framework = 'net46' }
        @{ Lang = '8'; Project = 'StyleCop.Analyzers.Test.CSharp8'; Framework = 'net472' }
        @{ Lang = '9'; Project = 'StyleCop.Analyzers.Test.CSharp9'; Framework = 'net472' }
        @{ Lang = '10'; Project = 'StyleCop.Analyzers.Test.CSharp10'; Framework = 'net472' }
        @{ Lang = '11'; Project = 'StyleCop.Analyzers.Test.CSharp11'; Framework = 'net472' }
        @{ Lang = '12'; Project = 'StyleCop.Analyzers.Test.CSharp12'; Framework = 'net472' }
        @{ Lang = '13'; Project = 'StyleCop.Analyzers.Test.CSharp13'; Framework = 'net472' }
    )

    foreach ($run in $runs) {
        $projectPath = Join-Path $scriptDir "..\StyleCop.Analyzers\$($run.Project)\$($run.Project).csproj"
        $runResultsDir = Join-Path $resultsRoot "CSharp$($run.Lang)"
        New-Item -ItemType Directory -Force -Path $runResultsDir | Out-Null

        $trxName = "StyleCopAnalyzers.CSharp$($run.Lang).trx"

        dotnet test $projectPath `
            --framework $run.Framework `
            --configuration $configuration `
            --no-build `
            --settings (Join-Path $scriptDir 'coverlet.runsettings') `
            --results-directory $runResultsDir `
            --logger "trx;LogFileName=$trxName" `
            --collect:"XPlat Code Coverage"

        $coverageFile = Get-ChildItem -Path $runResultsDir -Recurse -Filter 'coverage.cobertura.xml' -ErrorAction SilentlyContinue | Select-Object -First 1
        if (-not $coverageFile) {
            throw "Coverage file not found for C# $($run.Lang) in $runResultsDir"
        }

        Copy-Item $coverageFile.FullName (Join-Path $coverageRoot "OpenCover.StyleCopAnalyzers.CSharp$($run.Lang).xml") -Force
    }

    if (-not $NoReport) {
        $packageConfig = [xml](Get-Content ..\.nuget\packages.config)
        $packagesFolder = '..\packages'
        $reportGeneratorVersion = $packageConfig.SelectSingleNode('/packages/package[@id="ReportGenerator"]').version
        $reportGenerator = "$packagesFolder\ReportGenerator.$reportGeneratorVersion\tools\ReportGenerator.exe"

        &$reportGenerator -targetdir:$coverageRoot -reporttypes:Html;Cobertura "-reports:$coverageRoot\OpenCover.*.xml"
        Write-Host "Open $coverageRoot\index.htm to see code coverage results."
    }
}
finally {
    Pop-Location
}

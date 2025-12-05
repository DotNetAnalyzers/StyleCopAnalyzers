# Runs only impacted test classes locally based on git diff with a target branch.
# .SYNOPSIS
#   Runs locally impacted StyleCop analyzer tests based on git diff against a target branch.
# .DESCRIPTION
#   Builds (optional) and executes xUnit tests using the impacted-test planner. Always runs full
#   suites for C# 6 and the latest test project; other language versions run class-filtered tests
#   when possible. Writes xUnit XML results to artifacts\test-results.
# .PARAMETER Configuration
#   Build configuration to use (Debug/Release). Defaults to Debug.
# .PARAMETER TargetBranch
#   Branch or ref to diff against when computing impacted tests. If omitted, attempts to locate a
#   remote pointing at DotNetAnalyzers/StyleCopAnalyzers and uses its master branch.
# .PARAMETER LatestLangVersion
#   Highest C# test project version; treated as "latest" for always-full runs. Default: 13.
# .PARAMETER LangVersions
#   Optional list of C# language versions to run (e.g., 6,7,13). Defaults to all known versions in plan.
# .PARAMETER NoBuild
#   Skip building the solution before running tests.
# .PARAMETER VerboseLogging
#   Emit additional diagnostic output from the planner and runner.
# .PARAMETER DryRun
#   Compute and report the impacted test plan without executing any tests.
[CmdletBinding()]
param(
    [string]$Configuration = 'Debug',
    [string]$TargetBranch = '',
    [string]$LatestLangVersion = '13',
    [string[]]$LangVersions,
    [switch]$NoBuild,
    [switch]$VerboseLogging,
    [switch]$DryRun
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Resolve repository root (parent of /build)
$repoRoot = Split-Path -Parent $PSScriptRoot
$planPath = Join-Path $repoRoot 'artifacts\test-plan.json'
$resultsRoot = Join-Path $repoRoot 'artifacts\test-results'

function Write-Info($message) { Write-Host "[local-tests] $message" }
function Write-DebugInfo($message) { if ($VerboseLogging) { Write-Host "[local-tests:debug] $message" } }

Push-Location $repoRoot
try {
    if ($DryRun) {
        $NoBuild = $true
    }

    if (-not $NoBuild) {
        Write-Info "Restoring tools (init.ps1)"
        & "$repoRoot\init.ps1"
    } else {
        Write-Info "Skipping init.ps1 because -NoBuild was specified"
    }

    if (-not $NoBuild) {
        Write-Info "Building solution (Configuration=$Configuration)"
        & dotnet build "$repoRoot\StyleCopAnalyzers.sln" -c $Configuration
    } else {
        Write-Info "Skipping build because -NoBuild was specified"
    }

    $resolvedTargetBranch = $TargetBranch
    if ([string]::IsNullOrWhiteSpace($resolvedTargetBranch)) {
        $resolvedTargetBranch = 'origin/master'
        try {
            $remoteLines = @(git remote -v 2>$null)
            $targetRemote = $remoteLines |
                Where-Object { $_ -match '^(?<name>[^\s]+)\s+(?<url>\S+)' } |
                ForEach-Object {
                    $m = [regex]::Match($_, '^(?<name>[^\s]+)\s+(?<url>\S+)')
                    if ($m.Success) {
                        [pscustomobject]@{ Name = $m.Groups['name'].Value; Url = $m.Groups['url'].Value }
                    }
                } |
                Where-Object { $_.Url -match 'github\.com[:/]+DotNetAnalyzers/StyleCopAnalyzers(\.git)?' } |
                Select-Object -First 1

            if ($targetRemote) {
                $resolvedTargetBranch = "$($targetRemote.Name)/master"
                Write-Info "Detected target branch '$resolvedTargetBranch' from remote '$($targetRemote.Url)'"
            } else {
                Write-Info "No matching remote found; defaulting target branch to '$resolvedTargetBranch'"
            }
        } catch {
            Write-Info "Remote detection failed; defaulting target branch to '$resolvedTargetBranch'"
        }
    }

    Write-Info "Computing impacted tests relative to $resolvedTargetBranch"
    & "$PSScriptRoot\compute-impacted-tests.ps1" `
        -OutputPath $planPath `
        -LatestLangVersion $LatestLangVersion `
        -TargetBranch $resolvedTargetBranch `
        -AssumePullRequest `
        -VerboseLogging:$VerboseLogging

    if (-not (Test-Path $planPath)) {
        throw "Test plan not found at $planPath"
    }

    $plan = Get-Content -Path $planPath -Raw | ConvertFrom-Json

    $planLangs = @($plan.plans.PSObject.Properties.Name)
    if (-not $LangVersions -or $LangVersions.Count -eq 0) {
        $LangVersions = $planLangs | Sort-Object {[int]$_}
    }

    Write-Info ("Target languages: {0}" -f ($LangVersions -join ', '))

    if ($DryRun) {
        Write-Info "Dry run: computed plan only (no tests will be executed)."
        Write-Info "Plan file: $planPath"
        foreach ($lang in $LangVersions) {
            if (-not $plan.plans.$lang) {
                Write-Info ("C# {0}: no entry in plan." -f $lang)
                continue
            }

            $entry = $plan.plans.$lang
            $summary = if ($entry.fullRun) { 'full run' } else { "filtered ($($entry.classes.Count) classes)" }
            Write-Info ("C# {0}: {1} ({2})" -f $lang, $summary, $entry.reason)
        }
        return
    }

    $packageConfig = [xml](Get-Content "$repoRoot\.nuget\packages.config")
    $xunitrunner_version = $packageConfig.SelectSingleNode('/packages/package[@id="xunit.runner.console"]').version

    $frameworkMap = @{
        '6'  = 'net452'
        '7'  = 'net46'
        '8'  = 'net472'
        '9'  = 'net472'
        '10' = 'net472'
        '11' = 'net472'
        '12' = 'net472'
        '13' = 'net472'
    }

    New-Item -ItemType Directory -Force -Path $resultsRoot | Out-Null

    $failures = 0

    foreach ($lang in $LangVersions) {
        if (-not $plan.plans.$lang) {
            Write-Info "No plan entry for C# $lang. Skipping."
            continue
        }

        $entry = $plan.plans.$lang
        $fullRun = [bool]$entry.fullRun
        $classes = @($entry.classes)
        $reason = $entry.reason
        $frameworkVersion = $frameworkMap[$lang]

        if (-not $frameworkVersion) {
            Write-Info "Unknown framework mapping for C# $lang. Skipping."
            continue
        }

        $projectName = if ($lang -eq '6') { 'StyleCop.Analyzers.Test' } else { "StyleCop.Analyzers.Test.CSharp$lang" }
        $dllPath = Join-Path $repoRoot "StyleCop.Analyzers\$projectName\bin\$Configuration\$frameworkVersion\$projectName.dll"

        if (-not (Test-Path $dllPath)) {
            Write-Info "Test assembly not found for C# $lang at $dllPath. Re-run without -NoBuild."
            $failures++
            continue
        }

        $runner = Join-Path $repoRoot "packages\xunit.runner.console.$xunitrunner_version\tools\$frameworkVersion\xunit.console.x86.exe"
        if (-not (Test-Path $runner)) {
            Write-Info "xUnit runner not found at $runner. Ensure packages are restored."
            $failures++
            continue
        }

        $xmlPath = Join-Path $resultsRoot "StyleCopAnalyzers.CSharp$lang.xunit.xml"
        $args = @($dllPath, '-noshadow', '-xml', $xmlPath)

        if (-not $fullRun) {
            if ($classes.Count -eq 0) {
                Write-Info "No impacted tests for C# $lang ($reason). Skipping."
                continue
            }

            Write-Info ("Running {0} selected test classes for C# {1} ({2})" -f $classes.Count, $lang, $reason)
            foreach ($c in $classes) {
                $args += '-class'
                $args += $c
            }
        } else {
            Write-Info "Running full suite for C# $lang ($reason)"
        }

        Write-DebugInfo ("Runner: {0}" -f $runner)
        Write-DebugInfo ("Args: {0}" -f ($args -join ' '))

        & $runner @args
        if ($LASTEXITCODE -ne 0) {
            Write-Info "Tests failed for C# $lang (exit $LASTEXITCODE)"
            $failures++
        }
    }

    if ($failures -ne 0) {
        throw "$failures test invocation(s) failed."
    }
}
finally {
    Pop-Location
}

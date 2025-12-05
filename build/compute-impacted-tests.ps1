# Requires: PowerShell 5+
# .SYNOPSIS
#   Computes an impacted-test plan from git diff and writes test-plan.json for the pipeline.
# .DESCRIPTION
#   Identifies changed analyzers/code fixes/tests/dependencies relative to a target branch, selects
#   full vs filtered test execution per language version, and emits an Azure Pipelines variable
#   `AllTestsFull` indicating whether every language should run a full suite.
# .PARAMETER OutputPath
#   Path to write the generated JSON plan (default: artifacts\test-plan.json).
# .PARAMETER LatestLangVersion
#   Highest C# test project version; newest test project is always run in full.
# .PARAMETER TargetBranch
#   Branch or ref to diff against (e.g., upstream/master). Required when using -AssumePullRequest.
# .PARAMETER AssumePullRequest
#   Forces PR-mode selection when running locally (otherwise uses BUILD_REASON).
# .PARAMETER VerboseLogging
#   Emit additional diagnostic output while computing the plan.
[CmdletBinding()]
param(
    [string]$OutputPath = "$PSScriptRoot\..\artifacts\test-plan.json",
    [string]$LatestLangVersion,
    [string]$TargetBranch,
    [switch]$AssumePullRequest,
    [switch]$VerboseLogging
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Write-Info($message) {
    Write-Host "[plan] $message"
}

function Write-DebugInfo($message) {
    if ($VerboseLogging) {
        Write-Host "[plan:debug] $message"
    }
}

function Normalize-Path([string]$path) {
    return [System.IO.Path]::GetFullPath($path)
}

function Get-Namespace([string]$content) {
    $match = [regex]::Match($content, 'namespace\s+([A-Za-z0-9_.]+)')
    if ($match.Success) {
        return $match.Groups[1].Value
    }

    return ''
}

function Get-ClassNames([string]$content) {
    $matches = [regex]::Matches($content, '(class|struct)\s+([A-Za-z0-9_`]+)', 'IgnoreCase')
    return ($matches | ForEach-Object { $_.Groups[2].Value }) | Sort-Object -Unique
}

function Get-AnalyzerAndFixTypes([string]$content) {
    $analyzers = New-Object System.Collections.Generic.HashSet[string]
    $codeFixes = New-Object System.Collections.Generic.HashSet[string]

    $verifierMatches = [regex]::Matches(
        $content,
        'StyleCopCodeFixVerifier<\s*([A-Za-z0-9_.]+)\s*,\s*([A-Za-z0-9_.]+)\s*>',
        'IgnoreCase')
    foreach ($m in $verifierMatches) {
        $null = $analyzers.Add($m.Groups[1].Value)
        $null = $codeFixes.Add($m.Groups[2].Value)
    }

    $diagnosticMatches = [regex]::Matches(
        $content,
        'StyleCopDiagnosticVerifier<\s*([A-Za-z0-9_.]+)\s*>',
        'IgnoreCase')
    foreach ($m in $diagnosticMatches) {
        $null = $analyzers.Add($m.Groups[1].Value)
    }

    $inheritanceMatches = [regex]::Matches(
        $content,
        ':\s*StyleCopCodeFixVerifier<\s*([A-Za-z0-9_.]+)\s*,\s*([A-Za-z0-9_.]+)\s*>',
        'IgnoreCase')
    foreach ($m in $inheritanceMatches) {
        $null = $analyzers.Add($m.Groups[1].Value)
        $null = $codeFixes.Add($m.Groups[2].Value)
    }

    return ,@($analyzers, $codeFixes)
}

function Get-AnalyzerTypesFromSource([string]$content, [string]$namespace) {
    $names = New-Object System.Collections.Generic.HashSet[string]
    $matches = [regex]::Matches(
        $content,
        'class\s+([A-Za-z0-9_`]+)[^{]*DiagnosticAnalyzer',
        'IgnoreCase')
    foreach ($m in $matches) {
        $short = $m.Groups[1].Value
        $null = $names.Add($short)
        if ($namespace) {
            $null = $names.Add("$namespace.$short")
        }
    }

    return $names
}

function Get-CodeFixTypesFromSource([string]$content, [string]$namespace) {
    $names = New-Object System.Collections.Generic.HashSet[string]
    $matches = [regex]::Matches(
        $content,
        'class\s+([A-Za-z0-9_`]+)[^{]*CodeFixProvider',
        'IgnoreCase')
    foreach ($m in $matches) {
        $short = $m.Groups[1].Value
        $null = $names.Add($short)
        if ($namespace) {
            $null = $names.Add("$namespace.$short")
        }
    }

    return $names
}

function Get-TestMap([string]$testsRoot, [int[]]$languageVersions) {
    $map = @{}
    $filesToClasses = @{}

    foreach ($version in $languageVersions) {
        $projectName = "StyleCop.Analyzers.Test.CSharp$version"
        $projectPath = Join-Path $testsRoot $projectName

        if (-not (Test-Path $projectPath)) {
            Write-Info "Skip missing test project for C# $version ($projectName)."
            continue
        }

        Write-Info "Indexing test project $projectName"
        $sourceFiles = Get-ChildItem -Path $projectPath -Filter *.cs -Recurse -File
        foreach ($file in $sourceFiles) {
            $content = Get-Content -Path $file.FullName -Raw
            $namespace = Get-Namespace $content
            $classNames = Get-ClassNames $content
            $analyzerSets = Get-AnalyzerAndFixTypes $content
            $analyzerTypes = $analyzerSets[0]
            $codeFixTypes = $analyzerSets[1]

            foreach ($className in $classNames) {
                $fullName = if ($namespace) { "$namespace.$className" } else { $className }
                $entry = @{
                    Version   = $version
                    ClassName = $fullName
                    File      = Normalize-Path $file.FullName
                    Analyzers = @($analyzerTypes)
                    CodeFixes = @($codeFixTypes)
                }

                $map[$fullName] = $entry

                if (-not $filesToClasses.ContainsKey($entry.File)) {
                    $filesToClasses[$entry.File] = New-Object System.Collections.Generic.List[string]
                }

                $filesToClasses[$entry.File].Add($fullName)
            }
        }
    }

    return ,@($map, $filesToClasses)
}

function Try-GetChangedFiles([string]$targetRef) {
    if (-not $targetRef) {
        Write-Info "Target branch not provided; unable to compute diff."
        return @()
    }

    $shortTarget = $targetRef -replace '^refs/heads/', ''
    $candidates = New-Object System.Collections.Generic.List[hashtable]
    $candidates.Add(@{ Fetch = 'origin'; Ref = $shortTarget; DiffRef = "origin/$shortTarget" })

    if ($targetRef -notmatch '^origin/' -and $shortTarget -ne $targetRef) {
        $candidates.Add(@{ Fetch = 'origin'; Ref = $targetRef; DiffRef = "origin/$targetRef" })
    }

    $remotes = @()
    try {
        $remotes = @(git remote)
    } catch {
        $remotes = @()
    }

    $parts = $targetRef.Split('/', 2)
    if ($parts.Length -eq 2 -and $remotes -contains $parts[0]) {
        $candidates.Add(@{ Fetch = $parts[0]; Ref = $parts[1]; DiffRef = "$($parts[0])/$($parts[1])" })
    }

    $candidates.Add(@{ Fetch = $null; Ref = $null; DiffRef = $targetRef })

    $failureMessages = New-Object System.Collections.Generic.List[string]

    foreach ($candidate in $candidates) {
        $diffRef = $candidate.DiffRef

        if ($candidate.Fetch) {
            try {
                git fetch $candidate.Fetch $candidate.Ref --quiet 2>$null | Out-Null
            } catch {
                $failureMessages.Add("Git fetch failed for $($candidate.Fetch)/$($candidate.Ref): $_")
                continue
            }
        }

        try {
            $changes = @(git diff --name-only "$diffRef...HEAD" 2>$null)
            Write-Info "Using diff base $diffRef"
            return $changes
        } catch {
            $failureMessages.Add(("Git diff failed for {0}: {1}" -f $diffRef, $_))
            continue
        }
    }

    foreach ($msg in $failureMessages) {
        Write-Info $msg
    }

    Write-Info "Unable to compute diff for $targetRef; defaulting to full run."
    return @()
}

function Build-Plan {
    $repoRoot = Normalize-Path (Join-Path $PSScriptRoot '..')
    $testsRoot = Join-Path $repoRoot 'StyleCop.Analyzers'

    $isPullRequest = ($env:BUILD_REASON -eq 'PullRequest') -or $AssumePullRequest.IsPresent
    $targetBranch = if ($TargetBranch) { $TargetBranch } else { $env:SYSTEM_PULLREQUEST_TARGETBRANCH }
    if (-not $targetBranch -and $AssumePullRequest) {
        $targetBranch = 'upstream/master'
    }

    $testProjects = Get-ChildItem -Path $testsRoot -Directory -Filter 'StyleCop.Analyzers.Test.CSharp*'
    $versionNumbers = @($testProjects | ForEach-Object {
            $m = [regex]::Match($_.Name, 'CSharp(\d+)')
            if ($m.Success) { [int]$m.Groups[1].Value }
        } | Sort-Object -Unique)

    $latest = if ($LatestLangVersion) { [int]$LatestLangVersion } else { ($versionNumbers | Sort-Object | Select-Object -Last 1) }
    $allVersions = @(6) + $versionNumbers
    $targetVersions = $versionNumbers | Where-Object { $_ -ge 7 -and $_ -lt $latest }

    $plan = [ordered]@{
        generatedAt        = (Get-Date).ToString('o')
        isPullRequest      = $isPullRequest
        latestLangVersion  = $latest
        dependencyChange   = $false
        plans              = @{}
        changedAnalyzers   = @()
        changedCodeFixes   = @()
        changedFiles       = @()
    }

    if (-not $isPullRequest) {
        Write-Info "Build reason is '$($env:BUILD_REASON)'; default to full test matrix."
        foreach ($v in $allVersions) {
            $key = "$v"
            $plan.plans[$key] = @{
                fullRun = $true
                classes = @()
                reason  = 'Non-PR build'
            }
        }

        return $plan
    }

    $changedFiles = @(Try-GetChangedFiles $targetBranch)
    $changedFilesCount = ($changedFiles | Measure-Object).Count
    if ($changedFilesCount -eq 0) {
        Write-Info "No changed files detected (target branch: $targetBranch); default to full test matrix."
        foreach ($v in $allVersions) {
            $plan.plans[$v] = @{
                fullRun = $true
                classes = @()
                reason  = 'Unable to compute diff'
            }
        }

        return $plan
    }

    $plan.changedFiles = $changedFiles
    Write-Info ("Changed files ({0})" -f $changedFilesCount)

    $dependencyChange = $false
    $dependencyMarkers = @(
        'Directory.Build.props',
        'Directory.Build.targets',
        'global.json',
        'NuGet.config',
        '.nuget/packages.config',
        'StyleCopAnalyzers.sln',
        'azure-pipelines.yml',
        'appveyor.yml',
        'init.ps1',
        'stylecop.json'
    )

    $testInfrastructureRoots = @(
        'StyleCop.Analyzers/StyleCop.Analyzers.Test/Verifiers',
        'StyleCop.Analyzers/StyleCop.Analyzers.Test/Helpers'
    )

    $changedAnalyzerNames = New-Object System.Collections.Generic.HashSet[string]
    $changedCodeFixNames = New-Object System.Collections.Generic.HashSet[string]
    $changedTestClasses = @{}

    foreach ($file in $changedFiles) {
        $normalized = $file -replace '\\', '/'

        if ($dependencyMarkers | Where-Object { $normalized.StartsWith($_) }) {
            $dependencyChange = $true
        }

        if ($normalized -match '\.csproj$' -or $normalized -match '\.props$' -or $normalized -match '\.targets$') {
            $dependencyChange = $true
        }

        if ($normalized.StartsWith('build/')) {
            $dependencyChange = $true
        }

        foreach ($root in $testInfrastructureRoots) {
            if ($normalized.StartsWith($root)) {
                $dependencyChange = $true
            }
        }
    }

    $plan.dependencyChange = $dependencyChange

    $mapResult = Get-TestMap -testsRoot $testsRoot -languageVersions $targetVersions
    $testMap = $mapResult[0]
    $fileToClass = $mapResult[1]

    foreach ($file in $changedFiles) {
        $fullPath = Normalize-Path (Join-Path $repoRoot $file)

        if ($fileToClass.ContainsKey($fullPath)) {
            foreach ($className in $fileToClass[$fullPath]) {
                $version = $testMap[$className].Version
                if (-not $changedTestClasses.ContainsKey($version)) {
                    $changedTestClasses[$version] = New-Object System.Collections.Generic.HashSet[string]
                }

                $null = $changedTestClasses[$version].Add($className)
            }
        }

        if ($normalized -like 'StyleCop.Analyzers/StyleCop.Analyzers.CodeFixes/*.cs' -or
            $normalized -like 'StyleCop.Analyzers/StyleCop.Analyzers.CodeFixes/*/*.cs' -or
            $normalized -like 'StyleCop.Analyzers/StyleCop.Analyzers.PrivateCodeFixes/*.cs' -or
            $normalized -like 'StyleCop.Analyzers/StyleCop.Analyzers.PrivateCodeFixes/*/*.cs') {
            $content = Get-Content -Path $fullPath -Raw
            $ns = Get-Namespace $content
            $names = Get-CodeFixTypesFromSource $content $ns
            foreach ($n in $names) { $null = $changedCodeFixNames.Add($n) }
        }

        if ($normalized -like 'StyleCop.Analyzers/StyleCop.Analyzers/*.cs' -or
            $normalized -like 'StyleCop.Analyzers/StyleCop.Analyzers/*/*.cs' -or
            $normalized -like 'StyleCop.Analyzers/StyleCop.Analyzers.PrivateAnalyzers/*.cs' -or
            $normalized -like 'StyleCop.Analyzers/StyleCop.Analyzers.PrivateAnalyzers/*/*.cs') {
            $content = Get-Content -Path $fullPath -Raw
            $ns = Get-Namespace $content
            $names = Get-AnalyzerTypesFromSource $content $ns
            foreach ($n in $names) { $null = $changedAnalyzerNames.Add($n) }
        }
    }

    $plan.changedAnalyzers = @($changedAnalyzerNames)
    $plan.changedCodeFixes = @($changedCodeFixNames)

    Write-Info ("Changed analyzers: {0}" -f ($plan.changedAnalyzers -join ', '))
    Write-Info ("Changed code fixes: {0}" -f ($plan.changedCodeFixes -join ', '))

    $impactedByAnalyzer = @{}
    foreach ($entry in $testMap.Values) {
        $version = $entry.Version
        if (-not $impactedByAnalyzer.ContainsKey($version)) {
            $impactedByAnalyzer[$version] = New-Object System.Collections.Generic.HashSet[string]
        }

        $hitAnalyzer = $entry.Analyzers | Where-Object {
            $short = $_.Split('.')[-1]
            $changedAnalyzerNames.Contains($_) -or $changedAnalyzerNames.Contains($short)
        }

        $hitCodeFix = $entry.CodeFixes | Where-Object {
            $short = $_.Split('.')[-1]
            $changedCodeFixNames.Contains($_) -or $changedCodeFixNames.Contains($short)
        }

        if ($hitAnalyzer -or $hitCodeFix) {
            $null = $impactedByAnalyzer[$version].Add($entry.ClassName)
        }
    }

    foreach ($v in $allVersions) {
        $key = "$v"
        $entry = @{
            fullRun = $true
            classes = @()
            reason  = ''
        }

        if ($v -eq 6) {
            $entry.reason = 'Always full for baseline test project'
        }
        elseif ($v -eq $latest) {
            $entry.reason = 'Always full for newest language version'
        }
        elseif ($dependencyChange) {
            $entry.reason = 'Dependency change detected'
        }
        else {
            $impacted = @()
            if ($changedTestClasses.ContainsKey($v)) {
                $impacted += $changedTestClasses[$v]
            }

            if ($impactedByAnalyzer.ContainsKey($v)) {
                $impacted += $impactedByAnalyzer[$v]
            }

            $impacted = $impacted | Sort-Object -Unique

            $entry.fullRun = $false
            $entry.classes = $impacted
            $entry.reason = if ($impacted.Count -eq 0) { 'No impacted tests' } else { 'Selected impacted test classes' }
        }

        $plan.plans[$key] = $entry
    }

    return $plan
}

try {
    $plan = Build-Plan

    $allFull = (($plan.plans.GetEnumerator() | Where-Object { -not $_.Value.fullRun }) | Measure-Object).Count -eq 0
    $json = $plan | ConvertTo-Json -Depth 6

    $outFile = Normalize-Path $OutputPath
    $outDir = Split-Path $outFile -Parent
    if (-not (Test-Path $outDir)) {
        New-Item -ItemType Directory -Path $outDir -Force | Out-Null
    }

    Set-Content -Path $outFile -Value $json -Encoding UTF8
    Write-Info "Test plan written to $outFile"

    $allFullValue = $allFull.ToString().ToLowerInvariant()
    Write-Host "##vso[task.setvariable variable=AllTestsFull;isOutput=true]$allFullValue"
}
catch {
    Write-Error $_
    Write-Info "Planner failed; falling back to full test matrix."

    $fallbackLatest = if ($LatestLangVersion) { [int]$LatestLangVersion } else { 13 }
    $fallbackVersions = 6..$fallbackLatest
    $fallback = [ordered]@{
        generatedAt       = (Get-Date).ToString('o')
        isPullRequest     = $env:BUILD_REASON -eq 'PullRequest'
        latestLangVersion = $fallbackLatest
        dependencyChange  = $true
        plans             = @{}
    }

    foreach ($v in $fallbackVersions) {
        $key = "$v"
        $fallback.plans[$key] = @{
            fullRun = $true
            classes = @()
            reason  = 'Planner failure fallback'
        }
    }

    $fallbackJson = $fallback | ConvertTo-Json -Depth 5
    $outFile = Normalize-Path $OutputPath
    $outDir = Split-Path $outFile -Parent
    if (-not (Test-Path $outDir)) {
        New-Item -ItemType Directory -Path $outDir -Force | Out-Null
    }

    Set-Content -Path $outFile -Value $fallbackJson -Encoding UTF8
    Write-Host "##vso[task.setvariable variable=AllTestsFull;isOutput=true]true"
}

[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $Runner,
    [ValidateSet('xUnit', 'TUnit')]
    [string] $Framework = 'xUnit',
    [string] $Configuration = 'Release',
    [int[]] $LanguageVersions = @(15),
    [string[]] $ThreadCounts = @('default', '4', '8', '16', '32', '64'),
    [ValidateSet('collections', 'all')]
    [string] $Parallel = 'collections',
    [int] $Repetitions = 3,
    [int] $ExpectedTests = 0,
    [string] $OutputDirectory = (Join-Path $PSScriptRoot 'test-performance-results')
)

$ErrorActionPreference = 'Stop'
$Runner = (Resolve-Path $Runner).Path
$root = Split-Path $PSScriptRoot
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Force $OutputDirectory | Out-Null
$env:COMPlus_BuildFlavor = 'SVR'
$env:COMPlus_gcConcurrent = '0'
$env:TESTINGPLATFORM_TELEMETRY_OPTOUT = '1'

$assemblies = foreach ($version in $LanguageVersions) {
    if ($Framework -ne 'xUnit') { break }
    $project = if ($version -eq 6) { 'StyleCop.Analyzers.Test' } else { "StyleCop.Analyzers.Test.CSharp$version" }
    (Resolve-Path (Join-Path $root "StyleCop.Analyzers\$project\bin\$Configuration\net472\$project.dll")).Path
}

$environment = [ordered]@{
    revision = (git -C $root rev-parse HEAD)
    dirty = @(git -C $root status --short)
    processors = [Environment]::ProcessorCount
    architecture = $env:PROCESSOR_ARCHITECTURE
    os = [Environment]::OSVersion.VersionString
    runner = $Runner
    framework = $Framework
    configuration = $Configuration
    versions = $LanguageVersions
    parallel = $Parallel
    serverGC = $env:COMPlus_BuildFlavor
    concurrentGC = $env:COMPlus_gcConcurrent
    resourceMetricScope = if ($Framework -eq 'TUnit') { 'unavailable: MTP launches a test worker process' } else { 'runner process, including test AppDomains' }
}
$environment | ConvertTo-Json | Set-Content (Join-Path $OutputDirectory 'environment.json')

$rows = @()
for ($iteration = 1; $iteration -le $Repetitions; $iteration++) {
    # Reverse alternating runs to reduce systematic warm-cache/order bias.
    $counts = @($ThreadCounts)
    if ($iteration % 2 -eq 0) { [array]::Reverse($counts) }
    foreach ($threads in $counts) {
        $name = "threads-$threads-repeat-$iteration"
        $extension = if ($Framework -eq 'xUnit') { 'xml' } else { 'trx' }
        $xml = Join-Path $OutputDirectory "$name.$extension"
        $log = Join-Path $OutputDirectory "$name.log"
        if ($Framework -eq 'xUnit') {
            $arguments = @($assemblies) + @('-noshadow', '-noautoreporters', '-parallel', $Parallel, '-maxthreads', $threads, '-xml', $xml)
        } else {
            $arguments = @('--no-ansi', '--progress', 'off', '--report-trx', '--report-trx-filename', $xml)
            if ($threads -ne 'default') { $arguments += @('--maximum-parallel-tests', $threads) }
        }
        $start = [Diagnostics.ProcessStartInfo]::new($Runner)
        foreach ($argument in $arguments) { $start.ArgumentList.Add($argument) }
        $start.UseShellExecute = $false
        $start.RedirectStandardOutput = $true
        $start.RedirectStandardError = $true
        $timer = [Diagnostics.Stopwatch]::StartNew()
        $process = [Diagnostics.Process]::Start($start)
        $stdout = $process.StandardOutput.ReadToEndAsync()
        $stderr = $process.StandardError.ReadToEndAsync()
        $peak = 0L
        while (-not $process.WaitForExit(100)) {
            $process.Refresh()
            $peak = [Math]::Max($peak, $process.PeakWorkingSet64)
        }

        $timer.Stop()
        ($stdout.GetAwaiter().GetResult() + $stderr.GetAwaiter().GetResult()) | Set-Content $log
        if ($process.ExitCode -ne 0) { throw "Test runner exited $($process.ExitCode). See $log" }
        [xml] $results = Get-Content $xml
        $total = 0
        $passed = 0
        $failed = 0
        $skipped = 0
        if ($Framework -eq 'xUnit') {
            foreach ($assembly in $results.assemblies.assembly) {
                $total += [int] $assembly.total
                $passed += [int] $assembly.passed
                $failed += [int] $assembly.failed
                $skipped += [int] $assembly.skipped
            }
        } else {
            $counters = $results.TestRun.ResultSummary.Counters
            $total = [int] $counters.total
            $passed = [int] $counters.passed
            $failed = [int] $counters.failed
            $skipped = [int] $counters.notExecuted
        }

        if ($total -eq 0 -or $failed -ne 0 -or $skipped -ne 0 -or $passed -ne $total) {
            throw "Incomplete test execution in $xml"
        }

        if ($ExpectedTests -eq 0) { $ExpectedTests = $total }
        if ($total -ne $ExpectedTests) { throw "Expected $ExpectedTests tests, but ran $total in $xml" }
        $row = [pscustomobject]@{
            threads = $threads
            iteration = $iteration
            wallSeconds = [Math]::Round($timer.Elapsed.TotalSeconds, 3)
            cpuSeconds = if ($Framework -eq 'xUnit') { [Math]::Round($process.TotalProcessorTime.TotalSeconds, 3) } else { $null }
            peakWorkingSetMB = if ($Framework -eq 'xUnit') { [Math]::Round($peak / 1MB, 1) } else { $null }
            total = $total
            passed = $passed
        }
        $process.Dispose()
        $rows += $row
        $rows | Export-Csv (Join-Path $OutputDirectory 'measurements.csv') -NoTypeInformation
        $row | Format-Table | Out-Host
    }
}

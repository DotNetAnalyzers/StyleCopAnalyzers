[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $Runner,
    [string] $Configuration = 'Release',
    [int[]] $LanguageVersions = @(15),
    [string[]] $ThreadCounts = @('default', '4', '8', '16', '32', '64'),
    [ValidateSet('collections', 'all')]
    [string] $Parallel = 'collections',
    [int] $Repetitions = 3,
    [string] $OutputDirectory = (Join-Path $PSScriptRoot 'test-performance-results')
)

$ErrorActionPreference = 'Stop'
$Runner = (Resolve-Path $Runner).Path
$root = Split-Path $PSScriptRoot
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Force $OutputDirectory | Out-Null
$env:COMPlus_BuildFlavor = 'SVR'
$env:COMPlus_gcConcurrent = '0'

$assemblies = foreach ($version in $LanguageVersions) {
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
    configuration = $Configuration
    versions = $LanguageVersions
    parallel = $Parallel
    serverGC = $env:COMPlus_BuildFlavor
    concurrentGC = $env:COMPlus_gcConcurrent
}
$environment | ConvertTo-Json | Set-Content (Join-Path $OutputDirectory 'environment.json')

$rows = @()
for ($iteration = 1; $iteration -le $Repetitions; $iteration++) {
    # Reverse alternating runs to reduce systematic warm-cache/order bias.
    $counts = @($ThreadCounts)
    if ($iteration % 2 -eq 0) { [array]::Reverse($counts) }
    foreach ($threads in $counts) {
        $name = "threads-$threads-repeat-$iteration"
        $xml = Join-Path $OutputDirectory "$name.xml"
        $log = Join-Path $OutputDirectory "$name.log"
        $arguments = @($assemblies) + @('-noshadow', '-noautoreporters', '-parallel', $Parallel, '-maxthreads', $threads, '-xml', $xml)
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
        foreach ($assembly in $results.assemblies.assembly) {
            $total += [int] $assembly.total
            $passed += [int] $assembly.passed
            $failed += [int] $assembly.failed
            $skipped += [int] $assembly.skipped
        }

        if ($total -eq 0 -or $failed -ne 0 -or $skipped -ne 0 -or $passed -ne $total) {
            throw "Incomplete test execution in $xml"
        }

        $row = [pscustomobject]@{
            threads = $threads
            iteration = $iteration
            wallSeconds = [Math]::Round($timer.Elapsed.TotalSeconds, 3)
            cpuSeconds = [Math]::Round($process.TotalProcessorTime.TotalSeconds, 3)
            peakWorkingSetMB = [Math]::Round($peak / 1MB, 1)
            total = $total
            passed = $passed
        }
        $process.Dispose()
        $rows += $row
        $rows | Export-Csv (Join-Path $OutputDirectory 'measurements.csv') -NoTypeInformation
        $row | Format-Table | Out-Host
    }
}

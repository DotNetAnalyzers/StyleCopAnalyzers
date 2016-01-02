param (
	[switch]$Debug
)

# Run a build to ensure everything is up-to-date
If ($Debug) {
	.\build.ps1 -Debug -Incremental
} Else {
	.\build.ps1 -Incremental
}

If (-not $?) {
	$host.UI.WriteErrorLine('Build failed; coverage analysis aborted.')
	Exit $LASTEXITCODE
}

If ($Debug) {
	$Configuration = 'Debug'
} Else {
	$Configuration = 'Release'
}

$packages_folder = '..\packages'
$opencover_console = "$packages_folder\OpenCover.4.6.247-rc\tools\OpenCover.Console.exe"
$xunit_runner_console = "$packages_folder\xunit.runner.console.2.1.0\tools\xunit.console.x86.exe"
$report_generator = "$packages_folder\ReportGenerator.2.3.5.0\tools\ReportGenerator.exe"
$report_folder = '.\OpenCover.Reports'
$target_dll = "..\StyleCop.Analyzers\StyleCop.Analyzers.Test\bin\$Configuration\StyleCop.Analyzers.Test.dll"

If (Test-Path $report_folder) {
	Remove-Item -Recurse -Force $report_folder
}

mkdir $report_folder | Out-Null

&$opencover_console `
	-register:user `
	-threshold:1 `
	-returntargetcode `
	-hideskipped:All `
	-filter:"+[StyleCop*]*" `
	-excludebyattribute:*.ExcludeFromCodeCoverage* `
	-output:"$report_folder\OpenCover.StyleCopAnalyzers.xml" `
	-target:"$xunit_runner_console" `
	-targetargs:"$target_dll -noshadow"

&$report_generator -targetdir:$report_folder -reports:$report_folder\OpenCover.*.xml

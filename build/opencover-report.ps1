param (
	[switch]$Debug,
	[switch]$NoBuild,
	[switch]$NoReport,
	[switch]$AppVeyor
)

If (-not $NoBuild) {
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
}

If ($Debug) {
	$Configuration = 'Debug'
} Else {
	$Configuration = 'Release'
}

$packageConfig = [xml](Get-Content ..\.nuget\packages.config)
$reportgenerator_version = $packageConfig.SelectSingleNode('/packages/package[@id="ReportGenerator"]').version
$xunitrunner_version = $packageConfig.SelectSingleNode('/packages/package[@id="xunit.runner.console"]').version

$packages_folder = '..\packages'
$xunit_runner_console = "$packages_folder\xunit.runner.console.$xunitrunner_version\tools\xunit.console.x86.exe"
$report_generator = "$packages_folder\ReportGenerator.$reportgenerator_version\tools\ReportGenerator.exe"
$report_folder = '.\OpenCover.Reports'
$target_dll = "..\StyleCop.Analyzers\StyleCop.Analyzers.Test\bin\$Configuration\net452\StyleCop.Analyzers.Test.dll"
$target_dll_csharp7 = "..\StyleCop.Analyzers\StyleCop.Analyzers.Test.CSharp7\bin\$Configuration\net46\StyleCop.Analyzers.Test.CSharp7.dll"

If (Test-Path $report_folder) {
	Remove-Item -Recurse -Force $report_folder
}

mkdir $report_folder | Out-Null

If ($AppVeyor) {
	$AppVeyorArg = '-appveyor'
}

&$xunit_runner_console $target_dll -noshadow $AppVeyorArg
If ($AppVeyor -and -not $?) {
	$host.UI.WriteErrorLine('Build failed; coverage analysis aborted.')
	Exit $LASTEXITCODE
}

&$xunit_runner_console $target_dll_csharp7 -noshadow $AppVeyorArg
If ($AppVeyor -and -not $?) {
	$host.UI.WriteErrorLine('Build failed; coverage analysis aborted.')
	Exit $LASTEXITCODE
}

If (-not $NoReport) {
	&$report_generator -targetdir:$report_folder -reports:$report_folder\OpenCover.*.xml
	$host.UI.WriteLine("Open $report_folder\index.htm to see code coverage results.")
}

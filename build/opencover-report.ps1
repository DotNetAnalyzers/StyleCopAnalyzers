$packages_folder = '..\packages'
$opencover_console = "$packages_folder\OpenCover.4.6.247-rc\tools\OpenCover.Console.exe"
$xunit_runner_console = "$packages_folder\xunit.runner.console.2.1.0\tools\xunit.console.x86.exe"
$report_generator = "$packages_folder\ReportGenerator.2.3.5.0\tools\ReportGenerator.exe"
$report_folder = '.\OpenCover.Reports'
$target_dll = '..\StyleCop.Analyzers\StyleCop.Analyzers.Test\bin\Debug\StyleCop.Analyzers.Test.dll'

If (-not (Test-Path $target_dll)) {
	$host.UI.WriteErrorLine('target dll not found (build target?)')
	$host.UI.WriteErrorLine("$target_dll")
	Exit 1
}

If (-not (Test-Path $opencover_console)) {
	$host.UI.WriteErrorLine('OpenCover Console not found (nuget restore?)')
	$host.UI.WriteErrorLine("$opencover_console")
	Exit 1
}

If (-not (Test-Path $xunit_runner_console)) {
	$host.UI.WriteErrorLine('OpenCover Console not found (nuget restore?)')
	$host.UI.WriteErrorLine("$xunit_runner_console")
	Exit 1
}

If (-not (Test-Path $report_generator)) {
	$host.UI.WriteErrorLine('Report Generator not found (nuget restore?)')
	$host.UI.WriteErrorLine("$report_generator")
	Exit 1
}

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

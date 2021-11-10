param (
	[switch]$Debug,
	[switch]$NoBuild,
	[switch]$NoReport,
	[switch]$AppVeyor,
	[switch]$Azure
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
$opencover_version = $packageConfig.SelectSingleNode('/packages/package[@id="OpenCover"]').version
$reportgenerator_version = $packageConfig.SelectSingleNode('/packages/package[@id="ReportGenerator"]').version
$xunitrunner_version = $packageConfig.SelectSingleNode('/packages/package[@id="xunit.runner.console"]').version

$packages_folder = '..\packages'
$opencover_console = "$packages_folder\OpenCover.$opencover_version\tools\OpenCover.Console.exe"
$xunit_runner_console_net452 = "$packages_folder\xunit.runner.console.$xunitrunner_version\tools\net452\xunit.console.x86.exe"
$xunit_runner_console_net46 = "$packages_folder\xunit.runner.console.$xunitrunner_version\tools\net46\xunit.console.x86.exe"
$xunit_runner_console_net472 = "$packages_folder\xunit.runner.console.$xunitrunner_version\tools\net472\xunit.console.x86.exe"
$report_generator = "$packages_folder\ReportGenerator.$reportgenerator_version\tools\ReportGenerator.exe"
$report_folder = '.\OpenCover.Reports'
$target_dll = "..\StyleCop.Analyzers\StyleCop.Analyzers.Test\bin\$Configuration\net452\StyleCop.Analyzers.Test.dll"
$target_dll_csharp7 = "..\StyleCop.Analyzers\StyleCop.Analyzers.Test.CSharp7\bin\$Configuration\net46\StyleCop.Analyzers.Test.CSharp7.dll"
$target_dll_csharp8 = "..\StyleCop.Analyzers\StyleCop.Analyzers.Test.CSharp8\bin\$Configuration\net472\StyleCop.Analyzers.Test.CSharp8.dll"
$target_dll_csharp9 = "..\StyleCop.Analyzers\StyleCop.Analyzers.Test.CSharp9\bin\$Configuration\net472\StyleCop.Analyzers.Test.CSharp9.dll"
$target_dll_csharp10 = "..\StyleCop.Analyzers\StyleCop.Analyzers.Test.CSharp10\bin\$Configuration\net472\StyleCop.Analyzers.Test.CSharp10.dll"

If (Test-Path $report_folder) {
	Remove-Item -Recurse -Force $report_folder
}

mkdir $report_folder | Out-Null

$register_mode = 'user'
If ($AppVeyor) {
	$AppVeyorArg = '-appveyor'
	$register_mode = 'Path32'
} ElseIf ($Azure) {
	$register_mode = 'Path32'
}

$exitCode = 0

&$opencover_console `
	-register:$register_mode `
	-threshold:1 -oldStyle `
	-returntargetcode `
	-hideskipped:All `
	-filter:"+[StyleCop*]*" `
	-excludebyattribute:*.ExcludeFromCodeCoverage* `
	-excludebyfile:*\*Designer.cs `
	-output:"$report_folder\OpenCover.StyleCopAnalyzers.xml" `
	-target:"$xunit_runner_console_net452" `
	-targetargs:"$target_dll -noshadow $AppVeyorArg -xml StyleCopAnalyzers.xunit.xml"

If (($AppVeyor -or $Azure) -and -not $?) {
	$host.UI.WriteErrorLine('Build failed; coverage analysis may be incomplete.')
	$exitCode = $LASTEXITCODE
}

&$opencover_console `
	-register:$register_mode `
	-threshold:1 -oldStyle `
	-returntargetcode `
	-hideskipped:All `
	-filter:"+[StyleCop*]*" `
	-excludebyattribute:*.ExcludeFromCodeCoverage* `
	-excludebyfile:*\*Designer.cs `
	-output:"$report_folder\OpenCover.StyleCopAnalyzers.xml" `
	-mergebyhash -mergeoutput `
	-target:"$xunit_runner_console_net46" `
	-targetargs:"$target_dll_csharp7 -noshadow $AppVeyorArg -xml StyleCopAnalyzers.CSharp7.xunit.xml"

If (($AppVeyor -or $Azure) -and -not $?) {
	$host.UI.WriteErrorLine('Build failed; coverage analysis may be incomplete.')
	$exitCode = $LASTEXITCODE
}

&$opencover_console `
	-register:$register_mode `
	-threshold:1 -oldStyle `
	-returntargetcode `
	-hideskipped:All `
	-filter:"+[StyleCop*]*" `
	-excludebyattribute:*.ExcludeFromCodeCoverage* `
	-excludebyfile:*\*Designer.cs `
	-output:"$report_folder\OpenCover.StyleCopAnalyzers.xml" `
	-mergebyhash -mergeoutput `
	-target:"$xunit_runner_console_net472" `
	-targetargs:"$target_dll_csharp8 -noshadow $AppVeyorArg -xml StyleCopAnalyzers.CSharp8.xunit.xml"

If (($AppVeyor -or $Azure) -and -not $?) {
	$host.UI.WriteErrorLine('Build failed; coverage analysis may be incomplete.')
	$exitCode = $LASTEXITCODE
}

&$opencover_console `
	-register:$register_mode `
	-threshold:1 -oldStyle `
	-returntargetcode `
	-hideskipped:All `
	-filter:"+[StyleCop*]*" `
	-excludebyattribute:*.ExcludeFromCodeCoverage* `
	-excludebyfile:*\*Designer.cs `
	-output:"$report_folder\OpenCover.StyleCopAnalyzers.xml" `
	-mergebyhash -mergeoutput `
	-target:"$xunit_runner_console_net472" `
	-targetargs:"$target_dll_csharp9 -noshadow $AppVeyorArg -xml StyleCopAnalyzers.CSharp9.xunit.xml"

If (($AppVeyor -or $Azure) -and -not $?) {
	$host.UI.WriteErrorLine('Build failed; coverage analysis may be incomplete.')
	$exitCode = $LASTEXITCODE
}

&$opencover_console `
	-register:$register_mode `
	-threshold:1 -oldStyle `
	-returntargetcode `
	-hideskipped:All `
	-filter:"+[StyleCop*]*" `
	-excludebyattribute:*.ExcludeFromCodeCoverage* `
	-excludebyfile:*\*Designer.cs `
	-output:"$report_folder\OpenCover.StyleCopAnalyzers.xml" `
	-mergebyhash -mergeoutput `
	-target:"$xunit_runner_console_net472" `
	-targetargs:"$target_dll_csharp10 -noshadow $AppVeyorArg -xml StyleCopAnalyzers.CSharp10.xunit.xml"

If (($AppVeyor -or $Azure) -and -not $?) {
	$host.UI.WriteErrorLine('Build failed; coverage analysis may be incomplete.')
	$exitCode = $LASTEXITCODE
}

If (-not $NoReport) {
	&$report_generator -targetdir:$report_folder -reports:$report_folder\OpenCover.*.xml
	$host.UI.WriteLine("Open $report_folder\index.htm to see code coverage results.")
}

Exit $exitCode

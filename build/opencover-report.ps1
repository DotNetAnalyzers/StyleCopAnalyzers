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
$opencover_version = $packageConfig.SelectSingleNode('/packages/package[@id="OpenCover"]').version
$reportgenerator_version = $packageConfig.SelectSingleNode('/packages/package[@id="ReportGenerator"]').version
$xunitrunner_version = $packageConfig.SelectSingleNode('/packages/package[@id="xunit.runner.console"]').version
$pdb2pdb_version = $packageConfig.SelectSingleNode('/packages/package[@id="Microsoft.DiaSymReader.Pdb2Pdb"]').version

$packages_folder = '..\packages'
$opencover_console = "$packages_folder\OpenCover.$opencover_version\tools\OpenCover.Console.exe"
$xunit_runner_console = "$packages_folder\xunit.runner.console.$xunitrunner_version\tools\xunit.console.x86.exe"
$report_generator = "$packages_folder\ReportGenerator.$reportgenerator_version\tools\ReportGenerator.exe"
$pdb2pdb = "$packages_folder\Microsoft.DiaSymReader.Pdb2Pdb.$pdb2pdb_version\tools\Pdb2Pdb.exe"
$report_folder = '.\OpenCover.Reports'
$symbols_folder = '.\OpenCover.Symbols'
$target_dll = "..\StyleCop.Analyzers\StyleCop.Analyzers.Test\bin\$Configuration\net452\StyleCop.Analyzers.Test.dll"
$target_dll_csharp7 = "..\StyleCop.Analyzers\StyleCop.Analyzers.Test.CSharp7\bin\$Configuration\net46\StyleCop.Analyzers.Test.CSharp7.dll"
$target_dll_csharp8 = "..\StyleCop.Analyzers\StyleCop.Analyzers.Test.CSharp8\bin\$Configuration\net472\StyleCop.Analyzers.Test.CSharp8.dll"

If (Test-Path $symbols_folder) {
	Remove-Item -Recurse -Force $symbols_folder
}

$symbols_folder_csharp6 = Join-Path $symbols_folder 'CSharp6'
$symbols_folder_csharp7 = Join-Path $symbols_folder 'CSharp7'
$symbols_folder_csharp8 = Join-Path $symbols_folder 'CSharp8'
mkdir $symbols_folder | Out-Null
mkdir $symbols_folder_csharp6 | Out-Null
mkdir $symbols_folder_csharp7 | Out-Null
mkdir $symbols_folder_csharp8 | Out-Null

function Convert-Coverage-Pdb() {
	param (
		[string]$assembly,
		[string]$outputDir
	)
	$sourceDir = [IO.Path]::GetDirectoryName($assembly)
	$outputName = [IO.Path]::ChangeExtension([IO.Path]::GetFileName($assembly), 'pdb')
	$sourcePdb = Join-Path $sourceDir $outputName
	$output = Join-Path $outputDir $outputName
	if (Test-Path $sourcePdb) {
		&$pdb2pdb $assembly /out $output

		# Workaround for https://github.com/OpenCover/opencover/issues/800
		Remove-Item $sourcePdb
		Copy-Item $assembly $outputDir
	}
}

function Extract-Coverage-Pdb() {
	param (
		[string]$assembly,
		[string]$outputDir
	)
	$sourceDir = [IO.Path]::GetDirectoryName($assembly)
	$outputName = [IO.Path]::ChangeExtension([IO.Path]::GetFileName($assembly), 'pdb')
	$intermediatePdb = Join-Path $sourceDir $outputName
	$output = Join-Path $outputDir $outputName
	if (-not (Test-Path $output)) {
		&$pdb2pdb $assembly /out $intermediatePdb /extract
		if (Test-Path $intermediatePdb) {
			&$pdb2pdb $assembly /pdb $intermediatePdb /out $output
			Remove-Item $intermediatePdb

			# Workaround for https://github.com/OpenCover/opencover/issues/800
			Copy-Item $assembly $outputDir
		}
	}
}

$target_dir = [IO.Path]::GetDirectoryName($target_dll)
Get-ChildItem $target_dir -Filter *.dll | Foreach-Object { Convert-Coverage-Pdb -assembly $_.FullName -outputDir $symbols_folder_csharp6 }
Get-ChildItem $target_dir -Filter *.dll | Foreach-Object { Extract-Coverage-Pdb -assembly $_.FullName -outputDir $symbols_folder_csharp6 }

$target_dir = [IO.Path]::GetDirectoryName($target_dll_csharp7)
Get-ChildItem $target_dir -Filter *.dll | Foreach-Object { Convert-Coverage-Pdb -assembly $_.FullName -outputDir $symbols_folder_csharp7 }
Get-ChildItem $target_dir -Filter *.dll | Foreach-Object { Extract-Coverage-Pdb -assembly $_.FullName -outputDir $symbols_folder_csharp7 }

$target_dir = [IO.Path]::GetDirectoryName($target_dll_csharp8)
Get-ChildItem $target_dir -Filter *.dll | Foreach-Object { Convert-Coverage-Pdb -assembly $_.FullName -outputDir $symbols_folder_csharp8 }
Get-ChildItem $target_dir -Filter *.dll | Foreach-Object { Extract-Coverage-Pdb -assembly $_.FullName -outputDir $symbols_folder_csharp8 }

If (Test-Path $report_folder) {
	Remove-Item -Recurse -Force $report_folder
}

mkdir $report_folder | Out-Null

If ($AppVeyor) {
	$AppVeyorArg = '-appveyor'
}

&$opencover_console `
	-register:user `
	-threshold:1 -oldStyle `
	-returntargetcode `
	-hideskipped:All `
	-filter:"+[StyleCop*]*" `
	-excludebyattribute:*.ExcludeFromCodeCoverage* `
	-excludebyfile:*\*Designer.cs `
	-searchdirs:"$symbols_folder_csharp6" `
	-output:"$report_folder\OpenCover.StyleCopAnalyzers.xml" `
	-target:"$xunit_runner_console" `
	-targetargs:"$target_dll -noshadow $AppVeyorArg"

If ($AppVeyor -and -not $?) {
	$host.UI.WriteErrorLine('Build failed; coverage analysis aborted.')
	Exit $LASTEXITCODE
}

&$opencover_console `
	-register:user `
	-threshold:1 -oldStyle `
	-returntargetcode `
	-hideskipped:All `
	-filter:"+[StyleCop*]*" `
	-excludebyattribute:*.ExcludeFromCodeCoverage* `
	-excludebyfile:*\*Designer.cs `
	-searchdirs:"$symbols_folder_csharp7" `
	-output:"$report_folder\OpenCover.StyleCopAnalyzers.xml" `
	-mergebyhash -mergeoutput `
	-target:"$xunit_runner_console" `
	-targetargs:"$target_dll_csharp7 -noshadow $AppVeyorArg"

If ($AppVeyor -and -not $?) {
	$host.UI.WriteErrorLine('Build failed; coverage analysis aborted.')
	Exit $LASTEXITCODE
}

&$opencover_console `
	-register:user `
	-threshold:1 -oldStyle `
	-returntargetcode `
	-hideskipped:All `
	-filter:"+[StyleCop*]*" `
	-excludebyattribute:*.ExcludeFromCodeCoverage* `
	-excludebyfile:*\*Designer.cs `
	-searchdirs:"$symbols_folder_csharp8" `
	-output:"$report_folder\OpenCover.StyleCopAnalyzers.xml" `
	-mergebyhash -mergeoutput `
	-target:"$xunit_runner_console" `
	-targetargs:"$target_dll_csharp8 -noshadow $AppVeyorArg"

If ($AppVeyor -and -not $?) {
	$host.UI.WriteErrorLine('Build failed; coverage analysis aborted.')
	Exit $LASTEXITCODE
}

If (-not $NoReport) {
	&$report_generator -targetdir:$report_folder -reports:$report_folder\OpenCover.*.xml
	$host.UI.WriteLine("Open $report_folder\index.htm to see code coverage results.")
}

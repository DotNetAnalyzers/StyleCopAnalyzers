param (
	[switch]$Debug
)

cd StyleCop.Analyzers\StyleCop.Analyzers.Test

$packages_folder = "$env:USERPROFILE\.nuget\packages\"
$opencover_console = "$packages_folder\opencover\4.6.519\tools\OpenCover.Console.exe"

&$opencover_console `
	-register:user `
	-threshold:1 `
	-returntargetcode `
	-hideskipped:All `
	-filter:"+[StyleCop*]*" `
	-excludebyattribute:*.ExcludeFromCodeCoverage* `
	-excludebyfile:*\*Designer.cs `
	-output:"StyleCopAnalyzers_coverage.xml" `
	-target:"C:\Program Files\dotnet\dotnet.exe" `
	-targetargs:"test" `
    -oldstyle

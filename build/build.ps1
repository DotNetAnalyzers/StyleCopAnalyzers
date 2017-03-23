param (
	[switch]$Debug,
	[string]$VisualStudioVersion = '15.0',
	[switch]$SkipKeyCheck,
	[string]$Verbosity = 'minimal',
	[string]$Logger,
	[switch]$Incremental
)

# build the solution
$SolutionPath = "..\StyleCopAnalyzers.sln"

# make sure the script was run from the expected path
if (!(Test-Path $SolutionPath)) {
	$host.ui.WriteErrorLine('The script was run from an invalid working directory.')
	exit 1
}

. .\version.ps1

If ($Debug) {
	$BuildConfig = 'Debug'
} Else {
	$BuildConfig = 'Release'
}

If ($Version.Contains('-')) {
	$KeyConfiguration = 'Dev'
} Else {
	$KeyConfiguration = 'Final'
}

# build the main project
$dotnet = "dotnet"
If (-not (Get-Command $dotnet -ErrorAction SilentlyContinue)) {
	$host.UI.WriteErrorLine("Couldn't find dotnet.exe")
	exit 1
}

If ($Logger) {
	$LoggerArgument = "/logger:$Logger"
}

If ($Incremental) {
	$Target = 'build'
} Else {
	$Target = 'rebuild'
}

&$dotnet 'msbuild' '/nologo' '/m' "/t:$Target" $LoggerArgument "/verbosity:$Verbosity" "/p:Configuration=$BuildConfig" "/p:VisualStudioVersion=$VisualStudioVersion" "/p:KeyConfiguration=$KeyConfiguration" $SolutionPath
If (-not $?) {
	$host.ui.WriteErrorLine('Build failed, aborting!')
	exit $LASTEXITCODE
}

if ($Incremental) {
	# Skip NuGet validation and copying packages to the output directory
	exit 0
}

# By default, do not create a NuGet package unless the expected strong name key files were used
if (-not $SkipKeyCheck) {
	. .\keys.ps1

	foreach ($pair in $Keys.GetEnumerator()) {
		$assembly = Resolve-FullPath -Path "..\StyleCop.Analyzers\StyleCop.Analyzers.CodeFixes\bin\$BuildConfig\StyleCop.Analyzers.dll"
		# Run the actual check in a separate process or the current process will keep the assembly file locked
		powershell -Command ".\check-key.ps1 -Assembly '$assembly' -ExpectedKey '$($pair.Value)' -Build '$($pair.Key)'"
		If (-not $?) {
			$host.ui.WriteErrorLine('Failed to verify strong name key for build, aborting!')
			exit $LASTEXITCODE
		}

		$assembly = Resolve-FullPath -Path "..\StyleCop.Analyzers\StyleCop.Analyzers.CodeFixes\bin\$BuildConfig\StyleCop.Analyzers.CodeFixes.dll"
		# Run the actual check in a separate process or the current process will keep the assembly file locked
		powershell -Command ".\check-key.ps1 -Assembly '$assembly' -ExpectedKey '$($pair.Value)' -Build '$($pair.Key)'"
		If (-not $?) {
			$host.ui.WriteErrorLine('Failed to verify strong name key for build, aborting!')
			exit $LASTEXITCODE
		}
	}
}

if (-not (Test-Path 'nuget')) {
	mkdir "nuget"
}

Copy-Item "..\StyleCop.Analyzers\StyleCop.Analyzers.CodeFixes\bin\$BuildConfig\StyleCop.Analyzers.$Version.nupkg" 'nuget'
Copy-Item "..\StyleCop.Analyzers\StyleCop.Analyzers.CodeFixes\bin\$BuildConfig\StyleCop.Analyzers.$Version.symbols.nupkg" 'nuget'

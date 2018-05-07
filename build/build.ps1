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

# download nuget.exe if necessary
$nuget = '..\.nuget\nuget.exe'
If (-not (Test-Path $nuget)) {
	If (-not (Test-Path '..\.nuget')) {
		mkdir '..\.nuget'
	}

	$nugetSource = 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe'
	Invoke-WebRequest $nugetSource -OutFile $nuget
	If (-not $?) {
		$host.ui.WriteErrorLine('Unable to download NuGet executable, aborting!')
		exit $LASTEXITCODE
	}
}

# build the main project
$visualStudio = (Get-ItemProperty 'HKLM:\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\SxS\VS7')."$VisualStudioVersion"
$msbuild = "$visualStudio\MSBuild\$VisualStudioVersion\Bin\MSBuild.exe"
If (-not (Test-Path $msbuild)) {
	$host.UI.WriteErrorLine("Couldn't find MSBuild.exe")
	exit 1
}

# Attempt to restore packages up to 3 times, to improve resiliency to connection timeouts and access denied errors.
$maxAttempts = 3
For ($attempt = 0; $attempt -lt $maxAttempts; $attempt++) {
	&$nuget 'restore' $SolutionPath
	If ($?) {
		Break
	} ElseIf (($attempt + 1) -eq $maxAttempts) {
		$host.ui.WriteErrorLine('Failed to restore required NuGet packages, aborting!')
		exit $LASTEXITCODE
	}
}

If ($Logger) {
	$LoggerArgument = "/logger:$Logger"
}

If ($Incremental) {
	$Target = 'build'
} Else {
	$Target = 'rebuild'
}

&$msbuild '/nologo' '/m' '/nr:false' "/t:$Target" $LoggerArgument "/verbosity:$Verbosity" "/p:Configuration=$BuildConfig" "/p:VisualStudioVersion=$VisualStudioVersion" "/p:KeyConfiguration=$KeyConfiguration" $SolutionPath
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
		$assembly = Resolve-FullPath -Path "..\StyleCop.Analyzers\StyleCop.Analyzers.CodeFixes\bin\$BuildConfig\$($pair.Key)\StyleCop.Analyzers.dll"
		# Run the actual check in a separate process or the current process will keep the assembly file locked
		powershell -Command ".\check-key.ps1 -Assembly '$assembly' -ExpectedKey '$($pair.Value)' -Build '$($pair.Key)'"
		If (-not $?) {
			$host.ui.WriteErrorLine('Failed to verify strong name key for build, aborting!')
			exit $LASTEXITCODE
		}

		$assembly = Resolve-FullPath -Path "..\StyleCop.Analyzers\StyleCop.Analyzers.CodeFixes\bin\$BuildConfig\$($pair.Key)\StyleCop.Analyzers.CodeFixes.dll"
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

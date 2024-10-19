param (
	[switch]$Debug,
	[string]$VisualStudioVersion = '15.0',
	[string]$Verbosity = 'minimal',
	[string]$Logger,
	[switch]$Incremental
)

# build the solution
$SolutionPath = Resolve-Path "$PSScriptRoot\..\StyleCopAnalyzers.sln"

If ($Debug) {
	$BuildConfig = 'Debug'
} Else {
	$BuildConfig = 'Release'
}

# download nuget.exe if necessary
$nugetDir = "$PSScriptRoot\..\.nuget"
$nuget = "$nugetDir\nuget.exe"
If (-not (Test-Path $nuget)) {
	If (-not (Test-Path $nugetDir)) {
		mkdir $nugetDir
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

&$msbuild '/nologo' '/m' '/nr:false' "/t:$Target" $LoggerArgument "/verbosity:$Verbosity" "/p:Configuration=$BuildConfig" "/p:VisualStudioVersion=$VisualStudioVersion" $SolutionPath
If (-not $?) {
	$host.ui.WriteErrorLine('Build failed, aborting!')
	exit $LASTEXITCODE
}

$NuGetExe = "$PSScriptRoot\NuGet.exe"

& $NuGetExe restore "$PSScriptRoot\packages.config" -PackagesDirectory "$PSScriptRoot\..\..\packages"
& $NuGetExe restore "$PSScriptRoot\..\Analyzers.sln" -PackagesDirectory "$PSScriptRoot\..\..\packages"


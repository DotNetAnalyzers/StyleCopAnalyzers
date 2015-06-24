$NuGetExe = "$PSScriptRoot\NuGet.exe"

& $NuGetExe restore "$PSScriptRoot\packages.config" -PackagesDirectory "$PSScriptRoot\..\..\packages"
& $NuGetExe restore "$PSScriptRoot\..\MetaCompilation\MetaCompilation.sln"

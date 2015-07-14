$NuGetExe = "$PSScriptRoot\NuGet.exe"

& $NuGetExe restore "$PSScriptRoot\packages.config" -PackagesDirectory "$PSScriptRoot\..\..\packages"
& $NuGetExe restore "$PSScriptRoot\..\MetaCompilation\MetaCompilation.sln" -PackagesDirectory "$PSScriptRoot\..\..\packages"
& $NuGetExe restore "$PSScriptRoot\..\AsyncPackage\AsyncPackage.sln" -PackagesDirectory "$PSScriptRoot\..\..\packages"


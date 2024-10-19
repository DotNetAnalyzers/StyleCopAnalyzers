$NuGet = '..\..\.nuget\NuGet.exe'
&$NuGet update -Self -Verbosity quiet

# Make sure the project binaries are up-to-date
&$NuGet restore ..\..\StyleCopAnalyzers.sln
Push-Location
cd ..\..\build
.\build.ps1 -Incremental
Pop-Location

&$NuGet install Microsoft.CodeAnalysis -Version 1.1.0 -OutputDirectory ..\..\packages -Verbosity quiet
&$NuGet install Microsoft.CodeAnalysis -Version 1.1.1 -OutputDirectory ..\..\packages -Verbosity quiet

# Create folders for the testing tool
Remove-Item .\bin\StyleCopTester-Roslyn.1.0.0 -Force -Recurse -ErrorAction Ignore
Remove-Item .\bin\StyleCopTester-Roslyn.1.1.0 -Force -Recurse -ErrorAction Ignore
Remove-Item .\bin\StyleCopTester-Roslyn.1.1.1 -Force -Recurse -ErrorAction Ignore

New-Item .\bin\StyleCopTester-Roslyn.1.0.0 -ItemType Directory
New-Item .\bin\StyleCopTester-Roslyn.1.1.0 -ItemType Directory
New-Item .\bin\StyleCopTester-Roslyn.1.1.1 -ItemType Directory

Copy-Item ..\StyleCopTester\bin\Release\* .\bin\StyleCopTester-Roslyn.1.0.0
Copy-Item ..\StyleCopTester\bin\Release\* .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\StyleCopTester\bin\Release\* .\bin\StyleCopTester-Roslyn.1.1.1

Copy-Item .\StyleCopTester-Roslyn.1.0.exe.config .\bin\StyleCopTester-Roslyn.1.0.0\StyleCopTester.exe.config
Copy-Item .\StyleCopTester-Roslyn.1.1.exe.config .\bin\StyleCopTester-Roslyn.1.1.0\StyleCopTester.exe.config
Copy-Item .\StyleCopTester-Roslyn.1.1.exe.config .\bin\StyleCopTester-Roslyn.1.1.1\StyleCopTester.exe.config

Copy-Item ..\..\packages\Microsoft.CodeAnalysis.Common.1.1.0\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.Workspaces.Common.1.1.0\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\System.Collections.Immutable.1.1.37\lib\dotnet\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\System.Reflection.Metadata.1.1.0\lib\dotnet5.2\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\Microsoft.Composition.1.0.27\lib\portable-net45+win8+wp8+wpa81\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.CSharp.1.1.0\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.CSharp.Workspaces.1.1.0\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.VisualBasic.1.1.0\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.VisualBasic.Workspaces.1.1.0\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.0

Copy-Item ..\..\packages\Microsoft.CodeAnalysis.Common.1.1.1\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.1
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.Workspaces.Common.1.1.1\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.1
Copy-Item ..\..\packages\System.Collections.Immutable.1.1.37\lib\dotnet\*.dll .\bin\StyleCopTester-Roslyn.1.1.1
Copy-Item ..\..\packages\System.Reflection.Metadata.1.1.0\lib\dotnet5.2\*.dll .\bin\StyleCopTester-Roslyn.1.1.1
Copy-Item ..\..\packages\Microsoft.Composition.1.0.27\lib\portable-net45+win8+wp8+wpa81\*.dll .\bin\StyleCopTester-Roslyn.1.1.1
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.CSharp.1.1.1\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.1
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.CSharp.Workspaces.1.1.1\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.1
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.VisualBasic.1.1.1\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.1
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.VisualBasic.Workspaces.1.1.1\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.1

#
# Testing DartVS/DartVS@6f54d1d2
#

git clone https://github.com/DartVS/DartVS.git bin\DartVS
Push-Location
cd bin\DartVS
git checkout 6f54d1d2bf6a16aaac5a6add7e073716e35e21ba
Pop-Location

&$NuGet restore bin\DartVS\DanTup.DartVS.sln -Verbosity quiet

.\bin\StyleCopTester-Roslyn.1.0.0\StyleCopTester.exe bin\DartVS\DanTup.DartVS.sln /all /log:bin\DartVS-1.0.0.txt | Out-Null
.\bin\StyleCopTester-Roslyn.1.1.0\StyleCopTester.exe bin\DartVS\DanTup.DartVS.sln /all /log:bin\DartVS-1.1.0.txt | Out-Null
.\bin\StyleCopTester-Roslyn.1.1.1\StyleCopTester.exe bin\DartVS\DanTup.DartVS.sln /all /log:bin\DartVS-1.1.1.txt | Out-Null

#
# Testing JamesNK/Newtonsoft.Json@48786adc
#

git clone https://github.com/JamesNK/Newtonsoft.Json.git bin\Newtonsoft.Json
Push-Location
cd bin\Newtonsoft.Json
git checkout 48786adc5bf9e9bcaea52147f09d6022ae14082c
Pop-Location

&$NuGet restore bin\Newtonsoft.Json\Src\Newtonsoft.Json.Portable.sln -Verbosity quiet

.\bin\StyleCopTester-Roslyn.1.0.0\StyleCopTester.exe bin\Newtonsoft.Json\Src\Newtonsoft.Json.Portable.sln /all /log:bin\Newtonsoft.Json-1.0.0.txt | Out-Null
.\bin\StyleCopTester-Roslyn.1.1.0\StyleCopTester.exe bin\Newtonsoft.Json\Src\Newtonsoft.Json.Portable.sln /all /log:bin\Newtonsoft.Json-1.1.0.txt | Out-Null
.\bin\StyleCopTester-Roslyn.1.1.1\StyleCopTester.exe bin\Newtonsoft.Json\Src\Newtonsoft.Json.Portable.sln /all /log:bin\Newtonsoft.Json-1.1.1.txt | Out-Null

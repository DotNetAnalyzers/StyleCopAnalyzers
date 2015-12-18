$NuGet = '..\..\.nuget\NuGet.exe'
&$NuGet install Microsoft.CodeAnalysis -Version 1.1.0 -OutputDirectory ..\..\packages -Verbosity quiet

# Create folders for the testing tool
Remove-Item .\bin\StyleCopTester-Roslyn.1.0.0 -Force -Recurse -ErrorAction Ignore
Remove-Item .\bin\StyleCopTester-Roslyn.1.1.0 -Force -Recurse -ErrorAction Ignore

New-Item .\bin\StyleCopTester-Roslyn.1.0.0 -ItemType Directory
New-Item .\bin\StyleCopTester-Roslyn.1.1.0 -ItemType Directory

Copy-Item ..\StyleCopTester\bin\Release\* .\bin\StyleCopTester-Roslyn.1.0.0
Copy-Item ..\StyleCopTester\bin\Release\* .\bin\StyleCopTester-Roslyn.1.1.0

Copy-Item .\StyleCopTester-Roslyn.1.0.0.exe.config .\bin\StyleCopTester-Roslyn.1.0.0\StyleCopTester.exe.config
Copy-Item .\StyleCopTester-Roslyn.1.1.0.exe.config .\bin\StyleCopTester-Roslyn.1.1.0\StyleCopTester.exe.config

Copy-Item ..\..\packages\Microsoft.CodeAnalysis.Common.1.1.0\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.Workspaces.Common.1.1.0\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\System.Collections.Immutable.1.1.37\lib\dotnet\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\System.Reflection.Metadata.1.1.0\lib\dotnet5.2\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\Microsoft.Composition.1.0.27\lib\portable-net45+win8+wp8+wpa81\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.CSharp.1.1.0\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.CSharp.Workspaces.1.1.0\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.VisualBasic.1.1.0\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.0
Copy-Item ..\..\packages\Microsoft.CodeAnalysis.VisualBasic.Workspaces.1.1.0\lib\net45\*.dll .\bin\StyleCopTester-Roslyn.1.1.0

# Clone the project
git clone https://github.com/DartVS/DartVS.git bin\DartVS
cd bin\DartVS
git checkout 6f54d1d2bf6a16aaac5a6add7e073716e35e21ba
cd ..\..

&$NuGet restore bin\DartVS\DanTup.DartVS.sln

$StyleCopTester = '.\bin\StyleCopTester-Roslyn.1.0.0\StyleCopTester.exe'
&$StyleCopTester bin\DartVS\DanTup.DartVS.sln /all /log:bin\DartVS-1.0.0.txt | Out-Null

$StyleCopTester = '.\bin\StyleCopTester-Roslyn.1.1.0\StyleCopTester.exe'
&$StyleCopTester bin\DartVS\DanTup.DartVS.sln /all /log:bin\DartVS-1.1.0.txt | Out-Null

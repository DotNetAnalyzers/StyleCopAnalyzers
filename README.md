Projects
========

MetaCompilation
---------------
*A description of this project should go here.*

Getting Started
===============

1. Clone the repository
2. Install NuGet packages: `powershell -executionpolicy bypass src\.nuget\NuGetRestore.ps1`
3. Build: `msbuild src\MetaCompilation\MetaCompilation.sln`

Submitting Pull Requests
========================

Prior to submitting a pull request, ensure the build and all tests pass using BuildAndTest.proj:
```
msbuild BuildAndTest.proj
```
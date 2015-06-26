[![Build Status](http://dotnet-ci.cloudapp.net/job/dotnet_roslyn-analyzers_windows_debug/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_roslyn_windows_debug/)

[![Join the chat at https://gitter.im/dotnet/roslyn](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/dotnet/roslyn?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

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
Debug: [![Build Status](http://dotnet-ci.cloudapp.net/job/dotnet_roslyn-analyzers_windows_debug/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_roslyn_windows_debug/)

Release: [![Build Status](http://dotnet-ci.cloudapp.net/job/dotnet_roslyn-analyzers_windows_release/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_roslyn_windows_release/)

[![Join the chat at https://gitter.im/dotnet/roslyn](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/dotnet/roslyn?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Projects
========

MetaCompilation
---------------
The MetaCompilation Analyzer is an analyzer that functions as a tutorial to teach users how to write an analyzer. It uses diagnostics and code 
fixes to guide the user through the various steps required to create a simple analyzer. It is designed for a novice analyzer programmer with some previous programming
experience.

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
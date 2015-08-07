.NET Compiler Platform ("Roslyn") Analyzers
===========================================

This repository contains a number of [Roslyn](https://github.com/dotnet/roslyn) diagnostic analyzers initially developed to help flesh out the design and implementation of the static analysis APIs.

Currently there are only a few projects here. Over time the collection will grow as more are migrated from the [dotnet/roslyn](https://github.com/dotnet/roslyn) repository.

Debug | Release
------|--------
[![Build Status](http://dotnet-ci.cloudapp.net/job/dotnet_roslyn-analyzers_windows_debug/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_roslyn-analyzers_windows_debug/) | [![Build Status](http://dotnet-ci.cloudapp.net/job/dotnet_roslyn-analyzers_windows_release/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_roslyn-analyzers_windows_release/)

[![Join the chat at https://gitter.im/dotnet/roslyn](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/dotnet/roslyn?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Projects
========

AsyncPackage
-----------

*Created by summer 2014 interns Chardai Page, [Kendra Havens](https://github.com/kendrahavens), and Vivian Morgowicz*

The AsyncPackage analyzer enforces good practices when writing code that makes use of C#'s `async` and `await` language features.

MetaCompilation
---------------

*Created by summer 2015 interns [Zoë Petard](https://github.com/zoepetard), [Jessica Petty](https://github.com/jepetty), and [Daniel King](https://github.com/daking2014)*

The MetaCompilation Analyzer is an analyzer that functions as a tutorial to teach users how to write an analyzer. It uses diagnostics and code fixes to guide the user through the various steps required to create a simple analyzer. It is designed for a novice analyzer programmer with some previous programming experience.

Microsoft.AnalyzerPowerPack
---------------------------

General language rules implemented as analyzers using the .NET Compiler Platform ("Roslyn").

Microsoft.CodeAnalysis.Analyzers
--------------------------------

Provides guidelines for using .NET Compiler Platform ("Roslyn") APIs.

Getting Started
===============

1. Clone the repository
2. Install NuGet packages: `powershell -executionpolicy bypass src\.nuget\NuGetRestore.ps1`
3. Build: `msbuild src\Analyzers.sln`

Submitting Pull Requests
========================

Prior to submitting a pull request, ensure the build and all tests pass using BuildAndTest.proj:
```
msbuild BuildAndTest.proj
```
# StyleCop Analyzers for the .NET Compiler Platform

[![Join the chat at https://gitter.im/DotNetAnalyzers/StyleCopAnalyzers](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/DotNetAnalyzers/StyleCopAnalyzers?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![Build status](https://ci.appveyor.com/api/projects/status/8jw2lq431kgg44jl/branch/master?svg=true)](https://ci.appveyor.com/project/sharwell/stylecopanalyzers/branch/master)

This repository contains an implementation of the StyleCop rules using the .NET Compiler Platform. Where possible, code fixes are also provided to simplify the process of correcting violations.

## Using StyleCop.Analyzers

The preferable way to use the analyzers is to add the nuget package [StyleCop.Analyzers](http://www.nuget.org/packages/StyleCop.Analyzers/)
to the project where you want to enforce StyleCop rules.
You can also build a vsix extension from source and install it into visual studio to use it in all
C# projects opened in visual studio.

Currently the only way to configure the rules is to change the severity for them in a ruleset file.

## Building

Visual Studio 2015 RC is required for building this repository.
The Visual Studio 2015 RC SDK is required for building the vsix extension project and for
debugging in an experimental visual studio hive.

## Contributing

If you want to contribute code you can get started by looking for issues marked as
[up for grabs](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/labels/up%20for%20grabs).
We also have the [easy](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/labels/easy) tag
for issues suitable if you are unfamiliar with roslyn.

Also see the [contributing guide](CONTRIBUTING.md).

You can also help by filing issues, participating in discussions and doing code review.

## Current status

An up-to-date overview of the status can be found [here](http://stylecop.pdelvo.com/).

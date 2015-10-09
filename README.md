# StyleCop Analyzers for the .NET Compiler Platform

[![Join the chat at https://gitter.im/DotNetAnalyzers/StyleCopAnalyzers](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/DotNetAnalyzers/StyleCopAnalyzers?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![Build status](https://ci.appveyor.com/api/projects/status/8jw2lq431kgg44jl/branch/master?svg=true)](https://ci.appveyor.com/project/sharwell/stylecopanalyzers/branch/master)

[![codecov.io](http://codecov.io/github/DotNetAnalyzers/StyleCopAnalyzers/coverage.svg?branch=master)](http://codecov.io/github/DotNetAnalyzers/StyleCopAnalyzers?branch=master)

This repository contains an implementation of the StyleCop rules using the .NET Compiler Platform. Where possible, code fixes are also provided to simplify the process of correcting violations.

## Using StyleCop.Analyzers

The preferable way to use the analyzers is to add the nuget package [StyleCop.Analyzers](http://www.nuget.org/packages/StyleCop.Analyzers/)
to the project where you want to enforce StyleCop rules.

The severity of individual rules may be configured using [rule set files](https://msdn.microsoft.com/en-us/library/dd264996.aspx)
in Visual Studio 2015. **Settings.StyleCop** is not supported, but a **stylecop.json** file may be used to customize the
behavior of certain rules. See [Configuration.md](documentation/Configuration.md) for more information.

## Installation

StyleCopAnalyzers can be installed using the NuGet Package Manager in Visual Studio 2015.

![Install via nuget](https://cloud.githubusercontent.com/assets/1408396/8233513/491f301a-159c-11e5-8b7a-1e16a0695da6.png)

## Team Considerations

If you use older versions of Visual Studio in addition to Visual Studio 2015, you may still install these analyzers. They will be automatically disabled when you open the project back up in Visual Studio 2013 or earlier.

## Contributing

See [Contributing](CONTRIBUTING.md)

## Current status

An up-to-date list of which StyleCop rules are implemented and which have code fixes can be found [here](http://stylecop.pdelvo.com/).

## Source browser

The up-to-date source code can be browsed [here](http://source.pdelvo.com/).

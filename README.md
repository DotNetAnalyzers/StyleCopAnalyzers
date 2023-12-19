# StyleCop Analyzers for the .NET Compiler Platform

[![NuGet](https://img.shields.io/nuget/v/StyleCop.Analyzers.svg)](https://www.nuget.org/packages/StyleCop.Analyzers)[![NuGet Beta](https://img.shields.io/nuget/vpre/StyleCop.Analyzers.svg)](https://www.nuget.org/packages/StyleCop.Analyzers)

[![Join the chat at https://gitter.im/DotNetAnalyzers/StyleCopAnalyzers](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/DotNetAnalyzers/StyleCopAnalyzers?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![Build status](https://ci.appveyor.com/api/projects/status/8jw2lq431kgg44jl/branch/master?svg=true)](https://ci.appveyor.com/project/sharwell/stylecopanalyzers/branch/master)

[![codecov.io](http://codecov.io/github/DotNetAnalyzers/StyleCopAnalyzers/coverage.svg?branch=master)](http://codecov.io/github/DotNetAnalyzers/StyleCopAnalyzers?branch=master)

This repository contains an implementation of the StyleCop rules using the .NET Compiler Platform. Where possible, code fixes are also provided to simplify the process of correcting violations.

## Using StyleCop.Analyzers

The preferable way to use the analyzers is to add the nuget package [StyleCop.Analyzers](http://www.nuget.org/packages/StyleCop.Analyzers/)
to the project where you want to enforce StyleCop rules.

The severity of individual rules may be configured using [rule set files](https://docs.microsoft.com/en-us/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules)
in Visual Studio 2015 or newer. **Settings.StyleCop** is not supported, but a **stylecop.json** file may be used to
customize the behavior of certain rules. See [Configuration.md](documentation/Configuration.md) for more information.

For documentation and reasoning on the rules themselves, see the [Documentation](DOCUMENTATION.md).

For users upgrading from StyleCop Classic, see [KnownChanges.md](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/tree/master/documentation/KnownChanges.md)
for information about known differences which you may notice when switching to StyleCop Analyzers.

### C# language versions
Not all versions of StyleCop.Analyzers support all features of each C# language version. The table below shows the minimum version of StyleCop.Analyzers required for proper support of a C# language version.

| C# version | StyleCop.Analyzers version | Visual Studio version |
|------------|----------------------------|-----------------------|
| 1.0 - 6.0  | v1.0.2 or higher           | VS2015+               |
| 7.0 - 7.3  | v1.1.0-beta or higher      | VS2017+               |
|    8.0     | v1.2.0-beta or higher      | VS2019                |

## Installation

StyleCopAnalyzers can be installed using the NuGet command line or the NuGet Package Manager in Visual Studio 2015.

**Install using the command line:**
```bash
Install-Package StyleCop.Analyzers
```

**Install using the package manager:**
![Install via nuget](https://cloud.githubusercontent.com/assets/1408396/8233513/491f301a-159c-11e5-8b7a-1e16a0695da6.png)

## Team Considerations

If you use older versions of Visual Studio in addition to Visual Studio 2015 or Visual Studio 2017, you may still install these analyzers. They will be automatically disabled when you open the project back up in Visual Studio 2013 or earlier.

## Contributing

See [Contributing](CONTRIBUTING.md)

## Current status

An up-to-date list of which StyleCop rules are implemented and which have code fixes can be found [here](https://dotnetanalyzers.github.io/StyleCopAnalyzers/).

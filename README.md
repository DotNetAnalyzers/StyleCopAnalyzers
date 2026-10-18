# StyleCop Analyzers for the .NET Compiler Platform

<img src="artwork/stylecop-analyzers.svg" alt="StyleCop.Analyzers icon" width="128" align="right">

[![NuGet](https://img.shields.io/nuget/v/StyleCop.Analyzers.svg)](https://www.nuget.org/packages/StyleCop.Analyzers) [![NuGet Beta](https://img.shields.io/nuget/vpre/StyleCop.Analyzers.svg)](https://www.nuget.org/packages/StyleCop.Analyzers)
[![Build status](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/actions/workflows/build.yml) [![codecov.io](https://codecov.io/github/DotNetAnalyzers/StyleCopAnalyzers/coverage.svg?branch=master)](https://app.codecov.io/github/DotNetAnalyzers/StyleCopAnalyzers?branch=master)
[![Join the chat at https://gitter.im/DotNetAnalyzers/StyleCopAnalyzers](https://badges.gitter.im/join_chat.svg)](https://gitter.im/DotNetAnalyzers/StyleCopAnalyzers?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

This repository contains an implementation of the StyleCop rules using the .NET Compiler Platform. Where possible, code fixes are also provided to simplify the process of correcting violations.

## Installation

See our [getting started guide](https://dotnetanalyzers.github.io/StyleCopAnalyzers/).

## Using StyleCop.Analyzers

The severity of individual rules may be customized in the standard way for roslyn analyzers.
Many [configuration options](https://dotnetanalyzers.github.io/StyleCopAnalyzers/Configuration.html) allow you to further tailor StyleCop.Analyzers to your team's tastes.

### C# language versions

We generally recommend using the latest version of the StyleCop.Analyzers, especially if you're using recent C# language versions.

| C# version | StyleCop.Analyzers version |
|------------|----------------------------|
| 1.0 - 6.0  | v1.0.2                     |
| 7.0 - 7.3  | v1.1.0                     |
|    8.0     | v1.2.0-beta                |
| 9.0 - 15.0 | v1.3.0-beta                |

## Contributing

See our [contribution guide](CONTRIBUTING.md).

# StyleCop.Analyzers

An implementation of the StyleCop rules using Roslyn analyzers and code fixes. The analyzers report style and
consistency issues in C# code while you build or edit, and many of them include code fixes.

## Getting started

Add the package to each project you want to check:

```bash
dotnet add package StyleCop.Analyzers
```

The package is a development dependency, so it is not added as a dependency of your own package.

## Configuration

* Use an **.editorconfig** file or a rule set to change the severity of individual rules, or to turn rules off.
* Use a **stylecop.json** file to customize the behavior of certain rules, such as the file header or the ordering of
  `using` directives. See [Configuration.md](https://dotnetanalyzers.github.io/StyleCopAnalyzers/Configuration.html).

## Documentation

* [Rule documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/)
* [Known changes from StyleCop Classic](https://dotnetanalyzers.github.io/StyleCopAnalyzers/KnownChanges.html)
* [Release notes](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/releases)

## Feedback

Report bugs and ask questions in the [GitHub repository](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues).

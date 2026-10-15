<style>
#breadcrumb { display: none; }
</style>

# Overview

StyleCop.Analyzers provides warnings and code fixes for style and consistency rule violations in C# code.
These analyzers are distributed via a NuGet package.

[![NuGet package](https://img.shields.io/nuget/vpre/StyleCop.Analyzers?label=StyleCop.Analyzers)](https://nuget.org/packages/StyleCop.Analyzers)

Click the badge above and follow the instructions that match your environment to install the analyzers package.
This will activate the analyzers for your project.

There are many [configuration options](Configuration.md) for these analyzers.

## Enabling XML documentation processing

Analyzers for XML documentation can only run if the XML documentation comment processing is enabled. See [SA0001](SA0001.md) for more information.

Update the project file to include the following:

```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

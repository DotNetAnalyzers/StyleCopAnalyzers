
# Using StyleCop Analyzers with dotnet cli

StyleCop Analyzers can be used in dotnet cli projects, including asp.net core.
The tooling support is currently not great and the analyzers only run when the project is compiled and there is currently no way to invoke the code fixes. What does work however is running Stylecop Analyzers in ubuntu on coreclr and OSX (probably) works too.
To set this up a few steps are required:

First the StyleCopAnalyzers nuget package has to be added to the project. Optimally the package should be marked as `build` type so it is not included as a package by other projects consuming it.
For this add the following to the dependencies section of the project.json file:
```json
    "StyleCop.Analyzers": {
      "version": "1.0.0",
      "type": "build"
    }
```
For VS2017 / dotnet core 1.1.1 SDK 1.0.1 projects edit the `.csproj` file and mark the `StyleCop.Analyzers` as `PrivateAssets`:
```xml
    <ItemGroup>
        <PackageReference Include="StyleCop.Analyzers" Version="1.1.0-beta001" PrivateAssets="All" />
    </ItemGroup>
```
If the project is restored and built right now this will already run the analyzers. A few extra steps are needed if they should be configured.

## Rulesets and stylecop.json

To supply a ruleset file and a stylecop.json configuration file to the compiler they have to be manually added as arguments to the compiler. For this add the following under the `buildOptions` node in the project.json file:
```json
    "additionalArguments": [ "/ruleset:path/to/ruleset.ruleset", "/additionalfile:path/to/stylecop.json" ]
```

**Note: ** `additionalArguments` is not currently defined in the schema but does exist and is passed during the build

On VS VS2017 / dotnet core 1.1.1 SDK 1.0.1 projects update `.csproj` as follows to apply settings and custom rules:
```xml
    <PropertyGroup>
        ...
        <CodeAnalysisRuleSet>stylecop.ruleset</CodeAnalysisRuleSet>
    </PropertyGroup>
    <ItemGroup>
        <AdditionalFiles Include="stylecop.json" />
    </ItemGroup>
```

## Enabling xml documentation processing

All analyzers regarding xml documentation can only run if the xml processing is enabled. To do this add
```json
"xmlDoc": "true"
```

to the `buildOptions` node of the project.json file. Note that this might cause additional CS1591 diagnostics to appear by the compiler.
They can be suppressed in the ruleset file if necessary.

For VS VS2017 / dotnet core 1.1.1 SDK 1.0.1 projects update `.csproj`
```xml
    <PropertyGroup>
        ...
        <GenerateDocumentationFile>true</GenerateDocumentationFile>
    </PropertyGroup>
```

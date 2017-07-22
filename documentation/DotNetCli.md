# Using StyleCop Analyzers with .NET Core

StyleCop Analyzers can be used with the **dotnet** tooling, including ASP.NET Core.

## .NET SDK Projects (*.csproj)

Edit the project file and add a package reference to **StyleCop.Analyzers**. Make sure to set **PrivateAssets** so the
reference is not included when transitive dependencies are calculated across project references:

```xml
<ItemGroup>
    <PackageReference Include="StyleCop.Analyzers" Version="1.1.0-beta004" PrivateAssets="All" />
</ItemGroup>
```

If the project is restored and built right now this will already run the analyzers. A few extra steps are needed if they should be configured.

### Rulesets and stylecop.json

Update the project file as follows to apply settings and custom rules:

```xml
<PropertyGroup>
    ...
    <CodeAnalysisRuleSet>stylecop.ruleset</CodeAnalysisRuleSet>
</PropertyGroup>
<ItemGroup>
    <AdditionalFiles Include="stylecop.json" />
</ItemGroup>
```

### Enabling XML documentation processing

Analyzers for XML documentation can only run if the XML documentation comment processing is enabled. See [SA0001](SA0001.md) for more information.

Update the project file to include the following:

```xml
<PropertyGroup>
    ...
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

## Legacy Projects (*.xproj)

Legacy projects use **project.json** to configure analyzers and other build options. Start by adding the following to
the `dependencies` section of **project.json**:

```json
"StyleCop.Analyzers": {
  "version": "1.0.2",
  "type": "build"
}
```

The rule set and configuration file can be configured by adding the following to the `buildOptions` section:

```json
"additionalArguments": [
  "/ruleset:path/to/ruleset.ruleset",
  "/additionalfile:path/to/stylecop.json"
],
"xmlDoc": "true"
```

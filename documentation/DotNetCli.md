
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

If the project is restored and built right now this will already run the analyzers. A few extra steps are needed if they should be configured.

## Rulesets and stylecop.json

To supply a ruleset file and a stylecop.json configuration file to the compiler they have to be manually added as arguments to the compiler. For this add the follwing under the `compilationOptions` node in the project.json file:
```json
    "additionalArguments": [ "/ruleset:path/to/ruleset.ruleset", "/additionalfile:path/to/stylecop.json" ]
```

## Enabling xml documentation processing

All analyzers regarding xml documentation can only run if the xml processing is enabled. To do this add
```json
"xmlDoc": "true"
```

to the `compilationOptions` node of the project.json file. Note that this might cause additional CS1591 diagnostics to appear by the compiler.
They can be suppressed in the ruleset file if necessary.
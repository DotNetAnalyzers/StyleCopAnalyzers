### Special Rules (SA0000-)

Set category-level severity with this in your .editorconfig file:

```ini
dotnet_diagnostic.category-StyleCop.CSharp.SpecialRules.severity = none
```

Rules which provide special functionality like workarounds, configuration errors, etc.

Identifier | Name | Description
-----------|------|-------------
[SA0001](SA0001.md) | XmlCommentAnalysisDisabled | Diagnostics for documentation comments may be missing or incorrect, and code fixes for all diagnostics may misbehave in source files containing any documentation comments.
[SA0002](SA0002.md) | InvalidSettingsFile | The **stylecop.json** settings file could not be loaded due to a deserialization error.

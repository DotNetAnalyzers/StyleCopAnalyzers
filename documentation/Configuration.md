# Configuring StyleCop Analyzers

StyleCop Analyzers is configured using two separate mechanisms: code analysis rule set files, and **stylecop.json**.

1. Code analysis rule set files

   * Enable and disable individual rules
   * Configure the severity of violations reported by individual rules

2. **stylecop.json**

   * Specify project-specific text, such as the name of the company and the structure to use for copyright headers
   * Fine-tune the behavior of certain rules

Code analysis rule sets are the standard way to configure most diagnostic analyzers within Visual Studio 2015. Information about creating and customizing these files can be found in the [Using Rule Sets to Group Code Analysis Rules](https://msdn.microsoft.com/en-us/library/dd264996.aspx) documentation on MSDN.

## Getting Started with **stylecop.json**

The easiest way to add a **stylecop.json** configuration file to a new project is using a code fix provided by the project. To invoke the code fix, open any file where SA1633 is reported¹ and press Ctrl+. to bring up the Quick Fix menu. From the menu, select **Add StyleCop settings file to the project**.

### JSON Schema for IntelliSense

A JSON schema is available for **stylecop.json**. By including a reference in **stylecop.json** to this schema, Visual Studio will offer IntelliSense functionality (code completion, quick info, etc.) while editing this file. The schema may be configured by adding the following top-level property in **stylecop.json**:

```json
{
  "$schema": "https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json"
}
```

> :bulb: The code fix described previously automatically configures **stylecop.json** to reference the schema.

### Source Control

For best results, **stylecop.json** should be included in source control. This will automatically propagate the expected settings to all team members working on the project.

> :warning: If you are working in Git, make sure your **.gitignore** file *does not* contain the following line. This line should be removed if present.
>
> ```
> [Ss]tyle[Cc]op.*
> ```

## Spacing Rules

This section describes the features of spacing rules which can be configured in **stylecop.json**. Each of the described properties are configured in the `spacingRules` object, which is shown in the following sample file.

```json
{
  "settings": {
    "spacingRules": {
    }
  }
}
```

> Currently there are no configurable settings for spacing rules.

## Readability Rules

This section describes the features of readability rules which can be configured in **stylecop.json**. Each of the described properties are configured in the `readabilityRules` object, which is shown in the following sample file.

```json
{
  "settings": {
    "readabilityRules": {
    }
  }
}
```

> Currently there are no configurable settings for readability rules.

## Ordering Rules

This section describes the features of ordering rules which can be configured in **stylecop.json**. Each of the described properties are configured in the `orderingRules` object, which is shown in the following sample file.

```json
{
  "settings": {
    "orderingRules": {
    }
  }
}
```

> Currently there are no configurable settings for ordering rules.

## Naming Rules

This section describes the features of naming rules which can be configured in **stylecop.json**. Each of the described properties are configured in the `namingRules` object, which is shown in the following sample file.

```json
{
  "settings": {
    "namingRules": {
    }
  }
}
```

### Hungarian Notation

The following properties are used to configure allowable Hungarian notation prefixes in StyleCop Analyzers.

| Property | Default Value | Summary |
| --- | --- | --- |
| `allowCommonHungarianPrefixes` | **true** | Specifies whether common non-Hungarian notation prefixes should be allowed. When true, the two-letter words 'as', 'at', 'by', 'do', 'go', 'if', 'in', 'is', 'it', 'no', 'of', 'on', 'or', and 'to' are allowed to appear as prefixes for variable names. |
| `allowedHungarianPrefixes` | `[ ]` | Specifies additional prefixes which are allowed to be used in variable names. See the example below for more information. |

The following example shows a settings file which allows the common prefixes as well as the custom prefixes 'md' and 'cd'.

```json
{
  "settings": {
    "namingRules": {
      "allowedHungarianPrefixes": [
        "cd",
        "md"
      ]
    }
  }
}
```

## Maintainability Rules

This section describes the features of maintainability rules which can be configured in **stylecop.json**. Each of the described properties are configured in the `maintainabilityRules` object, which is shown in the following sample file.

```json
{
  "settings": {
    "maintainabilityRules": {
    }
  }
}
```

> Currently there are no configurable settings for maintainability rules.

## Documentation Rules

This section describes the features of documentation rules which can be configured in **stylecop.json**. Each of the described properties are configured in the `documentationRules` object, which is shown in the following sample file.

```json
{
  "settings": {
    "documentationRules": {
    }
  }
}
```

### Copyright Headers

The following properties are used to configure copyright headers in StyleCop Analyzers.

| Property | Default Value | Summary |
| --- | --- | --- |
| `companyName` | `"PlaceholderCompany"` | Specifies the company name which should appear in copyright notices |
| `copyrightText` | `"Copyright (c) {companyName}. All rights reserved."` | Specifies the default copyright text which should appear in copyright headers |
| `xmlHeader` | **true** | Specifies whether file headers should use standard StyleCop XML format, where the copyright notice is wrapped in a `<copyright>` element |
| `variables` | n/a | Specifies replacement variables which can be referenced in the `copyrightText` value |

#### Configuring Copyright Text

In order to successfully use StyleCop-checked file headers, most projects will need to configure the `companyName` property.

> The `companyName` property is so frequently customized that it is included in the default **stylecop.json** file produced by the code fix.

The `copyrightText` property is a string which may contain placeholders. Each placeholder has the form `{variable}`, where `variable` is either `companyName` or the name of a property in the `variables` property. The following sample file shows a custom **stylecop.json** file which references both `companyName` and two custom variables within the `copyrightText`.

```json
{
  "settings": {
    "documentationRules": {
      "companyName": "FooCorp",
      "copyrightText": "Copyright (c) {companyName}. All rights reserved.\nLicensed under the {licenseName} license. See {licenseFile} file in the project root for full license information.",
      "variables": {
        "licenseName": "MIT",
        "licenseFile": "LICENSE"
      }
    }
  }
}
```

With the above configuration, a file **TypeName.cs** would be expected to have the following header.

```csharp
// <copyright file="TypeName.cs" company="FooCorp">
// Copyright (c) FooCorp. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
```

#### Configuring XML Headers

When the `xmlHeader` property is **true** (the default), StyleCop Analyzers expects file headers to conform to the following standard StyleCop format.

```csharp
// <copyright file="{fileName}" company="{companyName}">
// {copyrightText}
// </copyright>
```

When the `xmlHeader` property is explicitly set to **false**, StyleCop Analyzers expects file headers to conform to the following customizable format.

```csharp
// {copyrightText}
```

### Documentation Requirements

StyleCop Analyzers includes rules which require developers to document the majority of a code base by default. This requirement can easily overwhelm a team which did not use StyleCop for the entire development process. To help guide developers towards a properly documented code base, several properties are available in **stylecop.json** to progressively increase the documentation requirements.

| Property | Default Value | Summary |
| --- | --- | --- |
| `documentInterfaces` | **true** | Specifies whether interface members need to be documented. When true, all interface members require documentation, regardless of accessibility. |
| `documentExposedElements` | **true** | Specifies whether exposed elements need to be documented. When true, all publicly-exposed types and members require documentation. |
| `documentInternalElements` | **true** | Specifies whether internal elements need to be documented. When true, all internally-exposed types and members require documentation. |
| `documentPrivateElements` | **false** | Specifies whether private elements need to be documented. When true, all types and members except for declared private fields require documentation. |
| `documentPrivateFields` | **false** | Specifies whether private fields need to be documented. When true, all fields require documentation, regardless of accessibility. |

These properties affect the behavior of the following rules which report missing documentation. Rules which report incorrect or incomplete documentation continue to apply to all documentation comments in the code.

* [SA1600 Elements must be documented](SA1600.md)
* [SA1601 Partial elements must be documented](SA1601.md)
* [SA1602 Enumeration items must be documented](SA1602.md)

The following example shows a configuration file which requires developers to document all publicly-accessible members and all interfaces (regardless of accessibility), but does not require other internal or private members to be documented.

> :memo: Documenting interfaces is a low-effort task compared to documenting an entire code base, but provides high value in the fact that it covers the sections of code most likely to impact cross-team usage scenarios.


```json
{
  "settings": {
    "documentationRules": {
      "documentInterfaces": true,
      "documentInternalMembers": false
    }
  }
}
```

### File naming conventions

The `fileNamingConvention` property will determine how the [SA1649 File name must match type name](SA1649.md) analyzer will check file names.
Given the following code:

```csharp
public class Class1<T1, T2, T3>
{
}
```

The analyzer will expect file names according the table below. When the `fileNamingConvention` property is not set, the `stylecop` convention is used as default.

File naming convention | Expected file name
-----------------------| ------------------
stylecop               | Class1{T1,T2,T3}.cs
metadata               | Class1`3.cs

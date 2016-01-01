# Known Changes

This document describes the known changes in behavior of StyleCop Analyzers relative to StyleCop Classic. The changes
here meet all of the following conditions:

1. The change in behavior affects code for C# 5 or earlier. StyleCop Classic did not support C# 6, so the necessary
   changes made solely to support new language features are not considered here.
2. The change in behavior is currently by-design. Unintentional changes in behavior are filed as bugs when they are
   discovered.
3. The change affects a rule which was present in StyleCop Classic. New rules introduced for StyleCop Analyzers can
   be disabled for closer adherence to the behavior of StyleCop Classic.

In many cases, the change in behavior was simply a change to the documentation, where the StyleCop Classic
implementation deviated from its own documented rules. For completeness, these changes are included below whenever we
found them. The following symbol is used to mark cases which are not simply a documentation error:

> :warning: Cases where the StyleCop Analyzers implementation will produce different warnings from StyleCop Classic for
the same code are marked with this symbol.

## Disabled Rules

Several rules present StyleCop Classic have been intentionally omitted from StyleCop Analyzers. The following table
lists each of these issues, along with a link to the issue where the decision was made to omit the rule.

| ID | Title | Issue |
| --- | --- | --- |
| SA1109 | Block statements must not contain embedded regions | [#998](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/998) |
| SA1126 | Prefix calls correctly | [#59](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/59) |
| SA1215 | Instance readonly elements must appear before instance non-readonly elements | [#1812](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/pull/1812) |
| SA1409 | Remove unnecessary code | [#1058](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1058) |
| SA1603 | Documentation must contain valid XML | [#1291](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1291) |
| SA1628 | Documentation text must begin with a capital letter | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1630 | Documentation text must contain whitespace | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1631 | Documentation must meet character percentage | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1632 | Documentation text must meet minimum character length | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1645 | Included documentation file does not exist | [#165](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/165) |
| SA1646 | Included documentation XPath does not exist | [#166](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/166) |
| SA1647 | Include node does not contain valid file and path | [#167](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/167) |
| SA1650 | Element documentation must be spelled correctly | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |

## Spacing Rules

### SA1000: Keywords must be spaced correctly

The following changes were made to SA1000:

1. :warning: A space is now required after `await` and `case`.

2. An exception to the requirement that a space follow the `throw` keyword was added when it appears in a re-throw
   statement. This exception was not mentioned in the documentation for StyleCop Classic.

   ```csharp
   throw;
   ```

3. :warning: An exception to the requirement that a space follow the `new` keyword was added when it appears as a
   generic type constraint. StyleCop Classic did not check the spacing around the `new` keyword in a generic type
   constraint.

   ```csharp
   public void Foo<T>() where T : IInterface, new()
   {
       // ...
   }
   ```

### SA1001

:warning: A slight change was made to the spacing around commas in open generic types. The following table demonstrates this
change.

| StyleCop Analyzers | StyleCop Classic |
| --- | --- |
| `typeof(Func<,>)` | `typeof(Func<, >)` |

### SA1002

:warning: StyleCop Classic required an infinite `for` loop to be written as follows:

```csharp
for (;;)
{
}
```

StyleCop Analyzers reports SA1002 for this same code, and instead requires the code be written as follows:

```csharp
for (; ;)
{
}
```

:bulb: When this code is encountered, users are encouraged to rewrite the code as follows for enhanced readability:

```csharp
while (true)
{
}
```

### SA1003

:warning: StyleCop Classic allowed a type cast to appear at the end of a line, such as the following:

```csharp
uint value = (uint)
    3;
```

StyleCop Analyzers forbids this same case, requiring the following instead:

```csharp
uint value = (uint)3;
```

### SA1025

StyleCop Classic allowed multiple spaces to precede a comment placed at the end of a line, such as the following:

```csharp
int x;    // comment
```

StyleCop Analyzers does not currently make an exception to the SA1025 rule for this case, although this decision is
still under review.

## Readability Rules

### SA1110, SA1111, SA1113, SA1114

StyleCop Analyzers examines several constructs which are not mentioned in the original documentation for these rules:

* Delegate declarations
* Anonymous method expressions
* Lambda expressions

### SA1119

:warning: StyleCop Classic did not report SA1119 for the following code:

```csharp
var a = (new[] { 1, 2, 3 }).ToArray();
```

StyleCop Analyzers reports SA1119 for this same code, and requires the following change to correct it:

```csharp
var a = new[] { 1, 2, 3 }.ToArray();
```

## Ordering Rules

### SA1208

StyleCop Analyzers only considers using directives to be "System" using directives if they are not alias-qualified,
while StyleCop Classic ignored the alias. For example, `using global::System;` would be not be considered a System using
directive by StyleCop Analyzers, but it would be considered a System using directive by StyleCop Classic.

### SA1214

StyleCop Classic only reports SA1214 for violations involving static fields. In StyleCop Analyzers, SA1214 and SA1215
were merged into a single rule to improve the ability of users to customize the behavior of several ordering rules
involving members of a type.

:warning: Violations reported as SA1215 in StyleCop Classic are reported as SA1214 in StyleCop Analyzers.

## Naming Rules

### SA1300

StyleCop Analyzers adds enum members to the list of elements which must start with an upper-case letter, and reports
SA1300 for violations. StyleCop Classic did not report any messages for enum members that did not start with an
upper-case letter.

### SA1305

This rule is disabled by default in StyleCop Analyzers, but can be enabled by users via a rule set file.

## Maintainability Rules

There are no known changes at this time.

## Layout Rules

### SA1515

:warning: The following code does not report a warning in StyleCop Classic:

```csharp
[ContractClassFor(typeof(ILinkActivator<>))]
// ReSharper disable once InconsistentNaming
// Contract class containing only metadata pertaining to an interface.
// Using naming convention adopted by the Code Contracts team for .NET framework contracts.
public abstract class ILinkActivatorContract<T> : ILinkActivator<T> where T : LinkTemplate
```

StyleCop Analyzers reports SA1515 for this code, and instead requires the following:

```csharp
[ContractClassFor(typeof(ILinkActivator<>))]

// ReSharper disable once InconsistentNaming
// Contract class containing only metadata pertaining to an interface.
// Using naming convention adopted by the Code Contracts team for .NET framework contracts.
public abstract class ILinkActivatorContract<T> : ILinkActivator<T> where T : LinkTemplate
```

## Documentation Rules

### SA1642

StyleCop Analyzers requires the use of a `<see>` element when referencing the class name in the standard constructor
text. In StyleCop Classic, this element was optional.

StyleCop Classic included a special case for constructor text used for `private` constructors. StyleCop Analyzers still
allows users to use this wording in the documentation of private constructors, but prefers the use of the standard
constructor text. When the code fix for SA1642 is applied to a private constructor, the inserted text will comply with
StyleCop Analyzers but will result in a warning in StyleCop Classic. For example:

The following code does not produce a warning in StyleCop Classic or StyleCop Analyzers:

```csharp
public class ApiStatus
{
    /// <summary>
    /// Prevents a default instance of the <see cref="ApiStatus"/> class from being created.
    /// </summary>
    private ApiStatus()
    {
    }
}
```

:warning: The following code produces a warning in StyleCop Classic, but does not produce a warning in StyleCop Analyzers:

```csharp
public class ApiStatus
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiStatus"/> class.
    /// </summary>
    private ApiStatus()
    {
    }
}
```

### SA1648

This rule has been modified to adhere more closely to the use of `<inheritdoc>` with Sandcastle Help File Builder. As a
result, some code which resulted in SA1648 being reported by StyleCop Classic will no longer report this warning in
StyleCop Analyzers.

### SA1649

StyleCop Analyzers modified SA1649 to check the first type against the actual file name as opposed to checking against a
value which appeared in the file header. For StyleCop Classic users who had both SA1638 and SA1649 enabled, this change
does not produce any new warnings in code which previously complied with StyleCop.

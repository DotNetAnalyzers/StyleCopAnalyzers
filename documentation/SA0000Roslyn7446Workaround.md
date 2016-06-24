## SA0000Roslyn7446Workaround

<table>
<tr>
  <td>TypeName</td>
  <td>SA0000Roslyn7446Workaround</td>
</tr>
<tr>
  <td>CheckId</td>
  <td>SA0000</td>
</tr>
<tr>
  <td>Category</td>
  <td>Special Rules</td>
</tr>
</table>

## Cause

Workaround incomplete diagnostics in Visual Studio 2015 Update 1.

## Rule description

Visual Studio 2015 Update 1 contains a bug which can cause diagnostics to occasionally not display in the Errors window.
When this occurs, it is impossible to use the code fixes to address style violations reported during a build. This
analyzer works around the bug [dotnet/roslyn#7446](https://github.com/dotnet/roslyn/issues/7446).

When this analyzer is enabled, all diagnostics will eventually be reported in the Error window, but the performance of
the analyzers is reduced. The rule is disabled for maximum performance, but can be enabled if users notice errors
appearing during a build but not while editing, and they wish to use the code fixes to correct them.

Note that some situations are not affected by the bug:

* When building a project, all relevant warnings are reported even if this rule is disabled.
* The various Fix All operations work properly for the selected scope, even if only a subset of the violations are
  appearing in the Errors window.

## How to fix violations

This analyzer does not report any diagnostics.

## How to suppress violations

This analyzer does not report any diagnostics.

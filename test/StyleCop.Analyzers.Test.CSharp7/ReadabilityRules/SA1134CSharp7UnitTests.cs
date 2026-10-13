// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.ReadabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.ReadabilityRules.SA1134AttributesMustNotShareLine,
    StyleCop.Analyzers.ReadabilityRules.SA1134CodeFixProvider>;

/// <summary>
/// This class contains the CSharp 7.x unit tests for SA1134.
/// </summary>
public partial class SA1134CSharp7UnitTests : SA1134UnitTests
{
    /// <summary>
    /// Verifies that attributes on single-line auto-property accessors do not produce diagnostics.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [WorkItem(3728, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3728")]
    public async Task VerifyAttributesOnAutoPropertyAccessorsDoNotProduceDiagnosticAsync()
    {
        var testCode = @"using System;

public class TestClass
{
    public bool DisableCaching { [UsedImplicitly] get; [UsedImplicitly] set; }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class UsedImplicitlyAttribute : Attribute
{
}
";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that attributes on single-line auto-property accessors with property attributes do not produce diagnostics.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [WorkItem(3728, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3728")]
    public async Task VerifyAttributesOnSingleLineAutoPropertyAccessorsWithPropertyAttributesDoNotProduceDiagnosticAsync()
    {
        var testCode = @"using System;

public class TestClass
{
    [Obsolete]
    public bool DisableCaching { [UsedImplicitly] get; [UsedImplicitly] set; }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class UsedImplicitlyAttribute : Attribute
{
}
";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that attributes on non-auto property accessors still produce diagnostics.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task VerifyAttributesOnNonAutoPropertyAccessorsProduceDiagnosticAsync()
    {
        var testCode = @"using System;

public class TestClass
{
    private bool value;

    public bool DisableCaching { {|#0:[|}UsedImplicitly] get { return value; } }

    public bool DisableCachingExpression { {|#1:[|}UsedImplicitly] get => value; }
}

public abstract class AbstractTestClass
{
    public abstract bool this[int index] { {|#2:[|}UsedImplicitly] get; }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class UsedImplicitlyAttribute : Attribute
{
}
";

        DiagnosticResult[] expected =
        {
            Diagnostic().WithLocation(0),
            Diagnostic().WithLocation(1),
            Diagnostic().WithLocation(2),
        };

        await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
    }
}

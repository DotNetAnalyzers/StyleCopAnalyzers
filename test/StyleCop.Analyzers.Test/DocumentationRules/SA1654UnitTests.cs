// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<StyleCop.Analyzers.DocumentationRules.SA1654ProperlyEncodeGenericBrackets, StyleCop.Analyzers.DocumentationRules.SA1654CodeFixProvider>;

/// <summary>
/// Tests for generic brackets in cref attributes.
/// </summary>
public class SA1654UnitTests
{
    [Fact]
    [WorkItem(758, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/758")]
    public async Task TestEncodedGenericBracketsAsync()
    {
        var source = @"/// <summary>
/// Initializes a new instance of the <see cref=""{|#0:DisposalScope&lt;T&gt;|}""/> class.
/// </summary>
public class DisposalScope<T>
{
}";

        var fixedSource = @"/// <summary>
/// Initializes a new instance of the <see cref=""DisposalScope{T}""/> class.
/// </summary>
public class DisposalScope<T>
{
}";

        await VerifyCSharpFixAsync(source, Diagnostic().WithLocation(0), fixedSource, CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestGenericBracesAsync()
    {
        var source = @"/// <summary>
/// Uses <see cref=""GenericClass{T}""/>.
/// </summary>
public class GenericClass<T>
{
}";

        await VerifyCSharpDiagnosticAsync(source, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestEncodedBracketsOutsideCrefAsync()
    {
        var source = @"/// <summary>
/// Uses the encoded text &lt;T&gt;.
/// </summary>
public class GenericClass<T>
{
}";

        await VerifyCSharpDiagnosticAsync(source, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestNestedGenericBracketsCodeFixAsync()
    {
        var source = @"/// <summary>
/// Uses <see cref=""{|#0:GenericClass&lt;Dictionary&lt;T, U&gt;&gt;|}""/>.
/// </summary>
public class GenericClass<T>
{
}";
        var fixedSource = @"/// <summary>
/// Uses <see cref=""GenericClass{Dictionary{T, U}}""/>.
/// </summary>
public class GenericClass<T>
{
}";

        await VerifyCSharpFixAsync(source, Diagnostic().WithLocation(0), fixedSource, CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestEncodedOperatorWithoutGenericArgumentsAsync()
    {
        var source = @"/// <summary>
/// Uses <see cref=""C.operator &lt;(C, C)""/>.
/// </summary>
public class C
{
    public static bool operator <(C left, C right) => false;
    public static bool operator >(C left, C right) => false;
}";

        await VerifyCSharpDiagnosticAsync(source, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestCodeFixPreservesEncodedOperatorAndFixesOtherDelimitersAsync()
    {
        var source = @"/// <summary>
/// Uses <see cref=""{|#0:C&lt;T&gt;.operator &lt;(C{T}, C{T})|}""/> and <see cref=""{|#1:C&lt;T&gt;.operator &gt;(C{T}, C{T})|}""/>.
/// Also uses <see cref=""{|#2:GenericClass{T&gt;|}""/>, <see cref=""{|#3:GenericClass&lt;T>|}""/>,
/// <see cref=""{|#4:GenericClass&#60;T&#62;|}""/> and <see cref=""{|#5:GenericClass&#x3C;T&#x3E;|}""/>.
/// </summary>
public class C<T>
{
    public static bool operator <(C<T> left, C<T> right) => false;
    public static bool operator >(C<T> left, C<T> right) => false;
}

public class GenericClass<T>
{
}";
        var fixedSource = @"/// <summary>
/// Uses <see cref=""C{T}.operator &lt;(C{T}, C{T})""/> and <see cref=""C{T}.operator &gt;(C{T}, C{T})""/>.
/// Also uses <see cref=""GenericClass{T}""/>, <see cref=""GenericClass{T}""/>,
/// <see cref=""GenericClass{T}""/> and <see cref=""GenericClass{T}""/>.
/// </summary>
public class C<T>
{
    public static bool operator <(C<T> left, C<T> right) => false;
    public static bool operator >(C<T> left, C<T> right) => false;
}

public class GenericClass<T>
{
}";
        var expected =
            new[]
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
                Diagnostic().WithLocation(3),
                Diagnostic().WithLocation(4),
                Diagnostic().WithLocation(5),
            };

        await VerifyCSharpFixAsync(source, expected, fixedSource, CancellationToken.None).ConfigureAwait(false);
    }
}

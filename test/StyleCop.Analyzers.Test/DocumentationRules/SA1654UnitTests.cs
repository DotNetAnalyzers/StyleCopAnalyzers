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
}

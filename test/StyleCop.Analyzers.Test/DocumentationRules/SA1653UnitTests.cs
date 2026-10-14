// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1653OnlyOnePartialDeclarationShouldContainSummaryDocumentation>;

/// <summary>
/// Tests for summary documentation on partial types.
/// </summary>
public class SA1653UnitTests
{
    /// <summary>
    /// Verifies the conflicting summaries from the original report.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    [WorkItem(2908, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2908")]
    public async Task TestReportedGenericPartialClassAsync()
    {
        var test = new CSharpTest
        {
            TestState =
            {
                Sources =
                {
                    @"/// <summary>blablabla.</summary>
/// <typeparam name=""TPaymentRequest"">aaa.</typeparam>
public abstract partial class {|#0:BasePostRequestProcessor|}<TPaymentRequest> { }",
                    @"/// <summary>blablabla.</summary>
public abstract partial class {|#1:BasePostRequestProcessor|}<TPaymentRequest> { }",
                },
            },
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithArguments("BasePostRequestProcessor"));
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(1).WithArguments("BasePostRequestProcessor"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Theory]
    [InlineData("class")]
    [InlineData("struct")]
    [InlineData("interface")]
    public async Task TestPartialTypesAsync(string keyword)
    {
        var source = $@"/// <summary>First.</summary>
public partial {keyword} {{|#0:TestType|}} {{ }}
/// <content>Additional details.</content>
public partial {keyword} TestType {{ }}
/// <summary/>
public partial {keyword} {{|#1:TestType|}} {{ }}
/** <summary>Third.</summary> */
public partial {keyword} {{|#2:TestType|}} {{ }}";

        var expected = new[]
        {
            Diagnostic().WithLocation(0).WithArguments("TestType"),
            Diagnostic().WithLocation(1).WithArguments("TestType"),
            Diagnostic().WithLocation(2).WithArguments("TestType"),
        };
        await VerifyCSharpDiagnosticAsync(source, expected, CancellationToken.None).ConfigureAwait(false);
    }

    [Theory]
    [InlineData("")]
    [InlineData("/// <content>Additional details.</content>")]
    [InlineData("/// <inheritdoc/>")]
    [InlineData("/// <remarks><summary>Nested summary.</summary></remarks>")]
    [InlineData("/// <include file='missing.xml' path='/doc/*'/>")]
    public async Task TestOtherPartWithoutSummaryAsync(string documentation)
    {
        var source = $@"/// <summary>Main documentation.</summary>
public partial class TestType {{ }}
{documentation}
public partial class TestType {{ }}";

        await VerifyCSharpDiagnosticAsync(source, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestUnrelatedTypesAsync()
    {
        var source = @"namespace First
{
    /// <summary>First.</summary>
    public partial class TestType { }
}
namespace Second
{
    /// <summary>Second.</summary>
    public partial class TestType { }
}
/// <summary>Non-partial.</summary>
public class TestType { }
/// <summary>Generic.</summary>
public partial class TestType<T> { }";

        await VerifyCSharpDiagnosticAsync(source, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestNestedPrivateTypeAsync()
    {
        var source = @"public class Container
{
    /// <summary>First.</summary>
    private partial class {|#0:TestType|} { }
    /// <summary>Second.</summary>
    private partial class {|#1:TestType|} { }
}";

        await VerifyCSharpDiagnosticAsync(
            source,
            new[]
            {
                Diagnostic().WithLocation(0).WithArguments("TestType"),
                Diagnostic().WithLocation(1).WithArguments("TestType"),
            },
            CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestSingleDocumentedPartAsync()
    {
        var source = @"public partial class TestType { }
/// <summary>Main documentation.</summary>
public partial class TestType { }
public partial class TestType { }";

        await VerifyCSharpDiagnosticAsync(source, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestNoSummariesAsync()
    {
        var source = @"/// <content>First.</content>
public partial class TestType { }
/// <content>Second.</content>
public partial class TestType { }";

        await VerifyCSharpDiagnosticAsync(source, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestIncludedSummaryNotExpandedAsync()
    {
        var test = new CSharpTest
        {
            TestCode = @"/// <summary>Main documentation.</summary>
public partial class TestType { }
/// <include file='Documentation.xml' path='/doc/*'/>
public partial class TestType { }",
            XmlReferences =
            {
                { "Documentation.xml", "<doc><summary>Included summary.</summary></doc>" },
            },
        };
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMultipleSummariesOnOnePartAsync()
    {
        var source = @"/// <summary>First.</summary>
/// <summary>Second.</summary>
public partial class TestType { }
public partial class TestType { }";

        await VerifyCSharpDiagnosticAsync(source, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestDocumentationDisabledAsync()
    {
        var test = new CSharpTest
        {
            TestCode = @"/// <summary>First.</summary>
public partial class TestType { }
/// <summary>Second.</summary>
public partial class TestType { }",
        };
        test.SolutionTransforms.Add((solution, projectId) =>
            solution.WithProjectParseOptions(projectId, ((CSharpParseOptions)solution.GetProject(projectId).ParseOptions).WithDocumentationMode(DocumentationMode.None)));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestGeneratedPartAsync()
    {
        var test = new CSharpTest
        {
            TestState =
            {
                Sources =
                {
                    ("TestType.cs", @"/// <summary>User documentation.</summary>
public partial class {|#0:TestType|} { }"),
                    ("TestType.g.cs", @"// <auto-generated/>
/// <summary>Generated documentation.</summary>
public partial class TestType { }"),
                },
            },
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithArguments("TestType"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }
}

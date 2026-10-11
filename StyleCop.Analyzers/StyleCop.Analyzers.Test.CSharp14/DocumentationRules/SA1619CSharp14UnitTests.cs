// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1619GenericTypeParametersMustBeDocumentedPartialClass>;

    public partial class SA1619CSharp14UnitTests : SA1619CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockDeclarationWithoutTypeParametersAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public static partial class TestClass
{
    /// <summary>
    /// Foo
    /// </summary>
    extension(string obj)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockDeclarationWithMissingDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public static partial class TestClass
{
    /// <summary>
    /// Foo
    /// </summary>
    extension<T>(T obj)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockDeclarationWithDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public static partial class TestClass
{
    /// <summary>
    /// Foo
    /// </summary>
    /// <typeparam name=""T"">Param 1</param>
    extension<T>(T obj)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

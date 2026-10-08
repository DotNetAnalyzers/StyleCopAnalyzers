// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1600ElementsMustBeDocumented>;

    public partial class SA1600CSharp13UnitTests : SA1600CSharp12UnitTests
    {
        /// <summary>
        /// Verifies that a partial property is not reported, since partial elements are reported by SA1601 instead.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPartialPropertyBothPartsMissingDocumentationAsync()
        {
            var testCode = @"
public partial class ClassName
{
    public partial int Test { get; set; }
}

public partial class ClassName
{
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a partial indexer is not reported, since partial elements are reported by SA1601 instead.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPartialIndexerBothPartsMissingDocumentationAsync()
        {
            var testCode = @"
public partial class ClassName
{
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4019, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4019")]
        public async Task TestRefStructExplicitInterfaceImplementationWithDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// A summary.
/// </summary>
public interface IInterface
{
    /// <summary>
    /// A summary.
    /// </summary>
    void TestMethod();
}

/// <summary>
/// A summary.
/// </summary>
public ref struct TestRefStruct : IInterface
{
    /// <summary>
    /// A summary.
    /// </summary>
    void IInterface.TestMethod()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4019, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4019")]
        public async Task TestRefStructExplicitInterfaceImplementationWithoutDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// A summary.
/// </summary>
public interface IInterface
{
    /// <summary>
    /// A summary.
    /// </summary>
    void TestMethod();
}

/// <summary>
/// A summary.
/// </summary>
public ref struct TestRefStruct : IInterface
{
    void IInterface.TestMethod()
    {
    }
}";

            // Explicit interface implementations don't need documentation, the same as in classes.
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an undocumented part of a partial property or indexer is not reported when the other part is
        /// documented. SA1601 does not report it either, since one documented part is enough for a partial member.
        /// </summary>
        /// <param name="documentDefinition"><see langword="true"/> to document the defining declaration;
        /// <see langword="false"/> to document the implementing declaration.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPartialPropertyAndIndexerOnePartDocumentedAsync(bool documentDefinition)
        {
            var documentation = @"/// <summary>
    /// Summary.
    /// </summary>
    ";
            var definitionDocumentation = documentDefinition ? documentation : string.Empty;
            var implementationDocumentation = documentDefinition ? string.Empty : documentation;
            var testCode = $@"
/// <summary>
/// Summary.
/// </summary>
public partial class ClassName
{{
    {definitionDocumentation}public partial int Test {{ get; set; }}

    {definitionDocumentation}public partial int this[int index] {{ get; set; }}
}}

public partial class ClassName
{{
    {implementationDocumentation}public partial int Test
    {{
        get => 0;
        set {{ }}
    }}

    {implementationDocumentation}public partial int this[int index]
    {{
        get => 0;
        set {{ }}
    }}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

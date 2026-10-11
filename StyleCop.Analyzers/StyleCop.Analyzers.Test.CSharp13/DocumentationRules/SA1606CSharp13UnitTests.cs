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
        StyleCop.Analyzers.DocumentationRules.SA1606ElementDocumentationMustHaveSummaryText>;

    public partial class SA1606CSharp13UnitTests : SA1606CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPartialPropertyWithoutDocumentationAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <summary>
    ///
    /// </summary>
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <summary>
    ///
    /// </summary>
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPartialIndexerWithoutDocumentationAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <summary>
    ///
    /// </summary>
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    ///
    /// </summary>
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

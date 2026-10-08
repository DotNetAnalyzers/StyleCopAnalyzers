// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1605PartialElementDocumentationMustHaveSummary>;

    public partial class SA1605CSharp14UnitTests : SA1605CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4029, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4029")]
        public async Task TestPartialConstructorAndEventWithoutSummaryAsync()
        {
            // NOTE: The Roslyn analyzer driver (verified with 5.0 and 5.9) does not invoke syntax node actions for the
            // defining declaration of a partial constructor, so no diagnostic is reported for it.
            var testCode = @"
using System;

/// <summary>
/// A class.
/// </summary>
public partial class TestClass
{
    /// <remarks>No summary.</remarks>
    /// <param name=""x"">The value.</param>
    public partial TestClass(int x);

    /// <remarks>No summary.</remarks>
    public partial event EventHandler {|#1:TestEvent|};
}

/// <content>
/// More.
/// </content>
public partial class TestClass
{
    /// <content>
    /// The implementation.
    /// </content>
    public partial TestClass(int x)
    {
    }

    /// <remarks>No content.</remarks>
    public partial event EventHandler {|#2:TestEvent|}
    {
        add { }
        remove { }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

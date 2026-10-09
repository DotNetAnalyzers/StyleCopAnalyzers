// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1604ElementDocumentationMustHaveSummary>;

    public partial class SA1604CSharp14UnitTests : SA1604CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4029, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4029")]
        public async Task TestPartialConstructorAndEventWithoutSummaryAsync()
        {
            // Partial members can use <content> instead of <summary>, and are checked by SA1605 instead.
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
    public partial event EventHandler TestEvent;
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

    /// <content>
    /// The implementation.
    /// </content>
    public partial event EventHandler TestEvent
    {
        add { }
        remove { }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

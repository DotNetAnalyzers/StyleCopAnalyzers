// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1202ElementsMustBeOrderedByAccess,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public partial class SA1202CSharp9UnitTests : SA1202CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3971, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3971")]
        public async Task TestPartialMethodExplicitAccessibilityDeclarationOrderingAsync()
        {
            var testCode = @"
public partial class TestClass
{
    private void Helper() { }

    public partial void {|#0:TestMethod|}();
}

public partial class TestClass
{
    public partial void TestMethod()
    {
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("public", "private");

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3971, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3971")]
        public async Task TestPartialMethodExplicitAccessibilityImplementationOrderingAsync()
        {
            var testCode = @"
public partial class TestClass
{
    public partial void TestMethod();
}

public partial class TestClass
{
    private void Helper() { }

    public partial void {|#0:TestMethod|}()
    {
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("public", "private");

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

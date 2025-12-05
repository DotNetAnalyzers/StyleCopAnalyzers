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
        StyleCop.Analyzers.OrderingRules.SA1205PartialElementsMustDeclareAccess,
        StyleCop.Analyzers.OrderingRules.SA1205CodeFixProvider>;

    public partial class SA1205CSharp9UnitTests : SA1205CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3971, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3971")]
        public async Task TestPartialMethodWithPublicAccessModifierAsync()
        {
            var testCode = @"
public partial class TestClass
{
    public partial int TestMethod();
}

public partial class TestClass
{
    public partial int TestMethod()
    {
        return 0;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3971, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3971")]
        public async Task TestPartialMethodWithoutAccessModifierAsync()
        {
            var testCode = @"
public partial class TestClass
{
    partial void {|#0:TestMethod|}();
}

public partial class TestClass
{
    public partial void {|CS8799:TestMethod|}()
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3971, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3971")]
        public async Task TestPartialMethodWithoutAccessModifierNonVoidAsync()
        {
            var testCode = @"
public partial class TestClass
{
    partial int {|CS8796:TestMethod|}();
}

public partial class TestClass
{
    public partial int {|CS8799:TestMethod|}()
    {
        return 0;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

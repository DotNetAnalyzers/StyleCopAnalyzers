// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1611ElementParametersMustBeDocumented>;

    public partial class SA1611CSharp14UnitTests : SA1611CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockDeclarationAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
    };
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4030, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4030")]
        public async Task TestCompoundAssignmentOperatorMissingParameterDocumentationAsync()
        {
            var testCode = @"
public class TestClass
{
    private int value;

    /// <summary>
    /// Adds a value to this instance.
    /// </summary>
    public void operator +=(int {|#0:x|})
    {
        this.value += x;
    }
}
";

            var expected = new[] { Diagnostic().WithLocation(0).WithArguments("x") };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4030, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4030")]
        public async Task TestInstanceIncrementOperatorWithZeroParametersAsync()
        {
            var testCode = @"
public class TestClass
{
    private int value;

    /// <summary>
    /// Increments this instance.
    /// </summary>
    public void operator ++()
    {
        this.value++;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

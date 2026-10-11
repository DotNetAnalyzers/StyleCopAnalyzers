// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1205PartialElementsMustDeclareAccess,
        StyleCop.Analyzers.OrderingRules.SA1205CodeFixProvider>;

    public partial class SA1205CSharp15UnitTests : SA1205CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that partial unions without an access modifier are reported exactly once and that the code fix adds
        /// the default access modifier.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestPartialUnionsAsync()
        {
            var testCode = @"
partial union {|#0:Pet|}(int, string);

public class Outer
{
    partial union {|#1:Shape|}(int, string);
}
";

            var fixedCode = @"
internal partial union Pet(int, string);

public class Outer
{
    private partial union Shape(int, string);
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

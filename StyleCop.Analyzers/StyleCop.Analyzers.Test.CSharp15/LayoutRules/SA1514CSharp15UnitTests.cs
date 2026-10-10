// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp14.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1514ElementDocumentationHeaderMustBePrecededByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1514CodeFixProvider>;

    public partial class SA1514CSharp15UnitTests : SA1514CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that the documentation header of a union must be preceded by a blank line, that the violation is
        /// reported exactly once, and that the code fix adds the blank line.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestNestedUnionAsync()
        {
            var testCode = @"
public class Outer
{
    public int Legs => 4;
    {|#0:///|} <summary>
    /// A union.
    /// </summary>
    public union Pet(int, string);
}
";

            var fixedCode = @"
public class Outer
{
    public int Legs => 4;

    /// <summary>
    /// A union.
    /// </summary>
    public union Pet(int, string);
}
";

            var expected = Diagnostic().WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

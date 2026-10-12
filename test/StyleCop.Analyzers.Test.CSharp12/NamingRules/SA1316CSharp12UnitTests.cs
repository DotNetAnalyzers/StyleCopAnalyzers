// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1316TupleElementNamesShouldUseCorrectCasing,
        StyleCop.Analyzers.NamingRules.SA1316CodeFixProvider>;

    public partial class SA1316CSharp12UnitTests : SA1316CSharp11UnitTests
    {
        /// <summary>
        /// Verifies that the element names of a tuple type in a using alias directive are checked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4011, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4011")]
        public async Task TestTupleTypeInUsingAliasAsync()
        {
            var testCode = @"using Point = (int {|#0:x|}, int Y);
using Unnamed = (int, int);

class TestClass
{
    Point field1;
    Unnamed field2;
}
";

            var fixedCode = @"using Point = (int X, int Y);
using Unnamed = (int, int);

class TestClass
{
    Point field1;
    Unnamed field2;
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

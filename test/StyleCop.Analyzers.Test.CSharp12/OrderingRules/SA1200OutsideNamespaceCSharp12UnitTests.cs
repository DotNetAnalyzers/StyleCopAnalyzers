// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using Xunit;

    public partial class SA1200OutsideNamespaceCSharp12UnitTests
    {
        [Fact]
        public async Task TestAliasAnyTypeInNamespaceIsMovedAndQualifiedAsync()
        {
            var testCode = @"namespace TestNamespace
{
    {|#0:using Point = (int X, System.IO.Stream S);|}
    {|#1:using Items = TestClass[];|}
    {|#2:using Numbers = int[];|}

    public class TestClass
    {
    }
}
";
            var fixedTestCode = @"using Items = TestNamespace.TestClass[];
using Numbers = int[];
using Point = (int X, System.IO.Stream S);

namespace TestNamespace
{
    public class TestClass
    {
    }
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(0),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(2),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

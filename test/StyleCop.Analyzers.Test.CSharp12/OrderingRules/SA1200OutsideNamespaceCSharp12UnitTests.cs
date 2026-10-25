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

        [Fact]
        public async Task TestAliasAnyTypeWithGlobalQualifiedGenericIsQualifiedAsync()
        {
            var testCode = @"namespace TestNamespace
{
    {|#0:using Items = global::G<TestClass>[];|}

    public class TestClass
    {
    }
}

public class G<T>
{
}
";
            var fixedTestCode = @"using Items = global::G<global::TestNamespace.TestClass>[];

namespace TestNamespace
{
    public class TestClass
    {
    }
}

public class G<T>
{
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(0),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAliasAnyTypeReferringToArrayAliasIsExpandedAsync()
        {
            var testCode = @"using Numbers = int[];

namespace TestNamespace
{
    {|#0:using Matrix = Numbers[];|}
}
";
            var fixedTestCode = @"using Matrix = int[][];
using Numbers = int[];

namespace TestNamespace
{
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(0),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAliasAnyTypeReferringToTupleAliasPreservesElementNamesAsync()
        {
            var testCode = @"using Pair = (int X, int Y);

namespace TestNamespace
{
    {|#0:using Pairs = Pair[];|}

    public class TestClass
    {
        public int M(Pairs pairs) => pairs[0].X;
    }
}
";
            var fixedTestCode = @"using Pair = (int X, int Y);
using Pairs = (int X, int Y)[];

namespace TestNamespace
{
    public class TestClass
    {
        public int M(Pairs pairs) => pairs[0].X;
    }
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(0),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

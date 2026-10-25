// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.OrderingRules
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Test.Verifiers;
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
            var fixedTestCode = @"using Items = global::G<TestNamespace.TestClass>[];

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

        [Fact]
        public async Task TestAliasAnyTypeReferringToArrayAliasPreservesRankOrderAsync()
        {
            var testCode = @"using Row = int[];

namespace TestNamespace
{
    {|#0:using Matrix = Row[,];|}

    public class TestClass
    {
        public int[] M(Matrix matrix) => matrix[0, 0];
    }
}
";
            var fixedTestCode = @"using Matrix = int[,][];
using Row = int[];

namespace TestNamespace
{
    public class TestClass
    {
        public int[] M(Matrix matrix) => matrix[0, 0];
    }
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(0),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAliasAnyTypeReferringToNullableAliasPreservesNullabilityAsync()
        {
            var testCode = @"using Maybe = TestNamespace.TestStruct?;

namespace TestNamespace
{
    {|#0:using Items = Maybe[];|}

    public struct TestStruct
    {
    }

    public class TestClass
    {
        public bool M(Items items) => items[0].HasValue;
    }
}
";
            var fixedTestCode = @"using Items = TestNamespace.TestStruct?[];
using Maybe = TestNamespace.TestStruct?;

namespace TestNamespace
{
    public struct TestStruct
    {
    }

    public class TestClass
    {
        public bool M(Items items) => items[0].HasValue;
    }
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(0),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAliasAnyTypeWithExternAliasPreservesQualifierAsync()
        {
            var testCode = @"extern alias corlib;

namespace TestNamespace
{
    {|#0:using Items = corlib::System.String[];|}
    {|#1:using Lists = corlib::System.Collections.Generic.List<TestClass>[];|}
    {|#2:using Wrappers = Wrapper<corlib::System.String>[];|}

    public class TestClass
    {
    }

    public class Wrapper<T>
    {
    }
}
";
            var fixedTestCode = @"extern alias corlib;
using Items = corlib::System.String[];
using Lists = corlib::System.Collections.Generic.List<TestNamespace.TestClass>[];
using Wrappers = TestNamespace.Wrapper<corlib::System.String>[];

namespace TestNamespace
{
    public class TestClass
    {
    }

    public class Wrapper<T>
    {
    }
}
";

            var test = new StyleCopCodeFixVerifier<SA1200UsingDirectivesMustBePlacedCorrectly, UsingCodeFixProvider>.CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedTestCode,
                Settings = TestSettings,
                ExpectedDiagnostics =
                {
                    Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(0),
                    Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(1),
                    Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(2),
                },
            };
            test.SolutionTransforms.Add((solution, projectId) =>
            {
                var project = solution.GetProject(projectId) ?? throw new InvalidOperationException();
                var references = project.MetadataReferences.OfType<PortableExecutableReference>()
                    .Where(reference => Path.GetFileName(reference.FilePath) is "System.Runtime.dll" or "System.Collections.dll")
                    .ToList();

                foreach (var reference in references)
                {
                    solution = solution
                        .RemoveMetadataReference(projectId, reference)
                        .AddMetadataReference(projectId, reference.WithAliases(new[] { "global", "corlib" }));
                }

                return solution;
            });

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAliasAnyTypePreservesNullableReferenceAnnotationsAsync()
        {
            var testCode = @"#nullable enable

namespace TestNamespace
{
    {|#0:using Items = TestClass?[];|}

    public class TestClass
    {
    }
}
";
            var fixedTestCode = @"using Items = TestNamespace.TestClass?[];

#nullable enable

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
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAliasAnyTypePreservesCommentsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    {|#0:using Point = (int X, /* coordinate */ TestClass Y);|}

    public class TestClass
    {
    }
}
";
            var fixedTestCode = @"using Point = (int X, /* coordinate */ TestNamespace.TestClass Y);

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
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1200UsingDirectivesMustBePlacedCorrectly,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    /// <summary>
    /// Unit tests for the <see cref="SA1200UsingDirectivesMustBePlacedCorrectly"/>.
    /// </summary>
    public class SA1200UnitTests
    {
        private const string ClassDefinition = @"public class TestClass
{
}";

        private const string StructDefinition = @"public struct TestStruct
{
}";

        private const string InterfaceDefinition = @"public interface TestInterface
{
}";

        private const string EnumDefinition = @"public enum TestEnum
{
    TestValue
}";

        private const string DelegateDefinition = @"public delegate void TestDelegate();";

        /// <summary>
        /// Verifies that valid using statements in a namespace does not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingStatementsInNamespaceAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;
    using System.Threading;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that having using statements in the compilation unit will not produce any diagnostics when there are type definition present.
        /// </summary>
        /// <param name="typeDefinition">The type definition to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(ClassDefinition)]
        [InlineData(StructDefinition)]
        [InlineData(InterfaceDefinition)]
        [InlineData(EnumDefinition)]
        [InlineData(DelegateDefinition)]
        public async Task TestValidUsingStatementsInCompilationUnitWithTypeDefinitionAsync(string typeDefinition)
        {
            var testCode = $@"using System;

{typeDefinition}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that having using statements in the compilation unit will not produce any diagnostics when there are attributes present.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingStatementsInCompilationUnitWithAttributesAsync()
        {
            var testCode = @"using System.Reflection;

[assembly: AssemblyVersion(""1.0.0.0"")]

namespace TestNamespace
{
    using System;
    using System.Threading;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that having using statements in the compilation unit will produce the expected diagnostics.
        /// </summary>
        /// <param name="lineEnding">The line ending to use in the test code.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonData.EndOfLineSequences), MemberType = typeof(CommonData))]
        public async Task TestInvalidUsingStatementsInCompilationUnitAsync(string lineEnding)
        {
            var testCode = @"{|#0:using System;|}
{|#1:using System.Threading;|}

namespace TestNamespace
{
}
".ReplaceLineEndings(lineEnding);

            var fixedTestCode = @"namespace TestNamespace
{
    using System;
    using System.Threading;
}
".ReplaceLineEndings(lineEnding);

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(0),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that extern alias directives in the compilation unit are moved into the namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestAliasReferringToNonGenericValueTupleAliasIsExpandedWhenMovedInsideNamespaceAsync()
        {
            var testCode = @"{|#0:using VT = System.ValueTuple;|}

namespace NamespaceName
{
    using OtherVT = VT;
}
";

            var fixedTestCode = @"namespace NamespaceName
{
    using OtherVT = System.ValueTuple;
    using VT = System.ValueTuple;
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(0),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1931, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1931")]
        public async Task TestInvalidExternAliasDirectivesInCompilationUnitAsync()
        {
            var testCode = @"{|#0:extern alias corlib;|} // alias comment

namespace TestNamespace
{
    using Core = corlib::System;
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    extern alias corlib; // alias comment
    using Core = corlib::System;
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(0),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1931, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1931")]
        public async Task TestExternAliasWithoutNamespaceDoesNotProduceDiagnosticAsync()
        {
            var testCode = @"extern alias corlib;
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixWithUsingDirectiveWithoutNamespaceDoesNotThrowAsync()
        {
            var testCode = @"{|#0:using System.Threading;|}
{|#1:using System;|}
";
            var fixedCode = @"{|#0:using System;|}
{|#1:using System.Threading;|}
";
            DiagnosticResult[] expected =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(0),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(1),
            };
            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                NumberOfFixAllIterations = 2,
            };
            test.ExpectedDiagnostics.AddRange(expected);
            test.FixedState.ExpectedDiagnostics.AddRange(expected);

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixWithNonGenericValueTupleAliasWithoutNamespaceAsync()
        {
            var testCode = @"{|#0:using VT = System.ValueTuple;|}
{|#1:using System;|}
";
            var fixedCode = @"{|#0:using System;|}
{|#1:using VT = System.ValueTuple;|}
";
            DiagnosticResult[] expected =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(0),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(1),
            };
            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                NumberOfFixAllIterations = 2,
            };
            test.ExpectedDiagnostics.AddRange(expected);
            test.FixedState.ExpectedDiagnostics.AddRange(expected);

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1931, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1931")]
        public async Task TestExternAliasWithDuplicateNamespaceAliasIsDiagnosedAsync()
        {
            var testCode = @"{|#0:extern alias corlib;|}

namespace TestNamespace
{
    extern alias corlib;
}
";

            DiagnosticResult expected = Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(0);
            await VerifyCSharpDiagnosticAsync(testCode, new[] { expected }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1931, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1931")]
        public async Task TestCodeFixIsNotOfferedForDuplicateExternAliasAsync()
        {
            var testCode = @"extern alias corlib;

namespace TestNamespace
{
    extern alias corlib;
}
";

            await VerifyCodeFixIsNotOfferedForExternAliasAsync(testCode).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1931, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1931")]
        public async Task TestCodeFixIsNotOfferedForDuplicateExternAliasOutsideNamespaceAsync()
        {
            var testCode = @"extern alias corlib;

namespace TestNamespace
{
    extern alias corlib;
}
";

            await VerifyCodeFixIsNotOfferedForExternAliasAsync(testCode, insideNamespacePlacement: false).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1931, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1931")]
        public async Task TestConditionalExternAliasIsDiagnosedAsync()
        {
            var testCode = @"#if true
{|#0:extern alias corlib;|}
#endif

namespace TestNamespace
{
}
";

            DiagnosticResult expected = Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(0);
            await VerifyCSharpDiagnosticAsync(testCode, new[] { expected }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1931, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1931")]
        public async Task TestCodeFixIsNotOfferedForConditionalExternAliasAsync()
        {
            var testCode = @"#if true
extern alias corlib;
#endif

namespace TestNamespace
{
}
";

            await VerifyCodeFixIsNotOfferedForExternAliasAsync(testCode).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1931, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1931")]
        public async Task TestCodeFixIsNotOfferedForConditionalExternAliasOutsideNamespaceAsync()
        {
            var testCode = @"namespace TestNamespace
{
#if true
    extern alias corlib;
#endif
}
";

            await VerifyCodeFixIsNotOfferedForExternAliasAsync(testCode, insideNamespacePlacement: false).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1931, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1931")]
        public async Task TestDisabledConditionalExternAliasDoesNotProduceDiagnosticAsync()
        {
            var testCode = @"#if false
extern alias corlib;
#endif

namespace TestNamespace
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidUsingStatementsInCompilationUnitWithPragmaAsync()
        {
            var testCode = @"#pragma warning disable 1573 // Comment
using System;
using System.Threading;

namespace TestNamespace
{
}
";

            var fixedTestCode = @"namespace TestNamespace
{
#pragma warning disable 1573 // Comment
    using System;
    using System.Threading;
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(2, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(3, 1),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidUsingStatementsInCompilationUnitWithRegionBeforeAsync()
        {
            var testCode = @"#region Comment
#endregion Comment
using System;
using System.Threading;

namespace TestNamespace
{
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    #region Comment
    #endregion Comment
    using System;
    using System.Threading;
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(3, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(4, 1),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2363, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2363")]
        public async Task TestInvalidUsingStatementsWithFileHeaderTriviaAsync()
        {
            var testCode = @"// Some comment
using System;
using System.Threading;

namespace TestNamespace
{
}
";

            var fixedTestCode = @"// Some comment
namespace TestNamespace
{
    using System;
    using System.Threading;
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(2, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(3, 1),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2363, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2363")]
        public async Task TestInvalidUsingStatementsWithSeparatedFileHeaderTriviaAsync()
        {
            var testCode = @"// Some comment

using System;
using System.Threading;

namespace TestNamespace
{
}
";

            var fixedTestCode = @"// Some comment

namespace TestNamespace
{
    using System;
    using System.Threading;
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(3, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(4, 1),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2363, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2363")]
        public async Task TestInvalidUsingStatementsWithSeparatedFileHeaderAndTriviaAsync()
        {
            var testCode = @"// File Header

// Leading Comment

using System;
using System.Threading;

namespace TestNamespace
{
}
";

            var fixedTestCode = @"// File Header

namespace TestNamespace
{
    // Leading Comment

    using System;
    using System.Threading;
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(5, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(6, 1),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2363, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2363")]
        public async Task TestInvalidUsingStatementsWithTriviaAsync()
        {
            var testCode = @"
// Some comment
using System;
using System.Threading;

namespace TestNamespace
{
}
";

            var fixedTestCode = @"
namespace TestNamespace
{
    // Some comment
    using System;
    using System.Threading;
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(3, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(4, 1),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2363, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2363")]
        public async Task TestInvalidUsingStatementsWithHeaderAndTriviaAsync()
        {
            var testCode = @"// Copyright notice here

// Some comment
using System;
using System.Threading;

namespace TestNamespace
{
}
";

            var fixedTestCode = @"// Copyright notice here

namespace TestNamespace
{
    // Some comment
    using System;
    using System.Threading;
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(4, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(5, 1),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an alias referring to an alias of the compilation unit is expanded when the aliases end up
        /// in the same namespace block, because the target of an alias is resolved without the other using
        /// directives of its block.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1771, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1771")]
        public async Task TestAliasReferringToAliasIsExpandedWhenMovedInsideNamespaceAsync()
        {
            var testCode = @"{|#0:using MyAction = System.Action;|}

namespace NamespaceName
{
    using MyOtherAction = MyAction;
}
";

            var fixedTestCode = @"namespace NamespaceName
{
    using MyAction = System.Action;
    using MyOtherAction = System.Action;
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(0),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an alias referring to an alias in a type argument is expanded when the aliases end up in
        /// the same namespace block.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1771, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1771")]
        public async Task TestAliasReferringToAliasInTypeArgumentIsExpandedWhenMovedInsideNamespaceAsync()
        {
            var testCode = @"{|#0:using MyAction = System.Action;|}

namespace NamespaceName
{
    using MyList = System.Collections.Generic.List<MyAction>;
}
";

            var fixedTestCode = @"namespace NamespaceName
{
    using MyAction = System.Action;
    using MyList = System.Collections.Generic.List<System.Action>;
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(0),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an alias target imported by a using directive of the compilation unit is fully qualified
        /// when that directive is moved into the namespace, while names declared in the namespace are preserved.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1771, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1771")]
        public async Task TestAliasTargetImportedByNamespaceUsingIsQualifiedWhenMovedInsideNamespaceAsync()
        {
            var testCode = @"{|#0:using System;|}

namespace NamespaceName
{
    using MyAction = Action;
    using MyLocal = Local;

    public class Local
    {
    }
}
";

            var fixedTestCode = @"namespace NamespaceName
{
    using System;
    using MyAction = System.Action;
    using MyLocal = Local;

    public class Local
    {
    }
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(0),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static async Task VerifyCodeFixIsNotOfferedForExternAliasAsync(string source, bool insideNamespacePlacement = true)
        {
            using var workspace = new AdhocWorkspace();
            var project = workspace.AddProject("Test", LanguageNames.CSharp);
            var document = workspace.AddDocument(project.Id, "Test.cs", SourceText.From(source));
            if (!insideNamespacePlacement)
            {
                const string settings = @"{
  ""settings"": {
    ""orderingRules"": {
      ""usingDirectivesPlacement"": ""outsideNamespace""
    }
  }
}";
                var solution = document.Project.Solution.AddAdditionalDocument(DocumentId.CreateNewId(project.Id), "stylecop.json", SourceText.From(settings));
                if (!workspace.TryApplyChanges(solution))
                {
                    throw new InvalidOperationException();
                }

                document = workspace.CurrentSolution.GetDocument(document.Id) ?? throw new InvalidOperationException();
            }

            var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false) ?? throw new InvalidOperationException();
            var aliases = syntaxRoot.DescendantNodes().OfType<ExternAliasDirectiveSyntax>();
            var externAlias = insideNamespacePlacement ? aliases.First() : aliases.Last();
            var diagnostic = Microsoft.CodeAnalysis.Diagnostic.Create(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside, externAlias.GetLocation());
            var codeActions = new List<CodeAction>();
            var context = new CodeFixContext(document, diagnostic, (action, _) => codeActions.Add(action), CancellationToken.None);

            await new UsingCodeFixProvider().RegisterCodeFixesAsync(context).ConfigureAwait(false);

            Assert.Empty(codeActions);
        }
    }
}

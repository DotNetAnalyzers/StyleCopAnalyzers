// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Test.CSharp9.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1200UsingDirectivesMustBePlacedCorrectly,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    public partial class SA1200CSharp10UnitTests : SA1200CSharp9UnitTests
    {
        [Fact]
        public async Task TestInvalidUsingStatementsInCompilationUnitWithFileScopedNamespaceAsync()
        {
            var testCode = @"{|#0:using System;|}
{|#1:using System.Threading;|}

namespace TestNamespace;
";

            var fixedTestCode = @"namespace TestNamespace;

using System;
using System.Threading;
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(0),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\n")]
        [InlineData("// A comment.\n")]
        [WorkItem(3875, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3875")]
        public async Task TestOnlyGlobalUsingStatementInFileAsync(string leadingTrivia)
        {
            var testCode = $@"{leadingTrivia}global using System;";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3875, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3875")]
        public async Task TestGlobalUsingStatementInFileWithNamespaceAsync()
        {
            var testCode = @"global using System;

namespace TestNamespace
{
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3875, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3875")]
        public async Task TestGlobalUsingStatementInFileWithOtherUsingDirectivesAsync()
        {
            var testCode = @"global using System;
[|using System.Linq;|]

namespace TestNamespace
{
}";

            await new CSharpTest
            {
                TestCode = testCode,

                // UsingCodeFixProvider currently leaves all using directives in the same location (either inside or
                // outside the namespace) when the file contains any global using directives.
                FixedCode = testCode,
                NumberOfIncrementalIterations = 1,
                NumberOfFixAllIterations = 1,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

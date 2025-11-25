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

    public partial class SA1200OutsideNamespaceCSharp10UnitTests : SA1200OutsideNamespaceCSharp9UnitTests
    {
        [Fact]
        public async Task TestInvalidUsingStatementsInFileScopedNamespaceAsync()
        {
            var testCode = @"namespace TestNamespace;

{|#0:using System;|}
{|#1:using System.Threading;|}
";
            var fixedTestCode = @"using System;
using System.Threading;

namespace TestNamespace;
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(0),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(1),
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
        public async Task TestGlobalUsingStatementInFileWithOtherUsingDirectivesAsync()
        {
            var testCode = @"global using System;

namespace TestNamespace
{
    [|using System.Linq;|]
}";
            var fixedTestCode = @"global using System;

using System.Linq;

namespace TestNamespace
{
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3875, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3875")]
        public async Task TestGlobalUsingStatementInFileWithOtherUsingDirectives2Async()
        {
            // This test specifically covers the case where the local using directive would sort before the global using
            // directive if the global using directive wasn't treated as always being first.
            var testCode = @"global using System.Linq;

namespace TestNamespace
{
    [|using System;|]
}";
            var fixedTestCode = @"global using System.Linq;

using System;

namespace TestNamespace
{
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

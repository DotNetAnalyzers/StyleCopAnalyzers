// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp10.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    public class SA1209CSharp10UnitTests : SA1209CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3439, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3439")]
        public async Task TestWhenUsingAliasDirectivesAreNotPlacedCorrectlyInFileScopedNamespaceAsync()
        {
            var testCodeNamespace = @"namespace Test;

using System.Net;
using System.Threading;
[|using L = System.Linq;|]
using System.IO;
using P = System.Threading.Tasks;

class A
{
}
";
            var fixedTestCodeNamespace = @"namespace Test;

using System.IO;
using System.Net;
using System.Threading;
using L = System.Linq;
using P = System.Threading.Tasks;

class A
{
}
";

            await VerifyCSharpFixAsync(testCodeNamespace, DiagnosticResult.EmptyDiagnosticResults, fixedTestCodeNamespace, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

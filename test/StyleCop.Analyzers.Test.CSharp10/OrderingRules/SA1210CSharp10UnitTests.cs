// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    public partial class SA1210CSharp10UnitTests : SA1210CSharp9UnitTests
    {
        [Fact]
        public async Task TestUsingDirectivesInFileScopedNamespaceDeclarationAsync()
        {
            await new CSharpTest
            {
                TestSources =
                {
                    @"namespace Foo;

[|using System.Threading;|]
using System;
",
                    @"namespace Bar;

[|using Foo;|]
using Bar;
[|using System.Threading;|]
using System;
",
                },
                FixedSources =
                {
                    @"namespace Foo;

using System;
using System.Threading;
",
                    @"namespace Bar;

using System;
using System.Threading;
using Bar;
using Foo;
",
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3964, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3964")]
        public async Task TestWhenGlobalUsingDirectivesAreNotOrderedAlphabeticallyAsync()
        {
            await new CSharpTest
            {
                TestSources =
                {
                    "namespace AlphaNamespace {}",
                    "namespace BetaNamespace {}",
                    "namespace ZNamespace {}",
                    @"
{|#0:global using ZNamespace;|}
global using AlphaNamespace;
global using BetaNamespace;

class TestClass
{
}
",
                },
                FixedSources =
                {
                    "namespace AlphaNamespace {}",
                    "namespace BetaNamespace {}",
                    "namespace ZNamespace {}",
                    @"
global using AlphaNamespace;
global using BetaNamespace;
global using ZNamespace;

class TestClass
{
}
",
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3964, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3964")]
        public async Task TestGlobalUsingDirectivesAreAnalyzedIndependentlyFromLocalUsingDirectivesAsync()
        {
            var testCode = @"global using ZNamespace;

using AlphaNamespace;

namespace ZNamespace
{
}

namespace AlphaNamespace
{
    public class Placeholder
    {
    }
}

class TestClass
{
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

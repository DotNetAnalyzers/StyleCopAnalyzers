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
        StyleCop.Analyzers.OrderingRules.SA1217UsingStaticDirectivesMustBeOrderedAlphabetically,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    public partial class SA1217CSharp10UnitTests : SA1217CSharp9UnitTests
    {
        [Fact]
        public async Task TestUsingDirectivesOrderingInFileScopedNamespaceAsync()
        {
            await new CSharpTest
            {
                TestSources =
                {
                    @"namespace Foo;

using System;
using Execute = System.Action;
{|#0:using static System.Math;|}
using static System.Array;
",
                    @"namespace Bar;

{|#1:using static System.Math;|}
using Execute = System.Action;
using static System.Array;
using System;
",
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0).WithArguments("System.Math", "System.Array"),
                    Diagnostic().WithLocation(1).WithArguments("System.Math", "System.Array"),
                },
                FixedSources =
                {
                    @"namespace Foo;

using System;
using static System.Array;
using static System.Math;
using Execute = System.Action;
",
                    @"namespace Bar;

using System;
using static System.Array;
using static System.Math;
using Execute = System.Action;
",
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3964, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3964")]
        public async Task TestWhenGlobalUsingStaticDirectivesAreNotOrderedAlphabeticallyAsync()
        {
            await new CSharpTest
            {
                TestSources =
                {
                    @"
{|#0:global using static System.Math;|}
global using static System.Array;

class TestClass
{
}
",
                },
                FixedSources =
                {
                    @"
global using static System.Array;
global using static System.Math;

class TestClass
{
}
",
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0).WithArguments("System.Math", "System.Array"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3964, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3964")]
        public async Task TestGlobalUsingStaticDirectivesAreAnalyzedIndependentlyFromLocalUsingStaticDirectivesAsync()
        {
            var testCode = @"global using static System.Math;

using static System.Array;

class TestClass
{
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3982, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3982")]
        public async Task TestAlphabeticalOrderingEnforcedSeparatelyForGlobalAndLocalStaticUsingDirectivesAsync()
        {
            await new CSharpTest
            {
                TestSources =
                {
                    @"
global using static System.Math;
{|#0:global using static System.Array;|}

using static System.Console;
{|#1:using static System.Array;|}
",
                },
                FixedSources =
                {
                    @"
global using static System.Array;
global using static System.Math;

using static System.Array;
using static System.Console;
",
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0).WithArguments("System.Array", "System.Math"),
                    Diagnostic().WithLocation(1).WithArguments("System.Array", "System.Console"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

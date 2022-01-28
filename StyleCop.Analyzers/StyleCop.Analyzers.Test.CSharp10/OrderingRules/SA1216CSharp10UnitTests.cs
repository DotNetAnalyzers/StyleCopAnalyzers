// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp9.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    public class SA1216CSharp10UnitTests : SA1216CSharp9UnitTests
    {
        [Fact]
        public async Task TestUsingDirectivesOrderingInFileScopedNamespaceAsync()
        {
            await new CSharpTest
            {
                TestSources =
                {
                    @"namespace Foo;

{|#0:using static System.Math;|}
using Execute = System.Action;
using System;
",
                    @"namespace Bar;

using Execute = System.Action;
{|#1:using static System.Array;|}
using static System.Math;
using System;
",
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0),
                    Diagnostic().WithLocation(1),
                },
                FixedSources =
                {
                    @"namespace Foo;
using System;
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
    }
}

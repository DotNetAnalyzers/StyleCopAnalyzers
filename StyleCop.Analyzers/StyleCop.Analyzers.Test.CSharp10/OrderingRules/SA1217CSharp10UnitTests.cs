// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp10.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp9.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1217UsingStaticDirectivesMustBeOrderedAlphabetically,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    public class SA1217CSharp10UnitTests : SA1217CSharp9UnitTests
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
    }
}

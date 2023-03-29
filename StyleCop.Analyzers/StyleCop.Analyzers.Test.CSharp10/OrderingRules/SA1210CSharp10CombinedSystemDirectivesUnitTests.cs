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
        StyleCop.Analyzers.OrderingRules.SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    public class SA1210CSharp10CombinedSystemDirectivesUnitTests : SA1210CSharp9CombinedSystemDirectivesUnitTests
    {
        [Fact]
        public async Task TestUsingDirectivesInFileScopedNamespaceDeclarationAsync()
        {
            await new CSharpTest
            {
                TestSources =
                {
                    @"namespace Food;

[|using System.Threading;|]
using System;
",
                    @"namespace Bar;

[|using Food;|]
using Bar;
[|using System.Threading;|]
using System;
",
                },
                FixedSources =
                {
                    @"namespace Food;

using System;
using System.Threading;
",
                    @"namespace Bar;

using Bar;
using Food;
using System;
using System.Threading;
",
                },
                Settings = CombinedUsingDirectivesTestSettings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

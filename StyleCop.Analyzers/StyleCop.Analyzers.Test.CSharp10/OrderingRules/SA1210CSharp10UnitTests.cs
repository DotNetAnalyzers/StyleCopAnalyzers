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

    public class SA1210CSharp10UnitTests : SA1210CSharp9UnitTests
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
    }
}

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
        StyleCop.Analyzers.OrderingRules.SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    public partial class SA1211CSharp10UnitTests : SA1211CSharp9UnitTests
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
using \u0069nt = System.Int32;
{|#0:using character = System.Char;|}
",
                    @"namespace Bar;

using System;
using Stream = System.IO.Stream;
using StringBuilder = System.Text.StringBuilder;
using StringWriter = System.IO.StringWriter;
{|#1:using MemoryStream = System.IO.MemoryStream;|}
",
                    @"namespace Spam;

using System;
using @int = System.Int32;
{|#2:using Character = System.Char;|}
",
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0).WithArguments("character", "int"),
                    Diagnostic().WithLocation(1).WithArguments("MemoryStream", "Stream"),
                    Diagnostic().WithLocation(2).WithArguments("Character", "int"),
                },
                FixedSources =
                {
                    @"namespace Foo;

using System;
using character = System.Char;
using \u0069nt = System.Int32;
",
                    @"namespace Bar;

using System;
using MemoryStream = System.IO.MemoryStream;
using Stream = System.IO.Stream;
using StringBuilder = System.Text.StringBuilder;
using StringWriter = System.IO.StringWriter;
",
                    @"namespace Spam;

using System;
using Character = System.Char;
using @int = System.Int32;
",
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

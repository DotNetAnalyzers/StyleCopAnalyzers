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
        StyleCop.Analyzers.OrderingRules.SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    public class SA1208CSharp10UnitTests : SA1208CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3437, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3437")]
        public async Task TestWhenSystemUsingDirectivesAreNotOnTopInFileScopedNamespaceAsync()
        {
            await new CSharpTest
            {
                TestSources =
                {
                    "namespace Xyz;",
                    "namespace AnotherNamespace;",
                    @"
namespace Test;

using Xyz;
{|#0:using System;|}
{|#1:using System.IO;|}
using AnotherNamespace;
{|#2:using System.Threading.Tasks;|}

class A
{
}",
                },
                FixedSources =
                {
                    "namespace Xyz;",
                    "namespace AnotherNamespace;",
                    @"
namespace Test;

using System;
using System.IO;
using System.Threading.Tasks;
using AnotherNamespace;
using Xyz;

class A
{
}",
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0).WithArguments("System", "Xyz"),
                    Diagnostic().WithLocation(1).WithArguments("System.IO", "Xyz"),
                    Diagnostic().WithLocation(2).WithArguments("System.Threading.Tasks", "Xyz"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

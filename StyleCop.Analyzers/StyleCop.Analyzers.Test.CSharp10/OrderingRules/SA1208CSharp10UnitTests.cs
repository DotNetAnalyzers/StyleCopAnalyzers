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
        StyleCop.Analyzers.OrderingRules.SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    public partial class SA1208CSharp10UnitTests : SA1208CSharp9UnitTests
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

        [Fact]
        [WorkItem(3964, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3964")]
        public async Task TestWhenSystemGlobalUsingDirectivesAreNotOnTopAsync()
        {
            await new CSharpTest
            {
                TestSources =
                {
                    "namespace Xyz {}",
                    "namespace AnotherNamespace {}",
                    @"
global using Xyz;
{|#0:global using System;|}
{|#1:global using System.IO;|}
global using AnotherNamespace;
{|#2:global using System.Threading.Tasks;|}

class A
{
}",
                },
                FixedSources =
                {
                    "namespace Xyz {}",
                    "namespace AnotherNamespace {}",
                    @"
global using System;
global using System.IO;
global using System.Threading.Tasks;
global using AnotherNamespace;
global using Xyz;

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

        [Fact]
        [WorkItem(3964, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3964")]
        public async Task TestGlobalUsingDirectivesAreAnalyzedIndependentlyFromLocalUsingDirectivesAsync()
        {
            var testCode = @"global using Xyz;

using System;

namespace Xyz
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

        [Fact]
        [WorkItem(3982, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3982")]
        public async Task TestGlobalAndLocalUsingDirectivesMaintainIndependentSystemOrderingAsync()
        {
            await new CSharpTest
            {
                TestSources =
                {
                    "namespace NameSpaceA { }",
                    "namespace OtherNamespace { }",
                    @"
global using NameSpaceA;
{|#0:global using System.Text;|}
global using System;

using OtherNamespace;
{|#1:using System.IO;|}
using System;
",
                },
                FixedSources =
                {
                    @"
global using System;
global using System.Text;
global using NameSpaceA;

using System;
using System.IO;
using OtherNamespace;
",
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0).WithArguments("System.Text", "NameSpaceA"),
                    Diagnostic().WithLocation(1).WithArguments("System.IO", "OtherNamespace"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

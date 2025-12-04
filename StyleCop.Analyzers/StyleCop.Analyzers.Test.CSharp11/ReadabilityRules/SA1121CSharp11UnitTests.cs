// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1121UseBuiltInTypeAlias,
        StyleCop.Analyzers.ReadabilityRules.SA1121CodeFixProvider>;

    public partial class SA1121CSharp11UnitTests : SA1121CSharp10UnitTests
    {
        // NOTE: This tests a fix for a c# 10 feature, but the Roslyn API used to solve it wasn't available in the version
        // we use in the c# 10 test project, so the test was added here instead.
        [Fact]
        [WorkItem(3594, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3594")]
        public async Task TestUsingNameChangeInGlobalUsingInAnotherFileAsync()
        {
            var source1 = @"
global using MyDouble = System.Double;";

            var oldSource2 = @"
class TestClass
{
    private [|MyDouble|] x;
}";

            var newSource2 = @"
class TestClass
{
    private double x;
}";

            await new CSharpTest()
            {
                TestSources = { source1, oldSource2 },
                FixedSources = { source1, newSource2 },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3594, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3594")]
        public async Task TestUsingNameChangeInGlobalUsingInSameFileAsync()
        {
            var source = @"global using MyDouble = System.Double;
class TestClass
{
    private [|MyDouble|] x;
}";

            var newSource = @"global using MyDouble = System.Double;
class TestClass
{
    private double x;
}";

            await new CSharpTest()
            {
                TestSources = { source },
                FixedSources = { newSource },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3969, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3969")]
        public async Task TestNativeSizedIntegersReportWithNet70Async()
        {
            var testCode = @"
using System;

class TestClass
{
    [|IntPtr|] field1;
    [|System.UIntPtr|] field2;
}";

            var fixedCode = @"
using System;

class TestClass
{
    nint field1;
    nuint field2;
}";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3969, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3969")]
        public async Task TestNativeSizedIntegersDoNotReportWithNet60Async()
        {
            var testCode = @"
using System;

class TestClass
{
    IntPtr field1;
    System.UIntPtr field2;
}";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
                TestCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

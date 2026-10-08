// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.ReadabilityRules;
    using StyleCop.Analyzers.Test.CSharp13.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1127GenericTypeConstraintsMustBeOnOwnLine,
        StyleCop.Analyzers.ReadabilityRules.SA1127CodeFixProvider>;

    public partial class SA1127CSharp13UnitTests : SA1127CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4020, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4020")]
        public async Task TestViolationWithAllowsRefStructConstraintAsync()
        {
            var testCode = @"
class Foo<T> [|where T : allows ref struct|] {}";

            var fixedCode = @"
class Foo<T>
    where T : allows ref struct
{}";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ReferenceAssemblies = RuntimeReferenceAssemblies.Net90,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4020, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4020")]
        public async Task TestViolationWithMultiConstraintAllowsRefStructAsync()
        {
            var testCode = @"
interface ISomeInterface
{
}

class Foo<T> [|where T : ISomeInterface, allows ref struct|] {}";
            var fixedCode = @"
interface ISomeInterface
{
}

class Foo<T>
    where T : ISomeInterface, allows ref struct
{}";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ReferenceAssemblies = RuntimeReferenceAssemblies.Net90,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

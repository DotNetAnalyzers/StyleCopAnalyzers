// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1212PropertyAccessorsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1212SA1213CodeFixProvider>;

    public partial class SA1212CSharp9UnitTests : SA1212CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3652, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3652")]
        public async Task TestAutoPropertyDeclarationInitBeforeGetterAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop { [|init;|] get; }
}";

            var fixedCode = @"
public class Foo
{
    public int Prop { get; init; }
}";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3652, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3652")]
        public async Task TestPropertyWithBackingFieldDeclarationInitBeforeGetterAsync()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        [|init
        {
            i = value;
        }|]

        get
        {
            return i;
        }
    }
}";

            var fixedCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        get
        {
            return i;
        }

        init
        {
            i = value;
        }
    }
}";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3966")]
        public async Task TestIndexerDeclarationInitBeforeGetterAsync()
        {
            var testCode = @"
public class Foo
{
    public int this[int index]
    {
        [|init
        {
        }|]

        get
        {
            return index;
        }
    }
}";

            var fixedCode = @"
public class Foo
{
    public int this[int index]
    {
        get
        {
            return index;
        }

        init
        {
        }
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

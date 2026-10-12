// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp13.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1212PropertyAccessorsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1212SA1213CodeFixProvider>;

    public partial class SA1212CSharp14UnitTests : SA1212CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestSetterBeforeGetterInExtensionBlockAsync()
        {
            var testCode = @"
public static class TestClass
{
    private static int storage;

    extension(string source)
    {
        public static int Value
        {
            set
            {
                storage = value;
            }

            get
            {
                return storage;
            }
        }
    }
}";

            var expected = Diagnostic().WithLocation(10, 13);

            var fixedCode = @"
public static class TestClass
{
    private static int storage;

    extension(string source)
    {
        public static int Value
        {
            get
            {
                return storage;
            }

            set
            {
                storage = value;
            }
        }
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp8.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1501StatementMustNotBeOnASingleLine,
        StyleCop.Analyzers.LayoutRules.SA1501CodeFixProvider>;

    public partial class SA1501CSharp9UnitTests : SA1501CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3978, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3978")]
        public async Task TestLocalFunctionWithAttributeOnSingleLineAsync()
        {
            var testCode = @"using System;

class TestClass
{
    void Outer()
    {
        [Obsolete]
        void Local(){|#0:{|} int value = 0; }
    }
}
";

            var fixedCode = @"using System;

class TestClass
{
    void Outer()
    {
        [Obsolete]
        void Local()
        {
            int value = 0;
        }
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

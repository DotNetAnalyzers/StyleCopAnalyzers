// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1015ClosingGenericBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1015CSharp8UnitTests : SA1015CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3302, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3302")]
        public async Task TestGenericTypePointerAsync()
        {
            const string testCode = @"using System;

public struct Foo<T>
{
    internal unsafe Foo<T [|>|] * Next1;
    internal unsafe Foo<T [|>|]* Next2;
}";
            const string fixedCode = @"using System;

public struct Foo<T>
{
    internal unsafe Foo<T> * Next1;
    internal unsafe Foo<T>* Next2;
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

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
        StyleCop.Analyzers.SpacingRules.SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1023CSharp8UnitTests : SA1023CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3302, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3302")]
        public async Task TestGenericTypePointerAsync()
        {
            const string testCode = @"using System;

public struct Foo<T>
{
    internal unsafe Foo<T> [|*|] Next1;
    internal unsafe Foo<T>[|*|]Next2;
    internal unsafe Foo<T> [|[|*|]|]Next3;
}";
            const string fixedCode = @"using System;

public struct Foo<T>
{
    internal unsafe Foo<T>* Next1;
    internal unsafe Foo<T>* Next2;
    internal unsafe Foo<T>* Next3;
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

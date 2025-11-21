// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1011ClosingSquareBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1011CSharp9UnitTests : SA1011CSharp8UnitTests
    {
        [Fact]
        public async Task TestFunctionPointerUnmanagedCallingConventionListAsync()
        {
            var testCode = @"public class TestClass
{
    unsafe delegate* unmanaged[Stdcall {|#0:]|}<void> FuncPtr1;
    unsafe delegate* unmanaged[Stdcall{|#1:]|} <void> FuncPtr2;
}
";

            var fixedCode = @"public class TestClass
{
    unsafe delegate* unmanaged[Stdcall]<void> FuncPtr1;
    unsafe delegate* unmanaged[Stdcall]<void> FuncPtr2;
}
";

            var expected = new[]
            {
                Diagnostic().WithArguments(" not", "preceded").WithLocation(0),
                Diagnostic().WithArguments(" not", "followed").WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

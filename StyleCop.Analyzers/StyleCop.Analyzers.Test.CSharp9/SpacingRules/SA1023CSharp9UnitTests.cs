// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1023CSharp9UnitTests : SA1023CSharp8UnitTests
    {
        [Fact]
        public async Task TestFunctionPointerParameterInvalidSpacingAsync()
        {
            var testCode = @"public class TestClass
{
    unsafe delegate*<int {|#0:*|}> FuncPtr1;
    unsafe delegate*<int{|#1:*|} > FuncPtr2;
}
";

            var fixedCode = @"public class TestClass
{
    unsafe delegate*<int*> FuncPtr1;
    unsafe delegate*<int*> FuncPtr2;
}
";

            var expected = new[]
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(0),
                Diagnostic(DescriptorNotFollowed).WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFunctionPointerTypeInvalidSpacingAsync()
        {
            var testCode = @"public class TestClass
{
    unsafe delegate {|#0:*|}<int*> FuncPtr1;
    unsafe delegate{|#1:*|} <int*> FuncPtr2;
    unsafe delegate {|#2:*|} managed<int*> FuncPtr3;
    unsafe delegate{|#3:*|}managed<int*> FuncPtr4;
    unsafe delegate {|#4:*|} unmanaged<int*> FuncPtr5;
    unsafe delegate{|#5:*|}unmanaged<int*> FuncPtr6;
}
";

            var fixedCode = @"public class TestClass
{
    unsafe delegate*<int*> FuncPtr1;
    unsafe delegate*<int*> FuncPtr2;
    unsafe delegate* managed<int*> FuncPtr3;
    unsafe delegate* managed<int*> FuncPtr4;
    unsafe delegate* unmanaged<int*> FuncPtr5;
    unsafe delegate* unmanaged<int*> FuncPtr6;
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(0),
                Diagnostic(DescriptorNotFollowed).WithLocation(1),
                Diagnostic(DescriptorNotPreceded).WithLocation(2),
                Diagnostic(DescriptorFollowed).WithLocation(3),
                Diagnostic(DescriptorNotPreceded).WithLocation(4),
                Diagnostic(DescriptorFollowed).WithLocation(5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

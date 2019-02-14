// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.SpacingRules.SA1006PreprocessorKeywordsMustNotBePrecededBySpace,
        Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1006PreprocessorKeywordsMustNotBePrecededBySpace"/> and
    /// <see cref="TokenSpacingCodeFixProvider"/>.
    /// </summary>
    public class SA1006UnitTests
    {
        [Fact]
        public async Task TestRegionDirectivesAsync()
        {
            string testCode = @"
class ClassName
{
    # region Methods
    void MethodName()
    {
    }
    #  endregion
}
";

            string fixedCode = @"
class ClassName
{
    #region Methods
    void MethodName()
    {
    }
    #endregion
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("region").WithLocation(4, 7),
                Diagnostic().WithArguments("endregion").WithLocation(8, 8),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIfElseDirectivesAsync()
        {
            string testCode = @"
class ClassName
{
# if true
# pragma warning disable IdentifierName1 // enabled code is checked
    void MethodName()
    {
    }
#  elif false
# pragma warning disable IdentifierName2 // disabled code is checked too
random invalid text
# else
more invalid text
#   endif
}
";

            string fixedCode = @"
class ClassName
{
#if true
#pragma warning disable IdentifierName1 // enabled code is checked
    void MethodName()
    {
    }
#elif false
#pragma warning disable IdentifierName2 // disabled code is checked too
random invalid text
#else
more invalid text
#endif
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("if").WithLocation(4, 3),
                Diagnostic().WithArguments("pragma").WithLocation(5, 3),
                Diagnostic().WithArguments("elif").WithLocation(9, 4),
                Diagnostic().WithArguments("pragma").WithLocation(10, 3),
                Diagnostic().WithArguments("else").WithLocation(12, 3),
                Diagnostic().WithArguments("endif").WithLocation(14, 5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissingDirectiveNameOnLastLineAsync()
        {
            string testCode = @"
class ClassName
{
}
# ";

            DiagnosticResult expected = DiagnosticResult.CompilerError("CS1024").WithMessage("Preprocessor directive expected").WithLocation(5, 1);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

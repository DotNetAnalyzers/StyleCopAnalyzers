// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1006PreprocessorKeywordsMustNotBePrecededBySpace"/> and
    /// <see cref="TokenSpacingCodeFixProvider"/>.
    /// </summary>
    public class SA1006UnitTests : CodeFixVerifier
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
                this.CSharpDiagnostic().WithArguments("region").WithLocation(4, 7),
                this.CSharpDiagnostic().WithArguments("endregion").WithLocation(8, 8),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments("if").WithLocation(4, 3),
                this.CSharpDiagnostic().WithArguments("pragma").WithLocation(5, 3),
                this.CSharpDiagnostic().WithArguments("elif").WithLocation(9, 4),
                this.CSharpDiagnostic().WithArguments("pragma").WithLocation(10, 3),
                this.CSharpDiagnostic().WithArguments("else").WithLocation(12, 3),
                this.CSharpDiagnostic().WithArguments("endif").WithLocation(14, 5),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1006PreprocessorKeywordsMustNotBePrecededBySpace();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }
    }
}

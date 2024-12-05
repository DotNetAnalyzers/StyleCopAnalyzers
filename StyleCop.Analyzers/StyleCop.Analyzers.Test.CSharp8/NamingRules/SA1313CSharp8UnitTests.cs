// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp8.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1313ParameterNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

    public partial class SA1313CSharp8UnitTests : SA1313CSharp7UnitTests
    {
        [Fact]
        [WorkItem(2974, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2974")]
        public async Task TestClosureParameterNamedUnusedAsync()
        {
            var testCode = @"public class TypeName
{
    public int MethodName(int _used, int _unused, int _1, int _2, int _, int __)
    {
        return _used + Closure(0, 1);

        int Closure(int _10, int _11)
        {
            return _2 + _11;
        }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("_used").WithLocation(3, 31),
                Diagnostic().WithArguments("_2").WithLocation(3, 63),
                Diagnostic().WithArguments("_11").WithLocation(7, 34),
            };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticResult[] GetInvalidMethodOverrideShouldNotProduceDiagnosticAsyncDiagnostics()
        {
            return new DiagnosticResult[]
            {
                DiagnosticResult.CompilerError("CS0534").WithLocation(9, 18).WithMessage("'TestClass' does not implement inherited abstract member 'BaseClass.TestMethod(int, int)'"),
                DiagnosticResult.CompilerError("CS0246").WithLocation(11, 49).WithMessage("The type or namespace name 'X' could not be found (are you missing a using directive or an assembly reference?)"),
                DiagnosticResult.CompilerError("CS1001").WithLocation(11, 51).WithMessage("Identifier expected"),
                DiagnosticResult.CompilerError("CS1003").WithLocation(11, 51).WithMessage("Syntax error, ',' expected"),
            };
        }
    }
}

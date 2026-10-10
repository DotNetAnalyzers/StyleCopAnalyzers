// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1317IdentifierShouldBeNamedOnlyWithLatinLetters,
        StyleCop.Analyzers.NamingRules.SA1317CodeFixProvider>;

    public class SA1317UnitTests
    {
        [Fact]
        public async Task TestClassNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName {}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassNameContainsNonLatinLettersAsync()
        {
            var testCode = @"public class СlаssNаmе {}";

            var expected = new DiagnosticResult[]
                {
                    Diagnostic().WithArguments("СlаssNаmе", 0).WithLocation(1, 14),
                    Diagnostic().WithArguments("СlаssNаmе", 2).WithLocation(1, 14),
                    Diagnostic().WithArguments("СlаssNаmе", 6).WithLocation(1, 14),
                    Diagnostic().WithArguments("СlаssNаmе", 8).WithLocation(1, 14),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public void MethodName() {}
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodNameContainsNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public void MеthоdNаmе() {}
}";

            var expected = new DiagnosticResult[]
                {
                    Diagnostic().WithArguments("MеthоdNаmе", 1).WithLocation(3, 17),
                    Diagnostic().WithArguments("MеthоdNаmе", 4).WithLocation(3, 17),
                    Diagnostic().WithArguments("MеthоdNаmе", 7).WithLocation(3, 17),
                    Diagnostic().WithArguments("MеthоdNаmе", 9).WithLocation(3, 17),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public ClassName() {}
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorNameContainsNonLatinLettersAsync()
        {
            var testCode = @"public class СlаssNаmе
{
    public СlаssNаmе() {}
}";

            var expected = new DiagnosticResult[]
                {
                    Diagnostic().WithArguments("СlаssNаmе", 0).WithLocation(1, 14),
                    Diagnostic().WithArguments("СlаssNаmе", 2).WithLocation(1, 14),
                    Diagnostic().WithArguments("СlаssNаmе", 6).WithLocation(1, 14),
                    Diagnostic().WithArguments("СlаssNаmе", 8).WithLocation(1, 14),
                    Diagnostic().WithArguments("СlаssNаmе", 0).WithLocation(3, 12),
                    Diagnostic().WithArguments("СlаssNаmе", 2).WithLocation(3, 12),
                    Diagnostic().WithArguments("СlаssNаmе", 6).WithLocation(3, 12),
                    Diagnostic().WithArguments("СlаssNаmе", 8).WithLocation(3, 12),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    ~ClassName() {}
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorNameContainsInvalidCharactersAsync()
        {
            var testCode = @"public class СlаssNаmе
{
    ~СlаssNаmе() {}
}";

            var expected = new DiagnosticResult[]
                {
                    Diagnostic().WithArguments("СlаssNаmе", 0).WithLocation(1, 14),
                    Diagnostic().WithArguments("СlаssNаmе", 2).WithLocation(1, 14),
                    Diagnostic().WithArguments("СlаssNаmе", 6).WithLocation(1, 14),
                    Diagnostic().WithArguments("СlаssNаmе", 8).WithLocation(1, 14),
                    Diagnostic().WithArguments("СlаssNаmе", 0).WithLocation(3, 6),
                    Diagnostic().WithArguments("СlаssNаmе", 2).WithLocation(3, 6),
                    Diagnostic().WithArguments("СlаssNаmе", 6).WithLocation(3, 6),
                    Diagnostic().WithArguments("СlаssNаmе", 8).WithLocation(3, 6),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"enum EnumName {}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumNameContainsNonLatinLettersAsync()
        {
            var testCode = @"enum ЕnumNаmе {}";

            var expected = new DiagnosticResult[]
                {
                    Diagnostic().WithArguments("ЕnumNаmе", 0).WithLocation(1, 6),
                    Diagnostic().WithArguments("ЕnumNаmе", 5).WithLocation(1, 6),
                    Diagnostic().WithArguments("ЕnumNаmе", 7).WithLocation(1, 6),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumValueNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"enum EnumName
{
    EnumValueName
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumValueNameContainsNonLatinLettersAsync()
        {
            var testCode = @"enum EnumName
{
    ЕnumVаluеNаmе
}";

            var expected = new DiagnosticResult[]
                {
                    Diagnostic().WithArguments("ЕnumVаluеNаmе", 0).WithLocation(3, 5),
                    Diagnostic().WithArguments("ЕnumVаluеNаmе", 5).WithLocation(3, 5),
                    Diagnostic().WithArguments("ЕnumVаluеNаmе", 8).WithLocation(3, 5),
                    Diagnostic().WithArguments("ЕnumVаluеNаmе", 10).WithLocation(3, 5),
                    Diagnostic().WithArguments("ЕnumVаluеNаmе", 12).WithLocation(3, 5),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"public struct StructName {}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructNameContainsNonLatinLettersAsync()
        {
            var testCode = @"public struct StruсtNаmе {}";

            var expected = new DiagnosticResult[]
                {
                    Diagnostic().WithArguments("StruсtNаmе", 4).WithLocation(1, 15),
                    Diagnostic().WithArguments("StruсtNаmе", 7).WithLocation(1, 15),
                    Diagnostic().WithArguments("StruсtNаmе", 9).WithLocation(1, 15),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"interface InterfaceName {}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceNameContainsNonLatinLettersAsync()
        {
            var testCode = @"interface IntеrfасеNаmе {}";

            var expected = new DiagnosticResult[]
                {
                    Diagnostic().WithArguments("IntеrfасеNаmе", 3).WithLocation(1, 11),
                    Diagnostic().WithArguments("IntеrfасеNаmе", 6).WithLocation(1, 11),
                    Diagnostic().WithArguments("IntеrfасеNаmе", 7).WithLocation(1, 11),
                    Diagnostic().WithArguments("IntеrfасеNаmе", 8).WithLocation(1, 11),
                    Diagnostic().WithArguments("IntеrfасеNаmе", 10).WithLocation(1, 11),
                    Diagnostic().WithArguments("IntеrfасеNаmе", 12).WithLocation(1, 11),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public int PropertyName { get; set; }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyNameContainsNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public int РrореrtуNаmе { get; set; }
}";

            var expected = new DiagnosticResult[]
                {
                    Diagnostic().WithArguments("РrореrtуNаmе", 0).WithLocation(3, 16),
                    Diagnostic().WithArguments("РrореrtуNаmе", 2).WithLocation(3, 16),
                    Diagnostic().WithArguments("РrореrtуNаmе", 3).WithLocation(3, 16),
                    Diagnostic().WithArguments("РrореrtуNаmе", 4).WithLocation(3, 16),
                    Diagnostic().WithArguments("РrореrtуNаmе", 7).WithLocation(3, 16),
                    Diagnostic().WithArguments("РrореrtуNаmе", 9).WithLocation(3, 16),
                    Diagnostic().WithArguments("РrореrtуNаmе", 11).WithLocation(3, 16),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public int FirstFieldName, SecondFieldName;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldNameContainsNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public int FirstFiеldNаmе, SесоndFiеldNаmе;
}";

            var expected = new DiagnosticResult[]
                {
                    Diagnostic().WithArguments("FirstFiеldNаmе", 7).WithLocation(3, 16),
                    Diagnostic().WithArguments("FirstFiеldNаmе", 11).WithLocation(3, 16),
                    Diagnostic().WithArguments("FirstFiеldNаmе", 13).WithLocation(3, 16),
                    Diagnostic().WithArguments("SесоndFiеldNаmе", 1).WithLocation(3, 32),
                    Diagnostic().WithArguments("SесоndFiеldNаmе", 2).WithLocation(3, 32),
                    Diagnostic().WithArguments("SесоndFiеldNаmе", 3).WithLocation(3, 32),
                    Diagnostic().WithArguments("SесоndFiеldNаmе", 8).WithLocation(3, 32),
                    Diagnostic().WithArguments("SесоndFiеldNаmе", 12).WithLocation(3, 32),
                    Diagnostic().WithArguments("SесоndFiеldNаmе", 14).WithLocation(3, 32),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public delegate void DelegateName();
    public event DelegateName EventName;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventNameContainsNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public delegate void DelegateName();
    public event DelegateName ЕvеntNаmе;
}";

            var expected = new DiagnosticResult[]
                {
                    Diagnostic().WithArguments("ЕvеntNаmе", 0).WithLocation(4, 31),
                    Diagnostic().WithArguments("ЕvеntNаmе", 2).WithLocation(4, 31),
                    Diagnostic().WithArguments("ЕvеntNаmе", 6).WithLocation(4, 31),
                    Diagnostic().WithArguments("ЕvеntNаmе", 8).WithLocation(4, 31),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public delegate void DelegateName();
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateNameContainsNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public delegate void DеlеgаtеNаmе();
}";

            var expected = new DiagnosticResult[]
                {
                    Diagnostic().WithArguments("DеlеgаtеNаmе", 1).WithLocation(3, 26),
                    Diagnostic().WithArguments("DеlеgаtеNаmе", 3).WithLocation(3, 26),
                    Diagnostic().WithArguments("DеlеgаtеNаmе", 5).WithLocation(3, 26),
                    Diagnostic().WithArguments("DеlеgаtеNаmе", 7).WithLocation(3, 26),
                    Diagnostic().WithArguments("DеlеgаtеNаmе", 9).WithLocation(3, 26),
                    Diagnostic().WithArguments("DеlеgаtеNаmе", 11).WithLocation(3, 26),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public void MethodName()
    {
        int variableName1, variableName2 = 0;

        for (var variableName3 = 0; variableName3 < 10; ++variableName3) {}
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableNameContainsNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public void MethodName()
    {
        int vаriаblеNаmе1, vаriаblеNаmе2 = 0;

        for (var vаriаblеNаmе3 = 0; vаriаblеNаmе3 < 10; ++vаriаblеNаmе3) {}
    }
}";

            var expected = new DiagnosticResult[]
                {
                    Diagnostic().WithArguments("vаriаblеNаmе1", 1).WithLocation(5, 13),
                    Diagnostic().WithArguments("vаriаblеNаmе1", 4).WithLocation(5, 13),
                    Diagnostic().WithArguments("vаriаblеNаmе1", 7).WithLocation(5, 13),
                    Diagnostic().WithArguments("vаriаblеNаmе1", 9).WithLocation(5, 13),
                    Diagnostic().WithArguments("vаriаblеNаmе1", 11).WithLocation(5, 13),
                    Diagnostic().WithArguments("vаriаblеNаmе2", 1).WithLocation(5, 28),
                    Diagnostic().WithArguments("vаriаblеNаmе2", 4).WithLocation(5, 28),
                    Diagnostic().WithArguments("vаriаblеNаmе2", 7).WithLocation(5, 28),
                    Diagnostic().WithArguments("vаriаblеNаmе2", 9).WithLocation(5, 28),
                    Diagnostic().WithArguments("vаriаblеNаmе2", 11).WithLocation(5, 28),
                    Diagnostic().WithArguments("vаriаblеNаmе3", 1).WithLocation(7, 18),
                    Diagnostic().WithArguments("vаriаblеNаmе3", 4).WithLocation(7, 18),
                    Diagnostic().WithArguments("vаriаblеNаmе3", 7).WithLocation(7, 18),
                    Diagnostic().WithArguments("vаriаblеNаmе3", 9).WithLocation(7, 18),
                    Diagnostic().WithArguments("vаriаblеNаmе3", 11).WithLocation(7, 18),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public void MethodName()
    {
        const int constName1 = 0;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantNameContainsNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public void MethodName()
    {
        const int соnstNаmе1 = 0;
    }
}";

            var expected = new DiagnosticResult[]
                  {
                    Diagnostic().WithArguments("соnstNаmе1", 0).WithLocation(5, 19),
                    Diagnostic().WithArguments("соnstNаmе1", 1).WithLocation(5, 19),
                    Diagnostic().WithArguments("соnstNаmе1", 6).WithLocation(5, 19),
                    Diagnostic().WithArguments("соnstNаmе1", 8).WithLocation(5, 19),
                  };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public void MethodName(int parameterName) {}
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterNameContainsNonLatinLettersAsync()
        {
            var testCode = @"public class ClassName
{
    public void MethodName(int раrаmеtеrNаmе) {}
}";

            var expected = new DiagnosticResult[]
                  {
                    Diagnostic().WithArguments("раrаmеtеrNаmе", 0).WithLocation(3, 32),
                    Diagnostic().WithArguments("раrаmеtеrNаmе", 1).WithLocation(3, 32),
                    Diagnostic().WithArguments("раrаmеtеrNаmе", 3).WithLocation(3, 32),
                    Diagnostic().WithArguments("раrаmеtеrNаmе", 5).WithLocation(3, 32),
                    Diagnostic().WithArguments("раrаmеtеrNаmе", 7).WithLocation(3, 32),
                    Diagnostic().WithArguments("раrаmеtеrNаmе", 10).WithLocation(3, 32),
                    Diagnostic().WithArguments("раrаmеtеrNаmе", 12).WithLocation(3, 32),
                  };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

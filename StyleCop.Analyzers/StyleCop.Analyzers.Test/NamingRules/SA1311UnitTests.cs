// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.NamingRules.SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter,
        Analyzers.NamingRules.RenameToUpperCaseCodeFixProvider>;

    public class SA1311UnitTests
    {
        [Fact]
        public async Task TestStaticReadonlyFieldStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    public static readonly string bar = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 35);

            var fixedCode = @"public class Foo
{
    public static readonly string Bar = ""baz"";
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticReadonlyFieldStartingWithLowerCaseWithConflictAsync()
        {
            var testCode = @"public class Foo
{
    public static readonly string bar = ""baz"";
    public int Bar => 0;
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 35);

            var fixedCode = @"public class Foo
{
    public static readonly string BarValue = ""baz"";
    public int Bar => 0;
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticReadonlyFieldStartingWithLoweCaseFieldIsJustOneLetterAsync()
        {
            var testCode = @"public class Foo
{
    internal static readonly string b = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 37);

            var fixedCode = @"public class Foo
{
    internal static readonly string B = ""baz"";
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticReadonlyFieldAssignmentInConstructorAsync()
        {
            var testCode = @"public class Foo
{
    public static readonly string bar;

    static Foo()
    {
        bar = ""aa"";
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 35);

            var fixedCode = @"public class Foo
{
    public static readonly string Bar;

    static Foo()
    {
        Bar = ""aa"";
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticReadonlyFieldStartingWithUpperCaseAsync()
        {
            var testCode = @"public class Foo
{
    public static readonly string Bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestReadonlyFieldStartingWithLoweCaseAsync()
        {
            var testCode = @"public class Foo
{
    public readonly string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticFieldStartingWithLoweCaseAsync()
        {
            var testCode = @"public class Foo
{
    public static string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassNameConflictAsync()
        {
            var testCode = @"public class Bar
{
    public static readonly string bar;

    static Bar()
    {
        bar = ""aa"";
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 35);

            var fixedCode = @"public class Bar
{
    public static readonly string BarValue;

    static Bar()
    {
        BarValue = ""aa"";
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMemberNameConflictAsync()
        {
            var testCode = @"public class Foo
{
    public static readonly string bar;

    static Foo()
    {
        bar = ""aa"";
    }

    public static readonly string Bar;
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 35);

            var fixedCode = @"public class Foo
{
    public static readonly string BarValue;

    static Foo()
    {
        BarValue = ""aa"";
    }

    public static readonly string Bar;
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

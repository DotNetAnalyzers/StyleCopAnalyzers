// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1304NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToUpperCaseCodeFixProvider>;

    public class SA1304UnitTests
    {
        [Fact]
        public async Task TestPublicReadonlyFieldStartingWithLowerCaseAsync()
        {
            // Should be reported by SA1307 instead
            var testCode = @"public class Foo
{
    public readonly string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPublicReadonlyFieldStartingWithUpperCaseAsync()
        {
            var testCode = @"public class Foo
{
    public readonly string Bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestProtectedReadonlyFieldStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    protected readonly string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestProtectedReadonlyFieldStartingWithUpperCaseAsync()
        {
            // Should be reported by SA1307 instead
            var testCode = @"public class Foo
{
    protected readonly string Bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInternalReadonlyFieldStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    internal readonly string bar = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 30);

            var fixedCode = @"public class Foo
{
    internal readonly string Bar = ""baz"";
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInternalReadonlyFieldStartingWithLowerCaseWithConflictAsync()
        {
            var testCode = @"public class Foo
{
    internal readonly string bar = ""baz"";
    public string Bar => this.bar;
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 30);

            var fixedCode = @"public class Foo
{
    internal readonly string BarValue = ""baz"";
    public string Bar => this.BarValue;
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInternalReadonlyFieldStartingWithLowerCaseWithTwoConflictsAsync()
        {
            var testCode = @"public class Foo
{
    internal readonly string bar = ""baz"";
    public string Bar => this.bar;
    public string BarValue => this.bar;
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 30);

            var fixedCode = @"public class Foo
{
    internal readonly string Bar1 = ""baz"";
    public string Bar => this.Bar1;
    public string BarValue => this.Bar1;
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInternalReadonlyFieldStartingWithUpperCaseAsync()
        {
            var testCode = @"public class Foo
{
    internal readonly string Bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestlWithNoAccessibilityKeywordReadonlyFieldStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    readonly string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestlPublicFieldStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPrivateReadonlyFieldStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    private readonly string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

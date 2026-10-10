// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.NamingRules;
    using StyleCop.Analyzers.Test.Helpers;
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
    protected readonly string {|#0:bar|} = ""baz"";
}";

            var fixedCode = @"public class Foo
{
    protected readonly string Bar = ""baz"";
}";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestProtectedReadonlyFieldStartingWithUpperCaseAsync()
        {
            var testCode = @"public class Foo
{
    protected readonly string Bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldInNativeMethodsClassAsync()
        {
            var testCode = @"public class FooNativeMethods
{
    internal readonly string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task TestInternalReadonlyFieldStartingWithLowerCaseAsync(string lineEnding)
        {
            var testCode = @"public class Foo
{
    internal readonly string {|#0:bar|} = ""baz"";
}".ReplaceLineEndings(lineEnding);

            DiagnosticResult expected = Diagnostic().WithLocation(0);

            var fixedCode = @"public class Foo
{
    internal readonly string Bar = ""baz"";
}".ReplaceLineEndings(lineEnding);
            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                DisabledDiagnostics = { SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.DiagnosticId },
            };
            test.ExpectedDiagnostics.Add(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
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
            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                DisabledDiagnostics = { SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.DiagnosticId },
            };
            test.ExpectedDiagnostics.Add(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
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
            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                DisabledDiagnostics = { SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.DiagnosticId },
            };
            test.ExpectedDiagnostics.Add(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

        /// <summary>
        /// Verifies that protected readonly fields are checked and accessible fields are reported exactly once.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        [WorkItem(3557, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3557")]
        public async Task TestReadonlyFieldsWithSA1307EnabledAsync()
        {
            var testCode = @"namespace Some.Space
{
    public abstract class SomeClass
    {
        protected readonly string {|#0:foo|};
        internal readonly string {|#1:bar|};
        public readonly int {|#2:a1|};
        public int {|#3:a2|};
        protected internal readonly int {|#4:c1|};
        protected internal int {|#5:c2|};
        internal int {|#6:b2|};
        protected int d2;
        private readonly int f1;
        private int f2;
        readonly int g1;
        protected readonly int Upper, _underscore;
    }
}";
            var fixedCode = @"namespace Some.Space
{
    public abstract class SomeClass
    {
        protected readonly string Foo;
        internal readonly string Bar;
        public readonly int A1;
        public int A2;
        protected internal readonly int C1;
        protected internal int C2;
        internal int B2;
        protected int d2;
        private readonly int f1;
        private int f2;
        readonly int g1;
        protected readonly int Upper, _underscore;
    }
}";
            var test = new CombinedCSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
            };
            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
            test.ExpectedDiagnostics.Add(new DiagnosticResult(SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.Descriptor).WithLocation(1).WithArguments("bar"));
            test.ExpectedDiagnostics.Add(new DiagnosticResult(SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.Descriptor).WithLocation(2).WithArguments("a1"));
            test.ExpectedDiagnostics.Add(new DiagnosticResult(SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.Descriptor).WithLocation(3).WithArguments("a2"));
            test.ExpectedDiagnostics.Add(new DiagnosticResult(SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.Descriptor).WithLocation(4).WithArguments("c1"));
            test.ExpectedDiagnostics.Add(new DiagnosticResult(SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.Descriptor).WithLocation(5).WithArguments("c2"));
            test.ExpectedDiagnostics.Add(new DiagnosticResult(SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.Descriptor).WithLocation(6).WithArguments("b2"));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that SA1304 covers every non-private readonly accessibility when SA1307 is disabled.
        /// </summary>
        /// <param name="accessibility">The accessibility modifiers of the field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Theory]
        [InlineData("public")]
        [InlineData("internal")]
        [InlineData("protected internal")]
        [InlineData("protected")]
        [WorkItem(3557, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3557")]
        public async Task TestReadonlyFieldsWithSA1307DisabledAsync(string accessibility)
        {
            var test = new CombinedCSharpTest
            {
                TestCode = "public class Foo { " + accessibility + " readonly int {|#0:bar|}, Upper; }",
                FixedCode = "public class Foo { " + accessibility + " readonly int Bar, Upper; }",
                DisabledDiagnostics = { SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.DiagnosticId },
            };
            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        internal class CombinedCSharpTest : CSharpTest
        {
            public CombinedCSharpTest(LanguageVersion? languageVersion = null)
                : base(languageVersion)
            {
            }

            protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
            {
                return new DiagnosticAnalyzer[]
                {
                    new SA1304NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter(),
                    new SA1307AccessibleFieldsMustBeginWithUpperCaseLetter(),
                };
            }
        }
    }
}

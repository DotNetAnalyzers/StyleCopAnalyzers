// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;

    public abstract class FileMayOnlyContainTestBase<TAnalyzer, TCodeFix>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        public abstract string Keyword { get; }

        public abstract bool SupportsCodeFix { get; }

        [Fact]
        public async Task TestOneElementAsync()
        {
            var testCode = @"%1 Foo
{
}";

            testCode = testCode.Replace("%1", this.Keyword);

            await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsAsync()
        {
            var testCode = @"%1 Foo
{
}
%1 Bar
{
}";

            var fixedCode = new[]
            {
                ("Test0.cs", @"%1 Foo
{
}
"),
                ("Test1.cs", @"%1 Bar
{
}"),
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(c => (c.Item1, c.Item2.Replace("%1", this.Keyword))).ToArray();

            DiagnosticResult expected = Diagnostic().WithLocation(4, this.Keyword.Length + 2);

            if (this.SupportsCodeFix)
            {
                await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), expected, CancellationToken.None).ConfigureAwait(false);
                foreach (var (_, code) in fixedCode)
                {
                    await VerifyCSharpDiagnosticAsync(code, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                }
            }
        }

        [Fact]
        public async Task TestThreeElementsAsync()
        {
            var testCode = @"%1 Foo
{
}
%1 Bar
{
}
%1 FooBar
{
}";

            var fixedCode = new[]
            {
                ("Test0.cs", @"%1 Foo
{
}
"),
                ("Test1.cs", @"%1 Bar
{
}
"),
                ("Test2.cs", @"%1 FooBar
{
}"),
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(code => (code.Item1, code.Item2.Replace("%1", this.Keyword))).ToArray();

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(4, this.Keyword.Length + 2),
                Diagnostic().WithLocation(7, this.Keyword.Length + 2),
            };

            if (this.SupportsCodeFix)
            {
                await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), expected, CancellationToken.None).ConfigureAwait(false);
                foreach (var (_, code) in fixedCode)
                {
                    await VerifyCSharpDiagnosticAsync(code, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                }
            }
        }

        [Fact]
        public async Task TestRemoveWarningSuppressionAsync()
        {
            var testCode = @"%1 Foo
{
}
%1 Bar
{
#pragma warning disable SomeWarning
#pragma warning restore SomeWarning
}";

            var fixedCode = new[]
            {
                ("Test0.cs", @"%1 Foo
{
}
"),
                ("Test1.cs", @"%1 Bar
{
#pragma warning disable SomeWarning
#pragma warning restore SomeWarning
}"),
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(code => (code.Item1, code.Item2.Replace("%1", this.Keyword))).ToArray();

            DiagnosticResult expected = Diagnostic().WithLocation(4, this.Keyword.Length + 2);

            if (this.SupportsCodeFix)
            {
                await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), expected, CancellationToken.None).ConfigureAwait(false);
                foreach (var (_, code) in fixedCode)
                {
                    await VerifyCSharpDiagnosticAsync(code, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                }
            }
        }

        [Fact]
        public async Task TestPreserveWarningSuppressionAsync()
        {
            var testCode = @"%1 Foo
{
}
%1 Bar
{
#pragma warning disable SomeWarning
}";

            // See https://github.com/dotnet/roslyn/issues/3999
            var fixedCode = new[]
            {
                ("Test0.cs", @"%1 Foo
{
}
"),
                ("Test1.cs", @"%1 Bar
{
#pragma warning disable SomeWarning
}"),
            };

            DiagnosticResult expected = Diagnostic().WithLocation(4, this.Keyword.Length + 2);

            if (this.SupportsCodeFix)
            {
                await VerifyCSharpFixAsync(testCode.Replace("%1", this.Keyword), this.GetSettings(), expected, fixedCode.Select(c => (c.Item1, c.Item2.Replace("%1", this.Keyword))).ToArray(), CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), this.GetSettings(), expected, CancellationToken.None).ConfigureAwait(false);

                foreach (var (_, code) in fixedCode)
                {
                    await VerifyCSharpDiagnosticAsync(code.Replace("%1", this.Keyword), this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                }
            }
        }

        [Fact]
        public async Task TestRemovePreprocessorDirectivesAsync()
        {
            var testCode = @"%1 Foo
{
}
%1 Bar
{
#if true
#endif
}";

            var fixedCode = new[]
            {
                ("Test0.cs", @"%1 Foo
{
}
"),
                ("Test1.cs", @"%1 Bar
{
#if true
#endif
}"),
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(code => (code.Item1, code.Item2.Replace("%1", this.Keyword))).ToArray();

            DiagnosticResult expected = Diagnostic().WithLocation(4, this.Keyword.Length + 2);

            if (this.SupportsCodeFix)
            {
                await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), expected, CancellationToken.None).ConfigureAwait(false);
                foreach (var (_, code) in fixedCode)
                {
                    await VerifyCSharpDiagnosticAsync(code, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                }
            }
        }

        [Fact]
        public async Task TestPreservePreprocessorDirectivesAsync()
        {
            var testCode = @"%1 Foo
{
#if true
}
%1 Bar
{
#endif
}";

            // See https://github.com/dotnet/roslyn/issues/3999
            var fixedCode = new[]
            {
                ("Test0.cs", @"%1 Foo
{
#if true
}

#endif
"),
                ("Test0.cs", @"
#if true
%1 Bar
{
#endif
}"),
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(code => (code.Item1, code.Item2.Replace("%1", this.Keyword))).ToArray();

            DiagnosticResult expected = Diagnostic().WithLocation(5, this.Keyword.Length + 2);

            if (this.SupportsCodeFix)
            {
                await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), expected, CancellationToken.None).ConfigureAwait(false);
                foreach (var (_, code) in fixedCode)
                {
                    await VerifyCSharpDiagnosticAsync(code, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                }
            }
        }

        protected static DiagnosticResult Diagnostic()
            => StyleCopDiagnosticVerifier<TAnalyzer>.Diagnostic();

        protected static Task VerifyCSharpDiagnosticAsync(string source, string testSettings, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, testSettings, new[] { expected }, cancellationToken);

        protected static Task VerifyCSharpDiagnosticAsync(string source, string testSettings, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<TAnalyzer, TCodeFix>.CSharpTest
            {
                TestCode = source,
                Settings = testSettings,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        protected static Task VerifyCSharpFixAsync(string source, string testSettings, DiagnosticResult expected, (string fileName, string content)[] fixedSources, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, testSettings, new[] { expected }, fixedSources, cancellationToken);

        protected static Task VerifyCSharpFixAsync(string source, string testSettings, DiagnosticResult[] expected, (string fileName, string content)[] fixedSources, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<TAnalyzer, TCodeFix>.CSharpTest
            {
                TestCode = source,
                Settings = testSettings,
            };

            foreach (var fixedSource in fixedSources)
            {
                test.FixedSources.Add(fixedSource);
            }

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        protected virtual string GetSettings() => null;
    }
}

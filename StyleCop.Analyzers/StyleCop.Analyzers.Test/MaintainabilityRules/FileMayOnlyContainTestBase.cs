// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;

    public abstract class FileMayOnlyContainTestBase
    {
        public abstract string Keyword { get; }

        public abstract bool SupportsCodeFix { get; }

        protected abstract DiagnosticAnalyzer Analyzer { get; }

        protected abstract CodeFixProvider CodeFix { get; }

        [Fact]
        public async Task TestOneElementAsync()
        {
            var testCode = @"%1 Foo
{
}";

            testCode = testCode.Replace("%1", this.Keyword);

            await this.VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                ("Bar.cs", @"%1 Bar
{
}"),
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(c => (c.Item1, c.Item2.Replace("%1", this.Keyword))).ToArray();

            DiagnosticResult expected = this.Diagnostic().WithLocation(4, this.Keyword.Length + 2);

            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), expected, CancellationToken.None).ConfigureAwait(false);
                foreach (var (_, code) in fixedCode)
                {
                    await this.VerifyCSharpDiagnosticAsync(code, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                ("Bar.cs", @"%1 Bar
{
}
"),
                ("FooBar.cs", @"%1 FooBar
{
}"),
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(code => (code.Item1, code.Item2.Replace("%1", this.Keyword))).ToArray();

            DiagnosticResult[] expected =
            {
                this.Diagnostic().WithLocation(4, this.Keyword.Length + 2),
                this.Diagnostic().WithLocation(7, this.Keyword.Length + 2),
            };

            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), expected, CancellationToken.None).ConfigureAwait(false);
                foreach (var (_, code) in fixedCode)
                {
                    await this.VerifyCSharpDiagnosticAsync(code, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                ("Bar.cs", @"%1 Bar
{
#pragma warning disable SomeWarning
#pragma warning restore SomeWarning
}"),
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(code => (code.Item1, code.Item2.Replace("%1", this.Keyword))).ToArray();

            DiagnosticResult expected = this.Diagnostic().WithLocation(4, this.Keyword.Length + 2);

            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), expected, CancellationToken.None).ConfigureAwait(false);
                foreach (var (_, code) in fixedCode)
                {
                    await this.VerifyCSharpDiagnosticAsync(code, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                ("Bar.cs", @"%1 Bar
{
#pragma warning disable SomeWarning
}"),
            };

            DiagnosticResult expected = this.Diagnostic().WithLocation(4, this.Keyword.Length + 2);

            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(testCode.Replace("%1", this.Keyword), this.GetSettings(), expected, fixedCode.Select(c => (c.Item1, c.Item2.Replace("%1", this.Keyword))).ToArray(), CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), this.GetSettings(), expected, CancellationToken.None).ConfigureAwait(false);

                foreach (var (_, code) in fixedCode)
                {
                    await this.VerifyCSharpDiagnosticAsync(code.Replace("%1", this.Keyword), this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                ("Bar.cs", @"%1 Bar
{
#if true
#endif
}"),
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(code => (code.Item1, code.Item2.Replace("%1", this.Keyword))).ToArray();

            DiagnosticResult expected = this.Diagnostic().WithLocation(4, this.Keyword.Length + 2);

            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), expected, CancellationToken.None).ConfigureAwait(false);
                foreach (var (_, code) in fixedCode)
                {
                    await this.VerifyCSharpDiagnosticAsync(code, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                ("Bar.cs", @"
#if true
%1 Bar
{
#endif
}"),
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(code => (code.Item1, code.Item2.Replace("%1", this.Keyword))).ToArray();

            DiagnosticResult expected = this.Diagnostic().WithLocation(5, this.Keyword.Length + 2);

            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), expected, CancellationToken.None).ConfigureAwait(false);
                foreach (var (_, code) in fixedCode)
                {
                    await this.VerifyCSharpDiagnosticAsync(code, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                }
            }
        }

        protected DiagnosticResult Diagnostic()
            => new DiagnosticResult(this.Analyzer.SupportedDiagnostics.Single());

        protected Task VerifyCSharpDiagnosticAsync(string source, string testSettings, DiagnosticResult expected, CancellationToken cancellationToken)
            => this.VerifyCSharpDiagnosticAsync(source, testSettings, new[] { expected }, cancellationToken);

        protected Task VerifyCSharpDiagnosticAsync(string source, string testSettings, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new CSharpTest(this)
            {
                TestCode = source,
                Settings = testSettings,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        protected Task VerifyCSharpFixAsync(string source, string testSettings, DiagnosticResult expected, (string fileName, string content)[] fixedSources, CancellationToken cancellationToken)
            => this.VerifyCSharpFixAsync(source, testSettings, new[] { expected }, fixedSources, cancellationToken);

        protected Task VerifyCSharpFixAsync(string source, string testSettings, DiagnosticResult[] expected, (string fileName, string content)[] fixedSources, CancellationToken cancellationToken)
        {
            var test = new CSharpTest(this)
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

        private class CSharpTest : StyleCopCodeFixVerifier<EmptyAnalyzer, EmptyCodeFixProvider>.CSharpTest
        {
            private readonly FileMayOnlyContainTestBase testFixture;

            public CSharpTest(FileMayOnlyContainTestBase testFixture)
            {
                this.testFixture = testFixture;
            }

            protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
            {
                yield return this.testFixture.Analyzer;
            }

            protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
            {
                yield return this.testFixture.CodeFix;
            }
        }
    }
}

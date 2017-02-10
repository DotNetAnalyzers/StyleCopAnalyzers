// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TestHelper;
    using Xunit;

    public abstract class FileMayOnlyContainTestBase : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                @"%1 Foo
{
}
",
                @"%1 Bar
{
}",
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(c => c.Replace("%1", this.Keyword)).ToArray();

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, this.Keyword.Length + 2);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                @"%1 Foo
{
}
",
                @"%1 Bar
{
}
",
                @"%1 FooBar
{
}",
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(code => code.Replace("%1", this.Keyword)).ToArray();

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, this.Keyword.Length + 2),
                this.CSharpDiagnostic().WithLocation(7, this.Keyword.Length + 2),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                @"%1 Foo
{
}
",
                @"%1 Bar
{
#pragma warning disable SomeWarning
#pragma warning restore SomeWarning
}",
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(code => code.Replace("%1", this.Keyword)).ToArray();

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, this.Keyword.Length + 2);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                @"%1 Foo
{
}
",
                @"%1 Bar
{
#pragma warning disable SomeWarning
}",
            };

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, this.Keyword.Length + 2);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), expected, CancellationToken.None).ConfigureAwait(false);

            foreach (var code in fixedCode)
            {
                await this.VerifyCSharpDiagnosticAsync(code.Replace("%1", this.Keyword), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }

            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(new[] { testCode.Replace("%1", this.Keyword) }, fixedCode.Select(c => c.Replace("%1", this.Keyword)).ToArray(), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                @"%1 Foo
{
}
",
                @"%1 Bar
{
#if true
#endif
}",
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(code => code.Replace("%1", this.Keyword)).ToArray();

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, this.Keyword.Length + 2);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                @"%1 Foo
{
#if true
}

#endif
",
                @"
#if true
%1 Bar
{
#endif
}",
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(code => code.Replace("%1", this.Keyword)).ToArray();

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, this.Keyword.Length + 2);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
            }
        }
    }
}

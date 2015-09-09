// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using TestHelper;
    using Xunit;

    public abstract class FileMayOnlyContainTestBase : CodeFixVerifier
    {
        public abstract string Keyword { get; }

        public virtual bool SupportsCodeFix => false;

        [Fact]
        public async Task TestOneElementAsync()
        {
            var testCode = @"%1 Foo
{
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            var fixedCode = @"%1 Foo
{
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, this.Keyword.Length + 2);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("%1", this.Keyword), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(testCode.Replace("%1", this.Keyword), fixedCode.Replace("%1", this.Keyword), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            var fixedCode = @"%1 Foo
{
}
";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(4, this.Keyword.Length + 2),
                    this.CSharpDiagnostic().WithLocation(7, this.Keyword.Length + 2)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("%1", this.Keyword), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(testCode.Replace("%1", this.Keyword), fixedCode.Replace("%1", this.Keyword), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            var fixedCode = @"%1 Foo
{
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, this.Keyword.Length + 2);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("%1", this.Keyword), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(testCode.Replace("%1", this.Keyword), fixedCode.Replace("%1", this.Keyword), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            var fixedCode = @"%1 Foo
{
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, this.Keyword.Length + 2);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("%1", this.Keyword), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(testCode.Replace("%1", this.Keyword), fixedCode.Replace("%1", this.Keyword), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            var fixedCode = @"%1 Foo
{
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, this.Keyword.Length + 2);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("%1", this.Keyword), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(testCode.Replace("%1", this.Keyword), fixedCode.Replace("%1", this.Keyword), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            var fixedCode = @"%1 Foo
{
#if true
}

#endif
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, this.Keyword.Length + 2);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("%1", this.Keyword), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            if (this.SupportsCodeFix)
            {
                await this.VerifyCSharpFixAsync(testCode.Replace("%1", this.Keyword), fixedCode.Replace("%1", this.Keyword), cancellationToken: CancellationToken.None).ConfigureAwait(false);
            }
        }
    }
}

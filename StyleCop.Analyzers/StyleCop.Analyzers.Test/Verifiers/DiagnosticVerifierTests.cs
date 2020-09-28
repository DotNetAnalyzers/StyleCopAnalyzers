// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;
    using Xunit.Sdk;
    using static StyleCopDiagnosticVerifier<StyleCop.Analyzers.SpacingRules.SA1002SemicolonsMustBeSpacedCorrectly>;

    /// <summary>
    /// This class verifies that <see cref="CSharpCodeFixVerifier{TAnalyzer, TCodeFix, TVerifier}"/> will correctly report failing tests.
    /// </summary>
    public class DiagnosticVerifierTests
    {
        [Fact]
        public async Task TestExpectedDiagnosticMissingAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        ;
    }
}
";

            var expected = Diagnostic();
            var ex = await Assert.ThrowsAnyAsync<XunitException>(
                async () =>
                {
                    await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                }).ConfigureAwait(false);
            Assert.StartsWith($"Context: Diagnostics of test state{Environment.NewLine}Mismatch between number of diagnostics returned, expected \"1\" actual \"0\"", ex.Message);
        }

        [Fact]
        public async Task TestValidBehaviorAsync()
        {
            string testCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property;}
    }
}
";

            DiagnosticResult expected = Diagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 33);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestValidBehaviorWithFullSpanAsync()
        {
            string testCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property;}
    }
}
";

            DiagnosticResult expected = Diagnostic().WithArguments(string.Empty, "followed").WithSpan(7, 33, 7, 34);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnexpectedLocationForProjectDiagnosticAsync()
        {
            string testCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property;}
    }
}
";

            // By failing to include a location, the verified thinks we're only trying to verify a project diagnostic.
            DiagnosticResult expected = Diagnostic().WithArguments(string.Empty, "followed");

            var ex = await Assert.ThrowsAnyAsync<XunitException>(
                async () =>
                {
                    await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                }).ConfigureAwait(false);

            var expectedMessage = $"Context: Diagnostics of test state{Environment.NewLine}"
                + $"Expected a project diagnostic with no location:{Environment.NewLine}"
                + $"{Environment.NewLine}"
                + $"Expected diagnostic:{Environment.NewLine}"
                + $"    // warning SA1002: Semicolons should be followed by a space{Environment.NewLine}"
                + $"new DiagnosticResult(SA1002SemicolonsMustBeSpacedCorrectly.SA1002).WithArguments(\"\", \"followed\"),{Environment.NewLine}"
                + $"{Environment.NewLine}"
                + $"Actual diagnostic:{Environment.NewLine}"
                + $"    // /0/Test0.cs(7,33): warning SA1002: Semicolons should be followed by a space{Environment.NewLine}"
                + $"VerifyCS.Diagnostic().WithSpan(7, 33, 7, 34).WithArguments(\"\", \"followed\"),{Environment.NewLine}"
                + $"{Environment.NewLine}"
                + $"{Environment.NewLine}"
                + $"Assert.Equal() Failure{Environment.NewLine}"
                + $"Expected: None{Environment.NewLine}"
                + $"Actual:   SourceFile(/0/Test0.cs[102..103))";

            new XUnitVerifier().EqualOrDiff(expectedMessage, ex.Message);
        }

        [Fact]
        public async Task TestUnexpectedMessageAsync()
        {
            string testCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property;}
    }
}
";

            var ex = await Assert.ThrowsAnyAsync<XunitException>(
                async () =>
                {
                    await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                }).ConfigureAwait(false);
            Assert.StartsWith($"Context: Diagnostics of test state{Environment.NewLine}Mismatch between number of diagnostics returned, expected \"0\" actual \"1\"", ex.Message);
            Assert.Contains("warning SA1002", ex.Message);
        }

        [Fact]
        public async Task TestUnexpectedAnalyzerErrorAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        ;
    }
}
";

            var ex = await Assert.ThrowsAnyAsync<XunitException>(
                async () =>
                {
                    await CSharpCodeFixVerifier<ErrorThrowingAnalyzer, EmptyCodeFixProvider, XUnitVerifier>.VerifyAnalyzerAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
                }).ConfigureAwait(false);
            Assert.StartsWith($"Context: Diagnostics of test state{Environment.NewLine}Mismatch between number of diagnostics returned, expected \"0\" actual \"1\"", ex.Message);
            Assert.Contains("error AD0001", ex.Message);
        }

        [Fact]
        public async Task TestUnexpectedCompilerErrorAsync()
        {
            string testCode = @"
class ClassName
{
    int property;
    Int32 PropertyName
    {
        get{return this.property;}
    }
}
";

            DiagnosticResult expected = Diagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 33);

            var ex = await Assert.ThrowsAnyAsync<XunitException>(
                async () =>
                {
                    await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                }).ConfigureAwait(false);
            Assert.StartsWith($"Context: Diagnostics of test state{Environment.NewLine}Mismatch between number of diagnostics returned, expected \"1\" actual \"2\"", ex.Message);
            Assert.Contains("error CS0246", ex.Message);
        }

        [Fact]
        public async Task TestUnexpectedCompilerWarningAsync()
        {
            string testCode = @"
class ClassName
{
    int property;
    Int32 PropertyName
    {
        ///
        get{return this.property;}
    }
}
";

            DiagnosticResult expected = Diagnostic().WithArguments(string.Empty, "followed").WithLocation(8, 33);

            var ex = await Assert.ThrowsAnyAsync<XunitException>(
                async () =>
                {
                    await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                }).ConfigureAwait(false);
            Assert.StartsWith($"Context: Diagnostics of test state{Environment.NewLine}Mismatch between number of diagnostics returned, expected \"1\" actual \"2\"", ex.Message);
            Assert.Contains("error CS0246", ex.Message);
        }

        [Fact]
        public async Task TestInvalidDiagnosticIdAsync()
        {
            string testCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property;}
    }
}
";

            var descriptor = new DiagnosticDescriptor("SA9999", "Title", "Message", "Category", DiagnosticSeverity.Warning, isEnabledByDefault: true);
            DiagnosticResult expected = Diagnostic(descriptor).WithLocation(7, 33);

            var ex = await Assert.ThrowsAnyAsync<XunitException>(
                async () =>
                {
                    await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                }).ConfigureAwait(false);
            Assert.StartsWith($"Context: Diagnostics of test state{Environment.NewLine}Expected diagnostic id to be \"SA9999\" was \"SA1002\"", ex.Message);
        }

        [Fact]
        public async Task TestInvalidSeverityAsync()
        {
            string testCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property;}
    }
}
";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 33).WithSeverity(DiagnosticSeverity.Error);

            var ex = await Assert.ThrowsAnyAsync<XunitException>(
                async () =>
                {
                    await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                }).ConfigureAwait(false);
            Assert.StartsWith($"Context: Diagnostics of test state{Environment.NewLine}Expected diagnostic severity to be \"Error\" was \"Warning\"", ex.Message);
        }

        [Fact]
        public async Task TestIncorrectLocationLine1Async()
        {
            string testCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property;}
    }
}
";

            DiagnosticResult expected = Diagnostic().WithArguments(string.Empty, "followed").WithLocation(8, 33);

            var ex = await Assert.ThrowsAnyAsync<XunitException>(
                async () =>
                {
                    await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                }).ConfigureAwait(false);
            Assert.StartsWith($"Context: Diagnostics of test state{Environment.NewLine}Expected diagnostic to start on line \"8\" was actually on line \"7\"", ex.Message);
        }

        [Fact]
        public async Task TestIncorrectLocationLine2Async()
        {
            string testCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property;}
        set{this.property = value;}
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 33),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 34),
            };

            var ex = await Assert.ThrowsAnyAsync<XunitException>(
                async () =>
                {
                    await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                }).ConfigureAwait(false);
            Assert.StartsWith($"Context: Diagnostics of test state{Environment.NewLine}Expected diagnostic to start on line \"7\" was actually on line \"8\"", ex.Message);
        }

        [Fact]
        public async Task TestIncorrectLocationColumnAsync()
        {
            string testCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property;}
    }
}
";

            DiagnosticResult expected = Diagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 34);

            var ex = await Assert.ThrowsAnyAsync<XunitException>(
                async () =>
                {
                    await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                }).ConfigureAwait(false);
            Assert.StartsWith($"Context: Diagnostics of test state{Environment.NewLine}Expected diagnostic to start at column \"34\" was actually at column \"33\"", ex.Message);
        }

        [Fact]
        public async Task TestIncorrectLocationEndColumnAsync()
        {
            string testCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property;}
    }
}
";

            DiagnosticResult expected = Diagnostic().WithArguments(string.Empty, "followed").WithSpan(7, 33, 7, 35);

            var ex = await Assert.ThrowsAnyAsync<XunitException>(
                async () =>
                {
                    await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                }).ConfigureAwait(false);
            Assert.StartsWith($"Context: Diagnostics of test state{Environment.NewLine}Expected diagnostic to end at column \"35\" was actually at column \"34\"", ex.Message);
        }

        [Fact]
        public async Task TestIncorrectMessageAsync()
        {
            string testCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property;}
    }
}
";

            DiagnosticResult expected = Diagnostic().WithArguments(string.Empty, "bogus argument").WithLocation(7, 33);

            var ex = await Assert.ThrowsAnyAsync<XunitException>(
                async () =>
                {
                    await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                }).ConfigureAwait(false);
            Assert.StartsWith($"Context: Diagnostics of test state{Environment.NewLine}Expected diagnostic message to be ", ex.Message);
        }

        [Fact]
        public async Task TestIncorrectAdditionalLocationAsync()
        {
            string testCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property;}
    }
}
";

            DiagnosticResult expected = Diagnostic().WithArguments(string.Empty, "bogus argument").WithLocation(7, 33).WithLocation(8, 34);

            var ex = await Assert.ThrowsAnyAsync<XunitException>(
                async () =>
                {
                    await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                }).ConfigureAwait(false);
            Assert.StartsWith($"Context: Diagnostics of test state{Environment.NewLine}Expected 1 additional locations but got 0 for Diagnostic", ex.Message);
        }

        private class ErrorThrowingAnalyzer : SA1002SemicolonsMustBeSpacedCorrectly
        {
            private static readonly Action<SyntaxNodeAnalysisContext> BlockAction = HandleBlock;

            /// <inheritdoc/>
            public override void Initialize(AnalysisContext context)
            {
                context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
                context.EnableConcurrentExecution();

                context.RegisterSyntaxNodeAction(BlockAction, SyntaxKind.Block);
            }

            private static void HandleBlock(SyntaxNodeAnalysisContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}

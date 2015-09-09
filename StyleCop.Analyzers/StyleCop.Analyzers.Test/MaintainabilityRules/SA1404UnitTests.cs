// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1404UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestSuppressionWithStringLiteralAsync()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = ""a justification"")]
    public void Bar()
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSuppressionWithStringLiteralAndUsingAliasDirectiveAsync()
        {
            var testCode = @"using SuppressMessageAttribute = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;
public class Foo
{
    [SuppressMessage(null, null, Justification = ""a justification"")]
    public void Bar()
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSuppressionWithPlaceholderValueAsync()
        {
            var testCode = @"using SuppressMessageAttribute = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;
public class Foo
{
    [SuppressMessage(null, null, Justification = """ + SA1404CodeAnalysisSuppressionMustHaveJustification.JustificationPlaceholder + @""")]
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 34);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSuppressionWithNoJustificationAsync()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null)]
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 6);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = """ + SA1404CodeAnalysisSuppressionMustHaveJustification.JustificationPlaceholder + @""")]
    public void Bar()
    {

    }
}";

            expected = this.CSharpDiagnostic().WithLocation(3, 66);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSuppressionWithUsingAliasDirectiveAndNoJustificationAsync()
        {
            var testCode = @"using SuppressMessageAttribute = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;
public class Foo
{
    [SuppressMessage(null, null)]
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 6);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"using SuppressMessageAttribute = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;
public class Foo
{
    [SuppressMessage(null, null, Justification = """ + SA1404CodeAnalysisSuppressionMustHaveJustification.JustificationPlaceholder + @""")]
    public void Bar()
    {

    }
}";

            expected = this.CSharpDiagnostic().WithLocation(4, 34);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSuppressionWithUsingDifferentAliasDirectiveAndNoJustificationAsync()
        {
            var testCode = @"using MySuppressionAttribute = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;
public class Foo
{
    [MySuppression(null, null)]
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 6);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"using MySuppressionAttribute = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;
public class Foo
{
    [MySuppression(null, null, Justification = """ + SA1404CodeAnalysisSuppressionMustHaveJustification.JustificationPlaceholder + @""")]
    public void Bar()
    {

    }
}";

            expected = this.CSharpDiagnostic().WithLocation(4, 32);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSuppressionWithEmptyJustificationAsync()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = """")]
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 66);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = """ + SA1404CodeAnalysisSuppressionMustHaveJustification.JustificationPlaceholder + @""")]
    public void Bar()
    {

    }
}";

            expected = this.CSharpDiagnostic().WithLocation(3, 66);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSuppressionWithEscapedIdentifierWithJustificationAsync()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justific\u0061tion = """")]
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 66);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justific\u0061tion = """ + SA1404CodeAnalysisSuppressionMustHaveJustification.JustificationPlaceholder + @""")]
    public void Bar()
    {

    }
}";

            expected = this.CSharpDiagnostic().WithLocation(3, 66);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSuppressionWithWhitespaceJustificationAsync()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = ""    "")]
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 66);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = """ + SA1404CodeAnalysisSuppressionMustHaveJustification.JustificationPlaceholder + @""")]
    public void Bar()
    {

    }
}";

            expected = this.CSharpDiagnostic().WithLocation(3, 66);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSuppressionWithNullJustificationAsync()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = null)]
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 66);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = """ + SA1404CodeAnalysisSuppressionMustHaveJustification.JustificationPlaceholder + @""")]
    public void Bar()
    {

    }
}";

            expected = this.CSharpDiagnostic().WithLocation(3, 66);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSuppressionWithComplexJustificationAsync()
        {
            var testCode = @"public class Foo
{
    const string JUSTIFICATION = ""Foo"";
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = """" + JUSTIFICATION)]
    public void Bar()
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSuppressionWithComplexWhitespaceJustificationAsync()
        {
            var testCode = @"public class Foo
{
    const string JUSTIFICATION = ""    "";
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = """" + JUSTIFICATION)]
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 66);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    const string JUSTIFICATION = ""    "";
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = """ + SA1404CodeAnalysisSuppressionMustHaveJustification.JustificationPlaceholder + @""")]
    public void Bar()
    {

    }
}";

            expected = this.CSharpDiagnostic().WithLocation(4, 66);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDiagnosticDoesNotThrowNullReferenceForWrongConstantTypeAsync()
        {
            var testCode = @"public class Foo
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = 5)]
    public void Bar()
    {

    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 66),
                new DiagnosticResult
                {
                    Id = "CS0029",
                    Message = "Cannot implicitly convert type 'int' to 'string'",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 82) }
                }
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1404CodeAnalysisSuppressionMustHaveJustification();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1404CodeFixProvider();
        }
    }
}

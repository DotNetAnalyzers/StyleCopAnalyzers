// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.ReadabilityRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    public class SA1132UnitTests : CodeFixVerifier
    {
        [Theory]
        [InlineData("private int a;")]
        [InlineData("public event System.Action a;")]
        public async Task TestValidDeclarationAsync(string declaration)
        {
            var testCode = GetTemplate(declaration);

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("private int a, b;", "private int a;\r\n    private int b;")]
        [InlineData("public event System.Action a, b;", "public event System.Action a;\r\n    public event System.Action b;")]
        public async Task TestInvalidDeclarationAsync(string declaration, string fixedDeclaration)
        {
            string testCode = GetTemplate(declaration);
            string fixedCode = GetTemplate(fixedDeclaration);

            var expected = this.CSharpDiagnostic().WithLocation(4, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidFieldDeclarationWithAttributesAsync()
        {
            const string testCode = @"
class Foo
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private int a, b;
}";
            const string fixedCode = @"
class Foo
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private int a;
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private int b;
}";

            var expected = this.CSharpDiagnostic().WithLocation(4, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidEventFieldDeclarationWithAttributesAsync()
        {
            const string testCode = @"
class Foo
{
#if true
    [Test]
    public event System.Action foo, bar;
#endif
}

[System.AttributeUsage(System.AttributeTargets.Event)]
class TestAttribute : System.Attribute
{
}";
            const string fixedCode = @"
class Foo
{
#if true
    [Test]
    public event System.Action foo;
    [Test]
    public event System.Action bar;
#endif
}

[System.AttributeUsage(System.AttributeTargets.Event)]
class TestAttribute : System.Attribute
{
}";

            var expected = this.CSharpDiagnostic().WithLocation(5, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1132CodeFixProvider();
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1132DoNotCombineFields();
        }

        private static string GetTemplate(string declaration)
        {
            return $@"
class Foo
{{
    {declaration}
}}";
        }
    }
}

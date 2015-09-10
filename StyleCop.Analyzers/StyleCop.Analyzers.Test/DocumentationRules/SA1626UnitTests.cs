// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    public class SA1626UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1626SingleLineCommentsMustNotUseDocumentationStyleSlashes.DiagnosticId;

        [Fact]
        public async Task TestClassWithXmlCommentAsync()
        {
            var testCode = @"/// <summary>
/// XML Documentation
/// </summary>
public class TypeName
{
    public void Bar()
    {
    }
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithCommentAsync()
        {
            var testCode = @"public class TypeName
{
    public void Bar()
    {
        // This is a comment
    }
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithOneLineThreeSlashCommentAsync()
        {
            var testCode = @"public class TypeName
{
    public void Bar()
    {
        /// This is a comment
    }
}
";
            var fixedCode = @"public class TypeName
{
    public void Bar()
    {
        // This is a comment
    }
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithMultiLineThreeSlashCommentAsync()
        {
            var testCode = @"public class TypeName
{
    public void Bar()
    {
        /// This is
        /// a comment
    }
}
";
            var fixedCode = @"public class TypeName
{
    public void Bar()
    {
        // This is
        // a comment
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 9),
                this.CSharpDiagnostic().WithLocation(6, 9),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithCodeCommentsAsync()
        {
            var testCode = @"public class TypeName
{
    public void Bar()
    {
        //// System.Console.WriteLine(""Bar"")
    }
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithSingeLineDocumentationAsync()
        {
            var testCode = @"public class TypeName
{
    /// <summary>Summary text</summary>
    public void Bar()
    {
    }
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1626SingleLineCommentsMustNotUseDocumentationStyleSlashes();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1626CodeFixProvider();
        }
    }
}

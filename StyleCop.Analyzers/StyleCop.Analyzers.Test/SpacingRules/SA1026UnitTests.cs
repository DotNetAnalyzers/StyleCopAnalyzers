// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation"/>
    /// </summary>
    public class SA1026UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestValidSpacingOfImplicitlyTypedArrayAsync()
        {
            const string testCode = @"public class Foo
{
    public Foo()
    {
        var ints = new[] { 1, 2, 3 };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        [InlineData(" \t \r\n")]
        [InlineData(" \t \r\n\t ")]
        public async Task TestInvalidSpacingOfImplicitlyTypedArrayAsync(string space)
        {
            string testCode = string.Format("public class Foo {{ public Foo() {{ var ints = new{0}[] {{ 1, 2, 3 }}; }} }}", space);
            const string expectedCode = "public class Foo { public Foo() { var ints = new[] { 1, 2, 3 }; } }";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(1, 46);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(expectedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, expectedCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }
    }
}

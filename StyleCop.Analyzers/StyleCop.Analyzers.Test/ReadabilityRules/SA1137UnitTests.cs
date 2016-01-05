// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1137ElementsShouldHaveTheSameIndentation"/>.
    /// </summary>
    public class SA1137UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestBlockAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
      label1:
        if (true)
        {
        }

     label2:
       while (true)
        {
        }

label3:
while (true)
    {
label4a:
 label4b:
int x;

        label5a:
label5b:
        int y;
    }
    }
}
";
            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
      label1:
        if (true)
        {
        }

      label2:
        while (true)
        {
        }

      label3:
        while (true)
    {
label4a:
label4b:
int x;

label5a:
label5b:
int y;
    }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(11, 1),
                this.CSharpDiagnostic().WithLocation(12, 1),
                this.CSharpDiagnostic().WithLocation(16, 1),
                this.CSharpDiagnostic().WithLocation(17, 1),
                this.CSharpDiagnostic().WithLocation(20, 1),
                this.CSharpDiagnostic().WithLocation(23, 1),
                this.CSharpDiagnostic().WithLocation(25, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1137ElementsShouldHaveTheSameIndentation();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new IndentationCodeFixProvider();
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.ReadabilityRules;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    public class SA1135UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestUnqualifiedUsingsAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    using IO;
    using Tasks;
}";
            const string fixedCode = @"
namespace System.Threading
{
    using System.IO;
    using System.Threading.Tasks;
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 5),
                this.CSharpDiagnostic().WithLocation(5, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1135CodeFixProvider();
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1135UsingDirectivesMustBeQualified();
        }
    }
}

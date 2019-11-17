// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.LayoutRules.SA1514ElementDocumentationHeaderMustBePrecededByBlankLine,
    StyleCop.Analyzers.LayoutRules.SA1514CodeFixProvider>;

    public class SA1514CSharp8UnitTests : SA1514CSharp7UnitTests
    {
        /// <summary>
        /// Verifies that method-like declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidPropertyDeclarationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string SomeString { get; set; } = null!;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string AnotherString { get; set; } = null!;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

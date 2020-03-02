// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1141UseTupleSyntax,
        StyleCop.Analyzers.ReadabilityRules.SA1141CodeFixProvider>;

    /// <summary>
    /// This class contains the unit tests for SA1141.
    /// </summary>
    /// <seealso cref="SA1141UseTupleSyntax"/>
    /// <seealso cref="SA1141CodeFixProvider"/>
    public class SA1141UnitTests
    {
        /// <summary>
        /// Verifies that usage of the ValueTuple type will not produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task ValueTupleTypesDoNotProduceDiagnosticAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public ValueTuple<int, int> TestMethod(ValueTuple<double, double> value)
    {
        return new ValueTuple<int, int>(1, 2);
    }

    public ValueTuple<int, int> TestProperty { get; set; }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        //// TODO: Make sure that all paths are covered!
    }
}

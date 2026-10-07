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
        StyleCop.Analyzers.ReadabilityRules.SA1142ReferToTupleElementsByName,
        StyleCop.Analyzers.ReadabilityRules.SA1142CodeFixProvider>;

    /// <summary>
    /// This class contains the unit tests for SA1142.
    /// </summary>
    /// <seealso cref="SA1142ReferToTupleElementsByName"/>
    /// <seealso cref="SA1142CodeFixProvider"/>
    public class SA1142UnitTests
    {
        /// <summary>
        /// Verify that ValueTuple names referenced by their metadata name will not produce diagnostics when no other
        /// name is available.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ValidateFieldNameReferencesWithoutReplacementAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public int TestMethod1(ValueTuple<int, int> p1)
    {
        return p1.Item1 + p1.Item2 + p1.ToString().Length;
    }

    public int TestMethod2(ValueTuple<int, ValueTuple<int, int>> p1)
    {
        return p1.Item1 + p1.Item2.Item1 + p1.Item2.Item2;
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

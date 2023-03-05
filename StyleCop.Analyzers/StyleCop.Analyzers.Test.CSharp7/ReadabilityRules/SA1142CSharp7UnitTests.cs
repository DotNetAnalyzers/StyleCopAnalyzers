// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1142ReferToTupleElementsByName,
        StyleCop.Analyzers.ReadabilityRules.SA1142CodeFixProvider>;

    /// <summary>
    /// This class contains the CSharp 7.x unit tests for SA1142.
    /// </summary>
    /// <seealso cref="SA1142ReferToTupleElementsByName"/>
    /// <seealso cref="SA1142CodeFixProvider"/>
    public class SA1142CSharp7UnitTests : SA1142UnitTests
    {
        /// <summary>
        /// Validate that tuple fields that are referenced by their name will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ValidateFieldNameReferencesAsync()
        {
            var testCode = @"
public class TestClass
{
    public int TestMethod((int nameA, int nameB) p1, (int, int) p2, (int, int nameC) p3)
    {
        return p1.nameA + p1.nameB + p2.Item1 + p2.Item2 + p3.Item1 + p3.nameC;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that tuple names referenced by their metadata name will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ValidateMetadataNameReferencesAsync()
        {
            var testCode = @"
public class TestClass
{
    public int TestMethod1((int nameA, int nameB) p1)
    {
        return p1.[|Item1|] + p1.[|Item2|] /* test */ + p1.ToString().Length;
    }

    public int TestMethod2((int nameA, (int subNameA, int subNameB) nameB) p1)
    {
        return p1.[|Item1|] + p1.nameB.[|Item1|] + p1.[|Item2|].[|Item2|];
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int TestMethod1((int nameA, int nameB) p1)
    {
        return p1.nameA + p1.nameB /* test */ + p1.ToString().Length;
    }

    public int TestMethod2((int nameA, (int subNameA, int subNameB) nameB) p1)
    {
        return p1.nameA + p1.nameB.subNameA + p1.nameB.subNameB;
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

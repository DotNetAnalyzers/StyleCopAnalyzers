// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1122UseStringEmptyForEmptyStrings,
        StyleCop.Analyzers.ReadabilityRules.SA1122CodeFixProvider>;

    public partial class SA1122CSharp10UnitTests : SA1122CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that the empty string passed to <c>[InterpolatedStringHandlerArgument]</c> to refer to the receiver
        /// is not reported, because attribute arguments must be constants.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3981, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3981")]
        public async Task TestInterpolatedStringHandlerArgumentAsync()
        {
            var testCode = @"using System.Runtime.CompilerServices;

[InterpolatedStringHandler]
public ref struct Handler
{
    public Handler(int literalLength, int formattedCount, TestClass receiver)
    {
    }

    public void AppendLiteral(string value)
    {
    }

    public void AppendFormatted<T>(T value)
    {
    }
}

public class TestClass
{
    public void Log([InterpolatedStringHandlerArgument("""")] Handler handler)
    {
    }

    public void Test(int x)
    {
        this.Log($""x = {x}"");
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

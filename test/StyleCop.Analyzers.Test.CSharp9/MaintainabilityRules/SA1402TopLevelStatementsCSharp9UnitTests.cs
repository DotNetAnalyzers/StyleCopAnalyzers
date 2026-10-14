// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;

    public partial class SA1402TopLevelStatementsCSharp9UnitTests
    {
        [Fact]
        [WorkItem(4068, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4068")]
        public async Task TestClassAfterTopLevelStatementsIsNotReportedAsync()
        {
            var testCode = @"var x = new MyData();

internal class MyData
{
}

internal class OtherType
{
}
";

            var test = new StyleCopDiagnosticVerifier<StyleCop.Analyzers.MaintainabilityRules.SA1402FileMayOnlyContainASingleType>.CSharpTest()
            {
                TestState =
                {
                    OutputKind = OutputKind.ConsoleApplication,
                    Sources = { ("Program.cs", testCode) },
                },
            };

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

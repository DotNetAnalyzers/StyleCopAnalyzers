// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName>;

    public partial class SA1649CSharp9UnitTests : SA1649CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3967, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3967")]
        public async Task VerifyTopLevelStatementsOnlyDoNotReportAsync()
        {
            var testCode = @"System.Console.WriteLine();
";

            var test = new StyleCopCodeFixVerifier<StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName, StyleCop.Analyzers.DocumentationRules.SA1649CodeFixProvider>.CSharpTest()
            {
                TestState =
                {
                    OutputKind = OutputKind.ConsoleApplication,
                    Sources = { ("NameNotBasedOnTheCode.cs", testCode) },
                },
            };

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4068, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4068")]
        public async Task VerifyTopLevelStatementsAndClassDoNotReportAsync()
        {
            var testCode = @"var x = new MyData();

internal class MyData
{
}
";

            var test = new StyleCopCodeFixVerifier<StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName, StyleCop.Analyzers.DocumentationRules.SA1649CodeFixProvider>.CSharpTest()
            {
                TestState =
                {
                    OutputKind = OutputKind.ConsoleApplication,
                    Sources = { ("Program.cs", testCode) },
                },
            };

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4068, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4068")]
        public async Task VerifyTopLevelStatementsFollowedByClassDoNotReportAsync()
        {
            var testCode = @"System.Console.WriteLine();

class Helper
{
}
";

            var test = new StyleCopCodeFixVerifier<StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName, StyleCop.Analyzers.DocumentationRules.SA1649CodeFixProvider>.CSharpTest()
            {
                TestState =
                {
                    OutputKind = OutputKind.ConsoleApplication,
                    Sources = { ("Program.cs", testCode) },
                },
            };

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4068, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4068")]
        public async Task VerifyTopLevelStatementsFollowedByEnumDoNotReportAsync()
        {
            var testCode = @"System.Console.WriteLine();

enum Helper
{
    Value,
}
";

            var test = new StyleCopCodeFixVerifier<StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName, StyleCop.Analyzers.DocumentationRules.SA1649CodeFixProvider>.CSharpTest()
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

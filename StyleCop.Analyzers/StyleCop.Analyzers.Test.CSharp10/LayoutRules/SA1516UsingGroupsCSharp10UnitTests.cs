// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.CSharp9.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1516ElementsMustBeSeparatedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1516CodeFixProvider>;

    public partial class SA1516UsingGroupsCSharp10UnitTests : SA1516UsingGroupsCSharp9UnitTests
    {
        [Fact]
        [WorkItem(3982, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3982")]
        public async Task TestBlankLineRequiredBetweenGlobalAndLocalUsingGroupsAsync()
        {
            var testCode = @"
global using System;
global using System.Threading;
{|#0:using System.IO;|}

Console.WriteLine();
";

            var fixedCode = @"
global using System;
global using System.Threading;

using System.IO;

Console.WriteLine();
";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                Settings = GetBlankLinesBetweenUsingGroupsSettings(OptionSetting.Require),
                ExpectedDiagnostics =
                {
                    Diagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorRequire).WithLocation(0),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        private static string GetBlankLinesBetweenUsingGroupsSettings(OptionSetting optionSetting)
        {
            return $@"{{
    ""settings"": {{
        ""orderingRules"": {{
            ""systemUsingDirectivesFirst"": true,
            ""blankLinesBetweenUsingGroups"": ""{optionSetting.ToString().ToLowerInvariant()}""
        }}
    }}
}}
";
        }
    }
}

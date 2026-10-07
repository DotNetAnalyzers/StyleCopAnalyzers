// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.OrderingRules
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.CSharp8.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1200UsingDirectivesMustBePlacedCorrectly,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    public partial class SA1200CSharp9UnitTests : SA1200CSharp8UnitTests
    {
        private const string UsingDirectivesPlacementOutsideNamespace = @"{
  ""settings"": {
    ""orderingRules"": {
      ""usingDirectivesPlacement"": ""outsideNamespace""
    }
  }
}";

        private const string UsingDirectivesPlacementInsideNamespace = @"{
  ""settings"": {
    ""orderingRules"": {
      ""usingDirectivesPlacement"": ""insideNamespace""
    }
  }
}";

        private const string UsingDirectivesPlacementPreserve = @"{
  ""settings"": {
    ""orderingRules"": {
      ""usingDirectivesPlacement"": ""preserve""
    }
  }
}";

        /// <summary>
        /// Verifies that having using statements in the compilation unit will not produce diagnostics for top-level
        /// programs.
        /// </summary>
        /// <param name="placement">The using directives placement configuration to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [WorkItem(3243, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3243")]
        [WorkItem(3967, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3967")]
        [InlineData(UsingDirectivesPlacement.OutsideNamespace)]
        [InlineData(UsingDirectivesPlacement.InsideNamespace)]
        [InlineData(UsingDirectivesPlacement.Preserve)]
        public async Task TestValidUsingStatementsInTopLevelProgramAsync(object placement)
        {
            var testCode = @"using System;
using System.Threading;

return 0;
";

            var file = (UsingDirectivesPlacement)placement switch
            {
                UsingDirectivesPlacement.InsideNamespace => UsingDirectivesPlacementInsideNamespace,
                UsingDirectivesPlacement.OutsideNamespace => UsingDirectivesPlacementOutsideNamespace,
                UsingDirectivesPlacement.Preserve => UsingDirectivesPlacementPreserve,
                _ => throw new NotImplementedException(),
            };

            await new CSharpTest()
            {
                TestState =
                {
                    OutputKind = OutputKind.ConsoleApplication,
                    Sources = { testCode },
                    AdditionalFiles = { ("stylecop.json", file) },
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

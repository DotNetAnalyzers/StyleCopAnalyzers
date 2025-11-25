// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1137ElementsShouldHaveTheSameIndentation>;

    public partial class SA1137CSharp12UnitTests : SA1137CSharp11UnitTests
    {
        [Fact]
        [WorkItem(3904, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3904")]
        public async Task TestCollectionInitializationAsync()
        {
            var csharp = new CSharpTest()
            {
                TestSources =
                {
                    @"
                    class FirstCase
                    {
                        private readonly System.Collections.Generic.List<string> example1 = 
                        [
                            ""a"",
                    ""b"",
                                    ""c"",
                        ];
                    }",
                    @"
                    class SecondCase
                    {
                        private readonly System.Collections.Generic.List<int> example2 = 
                        [
                            1,
                            2,
                    3,
                    4,
                        ];
                    }",
                    @"
                    class ThirdCase
                    {
                        private readonly System.Collections.Generic.List<int> example3 = [1, 2, 3, 4];
                    }",
                },
                ExpectedDiagnostics =
                {
                    DiagnosticResult.CompilerWarning("SA1137").WithLocation("/0/Test0.cs", 7, 1),
                    DiagnosticResult.CompilerWarning("SA1137").WithLocation("/0/Test0.cs", 8, 1),
                    DiagnosticResult.CompilerWarning("SA1137").WithLocation("/0/Test1.cs", 8, 1),
                    DiagnosticResult.CompilerWarning("SA1137").WithLocation("/0/Test1.cs", 9, 1),
                },
            };

            await csharp.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

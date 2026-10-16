// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName>;

    public partial class SA1649CSharp15UnitTests : SA1649CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that unions are considered configured struct types.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2223, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2223")]
        public async Task VerifyConfiguredUnionTypeIsUsedAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""maintainabilityRules"": {
      ""topLevelTypes"": [ ""struct"" ]
    }
  }
}
";
            var testCode = @"public interface FirstType { }
public union SecondType(int, string);
";

            await VerifyCSharpDiagnosticAsync("SecondType.cs", testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyWrongFileNameForUnionDeclarationAsync()
        {
            var testCode = @"public union {|#0:TestUnion|}(int, string);
";
            var fixedCode = @"public union TestUnion(int, string);
";

            var test = new StyleCopCodeFixVerifier<StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName, StyleCop.Analyzers.DocumentationRules.SA1649CodeFixProvider>.CSharpTest()
            {
                TestSources = { ("WrongFileName.cs", testCode) },
                FixedSources = { ("TestUnion.cs", fixedCode) },
            };

            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifySimpleFileNameForGenericUnionDeclarationAsync()
        {
            var testCode = @"public union TestUnion<T>(T, string);
";

            var test = new StyleCopCodeFixVerifier<StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName, StyleCop.Analyzers.DocumentationRules.SA1649CodeFixProvider>.CSharpTest()
            {
                TestSources = { ("TestUnion.cs", testCode) },
            };

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

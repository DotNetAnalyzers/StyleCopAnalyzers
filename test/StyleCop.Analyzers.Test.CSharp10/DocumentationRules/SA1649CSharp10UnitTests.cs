// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName>;

    public partial class SA1649CSharp10UnitTests : SA1649CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that the file name is based on the first type.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3435, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3435")]
        public async Task VerifyFirstTypeIsUsedWithFileScopedNamespacesAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace;

public enum IgnoredEnum {{ }}
public delegate void IgnoredDelegate();

{GetTypeDeclaration(typeKeyword, "TestType", diagnosticKey: 0)}

{GetTypeDeclaration(typeKeyword, "TestType2")}
";
            var fixedCode = $@"namespace TestNamespace;

public enum IgnoredEnum {{ }}
public delegate void IgnoredDelegate();

{GetTypeDeclaration(typeKeyword, "TestType")}

{GetTypeDeclaration(typeKeyword, "TestType2")}
";

            var expectedDiagnostic = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync("TestType2.cs", testCode, StyleCopTopLevelTypesSettings, expectedDiagnostic, "TestType.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that record declarations are matched to their configured top-level type kinds.
        /// </summary>
        /// <param name="topLevelType">The configured top-level type kind.</param>
        /// <param name="recordDeclaration">The record declaration to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class", "public record SecondType { }")]
        [InlineData("struct", "public record struct SecondType { }")]
        [WorkItem(2223, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2223")]
        public async Task VerifyConfiguredRecordTypeIsUsedAsync(string topLevelType, string recordDeclaration)
        {
            var settings = $@"
{{
  ""settings"": {{
    ""maintainabilityRules"": {{
      ""topLevelTypes"": [ ""{topLevelType}"" ]
    }}
  }}
}}
";
            var testCode = @"public interface FirstType { }
" + recordDeclaration;

            await VerifyCSharpDiagnosticAsync("SecondType.cs", testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3992, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3992")]
        public async Task VerifyMappedLineDirectiveDoesNotChangeFileNameCheckAsync()
        {
            var testCode = @"#line 1 ""DifferentName.cs""
public class {|#0:NotMatching|}
{
}
#line default";

            var expectedDiagnostic = Diagnostic().WithLocation(0);
            await VerifyCSharpDiagnosticAsync("ActualFile.cs", testCode, testSettings: null, new[] { expectedDiagnostic }, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

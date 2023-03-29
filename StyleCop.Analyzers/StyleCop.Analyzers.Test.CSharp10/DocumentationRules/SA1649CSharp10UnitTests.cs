// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp10.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp9.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName>;

    public class SA1649CSharp10UnitTests : SA1649CSharp9UnitTests
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
            await VerifyCSharpFixAsync("TestType2.cs", testCode, StyleCopSettings, expectedDiagnostic, "TestType.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1642ConstructorSummaryDocumentationMustBeginWithStandardText,
        StyleCop.Analyzers.DocumentationRules.SA1642SA1643CodeFixProvider>;

    public partial class SA1642CSharp10UnitTests : SA1642CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3980, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3980")]
        public async Task TestParameterlessStructConstructorAsync()
        {
            var testCode = @"
public struct TestStruct
{
    /// [|<summary>
    /// Creates a new value.
    /// </summary>|]
    public TestStruct()
    {
    }
}
";

            var fixedCode = @"
public struct TestStruct
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""TestStruct""/> struct.
    /// Creates a new value.
    /// </summary>
    public TestStruct()
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

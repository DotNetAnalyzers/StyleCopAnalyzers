// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.CSharp14.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.GenericTypeParameterDocumentationAnalyzer>;

    // Union declarations are only parsed with the preview language version, which is the default for this test project.
    // The reference assemblies used by these tests do not define System.Runtime.CompilerServices.IUnion and
    // UnionAttribute, so union declarations produce CS0518 and CS0656, and compiler diagnostics are therefore ignored.
    public partial class SA1620CSharp15UnitTests : SA1620CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that documentation for a type parameter that a generic union does not declare is reported exactly
        /// once.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestGenericUnionAsync()
        {
            var testCode = @"
/// <summary>
/// A union.
/// </summary>
/// <typeparam name=""T"">The value type.</typeparam>
/// <typeparam name=""{|#0:U|}"">Not a type parameter.</typeparam>
public union Result<T>(T, string);
";

            await new CSharpTest()
            {
                TestCode = testCode,
                ExpectedDiagnostics = { Diagnostic(GenericTypeParameterDocumentationAnalyzer.SA1620MissingTypeParameterDescriptor).WithLocation(0).WithArguments("U") },
                CompilerDiagnostics = CompilerDiagnostics.None,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

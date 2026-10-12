// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1004DocumentationLinesMustBeginWithSingleSpace,
        StyleCop.Analyzers.SpacingRules.SA1004CodeFixProvider>;

    public partial class SA1004CSharp12UnitTests : SA1004CSharp11UnitTests
    {
        [Fact]
        [WorkItem(3817, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3817")]
        public async Task TestParameterModifierReadOnlyFirstOnLineAsync()
        {
            string testCode = $@"
/// <summary>
/// Description of some remarks that refer to a method: <see cref=""SomeMethod(int, int, ref
/// readonly string)""/>.
/// </summary>
public class TypeName
{{
    public void SomeMethod(int x, int y, ref readonly string z)
    {{
        throw new System.Exception();
    }}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

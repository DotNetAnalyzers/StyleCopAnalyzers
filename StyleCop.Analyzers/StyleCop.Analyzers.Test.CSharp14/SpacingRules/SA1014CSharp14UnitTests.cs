// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1014OpeningGenericBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1014CSharp14UnitTests : SA1014CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4025, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4025")]
        public async Task TestUnboundGenericTypeInNameofAsync()
        {
            var testCode = @"
using System.Collections.Generic;

public class TestClass
{
    public string A => nameof(List<>);

    public string B => nameof(Dictionary<,>);

    public string C => nameof(Dictionary{|#0:<|} ,>);
}
";

            var fixedCode = @"
using System.Collections.Generic;

public class TestClass
{
    public string A => nameof(List<>);

    public string B => nameof(Dictionary<,>);

    public string C => nameof(Dictionary<,>);
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("followed");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

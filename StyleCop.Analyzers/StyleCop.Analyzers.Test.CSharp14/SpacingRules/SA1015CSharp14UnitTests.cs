// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using StyleCop.Analyzers.Test.CSharp13.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1015ClosingGenericBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1015CSharp14UnitTests : SA1015CSharp13UnitTests
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

    public string C => nameof(List<>.Count);

    public string D => nameof(Dictionary<, {|#0:>|});
}
";

            var fixedCode = @"
using System.Collections.Generic;

public class TestClass
{
    public string A => nameof(List<>);

    public string B => nameof(Dictionary<,>);

    public string C => nameof(List<>.Count);

    public string D => nameof(Dictionary<,>);
}
";

            var expected = Diagnostic(SA1015ClosingGenericBracketsMustBeSpacedCorrectly.DescriptorNotPreceded).WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

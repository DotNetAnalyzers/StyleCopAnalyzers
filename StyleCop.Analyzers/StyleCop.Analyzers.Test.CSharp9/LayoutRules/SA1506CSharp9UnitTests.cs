// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.LayoutRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1506CodeFixProvider>;

    public partial class SA1506CSharp9UnitTests : SA1506CSharp8UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestRecordDocumentationFollowedByBlankLineAsync(string keyword)
        {
            var testCode = $@"/// <summary>
/// A record.
/// </summary>

public {keyword} TestRecord
{{
}}
";

            var fixedCode = $@"/// <summary>
/// A record.
/// </summary>
public {keyword} TestRecord
{{
}}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(4, 1), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

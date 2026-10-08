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
        StyleCop.Analyzers.LayoutRules.SA1514ElementDocumentationHeaderMustBePrecededByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1514CodeFixProvider>;

    public partial class SA1514CSharp9UnitTests : SA1514CSharp8UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestRecordDocumentationNotPrecededByBlankLineAsync(string keyword)
        {
            var testCode = $@"public class TestClass
{{
    private int field;
    {{|#0:///|}} <summary>
    /// A record.
    /// </summary>
    public {keyword} TestRecord
    {{
    }}
}}
";

            var fixedCode = $@"public class TestClass
{{
    private int field;

    /// <summary>
    /// A record.
    /// </summary>
    public {keyword} TestRecord
    {{
    }}
}}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

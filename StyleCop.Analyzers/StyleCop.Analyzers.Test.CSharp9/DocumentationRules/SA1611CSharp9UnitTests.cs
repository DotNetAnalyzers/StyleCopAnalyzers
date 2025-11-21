// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;

    public partial class SA1611CSharp9UnitTests : SA1611CSharp8UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3770, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3770")]
        public async Task TestPrimaryRecordConstructorMissingParametersAsync(string keyword)
        {
            var testCode = $@"
/// <summary>
/// Record.
/// </summary>
public {keyword} R(int Param1, string Param2);";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3770, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3770")]
        public async Task TestPrimaryRecordConstructorIncludeMissingParametersAsync(string keyword)
        {
            var testCode = $@"
/// <include file='MissingClassDocumentation.xml' path='/TestType/*' />
public {keyword} R(int Param1, string Param2);";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

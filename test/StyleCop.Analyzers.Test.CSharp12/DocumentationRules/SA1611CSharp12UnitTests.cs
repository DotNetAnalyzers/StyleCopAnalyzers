// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1611ElementParametersMustBeDocumented>;

    public partial class SA1611CSharp12UnitTests : SA1611CSharp11UnitTests
    {
        public static TheoryData<string> NonRecordDeclarationKeywords { get; } = new TheoryData<string>() { "class", "struct" };

        [Theory]
        [MemberData(nameof(NonRecordDeclarationKeywords))]
        [WorkItem(3770, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3770")]
        public async Task TestPrimaryConstructorMissingParametersAsync(string keyword)
        {
            var testCode = $@"
/// <summary>
/// Type.
/// </summary>
public {keyword} C(int {{|#0:param1|}}, string {{|#1:param2|}}) {{ }}";

            DiagnosticResult[] expectedResults = new[]
            {
                Diagnostic().WithLocation(0).WithArguments("param1"),
                Diagnostic().WithLocation(1).WithArguments("param2"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expectedResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(NonRecordDeclarationKeywords))]
        [WorkItem(3770, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3770")]
        public async Task TestPrimaryConstructorPartiallyMissingParametersAsync(string keyword)
        {
            var testCode = $@"
/// <summary>
/// Type.
/// </summary>
/// <param name=""param1"">Parameter one.</param>
public {keyword} C(int param1, string {{|#0:param2|}}) {{ }}";

            await VerifyCSharpDiagnosticAsync(testCode, new[] { Diagnostic().WithLocation(0).WithArguments("param2") }, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(NonRecordDeclarationKeywords))]
        [WorkItem(3770, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3770")]
        public async Task TestPrimaryConstructorNoMissingParametersAsync(string keyword)
        {
            var testCode = $@"
/// <summary>
/// Type.
/// </summary>
/// <param name=""param1"">Parameter one.</param>
/// <param name=""param2"">Parameter two.</param>
public {keyword} C(int param1, string param2) {{ }}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(NonRecordDeclarationKeywords))]
        [WorkItem(3770, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3770")]
        public async Task TestPrimaryConstructorIncludeMissingParametersAsync(string keyword)
        {
            var testCode = $@"
/// <include file='MissingClassDocumentation.xml' path='/TestType/*' />
public {keyword} C(int {{|#0:param1|}}, string {{|#1:param2|}}, bool {{|#2:param3|}}) {{ }}";

            DiagnosticResult[] expectedResults = new[]
            {
                Diagnostic().WithLocation(0).WithArguments("param1"),
                Diagnostic().WithLocation(1).WithArguments("param2"),
                Diagnostic().WithLocation(2).WithArguments("param3"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expectedResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(NonRecordDeclarationKeywords))]
        [WorkItem(3770, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3770")]
        public async Task TestPrimaryConstructorIncludePartiallyMissingParametersAsync(string keyword)
        {
            var testCode = $@"
/// <include file='WithPartialClassDocumentation.xml' path='/TestType/*' />
public {keyword} C(int {{|#0:param1|}}, string param2, bool param3) {{ }}";

            await VerifyCSharpDiagnosticAsync(testCode, new[] { Diagnostic().WithLocation(0).WithArguments("param1") }, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(NonRecordDeclarationKeywords))]
        [WorkItem(3770, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3770")]
        public async Task TestPrimaryConstructorIncludeNoMissingParametersAsync(string keyword)
        {
            var testCode = $@"
/// <include file='WithClassDocumentation.xml' path='/TestType/*' />
public {keyword} C(int param1, string param2, bool param3) {{ }}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

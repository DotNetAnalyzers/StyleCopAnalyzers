// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1600ElementsMustBeDocumented>;

    public partial class SA1600CSharp9UnitTests : SA1600CSharp8UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordPrimaryConstructorNoParameterDocumentationAsync(string keyword)
        {
            string testCode = $@"
/// <summary>
/// Record.
/// </summary>
public {keyword} MyRecord(int {{|#0:Param1|}}, string {{|#1:Param2|}});";

            DiagnosticResult[] expectedResults = new[]
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };

            await VerifyCSharpDiagnosticAsync(testCode, this.GetExpectedResultTestRecordPrimaryConstructor(expectedResults), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordPrimaryConstructorNoDocumentationAsync(string keyword)
        {
            string testCode = $@"
public {keyword} {{|#0:MyRecord|}}(int {{|#1:Param1|}});";

            DiagnosticResult[] expectedResults = new[]
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };

            await VerifyCSharpDiagnosticAsync(testCode, this.GetExpectedResultTestRecordPrimaryConstructor(expectedResults), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordPrimaryConstructorCompleteParameterDocumentationAsync(string keyword)
        {
            string testCode = $@"
/// <summary>
/// Record.
/// </summary>
/// <param name=""Param1"">Parameter one.</param>
/// <param name=""Param2"">Parameter two.</param>
public {keyword} MyRecord(int Param1, string Param2);";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordPrimaryConstructorPartialParameterDocumentationAsync(string keyword)
        {
            string testCode = $@"
/// <summary>
/// Record.
/// </summary>
/// <param name=""Param1"">Parameter one.</param>
public {keyword} MyRecord(int Param1, string {{|#0:Param2|}});";

            DiagnosticResult[] expectedResults = new[]
            {
                Diagnostic().WithLocation(0),
            };

            await VerifyCSharpDiagnosticAsync(testCode, this.GetExpectedResultTestRecordPrimaryConstructor(expectedResults), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordPrimaryConstructorInheritdocAsync(string keyword)
        {
            string testCode = $@"
/// <inheritdoc />
public {keyword} MyRecord(int Param1, string Param2);";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordPrimaryConstructorIncludeParameterDocumentationAsync(string keyword)
        {
            string testCode = $@"
/// <include file='WithParameterDocumentation.xml' path='/TestType/*' />
public {keyword} MyRecord(int Param1, string Param2, bool Param3);";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordPrimaryConstructorIncludeInheritdocAsync(string keyword)
        {
            string testCode = $@"
/// <include file='WithInheritdoc.xml' path='/TestType/*' />
public {keyword} MyRecord(int Param1, string Param2, bool Param3);";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordPrimaryConstructorIncludePartialParameterDocumentationAsync(string keyword)
        {
            string testCode = $@"
/// <include file='WithPartialParameterDocumentation.xml' path='/TestType/*' />
public {keyword} MyRecord(int {{|#0:Param1|}}, string Param2, bool Param3);";

            DiagnosticResult[] expectedResults = new[]
            {
                Diagnostic().WithLocation(0),
            };

            await VerifyCSharpDiagnosticAsync(testCode, this.GetExpectedResultTestRecordPrimaryConstructor(expectedResults), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordPrimaryConstructorIncludeMissingParameterDocumentationAsync(string keyword)
        {
            string testCode = $@"
/// <include file='MissingParameterDocumentation.xml' path='/TestType/*' />
public {keyword} MyRecord(int {{|#0:Param1|}}, string {{|#1:Param2|}}, bool {{|#2:Param3|}});";

            DiagnosticResult[] expectedResults = new[]
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
            };

            await VerifyCSharpDiagnosticAsync(testCode, this.GetExpectedResultTestRecordPrimaryConstructor(expectedResults), CancellationToken.None).ConfigureAwait(false);
        }

        protected static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            string typeWithoutParameterDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
        <TestType>
            <summary>
                Foo
            </summary>
        </TestType>
        ";
            string typeWithPartialParameterDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
        <TestType>
            <summary>
                Foo
            </summary>
            <param name=""Param2"">Param 2.</param>
            <param name=""Param3"">Param 3.</param>
        </TestType>
        ";
            string typeWithParameterDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
        <TestType>
            <summary>
                Foo
            </summary>
            <param name=""Param1"">Param 1.</param>
            <param name=""Param2"">Param 2.</param>
            <param name=""Param3"">Param 3.</param>
        </TestType>
        ";
            string typeWithIncludedInheritdoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
        <TestType>
            <inheritdoc />
        </TestType>
        ";

            var test = new CSharpTest
            {
                TestCode = source,
                XmlReferences =
                        {
                            { "MissingParameterDocumentation.xml", typeWithoutParameterDocumentation },
                            { "WithParameterDocumentation.xml", typeWithParameterDocumentation },
                            { "WithPartialParameterDocumentation.xml", typeWithPartialParameterDocumentation },
                            { "WithInheritdoc.xml", typeWithIncludedInheritdoc },
                        },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        protected override DiagnosticResult[] GetExpectedResultTestRegressionMethodGlobalNamespace(string code)
        {
            if (code == "public void {|#0:TestMember|}() { }")
            {
                return new[]
                {
                    // error CS8805: Program using top-level statements must be an executable.
                    DiagnosticResult.CompilerError("CS8805"),

                    // /0/Test0.cs(4,1): error CS0106: The modifier 'public' is not valid for this item
                    DiagnosticResult.CompilerError("CS0106").WithSpan(4, 1, 4, 7).WithArguments("public"),
                };
            }

            return base.GetExpectedResultTestRegressionMethodGlobalNamespace(code);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestRecordPrimaryConstructor(DiagnosticResult[] results)
        {
            // Roslyn reports diagnostics twice for C# 9 and C# 10. Fixed in C# 11.
            return results.Concat(results).ToArray();
        }
    }
}

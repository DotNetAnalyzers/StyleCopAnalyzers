// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1608ElementDocumentationMustNotHaveDefaultSummary>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1608ElementDocumentationMustNotHaveDefaultSummary"/>.
    /// </summary>
    public class SA1608UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestTypeNoDocumentationAsync(string typeName)
        {
            var testCode = @"
{0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestTypeWithSummaryDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
{0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestTypeWithContentDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <content>
/// Foo
/// </content>
{0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestTypeWithInheritedDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <inheritdoc/>
{0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestTypeWithoutSummaryDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
{0}
TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestTypeWithoutContentDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <content>
/// 
/// </content>
partial {0}
TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestTypeWithDefaultDocumentationAsync(string typeName)
        {
            var testCode = $@"
/// <summary>
/// Summary description for the ClassName class.
/// </summary>
public {typeName} ClassName
{{
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(2, 5);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithDefaultDocumentationMultipleWhitespacesAsync()
        {
            var testCode = @"
/// <summary>
/// Summary           description 
/// for the      ClassName class.
/// </summary>
public class ClassName
{
}";

            DiagnosticResult expected = Diagnostic().WithLocation(2, 5);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumMemberWithContentDocumentationAsync()
        {
            var testCode = @"
public enum EnumName
{
    /// <summary>
    /// Foo.
    /// </summary>
    EnumMember1 = 0,
}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumMemberWithDefaultDocumentationAsync()
        {
            var testCode = @"
public enum EnumName
{
    /// <summary>
    /// Summary description for the EnumMember1 enum member.
    /// </summary>
    EnumMember1 = 0,

    /// <summary>
    /// Summary           description
    /// for the      EnumMember2 enum member.
    /// </summary>
    EnumMember2 = 1,
}
";
            DiagnosticResult[] expected = new[]
            {
                Diagnostic().WithLocation(4, 9),
                Diagnostic().WithLocation(9, 9),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithIncludedEmptyDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassWithoutSummary.xml' path='/ClassName/*' />
public class ClassName
{
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3150, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3150")]
        public async Task TestClassWithIncludedMissingDocumentationAsync()
        {
            var testCode = @"
/// <include file='MissingFile.xml' path='/ClassName/*' />
public class ClassName
{
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithIncludedSummaryDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassWithSummary.xml' path='/ClassName/*' />
public class ClassName
{
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithIncludedDefaultSummaryDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassWithDefaultSummary.xml' path='/ClassName/*' />
public class ClassName
{
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 14);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3150, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3150")]
        public async Task TestFieldWithIncludedSummaryDocumentationAsync()
        {
            var testCode = @"
public class ClassName
{
    /// <include file='FieldWithSummary.xml' path='/FieldName/*' />
    public int FieldName;
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3150, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3150")]
        public async Task TestFieldWithIncludedDefaultSummaryDocumentationAsync()
        {
            var testCode = @"
public class ClassName
{
    /// <include file='FieldWithDefaultSummary.xml' path='/FieldName/*' />
    public {|#0:int FieldName|};
}";

            DiagnosticResult expected = Diagnostic().WithLocation(0);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, new[] { expected }, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            string contentWithoutSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
</ClassName>
";
            string contentWithSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <summary>
    Foo
  </summary>
</ClassName>
";
            string contentWithDefaultSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <summary>
    Summary description for the ClassName class.
  </summary>
</ClassName>
";
            string fieldContentWithSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<FieldName>
  <summary>
    Foo
  </summary>
</FieldName>
";
            string fieldContentWithDefaultSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<FieldName>
  <summary>
    Summary description for the ClassName class.
  </summary>
</FieldName>
";

            var test = new StyleCopDiagnosticVerifier<SA1608ElementDocumentationMustNotHaveDefaultSummary>.CSharpTest
            {
                TestCode = source,
                XmlReferences =
                {
                    { "ClassWithoutSummary.xml", contentWithoutSummary },
                    { "ClassWithSummary.xml", contentWithSummary },
                    { "ClassWithDefaultSummary.xml", contentWithDefaultSummary },
                    { "FieldWithSummary.xml", fieldContentWithSummary },
                    { "FieldWithDefaultSummary.xml", fieldContentWithDefaultSummary },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}

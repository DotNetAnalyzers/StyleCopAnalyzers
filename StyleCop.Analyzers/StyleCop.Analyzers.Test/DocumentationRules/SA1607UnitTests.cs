// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<Analyzers.DocumentationRules.SA1607PartialElementDocumentationMustHaveSummaryText>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1607PartialElementDocumentationMustHaveSummaryText"/>.
    /// </summary>
    public class SA1607UnitTests
    {
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeNoDocumentationAsync(string typeName)
        {
            var testCode = @"
partial {0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithSummaryDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
partial {0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithContentDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <content>
/// Foo
/// </content>
partial {0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithInheritedDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <inheritdoc/>
partial {0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithoutSummaryDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
partial {0}
TypeName
{{
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("enum")]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestNonPartialTypeWithoutSummaryDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
{0} TypeName
{{
}}";

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("enum")]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestNonPartialTypeWithoutContentDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <content>
/// 
/// </content>
{0} TypeName
{{
}}";

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
    partial void Test();
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithSummaryDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    partial void Test();
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithContentDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
    /// <content>
    /// Foo
    /// </content>
    partial void Test();
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithInheritedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
    /// <inheritdoc/>
    partial void Test();
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithoutSummaryDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
/// <summary>
/// 
/// </summary>
    partial void Test();
}";

            DiagnosticResult expected = Diagnostic().WithLocation(10, 18);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNonPartialMethodWithoutSummaryDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
/// <summary>
/// 
/// </summary>
    public void Test() { }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithoutContentDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
/// <content>
/// 
/// </content>
    partial void Test();
}";

            DiagnosticResult expected = Diagnostic().WithLocation(10, 18);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNonPartialMethodWithoutContentDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
/// <content>
/// 
/// </content>
    public void Test() { }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedDocumentationWithoutSummaryOrContentAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
    /// <include file='MethodWithoutSummaryOrContent.xml' path='/ClassName/Test/*'/>
    partial void Test();
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedDocumentationWithEmptySummaryAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
    /// <include file='MethodWithEmptySummary.xml' path='/ClassName/Test/*'/>
    partial void Test();
}";
            var expected = Diagnostic().WithLocation(8, 18);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedDocumentationWithEmptyContentAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
    /// <include file='MethodWithEmptyContent.xml' path='/ClassName/Test/*'/>
    partial void Test();
}";
            var expected = Diagnostic().WithLocation(8, 18);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedDocumentationWithInheritdocAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
    /// <include file='MethodWithInheritdoc.xml' path='/ClassName/Test/*'/>
    partial void Test();
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedDocumentationWithSummaryAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
    /// <include file='MethodWithSummary.xml' path='/ClassName/Test/*'/>
    partial void Test();
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedDocumentationWithContentAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
    /// <include file='MethodWithContent.xml' path='/ClassName/Test/*'/>
    partial void Test();
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, new[] { expected }, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            string contentWithoutSummaryOrContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
  </Test>
</ClassName>
";
            string contentWithEmptySummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <summary>

    </summary>
  </Test>
</ClassName>
";
            string contentWithEmptyContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <content>

    </content>
  </Test>
</ClassName>
";
            string contentWithInheritdoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <inheritdoc/>
  </Test>
</ClassName>
";
            string contentWithSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <summary>
      Foo
    </summary>
  </Test>
</ClassName>
";
            string contentWithContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <content>
      Foo
    </content>
  </Test>
</ClassName>
";

            var test = new StyleCopDiagnosticVerifier<SA1607PartialElementDocumentationMustHaveSummaryText>.CSharpTest
            {
                TestCode = source,
                XmlReferences =
                {
                    { "MethodWithoutSummaryOrContent.xml", contentWithoutSummaryOrContent },
                    { "MethodWithEmptySummary.xml", contentWithEmptySummary },
                    { "MethodWithEmptyContent.xml", contentWithEmptyContent },
                    { "MethodWithInheritdoc.xml", contentWithInheritdoc },
                    { "MethodWithSummary.xml", contentWithSummary },
                    { "MethodWithContent.xml", contentWithContent },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}

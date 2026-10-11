// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.DocumentationRules;
    using Xunit;

    public partial class SA1607CSharp13UnitTests : SA1607CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPropertyNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPropertyWithSummaryDocumentationAsync()
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
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPropertyWithContentDocumentationAsync()
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
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPropertyWithInheritedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
    /// <inheritdoc/>
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPropertyWithoutSummaryDocumentationAsync()
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
    public partial int [|Test|] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPropertyWithoutContentDocumentationAsync()
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
    public partial int [|Test|] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestNonPartialPropertyWithoutSummaryDocumentationAsync()
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
    public int Test { get; set; }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIndexerNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIndexerWithSummaryDocumentationAsync()
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
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIndexerWithContentDocumentationAsync()
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
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIndexerWithInheritedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class ClassName
{
    /// <inheritdoc/>
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIndexerWithoutSummaryDocumentationAsync()
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
    public partial int [|this|][int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIndexerWithoutContentDocumentationAsync()
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
    public partial int [|this|][int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestNonPartialIndexerWithoutSummaryDocumentationAsync()
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
    public int this[int index] { get { return 0; } set { } }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIncludedPropertyDocumentationWithoutSummaryOrContentAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <include file='PropertyWithoutSummaryOrContent.xml' path='/ClassName/Test/*'/>
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "PropertyWithoutSummaryOrContent.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIncludedPropertyDocumentationWithEmptySummaryAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <include file='PropertyWithEmptySummary.xml' path='/ClassName/Test/*'/>
    public partial int [|Test|] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <summary>

    </summary>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "PropertyWithEmptySummary.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIncludedPropertyDocumentationWithEmptyContentAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <include file='PropertyWithEmptyContent.xml' path='/ClassName/Test/*'/>
    public partial int [|Test|] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <content>

    </content>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "PropertyWithEmptyContent.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIncludedPropertyDocumentationWithInheritdocAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <include file='PropertyWithInheritdoc.xml' path='/ClassName/Test/*'/>
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <inheritdoc/>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "PropertyWithInheritdoc.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIncludedPropertyDocumentationWithSummaryAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <include file='PropertyWithSummary.xml' path='/ClassName/Test/*'/>
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <summary>
      Foo
    </summary>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "PropertyWithSummary.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIncludedPropertyDocumentationWithContentAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <include file='PropertyWithContent.xml' path='/ClassName/Test/*'/>
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <content>
      Foo
    </content>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "PropertyWithContent.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIncludedIndexerDocumentationWithoutSummaryOrContentAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <include file='IndexerWithoutSummaryOrContent.xml' path='/ClassName/Test/*'/>
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "IndexerWithoutSummaryOrContent.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIncludedIndexerDocumentationWithEmptySummaryAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <include file='IndexerWithEmptySummary.xml' path='/ClassName/Test/*'/>
    public partial int [|this|][int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <summary>

    </summary>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "IndexerWithEmptySummary.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIncludedIndexerDocumentationWithEmptyContentAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <include file='IndexerWithEmptyContent.xml' path='/ClassName/Test/*'/>
    public partial int [|this|][int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <content>

    </content>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "IndexerWithEmptyContent.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIncludedIndexerDocumentationWithInheritdocAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <include file='IndexerWithInheritdoc.xml' path='/ClassName/Test/*'/>
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <inheritdoc/>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "IndexerWithInheritdoc.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIncludedIndexerDocumentationWithSummaryAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <include file='IndexerWithSummary.xml' path='/ClassName/Test/*'/>
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <summary>
      Foo
    </summary>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "IndexerWithSummary.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIncludedIndexerDocumentationWithContentAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <include file='IndexerWithContent.xml' path='/ClassName/Test/*'/>
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
    <content>
      Foo
    </content>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "IndexerWithContent.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticWithIncludeAsync(string source, DiagnosticResult[] expected, string xmlFileName, string xmlContent, CancellationToken cancellationToken)
        {
            var test = new StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1607PartialElementDocumentationMustHaveSummaryText>.CSharpTest
            {
                TestCode = source,
                XmlReferences =
                {
                    { xmlFileName, xmlContent },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}

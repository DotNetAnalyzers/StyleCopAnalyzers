// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1605PartialElementDocumentationMustHaveSummary>;

    public partial class SA1605CSharp13UnitTests : SA1605CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPropertyNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
///
/// </summary>
public partial class ClassName
{
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <summary>
    ///
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
///
/// </summary>
public partial class ClassName
{
    /// <summary>
    ///
    /// </summary>
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <summary>
    ///
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
///
/// </summary>
public partial class ClassName
{
    /// <content>
    ///
    /// </content>
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <summary>
    ///
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
///
/// </summary>
public partial class ClassName
{
    /// <inheritdoc/>
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <summary>
    ///
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
        public async Task TestPropertyWithoutDocumentationAsync()
        {
            var testCode = @"
/// <summary>
///
/// </summary>
public partial class ClassName
{
    ///
    public partial int [|Test|] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    ///
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
        public async Task TestNonPartialPropertyWithoutDocumentationAsync()
        {
            var testCode = @"
/// <summary>
///
/// </summary>
public partial class ClassName
{
    ///
    public int Test { get; set; }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIncludedPropertyDocumentationWithoutSummaryAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <include file='PropertyWithoutSummary.xml' path='/ClassName/Test/*'/>
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
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "PropertyWithoutSummary.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
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
        public async Task TestExpressionBodiedImplementingPropertyWithoutIncludedSummaryAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test { get; }
}

public partial class ClassName
{
    /// <include file='ImplementingPropertyWithoutSummary.xml' path='/ClassName/Test/*'/>
    public partial int [|Test|] => 0;
}";
            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "ImplementingPropertyWithoutSummary.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestBlockBodiedImplementingPropertyWithoutIncludedSummaryAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <include file='ImplementingPropertyBlockBodyWithoutSummary.xml' path='/ClassName/Test/*'/>
    public partial int [|Test|]
    {
        get { return 0; }
        set { }
    }
}";
            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "ImplementingPropertyBlockBodyWithoutSummary.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPropertyWithExpressionBodiedAccessorImplementingPartWithoutIncludedSummaryAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int Test { get; set; }
}

public partial class ClassName
{
    /// <include file='ImplementingPropertyAccessorExpressionBodyWithoutSummary.xml' path='/ClassName/Test/*'/>
    public partial int [|Test|]
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

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "ImplementingPropertyAccessorExpressionBodyWithoutSummary.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIndexerNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
///
/// </summary>
public partial class ClassName
{
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    ///
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
///
/// </summary>
public partial class ClassName
{
    /// <summary>
    ///
    /// </summary>
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    ///
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
///
/// </summary>
public partial class ClassName
{
    /// <content>
    ///
    /// </content>
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    ///
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
///
/// </summary>
public partial class ClassName
{
    /// <inheritdoc/>
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    ///
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
        public async Task TestIndexerWithoutDocumentationAsync()
        {
            var testCode = @"
/// <summary>
///
/// </summary>
public partial class ClassName
{
    ///
    public partial int [|this|][int index] { get; set; }
}

public partial class ClassName
{
    /// <summary>
    ///
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
        public async Task TestNonPartialIndexerWithoutDocumentationAsync()
        {
            var testCode = @"
/// <summary>
///
/// </summary>
public partial class ClassName
{
    ///
    public int this[int index] { get { return 0; } set { } }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIncludedIndexerDocumentationWithoutSummaryAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <include file='IndexerWithoutSummary.xml' path='/ClassName/Test/*'/>
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
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "IndexerWithoutSummary.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
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

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestExpressionBodiedImplementingIndexerWithoutIncludedSummaryAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index] { get; }
}

public partial class ClassName
{
    /// <include file='ImplementingIndexerWithoutSummary.xml' path='/ClassName/Test/*'/>
    public partial int [|this|][int index] => 0;
}";
            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "ImplementingIndexerWithoutSummary.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestBlockBodiedImplementingIndexerWithoutIncludedSummaryAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <include file='ImplementingIndexerBlockBodyWithoutSummary.xml' path='/ClassName/Test/*'/>
    public partial int [|this|][int index]
    {
        get { return 0; }
        set { }
    }
}";
            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Test>
  </Test>
</ClassName>
";

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "ImplementingIndexerBlockBodyWithoutSummary.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestIndexerWithExpressionBodiedAccessorImplementingPartWithoutIncludedSummaryAsync()
        {
            var testCode = @"
public partial class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public partial int this[int index] { get; set; }
}

public partial class ClassName
{
    /// <include file='ImplementingIndexerAccessorExpressionBodyWithoutSummary.xml' path='/ClassName/Test/*'/>
    public partial int [|this|][int index]
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

            await VerifyCSharpDiagnosticWithIncludeAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, "ImplementingIndexerAccessorExpressionBodyWithoutSummary.xml", xmlContent, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticWithIncludeAsync(string source, DiagnosticResult[] expected, string xmlFileName, string xmlContent, CancellationToken cancellationToken)
        {
            var test = new StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1605PartialElementDocumentationMustHaveSummary>.CSharpTest
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

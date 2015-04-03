namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1651DoNotUsePlaceholderElements"/>.
    /// </summary>
    public class SA1651UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1651DoNotUsePlaceholderElements.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEmptyDocumentation()
        {
            var testCode = @"namespace FooNamespace
{
    ///
    ///
    ///
    public class ClassName
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestDocumentationWithoutPlaceholders()
        {
            var testCode = @"namespace FooNamespace
{
    /// <summary>
    /// Content.
    /// </summary>
    public class ClassName
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestTopLevelPlaceholder()
        {
            var testCode = @"namespace FooNamespace
{
    /// <placeholder><summary>
    /// Content.
    /// </summary></placeholder>
    public class ClassName
    {
    }
}";

            var fixedCode = @"namespace FooNamespace
{
    /// <summary>
    /// Content.
    /// </summary>
    public class ClassName
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestTopLevelEmptyPlaceholder()
        {
            var testCode = @"namespace FooNamespace
{
    /// <placeholder/>
    public class ClassName
    {
    }
}";

            // Empty placeholders are not altered by the current code fix.
            var fixedCode = testCode;

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestEmbeddedPlaceholder()
        {
            var testCode = @"namespace FooNamespace
{
    /// <summary>
    /// <placeholder>Content.</placeholder>
    /// </summary>
    public class ClassName
    {
    }
}";

            var fixedCode = @"namespace FooNamespace
{
    /// <summary>
    /// Content.
    /// </summary>
    public class ClassName
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestEmbeddedEmptyPlaceholder()
        {
            var testCode = @"namespace FooNamespace
{
    /// <summary>
    /// Content.<placeholder/>
    /// </summary>
    public class ClassName
    {
    }
}";

            // Empty placeholders are not altered by the current code fix.
            var fixedCode = testCode;

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 17);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestDeeplyEmbeddedPlaceholder()
        {
            var testCode = @"namespace FooNamespace
{
    /// <summary>
    /// Content.
    /// </summary>
    /// <remarks>
    /// <list type=""bullet"">
    /// <item><placeholder>Nested content.</placeholder></item>
    /// </list>
    /// </remarks>
    public class ClassName
    {
    }
}";

            var fixedCode = @"namespace FooNamespace
{
    /// <summary>
    /// Content.
    /// </summary>
    /// <remarks>
    /// <list type=""bullet"">
    /// <item>Nested content.</item>
    /// </list>
    /// </remarks>
    public class ClassName
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 15);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestFormattingPreserved()
        {
            var testCode = @"namespace FooNamespace
{
   ///  <placeholder> <summary>
     /// Content <placeholder
            /// >.</placeholder>
  ///</summary>  </placeholder
///> <remarks/>
    public class ClassName
    {
    }
}";

            var fixedCode = @"namespace FooNamespace
{
   ///   <summary>
     /// Content .
  ///</summary>   <remarks/>
    public class ClassName
    {
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 9),
                this.CSharpDiagnostic().WithLocation(4, 18)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1651DoNotUsePlaceholderElements();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1651CodeFixProvider();
        }
    }
}

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1603DocumentationMustContainValidXml"/>.
    /// </summary>
    public class SA1603UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestTextDocumentationAsync()
        {
            var testCode = @"
/// Foo
public class Foo { }";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyElementDocumentationAsync()
        {
            var testCode = @"
/// <summary/>
public class Foo { }";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestElementDocumentationAsync()
        {
            var testCode = @"
/// <summary></summary>
public class Foo { }";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCDataDocumentationAsync()
        {
            var testCode = @"
/// <![CDATA[Foo]]>
public class Foo { }";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestElementStartTagSkippedTokenAsync()
        {
            var testCode = @"
/// <summary=></summary>
public class Foo { }";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Invalid token.").WithLocation(2, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestElementEndTagSkippedTokenAsync()
        {
            var testCode = @"
/// <summary></summary=>
public class Foo { }";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Invalid token.").WithLocation(2, 23);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyElementSkippedTokenAsync()
        {
            var testCode = @"
/// <summary=/>
public class Foo { }";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Invalid token.").WithLocation(2, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestElementTagsNotMatchingAsync()
        {
            var testCode = @"
/// <summary>a</sumary>
public class Foo { }";

            DiagnosticResult expected =
                this.CSharpDiagnostic().WithArguments("The 'summary' start tag does not match the end tag of 'sumary'.")
                    .WithLocation(2, 5)
                    .WithLocation(2, 15);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestElementMissingEndTagAsync()
        {
            var testCode = @"
/// <summary>a
public class Foo { }";

            DiagnosticResult expected =
                this.CSharpDiagnostic().WithArguments("The XML tag 'summary' is not closed.")
                    .WithLocation(2, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1603DocumentationMustContainValidXml();
        }
    }
}

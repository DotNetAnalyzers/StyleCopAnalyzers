namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1603DocumentationMustContainValidXml"/>-
    /// </summary>
    public class SA1603UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTextDocumentation()
        {
            var testCode = @"
/// Foo
public class Foo { }";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyElementDocumentation()
        {
            var testCode = @"
/// <summary/>
public class Foo { }";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestElementDocumentation()
        {
            var testCode = @"
/// <summary></summary>
public class Foo { }";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCDataDocumentation()
        {
            var testCode = @"
/// <![CDATA[Foo]]>
public class Foo { }";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestElementStartTagSkippedToken()
        {
            var testCode = @"
/// <summary=></summary>
public class Foo { }";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Invalid token.").WithLocation(2, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestElementEndTagSkippedToken()
        {
            var testCode = @"
/// <summary></summary=>
public class Foo { }";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Invalid token.").WithLocation(2, 23);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyElementSkippedToken()
        {
            var testCode = @"
/// <summary=/>
public class Foo { }";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Invalid token.").WithLocation(2, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestElementTagsNotMatching()
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
        public async Task TestElementMissingEndTag()
        {
            var testCode = @"
/// <summary>a
public class Foo { }";

            DiagnosticResult expected =
                this.CSharpDiagnostic().WithArguments("The XML tag 'summary' is not closed.")
                    .WithLocation(2, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1603DocumentationMustContainValidXml();
        }
    }
}

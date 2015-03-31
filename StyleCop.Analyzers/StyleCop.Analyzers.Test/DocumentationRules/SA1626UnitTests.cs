namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using StyleCop.Analyzers.DocumentationRules;
    using Xunit;

    public class SA1626UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1626SingleLineCommentsMustNotUseDocumentationStyleSlashes.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestClassWithXmlComment()
        {
            var testCode = @"/// <summary>
/// Xml Documentation
/// </summary>
public class Foo
{
    public void Bar()
    {
    }
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithComment()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        // This is a comment
    }
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithOneLineThreeSlashComment()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        /// This is a comment
    }
}
";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(5, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithMultiLineThreeSlashComment()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        /// This is
        /// a comment
    }
}
";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(5, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithCodeComments()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        //// System.Console.WriteLine(""Bar"")
    }
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithSingeLineDocumentation()
        {
            var testCode = @"public class Foo
{
    /// <summary>Summary text</summary>
    public void Bar()
    {
    }
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1626SingleLineCommentsMustNotUseDocumentationStyleSlashes();
        }
    }
}
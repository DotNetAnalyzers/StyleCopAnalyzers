namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;


    /// <summary>
    /// This class contains unit tests for <see cref="SA1613ElementParameterDocumentationMustDeclareParameterName"/>.
    /// </summary>
    public class SA1613UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1613ElementParameterDocumentationMustDeclareParameterName.DiagnosticId;

        public static IEnumerable<object[]> Declarations
        {
            get
            {
                yield return new[] { "    public ClassName Method(string foo, string bar) { return null; }" };
                yield return new[] { "    public ClassName this[string foo, string bar] { get { return null; } set { } }" };
            }
        }

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMemberWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ClassName Method() { return null; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithoutParams(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
$$
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithValidParams(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    ///<param name=""foo"">Test</param>
    ///<param name=""bar"">Test</param>
$$
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithInvalidParams(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    ///<param>Test</param>
    ///<param/>
    ///<param name="""">Test</param>
    ///<param name=""    "">Test</param>
$$
}";

        var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 8),
                this.CSharpDiagnostic().WithLocation(11, 8),
                this.CSharpDiagnostic().WithLocation(12, 15),
                this.CSharpDiagnostic().WithLocation(13, 15)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1613ElementParameterDocumentationMustDeclareParameterName();
        }
    }
}


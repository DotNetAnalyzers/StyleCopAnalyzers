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
    /// This class contains unit tests for <see cref="SA1611ElementParametersMustBeDocumented"/>-
    /// </summary>
    public class SA1611UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1611ElementParametersMustBeDocumented.DiagnosticId;

        public static IEnumerable<object[]> Data
        {
            get
            {
                // These method names are choosen so that the position of the parameters are always the same. This makes testing easier
                yield return new object[] { "void Foooooooooooo(string param1, string param2, string param3) { }" };
                yield return new object[] { "delegate void Fooo(string param1, string param2, string param3);" };
                yield return new object[] { "System.String this[string param1, string param2, string param3] { get { return param1; } }" };
            }
        }

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithAllDocumentation(string p)
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
    /// <param name=""param1"">Param 1</param>
    /// <param name=""param2""></param>
    /// <param name=""param3"">Param 3</param>
    public ##
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithAllDocumentationWrongOrder(string p)
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
    /// <param name=""param1"">Param 1</param>
    /// <param name=""param3"">Param 3</param>
    /// <param name=""param2""></param>
    public ##
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithNoDocumentation(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ##
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestInheritDoc(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ##
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestMissingParameters(string p)
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
    public ##
}";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 38),
                this.CSharpDiagnostic().WithLocation(10, 53),
                this.CSharpDiagnostic().WithLocation(10, 68),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1611ElementParametersMustBeDocumented();
        }
    }
}

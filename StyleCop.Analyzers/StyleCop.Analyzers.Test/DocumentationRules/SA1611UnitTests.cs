namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.DocumentationRules;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1611ElementParametersMustBeDocumented"/>.
    /// </summary>
    public class SA1611UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1611ElementParametersMustBeDocumented.DiagnosticId;

        public static IEnumerable<object[]> Data
        {
            get
            {
                // These method names are chosen so that the position of the parameters are always the same. This makes testing easier
                yield return new object[] { "void Foooooooooooo(string param1, string param2, string param3) { }" };
                yield return new object[] { "delegate void Fooo(string param1, string param2, string param3);" };
                yield return new object[] { "System.String this[string param1, string param2, string param3] { get { return param1; } }" };
                yield return new object[] { "void Foooooooooooo(string param1, string param2, string @param3) { }" };
                yield return new object[] { "delegate void Fooo(string param1, string param2, string @param3);" };
                yield return new object[] { "System.String this[string param1, string param2, string @param3] { get { return param1; } }" };
                yield return new object[] { "void Foooooooooooo(string param1, string param2, string p\\u0061ram3) { }" };
                yield return new object[] { "delegate void Fooo(string param1, string param2, string p\\u0061ram3);" };
                yield return new object[] { "System.String this[string param1, string param2, string p\\u0061ram3] { get { return param1; } }" };
            }
        }

        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithAllDocumentationAsync(string p)
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithAllDocumentationAlternativeSyntaxAsync(string p)
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
    /// <param name=""p&#97;ram2""></param>
    /// <param name=""p&#x61;ram3"">Param 3</param>
    public ##
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithAllDocumentationWrongOrderAsync(string p)
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithNoDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ##
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestInheritDocAsync(string p)
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestMissingParametersAsync(string p)
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
                this.CSharpDiagnostic().WithLocation(10, 38).WithArguments("param1"),
                this.CSharpDiagnostic().WithLocation(10, 53).WithArguments("param2"),
                this.CSharpDiagnostic().WithLocation(10, 68).WithArguments("param3"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1611ElementParametersMustBeDocumented();
        }
    }
}

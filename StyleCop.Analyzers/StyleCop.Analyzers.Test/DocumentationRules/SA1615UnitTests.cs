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
    /// This class contains unit tests for <see cref="SA1615ElementReturnValueMustBeDocumented"/>.
    /// </summary>
    public class SA1615UnitTests : DiagnosticVerifier
    {
        public static IEnumerable<object[]> WithReturnValue
        {
            get
            {
                yield return new[] { "    public          ClassName Method(string foo, string bar) { return null; }" };
                yield return new[] { "    public delegate ClassName Method(string foo, string bar);" };
            }
        }
        public static IEnumerable<object[]> WithoutReturnValue
        {
            get
            {
                yield return new[] { "    public void Method(string foo, string bar) { }" };
                yield return new[] { "    public delegate void Method(string foo, string bar);" };
            }
        }


        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(WithReturnValue))]
        [MemberData(nameof(WithoutReturnValue))]
        public async Task TestMethodWithoutDocumentation(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
$$
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(WithoutReturnValue))]
        public async Task TestMethodWithReturnTypeWithoutReturnTypeDocumentation(string declaration)
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
        [MemberData(nameof(WithoutReturnValue))]
        public async Task TestMethodWithVoidWithDocumentation(string declaration)
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
    /// <returns>Foo</returns>
$$
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(WithReturnValue))]
        public async Task TestMethodWithoutVoidWithoutDocumentation(string declaration)
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
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 21)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None);
        }
        
        [Theory]
        [MemberData(nameof(WithReturnValue))]
        public async Task TestMethodWithoutVoidWithDocumentation(string declaration)
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
    /// <returns>Foo</returns>
$$
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(WithReturnValue))]
        [MemberData(nameof(WithoutReturnValue))]
        public async Task TestMethodWithInheritedDocumentation(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ClassName Test() { return null; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1615ElementReturnValueMustBeDocumented();
        }
    }
}

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1617VoidReturnValueMustNotBeDocumented"/>-
    /// </summary>
    public class SA1617UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1617VoidReturnValueMustNotBeDocumented.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithReturnValueNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    public ClassName Method() { return null; }

    /// <value>
    /// Foo
    /// </value>
    public delegate ClassName MethodDelegate();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithReturnValueWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    /// <returns>null</returns>
    public ClassName Method() { return null; }

    /// <value>
    /// Foo
    /// </value>
    /// <returns>Some value</returns>
    public delegate ClassName MethodDelegate();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ClassName Method() { return null; }

    public delegate ClassName MethodDelegate();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ClassName Method() { return null; }

    /// <inheritdoc/>
    public delegate ClassName MethodDelegate();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithoutReturnValueNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    public void Method() { }

    /// <value>
    /// Foo
    /// </value>
    public delegate void MethodDelegate();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithoutReturnValueWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    /// <returns>null</returns>
    public void Method() { }

    /// <value>
    /// Foo
    /// </value>
    /// <returns>Some value</returns>
    public delegate void MethodDelegate();
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 9),
                this.CSharpDiagnostic().WithLocation(16, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1617VoidReturnValueMustNotBeDocumented();
        }
    }
}

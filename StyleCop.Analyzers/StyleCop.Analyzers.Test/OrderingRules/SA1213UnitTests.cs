namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    public class SA1213UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1213EventAccessorsMustFollowOrder.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAddAccessorAfterRemoveAccessor()
        {
            var testCode = @"
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        remove { this.nameChanged -= value; }
        add { this.nameChanged += value; }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestAddAccessorAfterRemoveAccessorSameLine()
        {
            var testCode = @"
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        remove { this.nameChanged -= value; } add { this.nameChanged += value; }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestAddAccessorBeforeRemoveAccessor()
        {
            var testCode = @"
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        add { this.nameChanged += value; }
        remove { this.nameChanged -= value; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAddAccessorBeforeRemoveAccessorSameLine()
        {
            var testCode = @"
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        add { this.nameChanged += value; } remove { this.nameChanged -= value; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1213EventAccessorsMustFollowOrder();
        }
    }
}
namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;

    public class SA1212UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1212PropertyAccessorsMustFollowOrder.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyWithBackingFieldDeclarationSetterBeforeGetter()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        set
        {
            i = value;
        }

        get
        {
            return i;
        }
    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = this.DiagnosticId,
                    Message = "A get accessor appears after a set accessor within a property or indexer.",
                    Severity =  DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 8, 9)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyWithBackingFieldDeclarationGetterBeforeSetter()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        get
        {
            return i;
        }

        set
        {
            i = value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyWithBackingFieldDeclarationOnlyGetter()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        get
        {
            return i;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyWithBackingFieldDeclarationOnlySetter()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        set
        {
            i = value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAutoPropertydDeclarationSetterBeforeGetter()
        {
            var testCode = @"
public class Foo
{
    public int Prop
    {
        set;get;
    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = this.DiagnosticId,
                    Message = "A get accessor appears after a set accessor within a property or indexer.",
                    Severity =  DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 6, 9)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerDeclarationSetterBeforeGetter()
        {
            var testCode = @"
public class Foo
{
    private int field;

    public int this[int index]
    {
        set
        {
            field = value;
        }

        get
        {
            return field;
        }
    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = this.DiagnosticId,
                    Message = "A get accessor appears after a set accessor within a property or indexer.",
                    Severity =  DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 8, 9)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerDeclarationGetterBeforeSetter()
        {
            var testCode = @"
public class Foo
{
    private int field;

    public int this[int index]
    {
        get
        {
            return field;
        }

        set
        {
            field = value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerDeclarationOnlySetter()
        {
            var testCode = @"
public class Foo
{
    private int field;

    public int this[int index]
    {
        set
        {
            field = value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerDeclarationOnlyGetter()
        {
            var testCode = @"
public class Foo
{
    private int field;

    public int this[int index]
    {
        get
        {
            return field;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1212PropertyAccessorsMustFollowOrder();
        }
    }
}

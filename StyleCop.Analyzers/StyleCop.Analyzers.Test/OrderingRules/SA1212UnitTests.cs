using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.OrderingRules;
using TestHelper;

namespace StyleCop.Analyzers.Test.OrderingRules
{
    [TestClass]
    public class SA1212UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1212PropertyAccessorsMustFollowOrder.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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
                    Id = DiagnosticId,
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
                    Id = DiagnosticId,
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

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1212PropertyAccessorsMustFollowOrder();
        }
    }
}
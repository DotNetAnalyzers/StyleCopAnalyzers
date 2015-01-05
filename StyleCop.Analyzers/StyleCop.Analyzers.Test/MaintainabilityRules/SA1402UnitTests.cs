using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    [TestClass]
    public class SA1402UnitTests : FileMayOnlyContainTestBase
    {

        public override string Keyword
        {
            get
            {
                return "class";
            }
        }

        public override string DiagnosticId
        {
            get
            {
                return SA1402FileMayOnlyContainASingleClass.DiagnosticId;
            }
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1402FileMayOnlyContainASingleClass();
        }

        [TestMethod]
        public async Task TestPartialClasses()
        {
            var testCode = @"public partial class Foo
{
}
public partial class Foo
{

}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);

        }

        [TestMethod]
        public async Task TestDifferentPartialClasses()
        {
            var testCode = @"public partial class Foo
{
}
public partial class Bar
{

}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = Message,
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 22)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

        }

        [TestMethod]
        public async Task TestNestedClasses()
        {
            var testCode = @"public class Foo
{
    public class Bar
    {
    
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);

        }
    }
}
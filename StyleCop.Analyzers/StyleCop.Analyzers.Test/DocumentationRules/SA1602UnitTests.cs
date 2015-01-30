namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1602EnumerationItemsMustBeDocumented"/>-
    /// </summary>
    [TestClass]
    public class SA1602UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1602EnumerationItemsMustBeDocumented.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEnumWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
enum TypeName
{{
    /// <summary>
    /// Some Documentation
    /// </summary>
    Bar
}}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEnumWithoutDocumentation()
        {
            var testCode = @"
enum TypeName
{{
    Bar
}}";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Enumeration items must be documented",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 4, 5)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEnumWithEmptyDocumentation()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
enum TypeName
{{
    /// <summary>
    /// 
    /// </summary>
    Bar
}}";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Enumeration items must be documented",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 10, 5)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1602EnumerationItemsMustBeDocumented();
        }
    }
}

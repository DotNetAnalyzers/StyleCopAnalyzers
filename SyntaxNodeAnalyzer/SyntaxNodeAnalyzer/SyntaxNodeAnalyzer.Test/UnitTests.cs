using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using SyntaxNodeAnalyzer;

namespace SyntaxNodeAnalyzer.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            static void Main(string[] args)
            {
                if                 (true)
                {
                    Console.WriteLine(""Hello World"");    
                }
            }          
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = SyntaxNodeAnalyzerAnalyzer.spacingRuleId,
                Message = String.Format("If statements must contain a space between the 'if' keyword and the boolean expression)"), //make sure this message matches the original message
                Severity = DiagnosticSeverity.Warning, //make sure this matches the original
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 17)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            static void Main(string[] args)
            {
                if (true)
                {
                    Console.WriteLine(""Hello World"");    
                }
            }          
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SyntaxNodeAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SyntaxNodeAnalyzerAnalyzer();
        }
    }
}
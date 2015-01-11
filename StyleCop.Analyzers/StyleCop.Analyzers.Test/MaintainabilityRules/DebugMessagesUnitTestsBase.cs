namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestHelper;

    public abstract class DebugMessagesUnitTestsBase : CodeFixVerifier
    {
        protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };

        protected abstract string DiagnosticId
        {
            get;
        }

        protected abstract string MethodName
        {
            get;
        }

        protected abstract IEnumerable<string> InitialArguments
        {
            get;
        }

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestWhitespaceMessage()
        {
            var testCodeFormat = @"using System.Diagnostics;
public class Foo
{{
    public void Bar()
    {{
        Debug.{0}({1}""     "");
    }}
}}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = string.Format("Debug.{0} must provide message text", MethodName),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 6, 9)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(BuildTestCode(testCodeFormat), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestNullMessage()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{{
    public void Bar()
    {{
        Debug.{0}({1}null);
    }}
}}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = string.Format("Debug.{0} must provide message text", MethodName),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 6, 9)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(BuildTestCode(testCode), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstantMessage()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{{
    public void Bar()
    {{
        Debug.{0}({1}""A message"");
    }}
}}";

            await VerifyCSharpDiagnosticAsync(BuildTestCode(testCode), EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestNotConstantMessage()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{{
    public void Bar()
    {{
        Debug.{0}({1}message);
    }}
}}";

            await VerifyCSharpDiagnosticAsync(BuildTestCode(testCode), EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestWrongDebugClass()
        {
            var testCode = @"
public class Foo
{{
    public void Bar()
    {{
        string message = ""A message"";
        Debug.{0}({1}message);
    }}
}}
class Debug
{{
    public static void Assert(bool b, string s)
    {{

    }}
}}
";

            await VerifyCSharpDiagnosticAsync(BuildTestCode(testCode), EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestWrongMethod()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{{
    public void Bar()
    {{
        Debug.Write(null);
    }}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected virtual string BuildTestCode(string format)
        {
            StringBuilder argumentList = new StringBuilder();
            foreach (var argument in InitialArguments)
            {
                if (argumentList.Length > 0)
                    argumentList.Append(", ");

                argumentList.Append(argument);
            }

            if (argumentList.Length > 0)
                argumentList.Append(", ");

            return string.Format(format, MethodName, argumentList);
        }
    }
}

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
        public async Task TestConstantMessage_Field_Pass()
        {
            await TestConstantMessage_Field_Pass("\" foo \"");
        }

        [TestMethod]
        public async Task TestConstantMessage_Field_PassExpression()
        {
            await TestConstantMessage_Field_Pass("\" \" + \"foo\" + \" \"");
        }

        [TestMethod]
        public async Task TestConstantMessage_Field_PassWrongType()
        {
            await TestConstantMessage_Field_Pass("3");
        }

        [TestMethod]
        public async Task TestConstantMessage_Local_Pass()
        {
            await TestConstantMessage_Local_Pass("\" foo \"");
        }

        [TestMethod]
        public async Task TestConstantMessage_Local_PassExpression()
        {
            await TestConstantMessage_Local_Pass("\" \" + \"foo\" + \" \"");
        }

        [TestMethod]
        public async Task TestConstantMessage_Local_PassWrongType()
        {
            await TestConstantMessage_Local_Pass("3");
        }

        [TestMethod]
        public async Task TestConstantMessage_Inline_Pass()
        {
            await TestConstantMessage_Inline_Pass("\" foo \"");
        }

        [TestMethod]
        public async Task TestConstantMessage_Inline_PassExpression()
        {
            await TestConstantMessage_Inline_Pass("\" \" + \"foo\" + \" \"");
        }

        [TestMethod]
        public async Task TestConstantMessage_Inline_PassWrongType()
        {
            await TestConstantMessage_Inline_Pass("3");
        }

        [TestMethod]
        public async Task TestConstantMessage_Field_FailNull()
        {
            await TestConstantMessage_Field_Fail("null");
        }

        [TestMethod]
        public async Task TestConstantMessage_Field_FailEmpty()
        {
            await TestConstantMessage_Field_Fail("\"\"");
        }

        [TestMethod]
        public async Task TestConstantMessage_Field_FailWhitespace()
        {
            await TestConstantMessage_Field_Fail("\"  \"");
        }

        [TestMethod]
        public async Task TestConstantMessage_Field_FailExpressionWhitespace()
        {
            await TestConstantMessage_Field_Fail("\"  \" + \"  \"");
        }

        [TestMethod]
        public async Task TestConstantMessage_Local_FailNull()
        {
            await TestConstantMessage_Local_Fail("null");
        }

        [TestMethod]
        public async Task TestConstantMessage_Local_FailEmpty()
        {
            await TestConstantMessage_Local_Fail("\"\"");
        }

        [TestMethod]
        public async Task TestConstantMessage_Local_FailWhitespace()
        {
            await TestConstantMessage_Local_Fail("\"  \"");
        }

        [TestMethod]
        public async Task TestConstantMessage_Local_FailExpressionWhitespace()
        {
            await TestConstantMessage_Local_Fail("\"  \" + \"  \"");
        }

        [TestMethod]
        public async Task TestConstantMessage_Inline_FailNull()
        {
            await TestConstantMessage_Inline_Fail("null");
        }

        [TestMethod]
        public async Task TestConstantMessage_Inline_FailEmpty()
        {
            await TestConstantMessage_Inline_Fail("\"\"");
        }

        [TestMethod]
        public async Task TestConstantMessage_Inline_FailWhitespace()
        {
            await TestConstantMessage_Inline_Fail("\"  \"");
        }

        [TestMethod]
        public async Task TestConstantMessage_Inline_FailExpressionWhitespace()
        {
            await TestConstantMessage_Inline_Fail("\"  \" + \"  \"");
        }

        private async Task TestConstantMessage_Field_Pass(string argument)
        {
            var testCodeFormat = @"using System.Diagnostics;
public class Foo
{{{{
    const string message = {{0}};
    public void Bar()
    {{{{
        Debug.{0}({1}message);
    }}}}
}}}}";

            await VerifyCSharpDiagnosticAsync(string.Format(BuildTestCode(testCodeFormat), argument), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestConstantMessage_Local_Pass(string argument)
        {
            var testCodeFormat = @"using System.Diagnostics;
public class Foo
{{{{
    public void Bar()
    {{{{
        const string message = {{0}};
        Debug.{0}({1}message);
    }}}}
}}}}";

            await VerifyCSharpDiagnosticAsync(string.Format(BuildTestCode(testCodeFormat), argument), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestConstantMessage_Inline_Pass(string argument)
        {
            var testCodeFormat = @"using System.Diagnostics;
public class Foo
{{{{
    public void Bar()
    {{{{
        Debug.{0}({1}{{0}});
    }}}}
}}}}";

            await VerifyCSharpDiagnosticAsync(string.Format(BuildTestCode(testCodeFormat), argument), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestConstantMessage_Field_Fail(string argument)
        {
            var testCodeFormat = @"using System.Diagnostics;
public class Foo
{{{{
    const string message = {{0}};
    public void Bar()
    {{{{
        Debug.{0}({1}message);
    }}}}
}}}}";

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
                                    new DiagnosticResultLocation("Test0.cs", 7, 9)
                                }
                        }
                    };

            await VerifyCSharpDiagnosticAsync(string.Format(BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None);
        }

        private async Task TestConstantMessage_Local_Fail(string argument)
        {
            var testCodeFormat = @"using System.Diagnostics;
public class Foo
{{{{
    public void Bar()
    {{{{
        const string message = {{0}};
        Debug.{0}({1}message);
    }}}}
}}}}";

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
                                    new DiagnosticResultLocation("Test0.cs", 7, 9)
                                }
                        }
                    };

            await VerifyCSharpDiagnosticAsync(string.Format(BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None);
        }

        private async Task TestConstantMessage_Inline_Fail(string argument)
        {
            var testCodeFormat = @"using System.Diagnostics;
public class Foo
{{{{
    public void Bar()
    {{{{
        Debug.{0}({1}{{0}});
    }}}}
}}}}";

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

            await VerifyCSharpDiagnosticAsync(string.Format(BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None);
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

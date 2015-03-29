namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using TestHelper;
    using Xunit;

    public abstract class DebugMessagesUnitTestsBase : CodeFixVerifier
    {
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

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstantMessage_Field_Pass()
        {
            await this.TestConstantMessage_Field_Pass("\" foo \"");
        }

        [Fact]
        public async Task TestConstantMessage_Field_PassExpression()
        {
            await this.TestConstantMessage_Field_Pass("\" \" + \"foo\" + \" \"");
        }

        [Fact]
        public async Task TestConstantMessage_Field_PassWrongType()
        {
            DiagnosticResult[] expected =
            {
                new DiagnosticResult
                {
                    Id = "CS0029",
                    Message = "Cannot implicitly convert type 'int' to 'string'",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 28) }
                }
            };

            await this.TestConstantMessage_Field_Pass("3", expected);
        }

        [Fact]
        public async Task TestConstantMessage_Local_Pass()
        {
            await this.TestConstantMessage_Local_Pass("\" foo \"");
        }

        [Fact]
        public async Task TestConstantMessage_Local_PassExpression()
        {
            await this.TestConstantMessage_Local_Pass("\" \" + \"foo\" + \" \"");
        }

        [Fact]
        public async Task TestConstantMessage_Local_PassWrongType()
        {
            DiagnosticResult[] expected =
            {
                new DiagnosticResult
                {
                    Id = "CS0029",
                    Message = "Cannot implicitly convert type 'int' to 'string'",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 32) }
                }
            };

            await this.TestConstantMessage_Local_Pass("3", expected);
        }

        [Fact]
        public async Task TestConstantMessage_Inline_Pass()
        {
            await this.TestConstantMessage_Inline_Pass("\" foo \"");
        }

        [Fact]
        public async Task TestConstantMessage_Inline_PassExpression()
        {
            await this.TestConstantMessage_Inline_Pass("\" \" + \"foo\" + \" \"");
        }

        [Fact]
        public async Task TestConstantMessage_Inline_PassWrongType()
        {
            DiagnosticResult[] expected =
            {
                new DiagnosticResult
                {
                    Id = "CS1503",
                    Message = $"Argument {1 + this.InitialArguments.Count()}: cannot convert from 'int' to 'string'",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 16 + this.MethodName.Length + this.InitialArguments.Sum(i => i.Length + ", ".Length)) }
                }
            };

            await this.TestConstantMessage_Inline_Pass("3", expected);
        }

        [Fact]
        public async Task TestConstantMessage_Field_FailNull()
        {
            await this.TestConstantMessage_Field_Fail("null");
        }

        [Fact]
        public async Task TestConstantMessage_Field_FailEmpty()
        {
            await this.TestConstantMessage_Field_Fail("\"\"");
        }

        [Fact]
        public async Task TestConstantMessage_Field_FailWhitespace()
        {
            await this.TestConstantMessage_Field_Fail("\"  \"");
        }

        [Fact]
        public async Task TestConstantMessage_Field_FailExpressionWhitespace()
        {
            await this.TestConstantMessage_Field_Fail("\"  \" + \"  \"");
        }

        [Fact]
        public async Task TestConstantMessage_Local_FailNull()
        {
            await this.TestConstantMessage_Local_Fail("null");
        }

        [Fact]
        public async Task TestConstantMessage_Local_FailEmpty()
        {
            await this.TestConstantMessage_Local_Fail("\"\"");
        }

        [Fact]
        public async Task TestConstantMessage_Local_FailWhitespace()
        {
            await this.TestConstantMessage_Local_Fail("\"  \"");
        }

        [Fact]
        public async Task TestConstantMessage_Local_FailExpressionWhitespace()
        {
            await this.TestConstantMessage_Local_Fail("\"  \" + \"  \"");
        }

        [Fact]
        public async Task TestConstantMessage_Inline_FailNull()
        {
            await this.TestConstantMessage_Inline_Fail("null");
        }

        [Fact]
        public async Task TestConstantMessage_Inline_FailEmpty()
        {
            await this.TestConstantMessage_Inline_Fail("\"\"");
        }

        [Fact]
        public async Task TestConstantMessage_Inline_FailWhitespace()
        {
            await this.TestConstantMessage_Inline_Fail("\"  \"");
        }

        [Fact]
        public async Task TestConstantMessage_Inline_FailExpressionWhitespace()
        {
            await this.TestConstantMessage_Inline_Fail("\"  \" + \"  \"");
        }

        private async Task TestConstantMessage_Field_Pass(string argument, params DiagnosticResult[] expected)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(this.BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None);
        }

        private async Task TestConstantMessage_Local_Pass(string argument, params DiagnosticResult[] expected)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(this.BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None);
        }

        private async Task TestConstantMessage_Inline_Pass(string argument, params DiagnosticResult[] expected)
        {
            var testCodeFormat = @"using System.Diagnostics;
public class Foo
{{{{
    public void Bar()
    {{{{
        Debug.{0}({1}{{0}});
    }}}}
}}}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(this.BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 9);

            await this.VerifyCSharpDiagnosticAsync(string.Format(this.BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 9);

            await this.VerifyCSharpDiagnosticAsync(string.Format(this.BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);

            await this.VerifyCSharpDiagnosticAsync(string.Format(this.BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestNotConstantMessage()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{{
    public void Bar()
    {{
        string message = ""A message"";
        Debug.{0}({1}message);
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(this.BuildTestCode(testCode), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
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
    public static void Fail(string s)
    {{

    }}
}}
";

            await this.VerifyCSharpDiagnosticAsync(this.BuildTestCode(testCode), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestWrongMethod()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar()
    {
        Debug.Write(null);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected virtual string BuildTestCode(string format)
        {
            StringBuilder argumentList = new StringBuilder();
            foreach (var argument in this.InitialArguments)
            {
                if (argumentList.Length > 0)
                {
                    argumentList.Append(", ");
                }

                argumentList.Append(argument);
            }

            if (argumentList.Length > 0)
            {
                argumentList.Append(", ");
            }

            return string.Format(format, this.MethodName, argumentList);
        }
    }
}

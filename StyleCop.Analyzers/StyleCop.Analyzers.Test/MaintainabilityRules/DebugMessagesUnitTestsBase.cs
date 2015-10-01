// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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

    public abstract class DebugMessagesUnitTestsBase : DiagnosticVerifier
    {
        protected bool IncludeSystemDll { get; set; } = true;

        protected abstract string MethodName
        {
            get;
        }

        protected abstract IEnumerable<string> InitialArguments
        {
            get;
        }

        [Fact]
        public async Task TestConstantMessage_Field_PassAsync()
        {
            await this.TestConstantMessage_Field_PassAsync("\" foo \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Field_PassExpressionAsync()
        {
            await this.TestConstantMessage_Field_PassAsync("\" \" + \"foo\" + \" \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Field_PassWrongTypeAsync()
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

            await this.TestConstantMessage_Field_PassAsync("3", expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Local_PassAsync()
        {
            await this.TestConstantMessage_Local_PassAsync("\" foo \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Local_PassExpressionAsync()
        {
            await this.TestConstantMessage_Local_PassAsync("\" \" + \"foo\" + \" \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Local_PassWrongTypeAsync()
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

            await this.TestConstantMessage_Local_PassAsync("3", expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Inline_PassAsync()
        {
            await this.TestConstantMessage_Inline_PassAsync("\" foo \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Inline_PassExpressionAsync()
        {
            await this.TestConstantMessage_Inline_PassAsync("\" \" + \"foo\" + \" \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Inline_PassWrongTypeAsync()
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

            await this.TestConstantMessage_Inline_PassAsync("3", expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Field_FailNullAsync()
        {
            await this.TestConstantMessage_Field_FailAsync("null").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Field_FailEmptyAsync()
        {
            await this.TestConstantMessage_Field_FailAsync("\"\"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Field_FailWhitespaceAsync()
        {
            await this.TestConstantMessage_Field_FailAsync("\"  \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Field_FailExpressionWhitespaceAsync()
        {
            await this.TestConstantMessage_Field_FailAsync("\"  \" + \"  \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Local_FailNullAsync()
        {
            await this.TestConstantMessage_Local_FailAsync("null").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Local_FailEmptyAsync()
        {
            await this.TestConstantMessage_Local_FailAsync("\"\"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Local_FailWhitespaceAsync()
        {
            await this.TestConstantMessage_Local_FailAsync("\"  \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Local_FailExpressionWhitespaceAsync()
        {
            await this.TestConstantMessage_Local_FailAsync("\"  \" + \"  \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Inline_FailNullAsync()
        {
            await this.TestConstantMessage_Inline_FailAsync("null").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Inline_FailEmptyAsync()
        {
            await this.TestConstantMessage_Inline_FailAsync("\"\"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Inline_FailWhitespaceAsync()
        {
            await this.TestConstantMessage_Inline_FailAsync("\"  \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Inline_FailExpressionWhitespaceAsync()
        {
            await this.TestConstantMessage_Inline_FailAsync("\"  \" + \"  \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNotConstantMessageAsync()
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

            await this.VerifyCSharpDiagnosticAsync(this.BuildTestCode(testCode), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWrongDebugClassAsync()
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

            await this.VerifyCSharpDiagnosticAsync(this.BuildTestCode(testCode), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWrongMethodAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar()
    {
        Debug.Write(null);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDifferentIdentifiersAsync()
        {
            var testCode = @"using System.Diagnostics;
using static System.Diagnostics.Debug;

public class Foo
{{
    public void Bar()
    {{
        Debug.{0}({1}"""");
        System.Diagnostics.Debug.{0}({1}"""");
        Debug.{0}({1}"""");
        global::System.Diagnostics.Debug.{0}({1}"""");
        {0}({1}"""");
    }}
}}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(8, 9),
                this.CSharpDiagnostic().WithLocation(9, 9),
                this.CSharpDiagnostic().WithLocation(10, 9),
                this.CSharpDiagnostic().WithLocation(11, 9),
                this.CSharpDiagnostic().WithLocation(12, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(this.BuildTestCode(testCode), expected, CancellationToken.None).ConfigureAwait(false);
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

        protected override Solution CreateSolution(ProjectId projectId, string language)
        {
            Solution solution = base.CreateSolution(projectId, language);

            if (this.IncludeSystemDll)
            {
                return solution;
            }
            else
            {
                IEnumerable<MetadataReference> references = solution.Projects.First().MetadataReferences;

                return solution.WithProjectMetadataReferences(solution.ProjectIds[0], references.Where(x => !x.Display.Contains("System.dll")));
            }
        }

        private async Task TestConstantMessage_Field_PassAsync(string argument, params DiagnosticResult[] expected)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(this.BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestConstantMessage_Local_PassAsync(string argument, params DiagnosticResult[] expected)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(this.BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestConstantMessage_Inline_PassAsync(string argument, params DiagnosticResult[] expected)
        {
            var testCodeFormat = @"using System.Diagnostics;
public class Foo
{{{{
    public void Bar()
    {{{{
        Debug.{0}({1}{{0}});
    }}}}
}}}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(this.BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestConstantMessage_Field_FailAsync(string argument)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(this.BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestConstantMessage_Local_FailAsync(string argument)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(this.BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestConstantMessage_Inline_FailAsync(string argument)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(this.BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

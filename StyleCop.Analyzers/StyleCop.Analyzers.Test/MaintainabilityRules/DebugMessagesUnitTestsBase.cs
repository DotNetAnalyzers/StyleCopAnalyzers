// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;

    public abstract class DebugMessagesUnitTestsBase
    {
        protected abstract string MethodName
        {
            get;
        }

        protected abstract IEnumerable<string> InitialArguments
        {
            get;
        }

        protected abstract DiagnosticAnalyzer Analyzer { get; }

        [Fact]
        public async Task TestConstantMessage_Field_PassAsync()
        {
            await this.TestConstantMessage_Field_PassExecuterAsync("\" foo \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Field_PassExpressionAsync()
        {
            await this.TestConstantMessage_Field_PassExecuterAsync("\" \" + \"foo\" + \" \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Field_PassWrongTypeAsync()
        {
            var startLinePosition = new LinePosition(3, 27);
            var endLinePosition = new LinePosition(3, 28);
            DiagnosticResult[] expected =
            {
                DiagnosticResult.CompilerError("CS0029").WithSpan(new FileLinePositionSpan("/0/Test0.cs", startLinePosition, endLinePosition)).WithMessage("Cannot implicitly convert type 'int' to 'string'"),
            };

            await this.TestConstantMessage_Field_PassExecuterAsync("3", expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Local_PassAsync()
        {
            await this.TestConstantMessage_Local_PassExecuterAsync("\" foo \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Local_PassExpressionAsync()
        {
            await this.TestConstantMessage_Local_PassExecuterAsync("\" \" + \"foo\" + \" \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Local_PassWrongTypeAsync()
        {
            var startLinePosition = new LinePosition(5, 31);
            var endLinePosition = new LinePosition(5, 32);
            DiagnosticResult[] expected =
            {
                DiagnosticResult.CompilerError("CS0029").WithSpan(new FileLinePositionSpan("/0/Test0.cs", startLinePosition, endLinePosition)).WithMessage("Cannot implicitly convert type 'int' to 'string'"),
            };

            await this.TestConstantMessage_Local_PassExecuterAsync("3", expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Inline_PassAsync()
        {
            await this.TestConstantMessage_Inline_PassExecuterAsync("\" foo \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Inline_PassExpressionAsync()
        {
            await this.TestConstantMessage_Inline_PassExecuterAsync("\" \" + \"foo\" + \" \"").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstantMessage_Inline_PassWrongTypeAsync()
        {
            var startLinePosition = new LinePosition(5, 15 + this.MethodName.Length + this.InitialArguments.Sum(i => i.Length + ", ".Length));
            var endLinePosition = new LinePosition(startLinePosition.Line, startLinePosition.Character + 1);
            DiagnosticResult[] expected =
            {
                DiagnosticResult.CompilerError("CS1503").WithSpan(new FileLinePositionSpan("/0/Test0.cs", startLinePosition, endLinePosition)).WithMessage($"Argument {1 + this.InitialArguments.Count()}: cannot convert from 'int' to 'string'"),
            };

            await this.TestConstantMessage_Inline_PassExecuterAsync("3", expected).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(this.BuildTestCode(testCode), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(this.BuildTestCode(testCode), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.Diagnostic().WithLocation(8, 9),
                this.Diagnostic().WithLocation(9, 9),
                this.Diagnostic().WithLocation(10, 9),
                this.Diagnostic().WithLocation(11, 9),
                this.Diagnostic().WithLocation(12, 9),
            };

            await this.VerifyCSharpDiagnosticAsync(this.BuildTestCode(testCode), expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected DiagnosticResult Diagnostic()
            => new DiagnosticResult(this.Analyzer.SupportedDiagnostics.Single());

        protected Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => this.VerifyCSharpDiagnosticAsync(source, new[] { expected }, includeSystemDll: true, cancellationToken);

        protected Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => this.VerifyCSharpDiagnosticAsync(source, expected, includeSystemDll: true, cancellationToken);

        protected Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, bool includeSystemDll, CancellationToken cancellationToken)
        {
            var test = new CSharpTest(this)
            {
                TestCode = source,
            };

            if (!includeSystemDll)
            {
                test.SolutionTransforms.Add((solution, projectId) =>
                {
                    var references = solution.GetProject(projectId).MetadataReferences;
                    return solution.WithProjectMetadataReferences(projectId, references.Where(x => !x.Display.Contains("System.dll")));
                });
            }

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        protected virtual string BuildTestCode(string format)
        {
            StringBuilder argumentList = new StringBuilder();
            argumentList.Append(string.Join(", ", this.InitialArguments));
            if (argumentList.Length > 0)
            {
                argumentList.Append(", ");
            }

            return string.Format(format, this.MethodName, argumentList);
        }

        private async Task TestConstantMessage_Field_PassExecuterAsync(string argument, params DiagnosticResult[] expected)
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

        private async Task TestConstantMessage_Local_PassExecuterAsync(string argument, params DiagnosticResult[] expected)
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

        private async Task TestConstantMessage_Inline_PassExecuterAsync(string argument, params DiagnosticResult[] expected)
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

            DiagnosticResult expected = this.Diagnostic().WithLocation(7, 9);
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

            DiagnosticResult expected = this.Diagnostic().WithLocation(7, 9);
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

            DiagnosticResult expected = this.Diagnostic().WithLocation(6, 9);
            await this.VerifyCSharpDiagnosticAsync(string.Format(this.BuildTestCode(testCodeFormat), argument), expected, CancellationToken.None).ConfigureAwait(false);
        }

        private class CSharpTest : StyleCopDiagnosticVerifier<EmptyDiagnosticAnalyzer>.CSharpTest
        {
            private readonly DebugMessagesUnitTestsBase testFixture;

            public CSharpTest(DebugMessagesUnitTestsBase testFixture)
            {
                this.testFixture = testFixture;
            }

            protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
            {
                yield return this.testFixture.Analyzer;
            }
        }
    }
}

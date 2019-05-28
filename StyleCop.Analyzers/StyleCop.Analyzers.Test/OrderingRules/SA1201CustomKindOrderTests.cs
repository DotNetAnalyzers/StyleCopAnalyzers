// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SA1201ElementsMustAppearInTheCorrectOrder"/> when configured with
    /// a custom <see cref="SyntaxKind"/> order in the <see cref="KindOrderingSettings"/>.
    /// </summary>
    public class SA1201CustomKindOrderTests
    {
        /// <summary>
        /// Verifies that a custom kind order within a type declaration produces expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTypeDeclarationCustomerOrderAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""orderingRules"": {
      ""kindOrder"": {
        ""typeDeclaration"": [ ""field"", ""property"", ""constructor"", ""method"" ]
      }
    }
  }
}
";
            var testCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        private int intField;
        
        public int IntProperty { get; set; }

        public TestClass()
        {
        }

        public int IntMethod()
        {
            return 0;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None, settings).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(testCode, Diagnostic(10, 16, "constructor", "property"), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a custom kind order within a namespace declaration produces expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNamespaceDeclarationCustomerOrderAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""orderingRules"": {
      ""kindOrder"": {
        ""namespaceDeclaration"": [ ""enum"", ""interface"", ""class"", ""struct"" ]
      }
    }
  }
}
";
            var testCode = @"
namespace TestNamespace
{
    public class TestClass
    {
    }
    
    public struct TestStruct
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None, settings).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(testCode, Diagnostic(8, 19, "struct", "class"), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a custom kind order within a compilation unit produces expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TesCompilationUnitCustomerOrderAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""orderingRules"": {
      ""kindOrder"": {
        ""compilationUnit"": [ ""enum"", ""interface"", ""class"", ""struct"" ]
      }
    }
  }
}
";
            var testCode = @"
    public class TestClass
    {
    }
    
    public struct TestStruct
    {
    }
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None, settings).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(testCode, Diagnostic(6, 19, "struct", "class"), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a custom kind order within a compilation unit produces expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOmittedKindsAreIgnoredAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""orderingRules"": {
      ""kindOrder"": {
        ""typeDeclaration"": [
          ""constructor"",
          ""destructor"",
          ""delegate"",
          ""event"",
          ""enum"",
          ""interface"",
          ""property"",
          ""indexer"",
          ""conversionOperator"",
          ""operator"",
          ""method"",
          ""struct"",
          ""class""
        ]
      }
    }
  }
}
";
            var fieldDeclaration = @"        private int intField;";
            var testCodeFormat = @"
namespace TestNamespace
{{
    public class TestClass
    {{
        {0}

        public TestClass()
        {{
        }}

        {1}

        public int IntProperty {{ get; set; }}

        {2}

        public int IntMethod()
        {{
            return 0;
        }}

        {3}
    }}
}}
";
            var tests = new[]
            {
                (string.Format(testCodeFormat, fieldDeclaration, string.Empty, string.Empty, string.Empty), DiagnosticResult.EmptyDiagnosticResults),
                (string.Format(testCodeFormat, string.Empty, fieldDeclaration, string.Empty, string.Empty), Diagnostic(12, 29, "field", "constructor")),
                (string.Format(testCodeFormat, string.Empty, string.Empty, fieldDeclaration, string.Empty), Diagnostic(16, 29, "field", "property")),
                (string.Format(testCodeFormat, string.Empty, string.Empty, string.Empty, fieldDeclaration), Diagnostic(23, 29, "field", "method")),
            };

            foreach (var (testCode, expectedResult) in tests)
            {
                await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None, settings).ConfigureAwait(false);
                await VerifyCSharpDiagnosticAsync(testCode, expectedResult, CancellationToken.None).ConfigureAwait(false);
            }
        }

        private static IEnumerable<DiagnosticResult> Diagnostic(int line, int column, params object[] arguments)
        {
            var diagnostic = StyleCopDiagnosticVerifier<SA1201ElementsMustAppearInTheCorrectOrder>
                .Diagnostic()
                .WithLocation(line, column)
                .WithArguments(arguments);

            return Enumerable.Repeat(diagnostic, 1);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, IEnumerable<DiagnosticResult> expected, CancellationToken cancellationToken, string settings = null)
        {
            var test = new StyleCopDiagnosticVerifier<SA1201ElementsMustAppearInTheCorrectOrder>.CSharpTest
            {
                TestCode = source,
                Settings = settings,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}

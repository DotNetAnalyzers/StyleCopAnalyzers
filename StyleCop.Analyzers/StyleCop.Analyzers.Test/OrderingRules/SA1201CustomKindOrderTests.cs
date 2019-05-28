﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
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
            var diagnosticResult = Diagnostic()
                .WithLocation(10, 16)
                .WithArguments("constructor", "property");

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None, settings).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(testCode, diagnosticResult, CancellationToken.None).ConfigureAwait(false);
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
            var diagnosticResult = Diagnostic()
                .WithLocation(8, 19)
                .WithArguments("struct", "class");

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None, settings).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(testCode, diagnosticResult, CancellationToken.None).ConfigureAwait(false);
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
            var diagnosticResult = Diagnostic()
                .WithLocation(6, 19)
                .WithArguments("struct", "class");

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None, settings).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(testCode, diagnosticResult, CancellationToken.None).ConfigureAwait(false);
        }

        private static DiagnosticResult Diagnostic()
        {
            return StyleCopDiagnosticVerifier<SA1201ElementsMustAppearInTheCorrectOrder>.Diagnostic();
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken, string settings = null)
        {
            var test = new StyleCopDiagnosticVerifier<SA1201ElementsMustAppearInTheCorrectOrder>.CSharpTest
            {
                TestCode = source,
                Settings = settings,
            };

            test.ExpectedDiagnostics.Add(expected);
            return test.RunAsync(cancellationToken);
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

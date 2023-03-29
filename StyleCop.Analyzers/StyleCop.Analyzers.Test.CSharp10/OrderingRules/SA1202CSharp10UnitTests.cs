// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp10.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1202ElementsMustBeOrderedByAccess,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public class SA1202CSharp10UnitTests : SA1202CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle ordering within a file-scoped namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileScopedNamespaceClassesAsync()
        {
            var testCode = @"namespace TestNamespace;

enum TestEnum1 { }
public enum {|#0:TestEnum2|} { }
class TestClass1 { }
public class {|#1:TestClass2|} { }
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0).WithArguments("public", "internal"),
                Diagnostic().WithLocation(1).WithArguments("public", "internal"),
            };

            var fixedCode = @"namespace TestNamespace;
public enum TestEnum2 { }

enum TestEnum1 { }
public class TestClass2 { }
class TestClass1 { }
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDefaultAccessModifierOrderWithFileScopedNamespaceAsync()
        {
            string testCode = @"namespace Foo;

public class Class1 { }
internal class Class2 { }
class Class3 { }
internal class Class4 { }
class Class5 { }
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

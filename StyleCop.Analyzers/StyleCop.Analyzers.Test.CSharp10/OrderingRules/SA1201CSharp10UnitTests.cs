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
        StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public class SA1201CSharp10UnitTests : SA1201CSharp9UnitTests
    {
        [Fact]
        public async Task TestOuterOrderCorrectOrderWithFileScopedNamespaceAsync()
        {
            string testCode = @"namespace Foo;

public delegate void bar();
public enum TestEnum { }
public interface IFoo { }
public struct FooStruct { }
public class FooClass { }
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOuterOrderWrongOrderWithFileScopedNamespaceAsync()
        {
            string testCode = @"
namespace Foo;

public enum TestEnum { }
public delegate void {|#0:bar|}();
public interface IFoo { }
public class FooClass { }
public struct {|#1:FooStruct|} { }
";
            var expected = new[]
            {
                Diagnostic().WithLocation(0).WithArguments("delegate", "enum"),
                Diagnostic().WithLocation(1).WithArguments("struct", "class"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOuterOrderWithRecordStructCorrectOrderAsync()
        {
            var testCode = @"namespace Foo { }
public delegate void bar();
public enum TestEnum { }
public interface IFoo { }
public record struct FooStruct { }
public class FooClass { }
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync("namespace OuterNamespace { " + testCode + " }", DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOuterOrderWithRecordStructWrongOrderAsync()
        {
            var testCode = @"
namespace Foo { }
public enum TestEnum { }
public delegate void {|#0:bar|}();
public interface IFoo { }
public class FooClass { }
public record struct {|#1:FooStruct|} { }
";

            var expected = new[]
            {
                Diagnostic().WithLocation(0).WithArguments("delegate", "enum"),
                Diagnostic().WithLocation(1).WithArguments("record struct", "class"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync("namespace OuterNamespace { " + testCode + " }", expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

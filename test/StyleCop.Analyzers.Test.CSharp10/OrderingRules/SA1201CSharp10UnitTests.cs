// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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

    public partial class SA1201CSharp10UnitTests : SA1201CSharp9UnitTests
    {
        [Theory]
        [InlineData("record")]
        [InlineData("record class")]
        public async Task TestConfiguredOrderTreatsRecordsAsClassesAsync(string record)
        {
            var test = new CSharpTest
            {
                Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["class", "struct"] } } }""",
                TestCode = $$"""
                    public class C
                    {
                        public {{record}} R { }
                        public class Nested { }
                        public record struct S { }
                        public struct Other { }
                    }
                    """,
            };
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("record", "A record")]
        [InlineData("record class", "A record")]
        [InlineData("record struct", "A record struct")]
        public async Task TestConfiguredRecordKindCodeFixAsync(string record, string displayName)
        {
            var test = new CSharpTest
            {
                Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["class", "struct", "method"] } } }""",
                TestCode = $$"""
                    public class C
                    {
                        public void M() { }

                        public {{record}} {|#0:R|} { }
                    }
                    """,
                FixedCode = $$"""
                    public class C
                    {
                        public {{record}} R { }

                        public void M() { }
                    }
                    """,
            };
            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithArguments(displayName, "a method"));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

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
                Diagnostic().WithLocation(0).WithArguments("A delegate", "an enum"),
                Diagnostic().WithLocation(1).WithArguments("A struct", "a class"),
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
                Diagnostic().WithLocation(0).WithArguments("A delegate", "an enum"),
                Diagnostic().WithLocation(1).WithArguments("A record struct", "a class"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync("namespace OuterNamespace { " + testCode + " }", expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public partial class SA1201CSharp15UnitTests : SA1201CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that a union declaration is ordered like a struct, so it may precede a class and may be mixed with
        /// structs.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionOrderedLikeStructAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    public interface ITest { }
    public struct Struct1 { }
    public union Union1(int, string);
    public struct Struct2 { }
    public class Class1
    {
        public void Method() { }
        public union NestedUnion(int, string);
        public struct NestedStruct { }
        public class NestedClass { }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a union declaration following a class is reported as a union, and that the code fix moves it
        /// before the class.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionAfterClassAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    public class Class1
    {
    }

    public union {|#0:Union1|}(int, string);
}
";

            var fixedCode = @"
namespace TestNamespace
{
    public union Union1(int, string);

    public class Class1
    {
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("A union", "a class");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a nested union declaration following a nested class is reported, and that a method following
        /// a nested union is reported as following a union.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestNestedUnionOrderingAsync()
        {
            var testCode = @"
public class OuterClass
{
    public union Union1(int, string);

    public void {|#0:Method|}()
    {
    }

    public class NestedClass
    {
    }

    public union {|#1:Union2|}(int, string);
}
";

            var fixedCode = @"
public class OuterClass
{
    public void Method()
    {
    }

    public union Union1(int, string);

    public union Union2(int, string);

    public class NestedClass
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0).WithArguments("A method", "a union"),
                Diagnostic().WithLocation(1).WithArguments("A union", "a class"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that members inside a union body are ordered by kind, that each misplaced member is reported
        /// exactly once, and that the code fix reorders them.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestMembersInsideUnionAsync()
        {
            var testCode = @"
public union Pet(int, string)
{
    public void Feed()
    {
    }

    public int {|#0:Legs|} => 4;
}
";

            var fixedCode = @"
public union Pet(int, string)
{
    public int Legs => 4;

    public void Feed()
    {
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("A property", "a method");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

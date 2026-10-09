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
        StyleCop.Analyzers.OrderingRules.SA1206DeclarationKeywordsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1206CodeFixProvider>;

    public partial class SA1206CSharp15UnitTests : SA1206CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that the C# 15 <c>closed</c> class modifier is ordered like the other non-access modifiers, after
        /// the access modifier.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4183, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4183")]
        public async Task TestClosedModifierAsync()
        {
            var testCode = @"
public closed class Shape
{
}

public closed partial class Shape2
{
}

public closed record class GateState;

internal closed class Shape3
{
}

closed {|#0:public|} class Shape4
{
}

partial class Outer
{
    closed {|#1:private|} class Nested
    {
    }
}
";

            var fixedCode = @"
public closed class Shape
{
}

public closed partial class Shape2
{
}

public closed record class GateState;

internal closed class Shape3
{
}

public closed class Shape4
{
}

partial class Outer
{
    private closed class Nested
    {
    }
}
";

            await new CSharpTest()
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0).WithArguments("public", "closed"),
                    Diagnostic().WithLocation(1).WithArguments("private", "closed"),
                },

                // The reference assemblies don't define IsClosedTypeAttribute yet.
                CompilerDiagnostics = CompilerDiagnostics.None,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the C# 15 <c>safe</c> modifier, used on <see langword="extern"/> members and on fields of
        /// explicit layout types, is ordered like the other non-access modifiers, after the access modifier and
        /// <see langword="static"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4186, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4186")]
        public async Task TestSafeModifierAsync()
        {
            var testCode = @"
using System.Runtime.InteropServices;

public class Native
{
    [DllImport(""native"")]
    public static safe extern int First();

    [DllImport(""native"")]
    public static extern safe int Second();

    [DllImport(""native"")]
    safe {|#0:public|} {|#3:static|} extern int Third();

    [DllImport(""native"")]
    public safe {|#1:static|} extern int Fourth();
}

[StructLayout(LayoutKind.Explicit)]
public struct Layout
{
    [FieldOffset(0)]
    public safe int First;

    [FieldOffset(0)]
    safe {|#2:public|} int Second;
}
";

            var fixedCode = @"
using System.Runtime.InteropServices;

public class Native
{
    [DllImport(""native"")]
    public static safe extern int First();

    [DllImport(""native"")]
    public static extern safe int Second();

    [DllImport(""native"")]
    public static safe extern int Third();

    [DllImport(""native"")]
    public static safe extern int Fourth();
}

[StructLayout(LayoutKind.Explicit)]
public struct Layout
{
    [FieldOffset(0)]
    public safe int First;

    [FieldOffset(0)]
    public safe int Second;
}
";

            await new CSharpTest()
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0).WithArguments("public", "safe"),
                    Diagnostic().WithLocation(3).WithArguments("static", "safe"),
                    Diagnostic().WithLocation(1).WithArguments("static", "safe"),
                    Diagnostic().WithLocation(2).WithArguments("public", "safe"),
                },

                // The safe modifier is still a preview feature and its rules depend on compiler feature flags.
                CompilerDiagnostics = CompilerDiagnostics.None,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that modifiers on a union declaration and on its members are checked, that each misordered modifier
        /// is reported exactly once, and that the code fix reorders them.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionDeclarationAsync()
        {
            // The reference assemblies used by these tests do not define System.Runtime.CompilerServices.IUnion and
            // UnionAttribute, so union declarations produce CS0518 and CS0656, and compiler diagnostics are ignored.
            var testCode = @"
readonly {|#0:public|} union Pet(int, string)
{
    static {|#1:public|} int Count => 0;
}

public readonly partial union Animal(int, string);

public class Outer
{
    partial {|#2:internal|} union NestedPet(int, string);
}
";

            var fixedCode = @"
public readonly union Pet(int, string)
{
    public static int Count => 0;
}

public readonly partial union Animal(int, string);

public class Outer
{
    internal partial union NestedPet(int, string);
}
";

            await new CSharpTest()
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0).WithArguments("public", "readonly"),
                    Diagnostic().WithLocation(1).WithArguments("public", "static"),
                    Diagnostic().WithLocation(2).WithArguments("internal", "partial"),
                },
                CompilerDiagnostics = CompilerDiagnostics.None,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

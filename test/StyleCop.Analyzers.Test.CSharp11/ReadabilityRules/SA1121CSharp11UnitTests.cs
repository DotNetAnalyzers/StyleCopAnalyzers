// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules
{
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1121UseBuiltInTypeAlias,
        StyleCop.Analyzers.ReadabilityRules.SA1121CodeFixProvider>;

    public partial class SA1121CSharp11UnitTests : SA1121CSharp10UnitTests
    {
        /// <summary>
        /// Verifies that fixes do not introduce native-sized integer aliases into incompatible linked projects.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNativeSizedIntegerFixesInIncompatibleLinkedProjectAsync()
        {
            var source = @"
using System;
using System.Runtime.CompilerServices;

class TestClass
{
    private static [|String|] Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + ([|IntPtr|].Size * 2) + ""})"";

    private static unsafe nint GetAddress(object value) => new [|IntPtr|](Unsafe.AsPointer(ref value));

    private static unsafe nuint GetUnsignedAddress(object value) => new [|UIntPtr|](Unsafe.AsPointer(ref value));
}";

            var linkedSource = @"
using System;
using System.Runtime.CompilerServices;

class TestClass
{
    private static [|String|] Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + (IntPtr.Size * 2) + ""})"";

    private static unsafe nint GetAddress(object value) => new IntPtr(Unsafe.AsPointer(ref value));

    private static unsafe nuint GetUnsignedAddress(object value) => new UIntPtr(Unsafe.AsPointer(ref value));
}";

            var fixedSource = @"
using System;
using System.Runtime.CompilerServices;

class TestClass
{
    private static string Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + ({|#0:IntPtr|}.Size * 2) + ""})"";

    private static unsafe nint GetAddress(object value) => new {|#1:IntPtr|}(Unsafe.AsPointer(ref value));

    private static unsafe nuint GetUnsignedAddress(object value) => new {|#2:UIntPtr|}(Unsafe.AsPointer(ref value));
}";

            var fixedLinkedSource = @"
using System;
using System.Runtime.CompilerServices;

class TestClass
{
    private static string Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + (IntPtr.Size * 2) + ""})"";

    private static unsafe nint GetAddress(object value) => new IntPtr(Unsafe.AsPointer(ref value));

    private static unsafe nuint GetUnsignedAddress(object value) => new UIntPtr(Unsafe.AsPointer(ref value));
}";

            var sharedPath = Path.Combine(Path.GetTempPath(), "SA1121.Shared.cs");
            await new CSharpTest
            {
                // The verifier only adds its suppression pragma to the primary project, not the linked copy.
                TestBehaviors = TestBehaviors.SkipSuppressionCheck,
                NumberOfFixAllInDocumentIterations = 2,
                NumberOfFixAllInProjectIterations = 2,
                SolutionTransforms =
                {
                    (solution, _) =>
                    {
                        var linkedProject = solution.Projects.Single(project => project.Name == "Linked");
                        return solution.WithProjectCompilationOptions(
                            linkedProject.Id,
                            ((CSharpCompilationOptions)linkedProject.CompilationOptions!).WithAllowUnsafe(true));
                    },
                },
                TestState =
                {
                    Sources = { (sharedPath, source) },
                    AdditionalProjects =
                    {
                        ["Linked"] =
                        {
                            ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
                            Sources = { (sharedPath, linkedSource) },
                        },
                    },
                },
                FixedState =
                {
                    Sources = { (sharedPath, fixedSource) },
                    ExpectedDiagnostics = { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1), Diagnostic().WithLocation(2) },
                    AdditionalProjects =
                    {
                        ["Linked"] =
                        {
                            ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
                            Sources = { (sharedPath, fixedLinkedSource) },
                        },
                    },
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that native-sized integer fixes are available when all linked projects support them.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNativeSizedIntegerFixesInCompatibleLinkedProjectAsync()
        {
            var source = @"
class TestClass
{
    private static int Size => [|System.IntPtr|].Size;
    private static nint Create() => new [|System.IntPtr|](1);
    private static nuint CreateUnsigned() => new [|System.UIntPtr|](1);
}";
            var fixedSource = @"
class TestClass
{
    private static int Size => nint.Size;
    private static nint Create() => new nint(1);
    private static nuint CreateUnsigned() => new nuint(1);
}";
            var sharedPath = Path.Combine(Path.GetTempPath(), "SA1121.Shared.cs");

            await new CSharpTest
            {
                // The verifier only adds its suppression pragma to the primary project, not the linked copy.
                TestBehaviors = TestBehaviors.SkipSuppressionCheck,
                NumberOfFixAllInDocumentIterations = 2,
                NumberOfFixAllInProjectIterations = 2,
                TestState =
                {
                    Sources = { (sharedPath, source) },
                    AdditionalProjects =
                    {
                        ["Linked"] =
                        {
                            ReferenceAssemblies = ReferenceAssemblies.Net.Net70,
                            Sources = { (sharedPath, source) },
                        },
                    },
                },
                FixedState =
                {
                    Sources = { (sharedPath, fixedSource) },
                    AdditionalProjects =
                    {
                        ["Linked"] =
                        {
                            ReferenceAssemblies = ReferenceAssemblies.Net.Net70,
                            Sources = { (sharedPath, fixedSource) },
                        },
                    },
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that solution-wide fixes respect each project's native-sized integer support.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFixAllPreservesNativeSizedIntegersInOlderProjectAsync()
        {
            var legacyCode = @"
using System;
using System.Runtime.CompilerServices;

class TestClass
{
    private static [|String|] Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + (IntPtr.Size * 2) + ""})"";

    private static unsafe IntPtr GetAddress(object value) => new IntPtr(Unsafe.AsPointer(ref value));
}";

            var fixedLegacyCode = @"
using System;
using System.Runtime.CompilerServices;

class TestClass
{
    private static string Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + (IntPtr.Size * 2) + ""})"";

    private static unsafe IntPtr GetAddress(object value) => new IntPtr(Unsafe.AsPointer(ref value));
}";

            await new CSharpTest
            {
                TestCode = "class CurrentTarget { private [|System.IntPtr|] address; }",
                FixedCode = "class CurrentTarget { private nint address; }",
                SolutionTransforms =
                {
                    (solution, _) =>
                    {
                        var legacyProject = solution.Projects.Single(project => project.Name == "Legacy");
                        return solution.WithProjectCompilationOptions(
                            legacyProject.Id,
                            ((CSharpCompilationOptions)legacyProject.CompilationOptions!).WithAllowUnsafe(true));
                    },
                },
                TestState =
                {
                    AdditionalProjects =
                    {
                        ["Legacy"] =
                        {
                            ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
                            Sources = { legacyCode },
                        },
                    },
                },
                FixedState =
                {
                    AdditionalProjects =
                    {
                        ["Legacy"] =
                        {
                            ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
                            Sources = { fixedLegacyCode },
                        },
                    },
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that native-sized integer names are preserved in documentation references.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNativeSizedIntegersInDocumentationReferencesAsync()
        {
            var testCode = @"
using System;
using System.Threading;

/// <see cref=""SynchronizationContext.Wait(IntPtr[], bool, int)""/>
/// <see cref=""IntPtr""/>
/// <see cref=""System.IntPtr""/>
/// <see cref=""IntPtr.Size""/>
/// <see cref=""UIntPtr""/>
/// <see cref=""System.UIntPtr.Size""/>
/// <see cref=""TestClass.M(UIntPtr[])""/>
/// <see cref=""Convert.ToInt32([|Int32|])""/>
class TestClass
{
    private [|IntPtr|] field;

    public void M([|UIntPtr|][] values) { }
}";

            var fixedCode = @"
using System;
using System.Threading;

/// <see cref=""SynchronizationContext.Wait(IntPtr[], bool, int)""/>
/// <see cref=""IntPtr""/>
/// <see cref=""System.IntPtr""/>
/// <see cref=""IntPtr.Size""/>
/// <see cref=""UIntPtr""/>
/// <see cref=""System.UIntPtr.Size""/>
/// <see cref=""TestClass.M(UIntPtr[])""/>
/// <see cref=""Convert.ToInt32(int)""/>
class TestClass
{
    private nint field;

    public void M(nuint[] values) { }
}";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a modern runtime does not enable native-sized integer fixes in older language versions.
        /// </summary>
        /// <param name="languageVersion">The language version to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(LanguageVersion.CSharp9)]
        [InlineData(LanguageVersion.CSharp10)]
        public async Task TestFixAllPreservesNativeSizedIntegersWithOlderLanguageVersionAsync(LanguageVersion languageVersion)
        {
            var testCode = @"
using System;
using System.Runtime.CompilerServices;

class TestClass
{
    private static [|String|] Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + (IntPtr.Size * 2) + ""})"";

    private static unsafe IntPtr GetAddress(object value) => new IntPtr(Unsafe.AsPointer(ref value));

    private static unsafe UIntPtr GetUnsignedAddress(object value) => new UIntPtr(Unsafe.AsPointer(ref value));
}";

            var fixedCode = @"
using System;
using System.Runtime.CompilerServices;

class TestClass
{
    private static string Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + (IntPtr.Size * 2) + ""})"";

    private static unsafe IntPtr GetAddress(object value) => new IntPtr(Unsafe.AsPointer(ref value));

    private static unsafe UIntPtr GetUnsignedAddress(object value) => new UIntPtr(Unsafe.AsPointer(ref value));
}";

            await new CSharpTest(languageVersion)
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net70,
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that fixing other built-in types preserves native-sized integers on .NET Framework.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFixAllPreservesNativeSizedIntegersWithNetFrameworkAsync()
        {
            var testCode = @"
using System;

class TestClass
{
    private static [|String|] Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + (IntPtr.Size * 2) + ""})"";

    private static unsafe IntPtr GetAddress(void* value) => new IntPtr(value);

    private static unsafe UIntPtr GetUnsignedAddress(void* value) => new UIntPtr(value);
}";

            var fixedCode = @"
using System;

class TestClass
{
    private static string Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + (IntPtr.Size * 2) + ""})"";

    private static unsafe IntPtr GetAddress(void* value) => new IntPtr(value);

    private static unsafe UIntPtr GetUnsignedAddress(void* value) => new UIntPtr(value);
}";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472.Default,
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that fixing other built-in types preserves native-sized integers on older runtimes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFixAllPreservesNativeSizedIntegersWithNet60Async()
        {
            var testCode = @"
using System;
using System.Runtime.CompilerServices;

class TestClass
{
    private static [|String|] Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + (IntPtr.Size * 2) + ""})"";

    private static unsafe IntPtr GetAddress(object value) => new IntPtr(Unsafe.AsPointer(ref value));

    private static unsafe UIntPtr GetUnsignedAddress(object value) => new UIntPtr(Unsafe.AsPointer(ref value));
}";

            var fixedCode = @"
using System;
using System.Runtime.CompilerServices;

class TestClass
{
    private static string Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + (IntPtr.Size * 2) + ""})"";

    private static unsafe IntPtr GetAddress(object value) => new IntPtr(Unsafe.AsPointer(ref value));

    private static unsafe UIntPtr GetUnsignedAddress(object value) => new UIntPtr(Unsafe.AsPointer(ref value));
}";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that native-sized integer member access and pointer constructors can be fixed on .NET 7.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFixAllNativeSizedIntegerMembersAndConstructorsAsync()
        {
            var testCode = @"
using System;
using System.Runtime.CompilerServices;

class TestClass
{
    private static [|String|] Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + ([|IntPtr|].Size * 2) + ""})"";

    private static unsafe [|IntPtr|] GetAddress(object value) => new [|IntPtr|](Unsafe.AsPointer(ref value));

    private static unsafe [|UIntPtr|] GetUnsignedAddress(object value) => new [|UIntPtr|](Unsafe.AsPointer(ref value));
}";

            var fixedCode = @"
using System;
using System.Runtime.CompilerServices;

class TestClass
{
    private static string Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + (nint.Size * 2) + ""})"";

    private static unsafe nint GetAddress(object value) => new nint(Unsafe.AsPointer(ref value));

    private static unsafe nuint GetUnsignedAddress(object value) => new nuint(Unsafe.AsPointer(ref value));
}";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        // NOTE: This tests a fix for a c# 10 feature, but the Roslyn API used to solve it wasn't available in the version
        // we use in the c# 10 test project, so the test was added here instead.
        [Fact]
        [WorkItem(3594, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3594")]
        public virtual async Task TestUsingNameChangeInGlobalUsingInAnotherFileAsync()
        {
            var source1 = @"
global using MyDouble = [|System.Double|];";

            var source2 = @"
class TestClass
{
    private MyDouble x;
}";

            await new CSharpTest()
            {
                TestSources = { source1, source2 },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3594, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3594")]
        public virtual async Task TestUsingNameChangeInGlobalUsingInSameFileAsync()
        {
            var source = @"global using MyDouble = [|System.Double|];
class TestClass
{
    private MyDouble x;
}";

            await new CSharpTest()
            {
                TestSources = { source },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3969, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3969")]
        public async Task TestNativeSizedIntegersReportWithNet70Async()
        {
            var testCode = @"
using System;

class TestClass
{
    [|IntPtr|] field1;
    [|System.UIntPtr|] field2;
}";

            var fixedCode = @"
using System;

class TestClass
{
    nint field1;
    nuint field2;
}";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3969, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3969")]
        public async Task TestNativeSizedIntegersDoNotReportWithNet60Async()
        {
            var testCode = @"
using System;

class TestClass
{
    IntPtr field1;
    System.UIntPtr field2;
}";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
                TestCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that type arguments of generic attributes are reported and fixed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3995, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3995")]
        public async Task TestGenericAttributeTypeArgumentAsync()
        {
            var testCode = @"using System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class TypeAttribute<T> : Attribute
{
}

[Type<[|Int32|]>]
[Type<[|System.String|]>]
class TestClass
{
}
";

            var fixedCode = @"using System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class TypeAttribute<T> : Attribute
{
}

[Type<int>]
[Type<string>]
class TestClass
{
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4005, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4005")]
        public async Task TestScopedRefLocalDeclarationAsync()
        {
            var testCode = @"namespace System
{
    public class Foo
    {
        public void Bar()
        {
            int value = 5;
            scoped ref [|Int32|] test = ref value;
        }
    }
}
";

            var fixedCode = @"namespace System
{
    public class Foo
    {
        public void Bar()
        {
            int value = 5;
            scoped ref int test = ref value;
        }
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

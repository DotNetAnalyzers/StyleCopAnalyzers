// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1135UsingDirectivesMustBeQualified,
        StyleCop.Analyzers.ReadabilityRules.SA1135CodeFixProvider>;

    public class SA1135UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonData.EndOfLineSequences), MemberType = typeof(CommonData))]
        public async Task TestUnqualifiedUsingsAsync(string lineEnding)
        {
            string testCode = @"
namespace System.Threading
{
    {|#0:using IO;|}
    {|#1:using Tasks;|}
}".ReplaceLineEndings(lineEnding);
            string fixedCode = @"
namespace System.Threading
{
    using System.IO;
    using System.Threading.Tasks;
}".ReplaceLineEndings(lineEnding);

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorNamespace).WithLocation(0).WithArguments("System.IO"),
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorNamespace).WithLocation(1).WithArguments("System.Threading.Tasks"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that extern-alias-qualified namespace imports are already qualified.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Fact]
        [WorkItem(4340, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4340")]
        public async Task TestNoDiagnosticForUsingAliasWithExternAliasQualifiedNamespaceAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    extern alias corlib;

    using NA = corlib::System;
}";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472.Default,
                TestCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that extern-alias-qualified namespace imports are already qualified.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Fact]
        [WorkItem(4340, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4340")]
        public async Task TestNoDiagnosticForExternAliasQualifiedNamespaceUsingAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    extern alias corlib;

    using corlib::System.Collections;
}";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472.Default,
                TestCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that extern-alias-qualified type aliases and static imports are already qualified.
        /// </summary>
        /// <param name="directive">The using directive to verify.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Theory]
        [InlineData("using SomeClass = corlib::System.String;")]
        [InlineData("using static corlib::System.Math;")]
        [InlineData("using Items = corlib::System.Collections.Generic.List<string>;")]
        [InlineData("using Items = corlib::System.Collections.Generic.List<string>.Enumerator;")]
        [WorkItem(4340, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4340")]
        public async Task TestExternAliasQualifiedTypeAsync(string directive)
        {
            var testCode = $@"
namespace TestNamespace
{{
    extern alias corlib;

    {directive}
}}";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472.Default,
                TestCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that namespace aliases do not hide unqualified generic type arguments.
        /// </summary>
        /// <param name="separator">The alias qualification separator.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Theory]
        [InlineData(".")]
        [InlineData("::")]
        [WorkItem(4340, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4340")]
        public async Task TestNamespaceAliasWithUnqualifiedTypeArgumentAsync(string separator)
        {
            var testCode = $@"
using System;
using Root = System;

namespace N
{{
    {{|#0:using L = Root{separator}Collections.Generic.List<Exception>;|}}
}}";
            var fixedCode = @"
using System;
using Root = System;

namespace N
{
    using L = System.Collections.Generic.List<System.Exception>;
}";

            var expected = Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorType)
                .WithLocation(0).WithArguments("System.Collections.Generic.List<System.Exception>");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that namespace aliases with qualified generic type arguments remain accepted.
        /// </summary>
        /// <param name="separator">The alias qualification separator.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Theory]
        [InlineData(".")]
        [InlineData("::")]
        [WorkItem(4340, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4340")]
        public async Task TestNamespaceAliasWithQualifiedTypeArgumentAsync(string separator)
        {
            var testCode = $@"
using Root = System;

namespace N
{{
    using L = Root{separator}Collections.Generic.List<System.Exception>;
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an extern alias does not hide unqualified generic type arguments.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Fact]
        [WorkItem(4340, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4340")]
        public async Task TestExternAliasWithUnqualifiedTypeArgumentAsync()
        {
            var testCode = @"
extern alias corlib;
using System;

namespace N
{
    {|#0:using L = corlib::System.Collections.Generic.List<Exception>;|}
}";
            var fixedCode = @"
extern alias corlib;
using System;

namespace N
{
    using L = System.Collections.Generic.List<System.Exception>;
}";

            var expected = Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorType)
                .WithLocation(0).WithArguments("System.Collections.Generic.List<System.Exception>");
            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472.Default,
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that extern aliases inside qualified generic types are expanded correctly.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Fact]
        [WorkItem(4340, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4340")]
        public async Task TestExternAliasInsideQualifiedTypeAsync()
        {
            var testCode = @"
extern alias corlib;

namespace N
{
    using L = System.Tuple<corlib::System.String>;
}";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472.Default,
                TestCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an extern alias inside a generic argument does not qualify the containing type.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Fact]
        [WorkItem(4340, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4340")]
        public async Task TestExternAliasInsideUnqualifiedTypeAsync()
        {
            var testCode = @"
extern alias corlib;
using System;

namespace N
{
    {|#0:using L = Tuple<corlib::System.String>;|}
}";
            var fixedCode = @"
extern alias corlib;
using System;

namespace N
{
    using L = System.Tuple<System.String>;
}";

            var expected = Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorType)
                .WithLocation(0).WithArguments("System.Tuple<System.String>");
            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472.Default,
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnqualifiedAliasedUsingsAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    using NA = IO;
    using NB = Tasks;
}";
            const string fixedCode = @"
namespace System.Threading
{
    using NA = System.IO;
    using NB = System.Threading.Tasks;
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorNamespace).WithLocation(4, 5).WithArguments("System.IO"),
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorNamespace).WithLocation(5, 5).WithArguments("System.Threading.Tasks"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnqualifiedAliasedUsingTypesAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    using TP = IO.Path;
    using TT = Tasks.Task;
}";
            const string fixedCode = @"
namespace System.Threading
{
    using TP = System.IO.Path;
    using TT = System.Threading.Tasks.Task;
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorType).WithLocation(4, 5).WithArguments("System.IO.Path"),
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorType).WithLocation(5, 5).WithArguments("System.Threading.Tasks.Task"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestGlobalUsingsAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    using global::System.IO;
    using global::System.Threading.Tasks;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFixAllAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    using NA = IO;
    using NB = Tasks;
}";
            const string fixedCode = @"
namespace System.Threading
{
    using NA = System.IO;
    using NB = System.Threading.Tasks;
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorNamespace).WithLocation(4, 5).WithArguments("System.IO"),
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorNamespace).WithLocation(5, 5).WithArguments("System.Threading.Tasks"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoWarningsForAliasesAsync()
        {
            var testCode = @"
using Tasks = System.Threading.Tasks;

namespace Namespace
{
    using Task = Tasks.Task;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAliasToGenericTypeAsync()
        {
            var testCode = @"
namespace System.Collections
{
    using Dictionary = Generic.Dictionary<int, string>;
}
";

            var fixedCode = @"
namespace System.Collections
{
    using Dictionary = System.Collections.Generic.Dictionary<int, string>;
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorType).WithLocation(4, 5).WithArguments("System.Collections.Generic.Dictionary<int, string>"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAliasToTypesInSameNamespaceAsync()
        {
            var testCode = @"
namespace Namespace
{
    using Class2 = Class;

    class Class { }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2690, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2690")]
        public async Task TestFullyQualifiedAliasAsync()
        {
            var testCode = @"
using Example = System.ValueTuple<System.Collections.IList, int>;
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2690, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2690")]
        public async Task TestFullyQualifiedAliasWithUnresolvedTypeParameterAsync()
        {
            var testCode = @"
using Example = System.ValueTuple<System.Collections.List, int>;
";

            var expected = DiagnosticResult.CompilerError("CS0234").WithLocation(2, 54).WithMessage("The type or namespace name 'List' does not exist in the namespace 'System.Collections' (are you missing an assembly reference?)");
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2690, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2690")]
        public async Task TestFullyQualifiedAliasInsideNamespaceAsync()
        {
            var testCode = @"
namespace Test {
    using Example = System.ValueTuple<System.Collections.IList, int>;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFullyQualifiedAliasWithWrappedTypeArgumentsAsync()
        {
            var testCode = @"
using Example = System.ValueTuple<
    System.Int32,
    System.Int32>;
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2820, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2820")]
        public async Task TestAliasWithWrappedTypeArgumentsInsideNamespaceAsync()
        {
            var testCode = @"
using System;
using MyAlias = System.Exception;

namespace Test
{
    using Example = System.ValueTuple<
        Exception,
        Exception>;

    using Example2 = ValueTuple<
        Exception[],
        Exception[,,]>;

    using Example3 = ValueTuple<
        ValueTuple<
            Exception,
            Exception>,
        Exception>;

    using Example4 = ValueTuple<
        MyAlias,
        Exception>;
}
";
            var fixedCode = @"
using System;
using MyAlias = System.Exception;

namespace Test
{
    using Example = System.ValueTuple<
        System.Exception,
        System.Exception>;

    using Example2 = System.ValueTuple<
        System.Exception[],
        System.Exception[,,]>;

    using Example3 = System.ValueTuple<
        System.ValueTuple<
            System.Exception,
            System.Exception>,
        System.Exception>;

    using Example4 = System.ValueTuple<
        System.Exception,
        System.Exception>;
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorType).WithLocation(7, 5).WithArguments("System.ValueTuple<System.Exception, System.Exception>"),
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorType).WithLocation(11, 5).WithArguments("System.ValueTuple<System.Exception[], System.Exception[,,]>"),
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorType).WithLocation(15, 5).WithArguments("System.ValueTuple<System.ValueTuple<System.Exception, System.Exception>, System.Exception>"),
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorType).WithLocation(21, 5).WithArguments("System.ValueTuple<System.Exception, System.Exception>"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFullyQualifiedTopLevelNamespaceAsync()
        {
            var testCode = @"
namespace MyNamespace {
  using System;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2879, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2879")]
        public async Task TestOmittedTypeInGenericAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    using Example = System.Collections.Generic.List<>;
}
";

            var expected = new DiagnosticResult("CS7003", DiagnosticSeverity.Error).WithLocation(4, 48).WithMessage("Unexpected use of an unbound generic name");
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2879, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2879")]
        public async Task TestNullableTypeInGenericAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    using Example = System.Collections.Generic.List<int?>;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2879, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2879")]
        public async Task TestGlobalQualifiedTypeInGenericAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    using Example = System.Collections.Generic.List<global::System.Int32>;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2879, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2879")]
        public async Task TestTypeInGlobalNamespaceAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    using Example = MyClass;
}

class MyClass
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2879, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2879")]
        public async Task TestAliasTypeNestedInGenericAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    using Example = System.Collections.Immutable.ImmutableDictionary<int, int>.Builder;
}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2879, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2879")]
        public async Task TestValueTupleInUsingAliasAsync()
        {
            var testCode = @"
namespace System
{
    using Example = System.Collections.Generic.List<ValueTuple<int, int>>;
}
";
            var fixedCode = @"
namespace System
{
    using Example = System.Collections.Generic.List<System.ValueTuple<int, int>>;
}
";

            var expected = Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorType).WithLocation(4, 5).WithArguments("System.Collections.Generic.List<System.ValueTuple<int, int>>");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3149, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3149")]
        public async Task TestAliasTypeClrTypeAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    using Example = System.Collections.Generic.List<System.Object>;
}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3149, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3149")]
        public async Task TestAliasTypeGenericNullableAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    using Example = System.Nullable<int>;
}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3166, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3166")]
        public async Task TestAliasClrTypeAsync()
        {
            var testCode = @"
namespace System
{
    using Float = System.Double;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2618, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2618")]
        public async Task TestUnqualifiedStaticUsingsAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    using static Console;
}
";

            const string fixedCode = @"
namespace System.Threading
{
    using static System.Console;
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorType).WithLocation(4, 5).WithArguments("System.Console"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3884, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3884")]
        public async Task TestAliasInsideConstructedTypeAsync()
        {
            var testCode = @"
using Tasks = System.Threading.Tasks;

namespace TestStuff
{
    using T1 = Tasks.Task;
    using T2 = System.ValueTuple<Tasks.Task, int>;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3884, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3884")]
        public async Task TestUnqualifiedTypeWithAliasQualifierInsideAsync()
        {
            var testCode = @"
using System;
using Tasks = System.Threading.Tasks;

namespace TestStuff
{
    {|#0:using T2 = ValueTuple<Tasks::Task, int>;|}
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorType).WithLocation(0).WithArguments("System.ValueTuple<System.Threading.Tasks.Task, int>"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

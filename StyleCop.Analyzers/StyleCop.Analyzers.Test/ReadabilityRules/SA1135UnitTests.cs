// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1135UsingDirectivesMustBeQualified,
        StyleCop.Analyzers.ReadabilityRules.SA1135CodeFixProvider>;

    public class SA1135UnitTests
    {
        [Fact]
        public async Task TestUnqualifiedUsingsAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    using IO;
    using Tasks;
}";
            const string fixedCode = @"
namespace System.Threading
{
    using System.IO;
    using System.Threading.Tasks;
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorNamespace).WithLocation(4, 5).WithArguments("System.IO"),
                Diagnostic(SA1135UsingDirectivesMustBeQualified.DescriptorNamespace).WithLocation(5, 5).WithArguments("System.Threading.Tasks"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
        public async Task TestStaticUsingsAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    using static Console;
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
    }
}

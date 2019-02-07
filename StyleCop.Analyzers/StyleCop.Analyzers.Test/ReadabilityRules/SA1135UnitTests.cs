﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticUsingsAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    using static Console;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2690, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2690")]
        public async Task TestFullyQualifiedAliasAsync()
        {
            var testCode = @"
using Example = System.ValueTuple<System.Collections.IList, int>;
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2690, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2690")]
        public async Task TestFullyQualifiedAliasWithUnresolvedTypeParameterAsync()
        {
            var testCode = @"
using Example = System.ValueTuple<System.Collections.List, int>;
";

            var expected = DiagnosticResult.CompilerError("CS0234").WithLocation(2, 54).WithMessage("The type or namespace name 'List' does not exist in the namespace 'System.Collections' (are you missing an assembly reference?)");
            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }
    }
}

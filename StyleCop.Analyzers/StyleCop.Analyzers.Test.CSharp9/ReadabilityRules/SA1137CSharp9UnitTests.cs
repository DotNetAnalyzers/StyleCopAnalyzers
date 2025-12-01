// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1137ElementsShouldHaveTheSameIndentation,
        StyleCop.Analyzers.ReadabilityRules.IndentationCodeFixProvider>;

    public partial class SA1137CSharp9UnitTests : SA1137CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3668, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3668")]
        public async Task TestInitAccessorAttributeListAsync()
        {
            string testCode = @"
using System;

class TestClass
{
    int Property
    {
        [My]
[| |][My]
        init { }
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";

            string fixedCode = @"
using System;

class TestClass
{
    int Property
    {
        [My]
        [My]
        init { }
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3966")]
        public async Task TestPropertyAccessorListWithInitAsync()
        {
            string testCode = @"
using System;

class Container
{
    int Property1
    {
      [My]
        get;

      init;
    }

    int Property2
    {
      [My]
get;

      init;
    }

    int Property3
    {
      [My] get;

       init;
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";
            string fixedCode = @"
using System;

class Container
{
    int Property1
    {
        [My]
        get;

        init;
    }

    int Property2
    {
[My]
get;

init;
    }

    int Property3
    {
       [My] get;

       init;
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 1),
                Diagnostic().WithLocation(11, 1),
                Diagnostic().WithLocation(16, 1),
                Diagnostic().WithLocation(19, 1),
                Diagnostic().WithLocation(24, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3966")]
        public async Task TestIndexerAccessorListWithInitAsync()
        {
            string testCode = @"
using System;

interface IContainer1
{
    int this[int arg]
    {
      [My]
        get;

      init;
    }
}

interface IContainer2
{
    int this[int arg]
    {
      [My]
get;

      init;
    }
}

interface IContainer3
{
    int this[int arg]
    {
      [My] get;

       init;
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";
            string fixedCode = @"
using System;

interface IContainer1
{
    int this[int arg]
    {
        [My]
        get;

        init;
    }
}

interface IContainer2
{
    int this[int arg]
    {
[My]
get;

init;
    }
}

interface IContainer3
{
    int this[int arg]
    {
       [My] get;

       init;
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 1),
                Diagnostic().WithLocation(11, 1),
                Diagnostic().WithLocation(19, 1),
                Diagnostic().WithLocation(22, 1),
                Diagnostic().WithLocation(30, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3966")]
        public async Task TestGetAndInitAccessorIndentationAsync()
        {
            string testCode = @"
class TestClass
{
    private int value;

    int Property
    {
        get
        {
            return this.value;
        }
{|#0:      |}init
        {
            this.value = 1;
        }
    }
}
";

            string fixedCode = @"
class TestClass
{
    private int value;

    int Property
    {
        get
        {
            return this.value;
        }
        init
        {
            this.value = 1;
        }
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

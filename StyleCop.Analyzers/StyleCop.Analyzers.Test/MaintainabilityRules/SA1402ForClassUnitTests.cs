// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class SA1402ForClassUnitTests : SA1402ForBlockDeclarationUnitTestsBase
    {
        public override string Keyword => "class";

        protected override bool IsConfiguredAsTopLevelTypeByDefault => true;

        [Fact]
        [WorkItem(3876, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3876")]
        public async Task TestExtractedTypeDoesNotGainBlankLineAfterNamespaceAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int MyProperty { get; set; }
    }

    public class {|#0:TestClass2|}
    {
        public string MyProperty { get; set; }
    }
}
";

            var fixedCode = new[]
            {
                ("/0/Test0.cs", @"namespace TestNamespace
{
    public class TestClass
    {
        public int MyProperty { get; set; }
    }
}
"),
                ("TestClass2.cs", @"namespace TestNamespace
{
    public class TestClass2
    {
        public string MyProperty { get; set; }
    }
}
"),
            };

            var expected = this.Diagnostic().WithLocation(0);
            await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3876, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3876")]
        public async Task TestExtractedTypeDoesNotGainBlankLineBeforeEndIfAsync()
        {
            var testCode = @"namespace TestNamespace
{
#if true
    using System;
#endif

    public class TestClass
    {
        public DateTime MyDate { get; set; }
    }

    public class {|#0:TestClass2|}
    {
        public DateTime MyDate2 { get; set; }
    }
}
";

            var fixedCode = new[]
            {
                ("/0/Test0.cs", @"namespace TestNamespace
{
#if true
    using System;
#endif

    public class TestClass
    {
        public DateTime MyDate { get; set; }
    }
}
"),
                ("TestClass2.cs", @"namespace TestNamespace
{
#if true
    using System;
#endif

    public class TestClass2
    {
        public DateTime MyDate2 { get; set; }
    }
}
"),
            };

            var expected = this.Diagnostic().WithLocation(0);
            await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3876, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3876")]
        public async Task TestExtractedTypeInSecondNamespaceKeepsBlankLineAfterBraceAsync()
        {
            var testCode = @"namespace OtherNamespace
{
    public class OtherClass
    {
    }
}

namespace TestNamespace
{

    public class {|#0:TestClass2|}
    {
    }
}
";

            var fixedCode = new[]
            {
                ("/0/Test0.cs", @"namespace OtherNamespace
{
    public class OtherClass
    {
    }
}

namespace TestNamespace
{
}
"),
                ("TestClass2.cs", @"namespace TestNamespace
{

    public class TestClass2
    {
    }
}
"),
            };

            var expected = this.Diagnostic().WithLocation(0);
            await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3876, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3876")]
        public async Task TestExtractedTypeInSecondNamespaceTrimsSiblingAndNamespaceSeparatorsAsync()
        {
            var testCode = @"namespace OtherNamespace
{
}

namespace TestNamespace
{
    public class TestClass
    {
    }

    public class {|#0:TestClass2|}
    {
    }
}
";

            var fixedCode = new[]
            {
                ("/0/Test0.cs", @"namespace OtherNamespace
{
}

namespace TestNamespace
{
    public class TestClass
    {
    }
}
"),
                ("TestClass2.cs", @"namespace TestNamespace
{
    public class TestClass2
    {
    }
}
"),
            };

            var expected = this.Diagnostic().WithLocation(0);
            await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3876, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3876")]
        public async Task TestExtractedTypeInSecondNamespaceKeepsBlankLineAfterUsingAsync()
        {
            var testCode = @"using System;

namespace OtherNamespace
{
    public class OtherClass
    {
    }
}

namespace TestNamespace
{

    public class {|#0:TestClass2|}
    {
    }
}
";

            var fixedCode = new[]
            {
                ("/0/Test0.cs", @"using System;

namespace OtherNamespace
{
    public class OtherClass
    {
    }
}

namespace TestNamespace
{
}
"),
                ("TestClass2.cs", @"using System;

namespace TestNamespace
{

    public class TestClass2
    {
    }
}
"),
            };

            var expected = this.Diagnostic().WithLocation(0);
            await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

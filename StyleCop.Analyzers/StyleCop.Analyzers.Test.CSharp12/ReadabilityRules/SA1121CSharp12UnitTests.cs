// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.ReadabilityRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1121UseBuiltInTypeAlias,
        StyleCop.Analyzers.ReadabilityRules.SA1121CodeFixProvider>;

    public partial class SA1121CSharp12UnitTests : SA1121CSharp11UnitTests
    {
        private const string AllowBuiltInTypeAliasesSettingsJson = @"
{
  ""settings"": {
    ""readabilityRules"": {
      ""allowBuiltInTypeAliases"": true
    }
  }
}
";

        [Theory]
        [MemberData(nameof(AllFullQualifiedTypes))]
        [WorkItem(4011, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4011")]
        public async Task TestUsingAliasTargetAsync(string predefined, string fullName)
        {
            string testCode = $@"
using MyAlias = {{|#0:{fullName}|}};

public class TestClass
{{
}}
";

            string fixedCode = $@"
using MyAlias = {predefined};

public class TestClass
{{
}}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4011, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4011")]
        public async Task TestGlobalUsingAliasTargetAsync()
        {
            string testCode = @"
global using MyInt = {|#0:System.Int32|};

public class TestClass
{
    private {|#1:MyInt|} value;
}
";

            string fixedCode = @"
global using MyInt = int;

public class TestClass
{
    private int value;
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4011, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4011")]
        public async Task TestUsingAliasTargetWithSameNameAsAliasAsync()
        {
            // The alias name is a declaration, not a reference to System.Int32, so only the target is reported.
            string testCode = @"
namespace TestNamespace
{
    using Int32 = {|#0:System.Int32|};

    public class TestClass
    {
        private {|#1:Int32|} value;
    }
}
";

            string fixedCode = @"
namespace TestNamespace
{
    using Int32 = int;

    public class TestClass
    {
        private int value;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4011, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4011")]
        public async Task TestUsingAliasTargetArrayAndTupleAsync()
        {
            string testCode = @"
using IntArray = {|#0:System.Int32|}[];
using Pair = ({|#1:System.Int32|} Number, {|#2:System.String|} Text);

public class TestClass
{
}
";

            string fixedCode = @"
using IntArray = int[];
using Pair = (int Number, string Text);

public class TestClass
{
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4011, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4011")]
        public async Task TestUsingAliasTargetWithAllowBuiltInTypeAliasesAsync()
        {
            // allowBuiltInTypeAliases allows referring to a built-in type through an alias (MyInt), but the alias target
            // itself is a direct reference to System.UInt32.
            string testCode = @"
using MyInt = {|#0:System.UInt32|};

public class TestClass
{
    private MyInt value;
}
";

            string fixedCode = @"
using MyInt = uint;

public class TestClass
{
    private MyInt value;
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0) },
                FixedCode = fixedCode,
                Settings = AllowBuiltInTypeAliasesSettingsJson,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4011, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4011")]
        public async Task TestUsingStaticAndNamespaceDirectivesAsync()
        {
            // Using static and namespace directives still require a type or namespace name, so they are not reported.
            string testCode = @"
using System;
using static System.Int32;

public class TestClass
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4011, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4011")]
        public async Task TestUsingAliasTargetBeforeCSharp12Async()
        {
            // Before C# 12, the target of a using alias cannot be written with a keyword.
            string testCode = @"
using MyInt = System.Int32;

public class TestClass
{
}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersion.CSharp11, testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        // The following tests are inherited from earlier language versions, where the target of a using alias directive
        // can't be written with a built-in type keyword. From C# 12 it can, so the target is reported as well.

        /// <inheritdoc/>
        //// xUnit reads the InlineData attributes from the base method as well, so they must not be repeated here.
#pragma warning disable xUnit1003 // Theory methods must have test data
        [Theory]
        public override async Task TestMissleadingUsingAsync(string lineEnding)
#pragma warning restore xUnit1003 // Theory methods must have test data
        {
            string oldSource = @"namespace Foo
{
  using Int32 = {|#1:System.UInt32|};
  class Bar
  {
    {|#0:Int32|} value = 3;
  }
}
".ReplaceLineEndings(lineEnding);

            string newSource = @"namespace Foo
{
  using Int32 = uint;
  class Bar
  {
    uint value = 3;
  }
}
".ReplaceLineEndings(lineEnding);

            await new CSharpTest
            {
                TestCode = oldSource,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1) },
                FixedCode = newSource,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        [Fact]
        public override async Task TestUsingNameChangeAsync()
        {
            string oldSource = @"namespace Foo
{
  using MyInt = {|#1:System.UInt32|};
  class Bar
  {
    {|#0:MyInt|} value = 3;
  }
}
";
            string newSource = @"namespace Foo
{
  using MyInt = uint;
  class Bar
  {
    uint value = 3;
  }
}
";

            await new CSharpTest
            {
                TestCode = oldSource,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1) },
                FixedCode = newSource,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        [Fact]
        public override async Task TestMissleadingUsingAllowAliasesAsync()
        {
            string oldSource = @"namespace Foo
{
  using Int32 = {|#1:System.UInt32|};
  class Bar
  {
    {|#0:Int32|} value = 3;
  }
}
";
            string newSource = @"namespace Foo
{
  using Int32 = uint;
  class Bar
  {
    uint value = 3;
  }
}
";

            await new CSharpTest
            {
                TestCode = oldSource,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1) },
                FixedCode = newSource,
                Settings = AllowBuiltInTypeAliasesSettingsJson,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        [Fact]
        public override async Task TestUsingNameChangeAllowAliasesAsync()
        {
            string testSource = @"namespace Foo
{
  using MyInt = {|#0:System.UInt32|};
  class Bar
  {
    MyInt value = 3;
  }
}
";
            string fixedSource = @"namespace Foo
{
  using MyInt = uint;
  class Bar
  {
    MyInt value = 3;
  }
}
";

            await new CSharpTest
            {
                TestCode = testSource,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0) },
                FixedCode = fixedSource,
                Settings = AllowBuiltInTypeAliasesSettingsJson,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        [Fact]
        public override async Task TestUsingNameChangeInFileScopedNamespaceAsync()
        {
            string oldSource = @"namespace Foo;

using MyInt = {|#1:System.UInt32|};
class Bar
{
{|#0:MyInt|} value = 3;
}
";
            string newSource = @"namespace Foo;

using MyInt = uint;
class Bar
{
uint value = 3;
}
";

            await new CSharpTest
            {
                TestCode = oldSource,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1) },
                FixedCode = newSource,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        [Fact]
        public override async Task TestUsingNameChangeInGlobalUsingInAnotherFileAsync()
        {
            var oldSource1 = @"
global using MyDouble = [|System.Double|];";

            var newSource1 = @"
global using MyDouble = double;";

            var oldSource2 = @"
class TestClass
{
    private [|MyDouble|] x;
}";

            var newSource2 = @"
class TestClass
{
    private double x;
}";

            await new CSharpTest()
            {
                TestSources = { oldSource1, oldSource2 },
                FixedSources = { newSource1, newSource2 },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        [Fact]
        public override async Task TestUsingNameChangeInGlobalUsingInSameFileAsync()
        {
            var source = @"global using MyDouble = [|System.Double|];
class TestClass
{
    private [|MyDouble|] x;
}";

            var newSource = @"global using MyDouble = double;
class TestClass
{
    private double x;
}";

            await new CSharpTest()
            {
                TestSources = { source },
                FixedSources = { newSource },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

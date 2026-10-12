// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1133DoNotCombineAttributes,
        StyleCop.Analyzers.ReadabilityRules.SA1133CodeFixProvider>;

    public partial class SA1133CSharp9UnitTests : SA1133CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3978, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3978")]
        public async Task TestLocalFunctionAttributeListAsync()
        {
            var testCode = @"using System;

class TestClass
{
    void Outer()
    {
        [Attr1, {|#0:Attr2|}]
        void Local()
        {
        }
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class Attr1Attribute : Attribute
{
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class Attr2Attribute : Attribute
{
}
";

            var fixedCode = @"using System;

class TestClass
{
    void Outer()
    {
        [Attr1]
        [Attr2]
        void Local()
        {
        }
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class Attr1Attribute : Attribute
{
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class Attr2Attribute : Attribute
{
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

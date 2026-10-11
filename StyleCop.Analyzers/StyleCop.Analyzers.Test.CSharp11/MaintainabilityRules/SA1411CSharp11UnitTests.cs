// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1411AttributeConstructorMustNotUseUnnecessaryParenthesis,
        StyleCop.Analyzers.MaintainabilityRules.SA1410SA1411CodeFixProvider>;

    public partial class SA1411CSharp11UnitTests : SA1411CSharp10UnitTests
    {
        /// <summary>
        /// Verifies that unnecessary parentheses after a generic attribute are reported and removed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3995, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3995")]
        public async Task TestGenericAttributeAsync()
        {
            var testCode = @"using System;

class TypeAttribute<T> : Attribute
{
}

[Type<int>{|#0:()|}]
class TestClass
{
}
";

            var fixedCode = @"using System;

class TypeAttribute<T> : Attribute
{
}

[Type<int>]
class TestClass
{
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

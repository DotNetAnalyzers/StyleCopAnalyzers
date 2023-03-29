// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp9.MaintainabilityRules;
    using Xunit;

    public class SA1402CSharp10ForClassUnitTests : SA1402CSharp9ForClassUnitTests
    {
        [Fact]
        [WorkItem(3435, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3435")]
        public async Task VerifyTwoClassesWithFileScopedNamespacesAsync()
        {
            var testCode = $@"namespace TestNamespace;

public class TestClass1 {{ }}
public class {{|#0:TestClass2|}} {{ }}
";
            var fixedCode1 = $@"namespace TestNamespace;

public class TestClass1 {{ }}
";
            var fixedCode2 = $@"namespace TestNamespace;
public class TestClass2 {{ }}
";

            var expectedDiagnostic = this.Diagnostic().WithLocation(0);
            await this.VerifyCSharpFixAsync(
                testCode,
                this.GetSettings(),
                expectedDiagnostic,
                new[]
                {
                    ("/0/Test0.cs", fixedCode1),
                    ("TestClass2.cs", fixedCode2),
                },
                CancellationToken.None).ConfigureAwait(false);
        }
    }
}

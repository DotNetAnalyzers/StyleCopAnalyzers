// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1135UsingDirectivesMustBeQualified,
        StyleCop.Analyzers.ReadabilityRules.SA1135CodeFixProvider>;

    public partial class SA1135CSharp10UnitTests : SA1135CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that extern-alias-qualified imports in file-scoped namespaces are already qualified.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Fact]
        [WorkItem(4340, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4340")]
        public async Task TestExternAliasInFileScopedNamespaceAsync()
        {
            var testCode = @"
extern alias corlib;

namespace N;

using SomeClass = corlib::System.String;
using corlib::System.Collections;
using static corlib::System.Math;
";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472.Default,
                TestCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3415, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3415")]
        public async Task TestFileScopedNamespaceAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    using KeyValue = System.Collections.Generic.KeyValuePair<string, object?>;
}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await new CSharpTest
            {
                TestState =
                {
                    Sources =
                    {
                        @"namespace A.B.C { }",
                        @"namespace A.B.D;

[|using C;|]
",
                    },
                },
                FixedState =
                {
                    Sources =
                    {
                        @"namespace A.B.C { }",
                        @"namespace A.B.D;

using A.B.C;
",
                    },
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

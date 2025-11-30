// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName>;

    public partial class SA1649CSharp8UnitTests : SA1649CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task VerifyReadonlyMembersDoNotAffectCorrectFileNameAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    public readonly struct TestType
    {
        private readonly int value;

        public TestType(int value)
        {
            this.value = value;
        }

        public readonly int GetValue() => this.value;

        public int Property
        {
            readonly get => this.value;
            set => _ = value;
        }

        public int this[int index]
        {
            readonly get => this.value + index;
            set => _ = value - index;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(fileName: "TestType.cs", testCode, testSettings: null, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task VerifyWrongFileNameWithReadonlyMembersAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    public readonly struct {|#0:TestType|}
    {
        private readonly int value;

        public TestType(int value)
        {
            this.value = value;
        }

        public readonly int GetValue() => this.value;

        public int Property
        {
            readonly get => this.value;
            set => _ = value;
        }

        public int this[int index]
        {
            readonly get => this.value + index;
            set => _ = value - index;
        }
    }
}
";
            var fixedCode = @"
namespace TestNamespace
{
    public readonly struct TestType
    {
        private readonly int value;

        public TestType(int value)
        {
            this.value = value;
        }

        public readonly int GetValue() => this.value;

        public int Property
        {
            readonly get => this.value;
            set => _ = value;
        }

        public int this[int index]
        {
            readonly get => this.value + index;
            set => _ = value - index;
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(oldFileName: "WrongFileName.cs", testCode, StyleCopSettings, expectedDiagnostic, newFileName: "TestType.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

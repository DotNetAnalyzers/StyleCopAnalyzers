// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1101PrefixLocalCallsWithThis,
        StyleCop.Analyzers.ReadabilityRules.SA1101CodeFixProvider>;

    public partial class SA1101CSharp14UnitTests : SA1101CSharp13UnitTests
    {
        [Fact]
        [WorkItem(3954, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3954")]
        public async Task TestExtensionBlockMethodUsingReceiverAsync()
        {
            var testCode = @"
public class MyClass
{
    public void Hi() { }
}

public static class MyExtensions
{
    extension(MyClass self)
    {
        public void Hey() => self.Hi();
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3954, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3954")]
        public async Task TestExtensionBlockPropertyAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int MyLength => source.Length;

        public int MyLength2
        {
            get { return source.Length; }
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3954, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3954")]
        public async Task TestExtensionBlockMethodAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int GetMyLength() => source.Length;

        public int GetMyLengthTwice()
        {
            return source.GetMyLength() + source.Length;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3954, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3954")]
        public async Task TestInstanceMemberOutsideExtensionBlockStillReportedAsync()
        {
            var testCode = @"
public class TestClass
{
    private int field;

    public int GetField() => {|#0:field|};
}

public static class TestExtensions
{
    extension(TestClass source)
    {
        public int GetFieldTwice() => source.GetField() * 2;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    private int field;

    public int GetField() => this.field;
}

public static class TestExtensions
{
    extension(TestClass source)
    {
        public int GetFieldTwice() => source.GetField() * 2;
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Lightup;
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

        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldKeywordDoesNotRequireThisPrefixAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Prop
    {
        get => field;
        set => field = value;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldKeywordWithMemberNamedFieldAsync()
        {
            // In C# 14, 'field' inside an accessor binds to the synthesized backing field even when the type declares a
            // member named 'field' (the compiler warns with CS9258), so SA1101 must not report it. Outside of
            // accessors, 'field' is an ordinary identifier that binds to the member and requires 'this.'.
            var testCode = @"
internal class TestClass
{
    private int field = 1;

    public int Prop
    {
        get => {|#1:field|};
        set => {|#2:field|} = value;
    }

    public int GetField() => {|#0:field|};
}
";

            var fixedCode = @"
internal class TestClass
{
    private int field = 1;

    public int Prop
    {
        get => {|#1:field|};
        set => {|#2:field|} = value;
    }

    public int GetField() => this.field;
}
";

            var test = new CSharpTest(LanguageVersionEx.CSharp14)
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                CompilerDiagnostics = CompilerDiagnostics.Warnings,
            };

            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("CS9258").WithLocation(1));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("CS9258").WithLocation(2));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("CS9258").WithLocation(1));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("CS9258").WithLocation(2));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestEscapedFieldIdentifierInAccessorRequiresThisPrefixAsync()
        {
            // '@field' is never the keyword, so in C# 14 it binds to the declared member (without CS9258) and requires
            // 'this.'.
            var testCode = @"
internal class TestClass
{
    private int field;

    public int Prop
    {
        get => {|#0:@field|};
        set => {|#1:@field|} = value;
    }
}
";

            var fixedCode = @"
internal class TestClass
{
    private int field;

    public int Prop
    {
        get => this.@field;
        set => this.@field = value;
    }
}
";

            var test = new CSharpTest(LanguageVersionEx.CSharp14)
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                CompilerDiagnostics = CompilerDiagnostics.Warnings,
            };

            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(1));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldIdentifierInAccessorBeforeCSharp14RequiresThisPrefixAsync()
        {
            // Before C# 14, 'field' inside an accessor is an ordinary identifier that binds to the declared member (the
            // compiler reports no warning), so it requires 'this.' just like any other instance member.
            var testCode = @"
internal class TestClass
{
    private int field;

    public int Prop
    {
        get => {|#0:field|};
        set => {|#1:field|} = value;
    }
}
";

            var fixedCode = @"
internal class TestClass
{
    private int field;

    public int Prop
    {
        get => this.field;
        set => this.field = value;
    }
}
";

            var test = new CSharpTest(LanguageVersionEx.CSharp13)
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                CompilerDiagnostics = CompilerDiagnostics.Warnings,
            };

            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(1));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

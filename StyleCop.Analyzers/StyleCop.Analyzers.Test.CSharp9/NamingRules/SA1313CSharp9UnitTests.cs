// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1313ParameterNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

    public partial class SA1313CSharp9UnitTests : SA1313CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3168, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3168")]
        [WorkItem(3181, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3181")]
        public async Task TestPositionalRecordAsync()
        {
            var testCode = @"
public record R(int A)
{
    public R(int [|A|], int [|B|])
        : this(A)
    {
    }
}
";

            var fixedCode = @"
public record R(int A)
{
    public R(int a, int b)
        : this(a)
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2974, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2974")]
        public async Task TestLambdaParameterWithAllUnderscoresAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        System.Action<int> action1 = _ => { };
        System.Action<int, int> action2 = (_, _) => { };
        System.Action<int, int, int> action3 = (_, _, _) => { };
        System.Action<int, int, int, int> action4 = (_, _, _, _) => { };
        System.Action<int> action5 = delegate(int _) { };
        System.Action<int, int> action6 = delegate(int _, int _) { };
        System.Action<int, int, int> action7 = delegate(int _, int _, int _) { };
        System.Action<int, int, int, int> action8 = delegate(int _, int _, int _, int _) { };

        System.Action<int, int> action9 = (_, a) => { };
        System.Action<int, int> action10 = (a, _) => { };
        System.Action<int, int, int> action11 = (_, a, _) => { };
        System.Action<int, int, int> action12 = (a, _, _) => { };
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

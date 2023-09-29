// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp10.SpacingRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1000KeywordsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1000CSharp11UnitTests : SA1000CSharp10UnitTests
    {
        [Fact]
        public async Task TestCheckedOperatorDeclarationAsync()
        {
            // NOTE: A checked operator requires a non-checked operator as well
            // NOTE: Implicit conversion operators can not be checked
            var testCode = @"
public class MyClass
{
    public static MyClass operator {|#0:checked|}-(MyClass x) => x;
    public static MyClass operator -(MyClass x) => x;

    public static explicit operator {|#1:checked|}@MyClass(int i) => new MyClass();
    public static explicit operator MyClass(int i) => new MyClass();
}";

            var fixedCode = @"
public class MyClass
{
    public static MyClass operator checked -(MyClass x) => x;
    public static MyClass operator -(MyClass x) => x;

    public static explicit operator checked @MyClass(int i) => new MyClass();
    public static explicit operator MyClass(int i) => new MyClass();
}";

            var expected = new[]
            {
                Diagnostic().WithArguments("checked", string.Empty, "followed").WithLocation(0),
                Diagnostic().WithArguments("checked", string.Empty, "followed").WithLocation(1),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

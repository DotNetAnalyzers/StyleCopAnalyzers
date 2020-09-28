// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1303ConstFieldNamesMustBeginWithUpperCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToUpperCaseCodeFixProvider>;

    public class SA1303UnitTests
    {
        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    public const string bar = ""baz"";
}";
            var fixedCode = @"public class Foo
{
    public const string Bar = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 25);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseWithConflictAsync()
        {
            var testCode = @"public class Foo
{
    public const string bar = ""baz"";
    public const int Bar = 0;
}";
            var fixedCode = @"public class Foo
{
    public const string BarValue = ""baz"";
    public const int Bar = 0;
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 25);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseNativeMethodsExampleOneAsync()
        {
            var testCode = @"public class NativeMethods
{        
    public const string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseNativeMethodsExampleTwoAsync()
        {
            var testCode = @"public class MyNativeMethods
{
    public const string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseInnerClassInNativeMethodsAsync()
        {
            var testCode = @"public class NativeMethods
{
    public class Foo
    {
        public const string bar = ""baz"";
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseInnerInnerClassInNativeMethodsAsync()
        {
            var testCode = @"public class NativeMethods
{
    public class Foo
    {
        public class FooInner
        {
            public const string bar = ""baz"";
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseNativeMethodsIncorrectNameAsync()
        {
            var testCode = @"public class MyNativeMethodsClass
{
    public const string bar = ""baz"";
}";
            var fixedCode = @"public class MyNativeMethodsClass
{
    public const string Bar = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 25);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseInnerInnerClassInNativeMethodsIncorrectNameAsync()
        {
            var testCode = @"
namespace Test
{
   public class NativeMethodsClass
   {
       public class Foo
       {
           public class FooInner
           {
               public const string bar = ""baz"";
           }
       }
   }
}";
            var fixedCode = @"
namespace Test
{
   public class NativeMethodsClass
   {
       public class Foo
       {
           public class FooInner
           {
               public const string Bar = ""baz"";
           }
       }
   }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(10, 36);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstFieldStartingWithUpperCaseAsync()
        {
            var testCode = @"public class Foo
{
    public const string Bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstFieldStartingWithUnderscoreAsync()
        {
            var testCode = @"public class Foo
{
    public const string _Bar = ""baz"";
}";

            // Fields starting with an underscore are reported as SA1309
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWhichIsNotConstStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1936.
        /// </summary>
        /// <remarks><para>SA1303 should not be reported on <c>enum</c> declarations. SA1300 will be reported in this
        /// case.</para></remarks>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        [Fact]
        public async Task TestEnumDeclarationsDoNotReportAsync()
        {
            var testCode = @"
public enum SpecialFile
{
    iTunesMetadata
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

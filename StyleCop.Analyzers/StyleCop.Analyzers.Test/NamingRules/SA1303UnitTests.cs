// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;

    public class SA1303UnitTests : CodeFixVerifier
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 25);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseNativeMethodsExampleOneAsync()
        {
            var testCode = @"public class NativeMethods
{        
    public const string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseNativeMethodsExampleTwoAsync()
        {
            var testCode = @"public class MyNativeMethods
{
    public const string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 25);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 36);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstFieldStartingWithUpperCaseAsync()
        {
            var testCode = @"public class Foo
{
    public const string Bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstFieldStartingWithUnderscoreAsync()
        {
            var testCode = @"public class Foo
{
    public const string _Bar = ""baz"";
}";

            // Fields starting with an underscore are reported as SA1309
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWhichIsNotConstStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1303ConstFieldNamesMustBeginWithUpperCaseLetter();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new RenameToUpperCaseCodeFixProvider();
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1025CodeMustNotContainMultipleWhitespaceInARow"/>
    /// </summary>
    public class SA1025UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics with single whitespace characters in code.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidWhitespaceAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;
    using static System.Math;
    using Action = System.Action;
    
    public class TestClass<T> : object where T : struct
    {
        //  comment  with  space
        public int TestMethod1(int a, int b)
        {
            /*  comment
            */
            return a + b;
        }

        public void TestMethod2(
            int a,
            int b)
        {
            for(var i = 0; i < a; i++)
            {
            }
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics with multiple whitespace characters in namespace declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleWhitespaceInNamespaceDeclarationAsync()
        {
            var testCode = @"namespace  TestNamespace
{
}
";

            var fixedCode = @"namespace TestNamespace
{
}
";

            DiagnosticResult[] expected = { this.CSharpDiagnostic().WithLocation(1, 10) };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics with multiple whitespace characters in using statements.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleWhitespaceInUsingStatementsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using  System;
    using  static  System.Math;
    using  Action  =  System.Action;
}
";

            var fixedCode = @"namespace TestNamespace
{
    using System;
    using static System.Math;
    using Action = System.Action;
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 10),
                this.CSharpDiagnostic().WithLocation(4, 10),
                this.CSharpDiagnostic().WithLocation(4, 18),
                this.CSharpDiagnostic().WithLocation(5, 10),
                this.CSharpDiagnostic().WithLocation(5, 18),
                this.CSharpDiagnostic().WithLocation(5, 21)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics with multiple whitespace characters in class declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleWhitespaceInClassDeclarationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public  class  TestClass<T>  :  object  where  T  :  struct
    {
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass<T> : object where T : struct
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 11),
                this.CSharpDiagnostic().WithLocation(3, 18),
                this.CSharpDiagnostic().WithLocation(3, 32),
                this.CSharpDiagnostic().WithLocation(3, 35),
                this.CSharpDiagnostic().WithLocation(3, 43),
                this.CSharpDiagnostic().WithLocation(3, 50),
                this.CSharpDiagnostic().WithLocation(3, 53),
                this.CSharpDiagnostic().WithLocation(3, 56)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics with multiple whitespace characters in method declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleWhitespaceInMethodDeclarationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public  void  TestMethod(int  a,  int  b)
        {
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int a,  int b)
        {
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 15),
                this.CSharpDiagnostic().WithLocation(5, 21),
                this.CSharpDiagnostic().WithLocation(5, 37),
                this.CSharpDiagnostic().WithLocation(5, 46)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics with multiple whitespace characters in method declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMutlipleWhitespaceInMethodBodyAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod(int a, int b)
        {
            var   x  =  10  +  20;
            return  TestMethod(x,  100);
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod(int a, int b)
        {
            var x = 10 + 20;
            return TestMethod(x,  100);
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(7, 16),
                this.CSharpDiagnostic().WithLocation(7, 20),
                this.CSharpDiagnostic().WithLocation(7, 23),
                this.CSharpDiagnostic().WithLocation(7, 27),
                this.CSharpDiagnostic().WithLocation(7, 30),
                this.CSharpDiagnostic().WithLocation(8, 19)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics with multiple whitespace characters in a property.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMutlipleWhitespaceInPropertyAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public  int  TestProperty1  {  get;  }
        public int TestProperty2 { get;  private  set; }
        public int TestProperty3  =>  10;
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestProperty1 { get;  }
        public int TestProperty2 { get;  private set; }
        public int TestProperty3 => 10;
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 15),
                this.CSharpDiagnostic().WithLocation(5, 20),
                this.CSharpDiagnostic().WithLocation(5, 35),
                this.CSharpDiagnostic().WithLocation(5, 38),
                this.CSharpDiagnostic().WithLocation(6, 49),
                this.CSharpDiagnostic().WithLocation(7, 33),
                this.CSharpDiagnostic().WithLocation(7, 37)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics with multiple whitespace characters in for loop.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMutlipleWhitespaceInForLoopAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            for(var  i  =  0;  i  <  10;  i++)
            {
            }
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            for(var i = 0;  i < 10;  i++)
            {
            }
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(7, 20),
                this.CSharpDiagnostic().WithLocation(7, 23),
                this.CSharpDiagnostic().WithLocation(7, 26),
                this.CSharpDiagnostic().WithLocation(7, 33),
                this.CSharpDiagnostic().WithLocation(7, 36)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will preserve comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPreservesCommentsAsync()
        {
            var testCode = @"namespace  /*comment*/  TestNamespace
{
    public  /*comment*/  class TestClass
    {
        public int TestMethod(int a, int b)
        {
            var     /*comment*/  x  =  10  +  20;
            return  TestMethod(x,  100);
        }
    }
}
";

            var fixedCode = @"namespace /*comment*/ TestNamespace
{
    public /*comment*/ class TestClass
    {
        public int TestMethod(int a, int b)
        {
            var /*comment*/ x = 10 + 20;
            return TestMethod(x,  100);
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(1, 10),
                this.CSharpDiagnostic().WithLocation(1, 23),
                this.CSharpDiagnostic().WithLocation(3, 11),
                this.CSharpDiagnostic().WithLocation(3, 24),
                this.CSharpDiagnostic().WithLocation(7, 16),
                this.CSharpDiagnostic().WithLocation(7, 32),
                this.CSharpDiagnostic().WithLocation(7, 35),
                this.CSharpDiagnostic().WithLocation(7, 38),
                this.CSharpDiagnostic().WithLocation(7, 42),
                this.CSharpDiagnostic().WithLocation(7, 45),
                this.CSharpDiagnostic().WithLocation(8, 19)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceAfterStatementAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("class ClassName")
                .AppendLine("{")
                .AppendLine("    void MethodName()")
                .AppendLine("    {")
                .AppendLine("        System.Console.WriteLine();  ")
                .AppendLine("    }")
                .AppendLine("}")
                .ToString();

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceAfterDeclarationAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("class ClassName      ")
                .AppendLine("{")
                .AppendLine("}")
                .ToString();

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceAfterSingleLineCommentAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("// hi there    ")
                .ToString();

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceInsideMultiLineCommentAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("/*      ")
                .AppendLine(" foo   ")
                .AppendLine("  bar   ")
                .AppendLine("*/  ")
                .ToString();

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceInsideXmlDocCommentAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("/// <summary>  ")
                .AppendLine("/// Some description    ")
                .AppendLine("/// </summary>  ")
                .AppendLine("class Foo { }")
                .ToString();

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceWithinAndAfterHereStringAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("class ClassName")
                .AppendLine("{")
                .AppendLine("    string foo = @\"      ")
                .AppendLine("more text    ")
                .AppendLine("\";  ")
                .AppendLine("}")
                .ToString();

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceAfterDirectivesAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("#define Zoot  ")
                .AppendLine("#undef Zoot2  ")
                .AppendLine("using System;  ")
                .AppendLine("#if Foo  ")
                .AppendLine("#elif Bar  ")
                .AppendLine("#else  ")
                .AppendLine("#endif ")
                .AppendLine("#warning Some warning  ")
                .AppendLine("#region Some region  ")
                .AppendLine("#endregion  ")
                .ToString();

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceFoundWithinFalseConditionalDirectiveBlocksAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("#if false")
                .AppendLine("using System;  ")
                .AppendLine("#endif")
                .ToString();

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task NoTrailingWhitespaceAfterBlockCommentAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("class Program    /* some block comment that follows several spaces */")
                .AppendLine("{")
                .AppendLine("}")
                .ToString();

            var expected = this.CSharpDiagnostic().WithLocation(1, 14);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1025CodeMustNotContainMultipleWhitespaceInARow();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1025CodeFixProvider();
        }
    }
}

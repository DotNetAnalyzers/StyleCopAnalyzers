// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    public class SA1509UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestClassDeclarationOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
class Foo

{

}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class Foo
{

}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassDeclarationOpeningBraceHasThreeBlankLineAsync()
        {
            var testCode = @"
class Foo



{

}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class Foo
{

}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassDeclarationOpeningBraceDoesntHaveBlankLineAsync()
        {
            var testCode = @"
class Foo
{

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassDeclarationCommentBeforeOpeningBraceAsync()
        {
            var testCode = @"
class Foo
//this is a comment
{

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassDeclarationMultilineCommentBeforeOpeningBraceAsync()
        {
            var testCode = @"
class Foo
/*this is a comment
that spans 2 lines */
{

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassDeclarationMultilineCommentBeforeOpeningBraceButBlankLineBetweenCommentsExistsAsync()
        {
            var testCode = @"
class Foo
/*this is a comment
that spans 2 lines */

//another comment
{

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassDeclarationMultilineCommentBlankLineBeforeOpeningBraceAsync()
        {
            var testCode = @"
class Foo
/*this is a comment
that spans 2 lines */
//another comment

{

}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class Foo
/*this is a comment
that spans 2 lines */
//another comment
{

}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructDeclarationOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
struct Foo

{

}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
struct Foo
{

}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructDeclarationOpeningBraceDoesntHaveBlankLineAsync()
        {
            var testCode = @"
struct Foo
{

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Bar()

    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class Foo
{
    void Bar()
    {
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationOpeningBraceHasTwoBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Bar()


    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class Foo
{
    void Bar()
    {
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationOpeningBraceDoesntHaveBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Bar()
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNamespaceDeclarationOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
namespace Bar

{
    class Foo
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
namespace Bar
{
    class Foo
    {
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNamespaceDeclarationOpeningBraceDoesntHaveBlankLineAsync()
        {
            var testCode = @"
namespace Bar{
    class Foo
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyDeclarationOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    string Prop

    {
        get;set;}
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class Foo
{
    string Prop
    {
        get;set;}
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyDeclarationOpeningBraceDoesntHaveBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    string Prop { get;set; }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIfStatementOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Bar()
    {
        if(1 == 1)

        {}
        else
        

        {
        }
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(8, 9),
                this.CSharpDiagnostic().WithLocation(12, 9),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class Foo
{
    void Bar()
    {
        if(1 == 1)
        {}
        else
        {
        }
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIfStatementOpeningBraceDoesntHaveBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Bar()
    {
        if(1 == 1)
        {}
        else
        {
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhileStatementOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Bar()
    {
        while(1 == 1)
        
        {}
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(8, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class Foo
{
    void Bar()
    {
        while(1 == 1)
        {}
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhileStatementOpeningBraceDoesntHaveBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Bar()
    {
        while(1 == 1)
        {}
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
using System;
class Foo
{
    void Bar()
    {
        Action a = () =>  

{ };
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
using System;
class Foo
{
    void Bar()
    {
        Action a = () =>  
{ };
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaOpeningBraceDoesntHaveBlankLineAsync()
        {
            var testCode = @"
using System;
class Foo
{
    void Bar()
    {
        Action a = () =>  { };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayInitializationOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Bar()
    {
        var a = new[]

{1, 2, 3};
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class Foo
{
    void Bar()
    {
        var a = new[]
{1, 2, 3};
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayInitializationOpeningBraceDoesntHaveBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Bar()
    {
        var a = new[] {1, 2, 3};
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyInitializerOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
class Person
{
    internal string Name {get;set;}
}

class Foo
{
    void Bar()
    {
        var p = new Person()

        { Name = ""qwe""};
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class Person
{
    internal string Name {get;set;}
}

class Foo
{
    void Bar()
    {
        var p = new Person()
        { Name = ""qwe""};
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyInitializerOpeningBraceDoesntHaveBlankLineAsync()
        {
            var testCode = @"
class Person
{
    internal string Name {get;set;}
}

class Foo
{
    void Bar()
    {
        var p = new Person()
        { 
            Name = ""qwe""
        };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousTypeOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Bar()
    {
        var p = new 

        { Name = ""qwe""};
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class Foo
{
    void Bar()
    {
        var p = new 
        { Name = ""qwe""};
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousTypeOpeningBraceDoesntHaveBlankLineAsync()
        {
            var testCode = @"
class Person
{
    internal string Name {get;set;}
}

class Foo
{
    void Bar()
    {
        var p = new { Name = ""qwe""};
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestComplex1Async()
        {
            var testCode = @"namespace Test

    {
    class Person
    
    {
        internal string Name {get;set;}
    }

    class Foo  
    {
        void Bar()
        

        {
            var a = new
//this is a comment 

{Age = 5};

            var b = new {Week = 5};
        }
    }
}";
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 5),
                this.CSharpDiagnostic().WithLocation(6, 5),
                this.CSharpDiagnostic().WithLocation(15, 9),
                this.CSharpDiagnostic().WithLocation(19, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"namespace Test
    {
    class Person
    {
        internal string Name {get;set;}
    }

    class Foo  
    {
        void Bar()
        {
            var a = new
//this is a comment 
{Age = 5};

            var b = new {Week = 5};
        }
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an interface declaration surrounded by pragma statements will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterfaceDeclarationWithSurroundingPragmasAsync()
        {
            var testCode = @"#define TEST1

namespace TestNamespace
{

#pragma warning disable SA1302 // Interface names must begin with I
    public interface ActiveConfiguredProject<out T>
#pragma warning restore SA1302 // Interface names must begin with I
    {
        /// <summary>
        /// Gets the ConfiguredProject exported value.
        /// </summary>
        T Value { get; }
    }

    public interface TestInterface1
#if TEST1
    {
        int Value { get; }
    }
#else
    {
    }
#endif

    public interface TestInterface2
#if TEST2
    {
        int Value { get; }
    }
#else
    {
    }
#endif

    public interface TestInterface3
#if TEST2
    {
        double Value { get; }
    }
#elif TEST1
    {
        int Value { get; }
    }
#endif
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1509OpeningCurlyBracketsMustNotBePrecededByBlankLine();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1509CodeFixProvider();
        }
    }
}

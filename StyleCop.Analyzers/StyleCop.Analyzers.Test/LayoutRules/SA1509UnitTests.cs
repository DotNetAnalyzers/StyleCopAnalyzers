// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1509OpeningBracesMustNotBePrecededByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1509CodeFixProvider>;

    public class SA1509UnitTests
    {
        [Fact]
        public async Task TestClassDeclarationOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
class Foo

{

}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 1);

            var fixedCode = @"
class Foo
{

}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassDeclarationOpeningBraceHasThreeBlankLineAsync()
        {
            var testCode = @"
class Foo



{

}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            var fixedCode = @"
class Foo
{

}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassDeclarationOpeningBraceDoesntHaveBlankLineAsync()
        {
            var testCode = @"
class Foo
{

}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassDeclarationCommentBeforeOpeningBraceAsync()
        {
            var testCode = @"
class Foo
//this is a comment
{

}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(7, 1);

            var fixedCode = @"
class Foo
/*this is a comment
that spans 2 lines */
//another comment
{

}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructDeclarationOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
struct Foo

{

}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 1);

            var fixedCode = @"
struct Foo
{

}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructDeclarationOpeningBraceDoesntHaveBlankLineAsync()
        {
            var testCode = @"
struct Foo
{

}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 5);

            var fixedCode = @"
class Foo
{
    void Bar()
    {
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(7, 5);

            var fixedCode = @"
class Foo
{
    void Bar()
    {
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(4, 1);

            var fixedCode = @"
namespace Bar
{
    class Foo
    {
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 5);

            var fixedCode = @"
class Foo
{
    string Prop
    {
        get;set;}
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyDeclarationOpeningBraceDoesntHaveBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    string Prop { get;set; }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(8, 9),
                Diagnostic().WithLocation(12, 9),
            };

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
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(8, 9),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class Foo
{
    void Bar()
    {
        while(1 == 1)
        {}
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(9, 1);

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
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            var fixedCode = @"
class Foo
{
    void Bar()
    {
        var a = new[]
{1, 2, 3};
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(13, 9);

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
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);

            var fixedCode = @"
class Foo
{
    void Bar()
    {
        var p = new 
        { Name = ""qwe""};
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestBlockStatementsAsync()
        {
            var testCode = @"
class Foo
{
    void Bar()
    {

        {
        }

        {
        }
    }
}";

            var fixedCode = @"
class Foo
{
    void Bar()
    {
        {
        }

        {
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestBlockStatementsWithBlockCommentAsync()
        {
            var testCode = @"
class Foo
{
    void Bar()
    {
        /* Comment */

        {
        }

        {
        }
    }
}";

            var fixedCode = @"
class Foo
{
    void Bar()
    {
        /* Comment */
        {
        }

        {
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestBlockStatementsWithLineCommentAsync()
        {
            var testCode = @"
class Foo
{
    void Bar()
    {
        // Comment

        {
        }

        {
        }
    }
}";

            var fixedCode = @"
class Foo
{
    void Bar()
    {
        // Comment
        {
        }

        {
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestBlockStatementsWithRegionAsync()
        {
            var testCode = @"
class Foo
{
    void Bar()
    {
        #region Region

        {
        }

        {
        }
        #endregion
    }
}";

            var fixedCode = @"
class Foo
{
    void Bar()
    {
        #region Region
        {
        }

        {
        }
        #endregion
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(3, 5),
                Diagnostic().WithLocation(6, 5),
                Diagnostic().WithLocation(15, 9),
                Diagnostic().WithLocation(19, 1),
            };

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
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

#pragma warning disable SA1302 // Interface names should begin with I
    public interface ActiveConfiguredProject<out T>
#pragma warning restore SA1302 // Interface names should begin with I
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

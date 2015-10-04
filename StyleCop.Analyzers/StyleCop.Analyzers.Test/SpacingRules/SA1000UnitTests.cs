// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1000KeywordsMustBeSpacedCorrectly"/> and
    /// <see cref="TokenSpacingCodeFixProvider"/>.
    /// </summary>
    public class SA1000UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestCatchallStatementAsync()
        {
            string statement = @"try
{
}
catch
{
}
";

            await this.TestKeywordStatementAsync(statement, EmptyDiagnosticResults, statement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCatchStatementAsync()
        {
            string statementWithoutSpace = @"try
{
}
catch(Exception ex)
{
}
";

            string statementWithSpace = @"try
{
}
catch (Exception ex)
{
}
";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("catch", string.Empty, "followed").WithLocation(15, 1);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFixedStatementAsync()
        {
            string statementWithoutSpace = @"byte[] y = new byte[10];
fixed(byte* b = &y[0])
{
}
";

            string statementWithSpace = @"byte[] y = new byte[10];
fixed (byte* b = &y[0])
{
}
";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("fixed", string.Empty, "followed").WithLocation(13, 1);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestForStatementAsync()
        {
            string statementWithoutSpace = @"for(int x = 0; x < 10; x++)
{
}
";

            string statementWithSpace = @"for (int x = 0; x < 10; x++)
{
}
";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("for", string.Empty, "followed").WithLocation(12, 13);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestForeachStatementAsync()
        {
            string statementWithoutSpace = @"foreach(int x in new int[0])
{
}
";

            string statementWithSpace = @"foreach (int x in new int[0])
{
}
";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("foreach", string.Empty, "followed").WithLocation(12, 13);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFromStatementAsync()
        {
            string statementWithoutSpace = @"var result = from@x in new int[3] select x;";

            string statementWithSpace = @"var result = from @x in new int[3] select x;";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("from", string.Empty, "followed").WithLocation(12, 26);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestGroupStatementAsync()
        {
            string statementWithoutSpace = @"var result = from x in new[] { new { A = 3 } }
group@x by x.A into z
select z;";

            string statementWithSpace = @"var result = from x in new[] { new { A = 3 } }
group @x by x.A into z
select z;";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("group", string.Empty, "followed").WithLocation(13, 1);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIfStatementAsync()
        {
            string statementWithoutSpace = @"if(true)
{
}
";

            string statementWithSpace = @"if (true)
{
}
";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("if", string.Empty, "followed").WithLocation(12, 13);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInStatementAsync()
        {
            string statementWithoutSpace = @"var y = new int[3]; var result = from x in@y select x;";

            string statementWithSpace = @"var y = new int[3]; var result = from x in @y select x;";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("in", string.Empty, "followed").WithLocation(12, 53);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIntoStatementAsync()
        {
            string statementWithoutSpace = @"var result = from x in new[] { new { A = 3 } }
group x by x.A into@z
select z;";

            string statementWithSpace = @"var result = from x in new[] { new { A = 3 } }
group x by x.A into @z
select z;";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("into", string.Empty, "followed").WithLocation(13, 16);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestJoinStatementAsync()
        {
            string statementWithoutSpace = @"var result = from x in new[] { new { A = 3 } }
join@a in new[] { new { B = 3 } } on x.A equals a.B
select x;";

            string statementWithSpace = @"var result = from x in new[] { new { A = 3 } }
join @a in new[] { new { B = 3 } } on x.A equals a.B
select x;";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("join", string.Empty, "followed").WithLocation(13, 1);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLetStatementAsync()
        {
            string statementWithoutSpace = @"var result = from x in new int[3]
let@z = 3
select x;";

            string statementWithSpace = @"var result = from x in new int[3]
let @z = 3
select x;";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("let", string.Empty, "followed").WithLocation(13, 1);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLockStatementAsync()
        {
            string statementWithoutSpace = @"lock(new object())
{
}
";

            string statementWithSpace = @"lock (new object())
{
}
";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("lock", string.Empty, "followed").WithLocation(12, 13);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOrderbyStatementAsync()
        {
            string statementWithoutSpace = @"var result = from x in new[] { new { A = 3 } }
orderby(x.A)
select x;";

            string statementWithSpace = @"var result = from x in new[] { new { A = 3 } }
orderby (x.A)
select x;";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("orderby", string.Empty, "followed").WithLocation(13, 1);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestReturnVoidStatementAsync()
        {
            string statementWithoutSpace = @"return;";

            string statementWithSpace = @"return ;";

            await this.TestKeywordStatementAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("return", " not", "followed").WithLocation(12, 13);

            await this.TestKeywordStatementAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestReturnIntStatementAsync()
        {
            string statementWithoutSpace = @"return(3);";

            string statementWithSpace = @"return (3);";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace, returnType: "int").ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("return", string.Empty, "followed").WithLocation(12, 13);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace, returnType: "int").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSelectStatementAsync()
        {
            string statementWithoutSpace = @"var result = from x in new int[3] select@x;";

            string statementWithSpace = @"var result = from x in new int[3] select @x;";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("select", string.Empty, "followed").WithLocation(12, 47);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStackallocStatementAsync()
        {
            string statementWithoutSpace = @"int* x = stackalloc@Int32[3];";

            string statementWithSpace = @"int* x = stackalloc @Int32[3];";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("stackalloc", string.Empty, "followed").WithLocation(12, 22);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSwitchStatementAsync()
        {
            string statementWithoutSpace = @"switch(3)
{
default:
    break;
}
";

            string statementWithSpace = @"switch (3)
{
default:
    break;
}
";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("switch", string.Empty, "followed").WithLocation(12, 13);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThrowStatementAsync()
        {
            string statementWithoutSpace = @"throw(new Exception());";

            string statementWithSpace = @"throw (new Exception());";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("throw", string.Empty, "followed").WithLocation(12, 13);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRethrowStatementAsync()
        {
            string statementWithoutSpace = @"try
{
}
catch (Exception ex)
{
    throw;
}
";

            string statementWithSpace = @"try
{
}
catch (Exception ex)
{
    throw ;
}
";

            await this.TestKeywordStatementAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("throw", " not", "followed").WithLocation(17, 5);

            await this.TestKeywordStatementAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUsingStatementAsync()
        {
            string statementWithoutSpace = @"using(default(IDisposable))
{
}
";

            string statementWithSpace = @"using (default(IDisposable))
{
}
";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("using", string.Empty, "followed").WithLocation(12, 13);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhereStatementAsync()
        {
            string statementWithoutSpace = @"var result = from x in new[] { new { A = true } }
where(x.A)
select x;";

            string statementWithSpace = @"var result = from x in new[] { new { A = true } }
where (x.A)
select x;";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("where", string.Empty, "followed").WithLocation(13, 1);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhileStatementAsync()
        {
            string statementWithoutSpace = @"while(false)
{
}
";

            string statementWithSpace = @"while (false)
{
}
";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("while", string.Empty, "followed").WithLocation(12, 13);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestYieldReturnStatementAsync()
        {
            // There is no way to have a 'yield' keyword which is not followed by a space. All we need to do is verify
            // that no diagnostic is reported for its use with a space.
            string statementWithSpace = @"yield return 3;";
            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace, returnType: "IEnumerable<int>").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestYieldBreakStatementAsync()
        {
            // There is no way to have a 'yield' keyword which is not followed by a space. All we need to do is verify
            // that no diagnostic is reported for its use with a space.
            string statementWithSpace = @"yield break;";
            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace, returnType: "IEnumerable<int>").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCheckedStatementAsync()
        {
            string statementWithoutSpace = @"int x = checked(3);";

            string statementWithSpace = @"int x = checked (3);";

            await this.TestKeywordStatementAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("checked", " not", "followed").WithLocation(12, 21);

            await this.TestKeywordStatementAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);

            statementWithoutSpace = @"checked{ };";

            statementWithSpace = @"checked { };";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            expected = this.CSharpDiagnostic().WithArguments("checked", string.Empty, "followed").WithLocation(12, 13);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDefaultCaseStatementAsync()
        {
            string statementWithoutSpace = @"switch (3)
{
default:
    break;
}
";

            string statementWithSpace = @"switch (3)
{
default :
    break;
}
";

            await this.TestKeywordStatementAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("default", " not", "followed").WithLocation(14, 1);

            await this.TestKeywordStatementAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDefaultValueStatementAsync()
        {
            string statementWithoutSpace = @"int x = default(int);";

            string statementWithSpace = @"int x = default (int);";

            await this.TestKeywordStatementAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("default", " not", "followed").WithLocation(12, 21);

            await this.TestKeywordStatementAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNameofStatementAsync()
        {
            string statementWithoutSpace = @"string x = nameof(x);";

            string statementWithSpace = @"string x = nameof (x);";

            await this.TestKeywordStatementAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("nameof", " not", "followed").WithLocation(12, 24);

            await this.TestKeywordStatementAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSizeofStatementAsync()
        {
            string statementWithoutSpace = @"int x = sizeof(int);";

            string statementWithSpace = @"int x = sizeof (int);";

            await this.TestKeywordStatementAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("sizeof", " not", "followed").WithLocation(12, 21);

            await this.TestKeywordStatementAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeofStatementAsync()
        {
            string statementWithoutSpace = @"Type x = typeof(int);";

            string statementWithSpace = @"Type x = typeof (int);";

            await this.TestKeywordStatementAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("typeof", " not", "followed").WithLocation(12, 22);

            await this.TestKeywordStatementAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUncheckedStatementAsync()
        {
            string statementWithoutSpace = @"int x = unchecked(3);";

            string statementWithSpace = @"int x = unchecked (3);";

            await this.TestKeywordStatementAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("unchecked", " not", "followed").WithLocation(12, 21);

            await this.TestKeywordStatementAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);

            statementWithoutSpace = @"unchecked{ };";

            statementWithSpace = @"unchecked { };";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            expected = this.CSharpDiagnostic().WithArguments("unchecked", string.Empty, "followed").WithLocation(12, 13);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNewObjectStatementAsync()
        {
            string statementWithoutSpace = @"int x = new@Int32();";

            string statementWithSpace = @"int x = new @Int32();";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("new", string.Empty, "followed").WithLocation(12, 21);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNewArrayStatementAsync()
        {
            string statementWithoutSpace = @"int[] x = new@Int32[3];";

            string statementWithSpace = @"int[] x = new @Int32[3];";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("new", string.Empty, "followed").WithLocation(12, 23);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNewImplicitArrayStatementAsync()
        {
            string statementWithoutSpace = @"int[] x = new[] { 3 };";

            string statementWithSpace = @"int[] x = new [] { 3 };";

            await this.TestKeywordStatementAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            // this case is handled by SA1026, so it shouldn't be reported here
            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNewConstructorContraintStatement_TypeAsync()
        {
            string statementWithSpace = @"public class Foo<T> where T : new ()
{
}";

            string statementWithoutSpace = @"public class Foo<T> where T : new()
{
}";

            await this.TestKeywordDeclarationAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("new", " not", "followed").WithLocation(9, 39);

            await this.TestKeywordDeclarationAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNewConstructorContraintStatement_TypeWithMultipleConstraintsAsync()
        {
            string statementWithSpace = @"public class Foo<T> where T : IDisposable, new ()
{
}";

            string statementWithoutSpace = @"public class Foo<T> where T : IDisposable, new()
{
}";

            await this.TestKeywordDeclarationAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("new", " not", "followed").WithLocation(9, 52);

            await this.TestKeywordDeclarationAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNewConstructorContraintStatement_TypeWithClassConstraintsAsync()
        {
            string statementWithSpace = @"public class Foo<T> where T : class, new ()
{
}";

            string statementWithoutSpace = @"public class Foo<T> where T : class, new()
{
}";

            await this.TestKeywordDeclarationAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("new", " not", "followed").WithLocation(9, 46);

            await this.TestKeywordDeclarationAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNewConstructorContraintStatement_MethodAsync()
        {
            string statementWithSpace = @"public void Foo<T>() where T : new ()
{
}";

            string statementWithoutSpace = @"public void Foo<T>() where T : new()
{
}";

            await this.TestKeywordDeclarationAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("new", " not", "followed").WithLocation(9, 40);

            await this.TestKeywordDeclarationAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNewConstructorContraintStatement_MethodWithMultipleConstraintsAsync()
        {
            string statementWithSpace = @"public void Foo<T>() where T : IDisposable, new ()
{
}";

            string statementWithoutSpace = @"public void Foo<T>() where T : IDisposable, new()
{
}";

            await this.TestKeywordDeclarationAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("new", " not", "followed").WithLocation(9, 53);

            await this.TestKeywordDeclarationAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNewConstructorContraintStatement_MethodWithClassConstraintsAsync()
        {
            string statementWithSpace = @"public void Foo<T>() where T : class, new ()
{
}";

            string statementWithoutSpace = @"public void Foo<T>() where T : class, new()
{
}";

            await this.TestKeywordDeclarationAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("new", " not", "followed").WithLocation(9, 47);

            await this.TestKeywordDeclarationAsync(statementWithSpace, expected, statementWithoutSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAwaitIdentifierAsync()
        {
            string statementWithoutSpace = @"var result = await(default(Task<int>));";

            string statementWithSpace = @"var result = await (default(Task<int>));";

            await this.TestKeywordStatementAsync(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace, asyncMethod: false).ConfigureAwait(false);
            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace, asyncMethod: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAwaitStatementAsync()
        {
            string statementWithoutSpace = @"var result = await(default(Task<int>));";

            string statementWithSpace = @"var result = await (default(Task<int>));";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace, asyncMethod: true).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("await", string.Empty, "followed").WithLocation(12, 26);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace, asyncMethod: true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCaseStatementAsync()
        {
            string statementWithoutSpace = @"switch (3)
{
case(3):
default:
    break;
}
";

            string statementWithSpace = @"switch (3)
{
case (3):
default:
    break;
}
";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("case", string.Empty, "followed").WithLocation(14, 1);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestGotoCaseStatementAsync()
        {
            string statementWithoutSpace = @"switch (3)
{
case 2:
    goto case(3);

case 3:
default:
    break;
}
";

            string statementWithSpace = @"switch (3)
{
case 2:
    goto case (3);

case 3:
default:
    break;
}
";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("case", string.Empty, "followed").WithLocation(15, 10);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissingSelectTokenAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        var result = from x in new int[0];
    }
}
";

            DiagnosticResult[] expected =
            {
                new DiagnosticResult
                {
                    Id = "CS0742",
                    Severity = DiagnosticSeverity.Error,
                    Message = "A query body must end with a select clause or a group clause",
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 42) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTrailingCommentAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        if/*comment*/ (true)
        {
        }
    }
}
";
            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
        if /*comment*/ (true)
        {
        }
    }
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("if", string.Empty, "followed").WithLocation(6, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1000KeywordsMustBeSpacedCorrectly();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }

        private Task TestKeywordStatementAsync(string statement, DiagnosticResult expected, string fixedStatement, string returnType = "void", bool asyncMethod = false)
        {
            return this.TestKeywordStatementAsync(statement, new[] { expected }, fixedStatement, returnType, asyncMethod);
        }

        private async Task TestKeywordStatementAsync(string statement, DiagnosticResult[] expected, string fixedStatement, string returnType = "void", bool asyncMethod = false)
        {
            string testCodeFormat = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Namespace
{{
    {2}class ClassName
    {{
        {0}{4} Foo()
        {{
            {1}
        }}
        {3}
    }}
}}
";

            string unsafeModifier = asyncMethod ? string.Empty : "unsafe ";
            string asyncModifier = asyncMethod ? "async " : string.Empty;
            string awaitMethod = asyncMethod ? string.Empty : "int await(Task task) { return 0; }";
            string testCode = string.Format(testCodeFormat, asyncModifier, statement, unsafeModifier, awaitMethod, returnType);
            string fixedTest = string.Format(testCodeFormat, asyncModifier, fixedStatement, unsafeModifier, awaitMethod, returnType);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        private Task TestKeywordDeclarationAsync(string statement, DiagnosticResult expected, string fixedStatement)
        {
            return this.TestKeywordDeclarationAsync(statement, new[] { expected }, fixedStatement);
        }

        private async Task TestKeywordDeclarationAsync(string statement, DiagnosticResult[] expected, string fixedStatement)
        {
            string testCodeFormat = @"
using System;
using System.Linq;
using System.Threading.Tasks;
namespace Namespace
{{
    class ClassName
    {{
        {0}
    }}
}}
";

            string testCode = string.Format(testCodeFormat, statement);
            string fixedTest = string.Format(testCodeFormat, fixedStatement);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }
    }
}

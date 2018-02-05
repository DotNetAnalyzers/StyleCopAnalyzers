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
    using static StyleCop.Analyzers.SpacingRules.TS1000OpeningParenthesisMustBeFollowedByASpace;

   /// <summary>
   /// Unit tests for the <see cref="TS1000OpeningParenthesisMustBeFollowedByASpace"/> class.
   /// </summary>
   public class TS1000UnitTests : CodeFixVerifier
   {
      [Fact]
      public async Task IfStatement_FineParens_NoErrorsAsync()
      {
         var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            int n = 1;
            if( n == 1 ){}
        }
    }
}
";

         await this.VerifyCSharpDiagnosticAsync( testCode, EmptyDiagnosticResults, CancellationToken.None ).ConfigureAwait( false );
      }

      [Fact]
      public async Task CallMethod_FineParens_NoErrorsAsync()
      {
         var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void SomeMethod()
        {
        }
        public void TestMethod()
        {
            SomeMethod();
        }
    }
}
";

         await this.VerifyCSharpDiagnosticAsync( testCode, EmptyDiagnosticResults, CancellationToken.None ).ConfigureAwait( false );
      }

      [Fact]
      public async Task CastType_FineParens_NoErrorsAsync()
      {
         var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            double d = 1d;
            int n = (int)d;
        }
    }
}
";

         await this.VerifyCSharpDiagnosticAsync( testCode, EmptyDiagnosticResults, CancellationToken.None ).ConfigureAwait( false );
      }

      [Fact]
      public async Task CastTypeWithExtraParens_FineParens_NoErrorsAsync()
      {
         var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            double d = 1d;
            int n = ( (int)d );
        }
    }
}
";

         await this.VerifyCSharpDiagnosticAsync( testCode, EmptyDiagnosticResults, CancellationToken.None ).ConfigureAwait( false );
      }

      [Fact]
      public async Task IfStatement_WrongParens_ReportsAndFixesAsync()
      {
         var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            int n = 1;
            if(n == 1 ){}
        }
    }
}
";

         var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            int n = 1;
            if( n == 1 ){}
        }
    }
}
";

         DiagnosticResult[] expectedDiagnostic =
         {
            this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(8, 15),
         };

         await this.VerifyCSharpDiagnosticAsync( testCode, expectedDiagnostic, CancellationToken.None ).ConfigureAwait( false );
         await this.VerifyCSharpDiagnosticAsync( fixedTestCode, EmptyDiagnosticResults, CancellationToken.None ).ConfigureAwait( false );
         await this.VerifyCSharpFixAsync( testCode, fixedTestCode ).ConfigureAwait( false );
      }

      protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
      {
         yield return new TS1000OpeningParenthesisMustBeFollowedByASpace();
      }

      protected override CodeFixProvider GetCSharpCodeFixProvider()
      {
         return new TokenSpacingCodeFixProvider();
      }
   }
}

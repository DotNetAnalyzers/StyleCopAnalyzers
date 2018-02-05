// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.MaintainabilityRules;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;
   using static StyleCop.Analyzers.MaintainabilityRules.TS1002NumericLiteralsMustHaveSuffix;

    public class TS1002UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = TS1002NumericLiteralsMustHaveSuffix.DiagnosticId;

        [Fact]
        public async Task IntLiteral_NoSuffix_NoProblemAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        int n = 1;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

      [Fact]
      public async Task DoubleLiteral_WithSuffix_NoProblemAsync()
      {
         var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        double d1 = 1d;
    }
}";
         await this.VerifyCSharpDiagnosticAsync( testCode, EmptyDiagnosticResults, CancellationToken.None ).ConfigureAwait( false );
      }

      [Fact]
      public async Task DoubleLiteral_NoSuffix_ShouldHaveSuffixAsync()
      {
         var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        double d1 = 1.0;
    }
}";

         DiagnosticResult[] expectedDiagnostic =
         {
            this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(5, 21),
         };

         await this.VerifyCSharpDiagnosticAsync( testCode, expectedDiagnostic, CancellationToken.None ).ConfigureAwait( false );
      }

      [Fact]
      public async Task DoubleLiteral_NoSuffix_ProperFixAsync()
      {
         var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        double d1 = 1.0;
    }
}";

         var fixedTestCode = @"public class TestClass
{
    public void TestMethod()
    {
        double d1 = 1d;
    }
}";

         DiagnosticResult[] expectedDiagnostic =
         {
            this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(5, 21),
         };

         await this.VerifyCSharpDiagnosticAsync( testCode, expectedDiagnostic, CancellationToken.None ).ConfigureAwait( false );
         await this.VerifyCSharpDiagnosticAsync( fixedTestCode, EmptyDiagnosticResults, CancellationToken.None ).ConfigureAwait( false );
         await this.VerifyCSharpFixAsync( testCode, fixedTestCode ).ConfigureAwait( false );
      }

      /// <inheritdoc/>
      protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new TS1002NumericLiteralsMustHaveSuffix();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
         return new TS1002CodeFixProvider();
        }
    }
}

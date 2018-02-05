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
   using static StyleCop.Analyzers.MaintainabilityRules.TS1003UseLinqAnyOverCountGreaterThan0;

    public class TS1003UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = TS1003UseLinqAnyOverCountGreaterThan0.DiagnosticId;

        [Fact]
        public async Task UsesAnySyntax_NoCountGreaterThan0_NoProblemAsync()
        {
            var testCode = @"using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
internal class TestClass
{
    internal void TestMethod()
    {
        List<int> myList = new List<int>() { };
        Debug.Assert( !myList.Any() );
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

      [Fact]
      public async Task UsesCountSyntax_ComparesGreaterThan0_HasProblemAsync()
      {
         var testCode = @"using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
internal class TestClass
{
    internal void TestMethod()
    {
        List<int> myList = new List<int>() { 1, 2, 3 };
        Debug.Assert(myList.Count > 0);
    }
}";

         DiagnosticResult[] expectedDiagnostic =
         {
            this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(9, 28),
         };

         await this.VerifyCSharpDiagnosticAsync( testCode, expectedDiagnostic, CancellationToken.None ).ConfigureAwait( false );
      }

      [Fact]
      public async Task UsesCountSyntax_ComparesGreaterThan0_HasProblemButFixedAsync()
      {
         var testCode = @"using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

internal class TestClass
{
    internal void TestMethod()
    {
        List<int> myList = new List<int>() { 1, 2, 3 };
        Debug.Assert(myList.Count > 0);
    }
}";

         var fixedTestCode = @"using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

internal class TestClass
{
    internal void TestMethod()
    {
        List<int> myList = new List<int>() { 1, 2, 3 };
        Debug.Assert(myList.Any());
    }
}";

         DiagnosticResult[] expectedDiagnostic =
         {
            this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(10, 28),
         };

         await this.VerifyCSharpDiagnosticAsync( testCode, expectedDiagnostic, CancellationToken.None ).ConfigureAwait( false );
         await this.VerifyCSharpDiagnosticAsync( fixedTestCode, EmptyDiagnosticResults, CancellationToken.None ).ConfigureAwait( false );
         await this.VerifyCSharpFixAsync( testCode, fixedTestCode ).ConfigureAwait( false );
      }

      [Fact]
      public async Task UsesCountSyntax_NoUsingStatement_HasUsingStatementAndFixedAsync()
      {
         var testCode = @"using System.Collections.Generic;
using System.Diagnostics;

internal class TestClass
{
    internal void TestMethod()
    {
        List<int> myList = new List<int>() { 1, 2, 3 };
        Debug.Assert(myList.Count > 0);
    }
}";

         var fixedTestCode = @"using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

internal class TestClass
{
    internal void TestMethod()
    {
        List<int> myList = new List<int>() { 1, 2, 3 };
        Debug.Assert(myList.Any());
    }
}";

         DiagnosticResult[] expectedDiagnostic =
         {
            this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(9, 28),
         };

         await this.VerifyCSharpDiagnosticAsync( testCode, expectedDiagnostic, CancellationToken.None ).ConfigureAwait( false );
         await this.VerifyCSharpDiagnosticAsync( fixedTestCode, EmptyDiagnosticResults, CancellationToken.None ).ConfigureAwait( false );
         await this.VerifyCSharpFixAsync( testCode, fixedTestCode ).ConfigureAwait( false );
      }

      [Fact]
      public async Task UsesCountSyntax_NoUsingStatement_PutsUsingInCorrectOrderAsync()
      {
         var testCode = @"using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

internal class TestClass
{
    internal void TestMethod()
    {
        List<int> myList = new List<int>() { 1, 2, 3 };
        Debug.Assert(myList.Count > 0);
    }
}";

         var fixedTestCode = @"using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

internal class TestClass
{
    internal void TestMethod()
    {
        List<int> myList = new List<int>() { 1, 2, 3 };
        Debug.Assert(myList.Any());
    }
}";

         DiagnosticResult[] expectedDiagnostic =
         {
            this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(10, 28),
         };

         await this.VerifyCSharpDiagnosticAsync( testCode, expectedDiagnostic, CancellationToken.None ).ConfigureAwait( false );
         await this.VerifyCSharpDiagnosticAsync( fixedTestCode, EmptyDiagnosticResults, CancellationToken.None ).ConfigureAwait( false );
         await this.VerifyCSharpFixAsync( testCode, fixedTestCode ).ConfigureAwait( false );
      }

      /// <inheritdoc/>
      protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new TS1003UseLinqAnyOverCountGreaterThan0();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
         return new TS1003CodeFixProvider();
        }
    }
}

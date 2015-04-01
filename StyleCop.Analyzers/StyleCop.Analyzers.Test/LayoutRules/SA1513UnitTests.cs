using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1513ClosingCurlyBracketMustBeFollowedByBlankLine"/>
    /// </summary>
    public class SA1513UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that all valid usages of a closing curly brace without a following blank line will report no diagnostic.
        /// </summary>
        [Fact]
        public async Task TestValid()
        {
            var testCode = @"using System;
using System.Linq;

public class Foo
{
    private int x;

    // Valid #1
    public int Bar
    {
        get { return this.x; }
        set { this.x = value; }
    }

    public void Baz()
    {
        // Valid #2
        try
        {
            this.x++;
        }
        catch (Exception)
        {
            this.x = 0;
        }
        finally
        {
            this.x++;
        }

        // Valid #3
        do
        {
            this.x++;
        }
        while (this.x < 10);

        // Valid #4
        if (this.x > 0)
        {
            this.x++;
        }
        else
        {
            this.x = 0;
        }

        // Valid #5
        var y = new[] { 1, 2, 3 };

        // Valid #6
        if (this.x > 0)
        {
            if (y != null)
            {
                this.x = -this.x;
            }
        }

        // Valid #7
        if (this.x > 0)
        {
            this.x = 0;
        }
#if !SOMETHING
        else        
        {
            this.x++;    
        }
#endif

        // Valid #8
#if !SOMETHING
        if (this.x > 0)
        {
            this.x = 0;
        }
#else
        if (this.x < 0)        
        {
            this.x++;    
        }
#endif

        // Valid #9
        var q1 = 
            from a in new[] 
            {
                1,
                2,
                3
            }
            from b in new[] { 4, 5, 6}
            select a*b;

        // Valid #10
        var q2 = 
            from a in new[] 
            { 
                1,
                2,
                3
            }
            let b = new[] 
            { 
                a, 
                a * a, 
                a * a * a 
            }
            select b;

        // Valid #11
        var q3 = 
            from a in new[] 
            {
                1,
                2,
                3
            }
            where a > 0
            select a;

        // Valid #12
        var q4 = 
            from a in new[] 
            {
                new { Number = 1 },
                new { Number = 2 },
                new { Number = 3 }
            }
            join b in new[] 
            { 
                new { Number = 2 },
                new { Number = 3 },
                new { Number = 4 }
            }
            on a.Number equals b.Number
            select new { Number1 = a.Number, Number2 = b.Number };

        // Valid #13
        var q5 = 
            from a in new[] 
            {
                new { Number = 1 },
                new { Number = 2 },
                new { Number = 3 }
            }
            orderby a.Number descending
            select a;

        // Valid #14
        var q6 = 
            from a in new[] 
            { 
                1,
                2,
                3
            }
            group new
            {
                Number = a,
                Square = a * a
            }
            by a;

        // Valid #15
        switch (this.x)
        {
            case 0:
            if (this.x < 0)
            {
                this.x = -1;
            }
            break;

            case 1:
            {
                var temp = this.x * this.x;
                this.x = temp;
            }
            break;
        }

        // Valid #16
        var d = new[]
        {
            1, 2, 3
        };

        // Valid #17
        this.Qux(i =>
        {
            return d[i] * 2;
        });
    }

    public void Qux(Func<int, int> function)
    {
        this.x = function(this.x);
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestInvalid()
        {
            var testCode = @"using System;

public class Foo
{
    private int x;

    // Invalid #1
    public int Property1
    {
        get
        {        
            return this.x;
        }
        set
        {
            this.x = value;
        }
        /* some comment */
    }

    // Invalid #2
    public int Property2
    {
        get { return this.x; }
    }
    public void Baz()
    {
        // Invalid #3
        switch (this.x)
        {
            case 1:
            {
                this.x = 1;
                break;
            }
            case 2:
                this.x = 2;
                break;
        }

        // Invalid #4
        {
            var temp = this.x;
            this.x = temp * temp;
        }
        this.x++;

        // Invalid #5
        if (this.x > 1)
        {
            this.x = 1;
        }
        if (this.x < 0)
        {
            this.x = 0;
        }
    }
}
";
            var expected = new[]
            {
                // Invalid #1
                this.CSharpDiagnostic().WithLocation(13, 10),
                this.CSharpDiagnostic().WithLocation(17, 10),
                // Invalid #2
                this.CSharpDiagnostic().WithLocation(25, 6),
                // Invalid #3
                this.CSharpDiagnostic().WithLocation(35, 14),
                // Invalid #4
                this.CSharpDiagnostic().WithLocation(45, 10),
                // Invalid #5
                this.CSharpDiagnostic().WithLocation(52, 10)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1513ClosingCurlyBracketMustBeFollowedByBlankLine();
        }
    }
}

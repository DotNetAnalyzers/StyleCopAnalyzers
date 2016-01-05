// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1137ElementsShouldHaveTheSameIndentation"/>.
    /// </summary>
    public class SA1137UnitTests : CodeFixVerifier
    {
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        [InlineData("enum")]
        public async Task TestBaseTypeDeclarationAsync(string baseTypeKind)
        {
            // Need to test attribute lists here
            string testCode = $@"
using System;

namespace Namespace0
{{
    [My] [My] {baseTypeKind} TypeName {{ }}
}}

namespace Namespace1
{{
    [My]
  [My] {baseTypeKind} TypeName {{ }}
}}

namespace Namespace2
{{
  [My]
    [My]
  {baseTypeKind} TypeName {{ }}
}}

namespace Namespace3
{{
    [My]
    [My]
  {baseTypeKind} TypeName {{ }}
}}

namespace Namespace4
{{
    {baseTypeKind} TypeName1 {{ }}

  [My] {baseTypeKind} TypeName2 {{ }}
}}

namespace Namespace5
{{
    {baseTypeKind} TypeName1 {{ }}

    [My]
  [My] {baseTypeKind} TypeName2 {{ }}
}}

namespace Namespace6
{{
    {baseTypeKind} TypeName1 {{ }}

  [My]
    [My] {baseTypeKind} TypeName2 {{ }}
}}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute {{ }}
";
            string fixedCode = $@"
using System;

namespace Namespace0
{{
    [My] [My] {baseTypeKind} TypeName {{ }}
}}

namespace Namespace1
{{
    [My]
    [My] {baseTypeKind} TypeName {{ }}
}}

namespace Namespace2
{{
  [My]
  [My]
  {baseTypeKind} TypeName {{ }}
}}

namespace Namespace3
{{
  [My]
  [My]
  {baseTypeKind} TypeName {{ }}
}}

namespace Namespace4
{{
    {baseTypeKind} TypeName1 {{ }}

    [My] {baseTypeKind} TypeName2 {{ }}
}}

namespace Namespace5
{{
    {baseTypeKind} TypeName1 {{ }}

    [My]
    [My] {baseTypeKind} TypeName2 {{ }}
}}

namespace Namespace6
{{
    {baseTypeKind} TypeName1 {{ }}

    [My]
    [My] {baseTypeKind} TypeName2 {{ }}
}}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute {{ }}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(12, 1),
                this.CSharpDiagnostic().WithLocation(18, 1),
                this.CSharpDiagnostic().WithLocation(24, 1),
                this.CSharpDiagnostic().WithLocation(25, 1),
                this.CSharpDiagnostic().WithLocation(33, 1),
                this.CSharpDiagnostic().WithLocation(41, 1),
                this.CSharpDiagnostic().WithLocation(48, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeDeclarationConstraintClausesAsync(string typeKind)
        {
            string testCode = $@"
{typeKind} NonGenericType
{{
}}

{typeKind} TypeWithoutConstraints<T>
{{
}}

{typeKind} TypeWithOneConstraint<T>
    where T : new()
{{
}}

{typeKind} TypeWithMultipleConstraints1<T1, T2, T3> where T1 : new()
    where T2 : new()
     where T3 : new()
{{
}}

{typeKind} TypeWithMultipleConstraints2<T1, T2, T3>
where T1 : new()
    where T2 : new()
     where T3 : new()
{{
}}
";
            string fixedCode = $@"
{typeKind} NonGenericType
{{
}}

{typeKind} TypeWithoutConstraints<T>
{{
}}

{typeKind} TypeWithOneConstraint<T>
    where T : new()
{{
}}

{typeKind} TypeWithMultipleConstraints1<T1, T2, T3> where T1 : new()
    where T2 : new()
    where T3 : new()
{{
}}

{typeKind} TypeWithMultipleConstraints2<T1, T2, T3>
where T1 : new()
where T2 : new()
where T3 : new()
{{
}}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(17, 1),
                this.CSharpDiagnostic().WithLocation(23, 1),
                this.CSharpDiagnostic().WithLocation(24, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestBlockAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
      label1:
        if (true)
        {
        }

     label2:
       while (true)
        {
        }

label3:
while (true)
    {
label4a:
 label4b:
int x;

        label5a:
label5b:
        int y;
    }
    }
}
";
            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
      label1:
        if (true)
        {
        }

      label2:
        while (true)
        {
        }

      label3:
        while (true)
    {
label4a:
label4b:
int x;

label5a:
label5b:
int y;
    }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(11, 1),
                this.CSharpDiagnostic().WithLocation(12, 1),
                this.CSharpDiagnostic().WithLocation(16, 1),
                this.CSharpDiagnostic().WithLocation(17, 1),
                this.CSharpDiagnostic().WithLocation(20, 1),
                this.CSharpDiagnostic().WithLocation(23, 1),
                this.CSharpDiagnostic().WithLocation(25, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSwitchStatementAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        switch (0)
        {
        case 0:
      label1:
            if (true)
            {
            }

                break;

       case 1:
case 2:
     label2:
           while (true)
            {
            }

           break;

default:
label3a:
 label3b:
break;
        }
    }
}
";
            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
        switch (0)
        {
        case 0:
      label1:
            if (true)
            {
            }

            break;

        case 1:
        case 2:
      label2:
            while (true)
            {
            }

            break;

        default:
      label3a:
      label3b:
            break;
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(14, 1),
                this.CSharpDiagnostic().WithLocation(16, 1),
                this.CSharpDiagnostic().WithLocation(17, 1),
                this.CSharpDiagnostic().WithLocation(18, 1),
                this.CSharpDiagnostic().WithLocation(19, 1),
                this.CSharpDiagnostic().WithLocation(23, 1),
                this.CSharpDiagnostic().WithLocation(25, 1),
                this.CSharpDiagnostic().WithLocation(26, 1),
                this.CSharpDiagnostic().WithLocation(27, 1),
                this.CSharpDiagnostic().WithLocation(28, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestValidInitializerExpressionAsync()
        {
            string testCode = @"
using System.Collections.Generic;
class ClassName
{
    void EmptyInitializersMethod()
    {
        // array initializer
        int[] array = { };

        // collection initializer
        List<int> list = new List<int> { };

        // complex element initializer
        Dictionary<int, int> dictionary = new Dictionary<int, int> { };

        // object initializer
        var obj = new StructName { };
    }

    void SingleLineInitializersMethod()
    {
        // array initializer
        int[] array = { 0 };

        // collection initializer
        List<int> list = new List<int> { 0 };

        // complex element initializer
        Dictionary<int, int> dictionary = new Dictionary<int, int> { { 0, 0 } };

        // object initializer
        var obj = new StructName { X = 0 };
    }

    void SingleElementInitializersMethod()
    {
        // array initializer
        int[] array =
        {
            0,
        };

        // collection initializer
        List<int> list =
            new List<int>
            {
                0,
            };

        // complex element initializer
        Dictionary<int, int> dictionary =
            new Dictionary<int, int>
            {
                { 0, 0 },
            };

        // object initializer
        var obj =
            new StructName
            {
                X = 0,
            };
    }

    void SharedLineInitializersMethod()
    {
        // array initializer
        int[] array =
        {
            0, 0,
        };

        // collection initializer
        List<int> list =
            new List<int>
            {
                0, 0,
            };

        // complex element initializer
        Dictionary<int, int> dictionary =
            new Dictionary<int, int>
            {
                { 0, 0 }, { 0, 0 },
            };

        // object initializer
        var obj =
            new StructName
            {
                X = 0, Y = 0,
            };
    }
}

struct StructName
{
    public int X, Y, Z;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInitializerExpressionAsync()
        {
            string testCode = @"
using System.Collections.Generic;
class ClassName
{
    void NonZeroAlignmentMethod()
    {
        // array initializer
        int[] array =
        {
            0,
          0,
0,
        };

        // collection initializer
        List<int> list =
            new List<int>
            {
                0,
              0,
0,
            };

        // complex element initializer
        Dictionary<int, int> dictionary =
            new Dictionary<int, int>
            {
                { 0, 0 },
              { 0, 0 },
{ 0, 0 },
            };

        // object initializer
        var obj =
            new StructName
            {
                X = 0,
              Y = 0,
Z = 0,
            };
    }

    void ZeroAlignmentMethod()
    {
        // array initializer
        int[] array =
        {
0,
          0,
            0,
        };

        // collection initializer
        List<int> list =
            new List<int>
            {
0,
              0,
                0,
            };

        // complex element initializer
        Dictionary<int, int> dictionary =
            new Dictionary<int, int>
            {
{ 0, 0 },
              { 0, 0 },
                { 0, 0 },
            };

        // object initializer
        var obj =
            new StructName
            {
X = 0,
              Y = 0,
                Z = 0,
            };
    }
}

struct StructName
{
    public int X, Y, Z;
}
";
            string fixedCode = @"
using System.Collections.Generic;
class ClassName
{
    void NonZeroAlignmentMethod()
    {
        // array initializer
        int[] array =
        {
            0,
            0,
            0,
        };

        // collection initializer
        List<int> list =
            new List<int>
            {
                0,
                0,
                0,
            };

        // complex element initializer
        Dictionary<int, int> dictionary =
            new Dictionary<int, int>
            {
                { 0, 0 },
                { 0, 0 },
                { 0, 0 },
            };

        // object initializer
        var obj =
            new StructName
            {
                X = 0,
                Y = 0,
                Z = 0,
            };
    }

    void ZeroAlignmentMethod()
    {
        // array initializer
        int[] array =
        {
0,
0,
0,
        };

        // collection initializer
        List<int> list =
            new List<int>
            {
0,
0,
0,
            };

        // complex element initializer
        Dictionary<int, int> dictionary =
            new Dictionary<int, int>
            {
{ 0, 0 },
{ 0, 0 },
{ 0, 0 },
            };

        // object initializer
        var obj =
            new StructName
            {
X = 0,
Y = 0,
Z = 0,
            };
    }
}

struct StructName
{
    public int X, Y, Z;
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(11, 1),
                this.CSharpDiagnostic().WithLocation(12, 1),
                this.CSharpDiagnostic().WithLocation(20, 1),
                this.CSharpDiagnostic().WithLocation(21, 1),
                this.CSharpDiagnostic().WithLocation(29, 1),
                this.CSharpDiagnostic().WithLocation(30, 1),
                this.CSharpDiagnostic().WithLocation(38, 1),
                this.CSharpDiagnostic().WithLocation(39, 1),
                this.CSharpDiagnostic().WithLocation(49, 1),
                this.CSharpDiagnostic().WithLocation(50, 1),
                this.CSharpDiagnostic().WithLocation(58, 1),
                this.CSharpDiagnostic().WithLocation(59, 1),
                this.CSharpDiagnostic().WithLocation(67, 1),
                this.CSharpDiagnostic().WithLocation(68, 1),
                this.CSharpDiagnostic().WithLocation(76, 1),
                this.CSharpDiagnostic().WithLocation(77, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1137ElementsShouldHaveTheSameIndentation();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new IndentationCodeFixProvider();
        }
    }
}

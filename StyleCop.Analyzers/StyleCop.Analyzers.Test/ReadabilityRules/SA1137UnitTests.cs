﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
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
        public async Task TestNamespaceDeclarationAsync(string baseTypeKind)
        {
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

        [Theory]
        [InlineData("class", "int", " { }")]
        [InlineData("struct", "int", " { }")]
        [InlineData("interface", "event System.EventHandler", ";")]
        public async Task TestTypeDeclarationMembersAsync(string typeKind, string fieldType, string methodBody)
        {
            string testCode = $@"
{typeKind} Container1
{{
        {fieldType} X1;
      int Y1 {{ get; }}
void Z1(){methodBody}
}}

{typeKind} Container2
{{
{fieldType} X2;
      int Y2 {{ get; }}
        void Z2(){methodBody}
}}
";
            string fixedCode = $@"
{typeKind} Container1
{{
        {fieldType} X1;
        int Y1 {{ get; }}
        void Z1(){methodBody}
}}

{typeKind} Container2
{{
{fieldType} X2;
int Y2 {{ get; }}
void Z2(){methodBody}
}}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 1),
                this.CSharpDiagnostic().WithLocation(6, 1),
                this.CSharpDiagnostic().WithLocation(12, 1),
                this.CSharpDiagnostic().WithLocation(13, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This test demonstrates the behavior of SA1137 and its code fix with respect to documentation comments.
        /// Currently both operations ignore documentation comments, but in the future the implementation may be updated
        /// to examine and correct them similarly to attribute lists.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDocumentationCommentBehaviorAsync()
        {
            string testCode = @"
using System;
enum Enum1
{
  /// <summary>
  /// Summary.
  /// </summary>
  [My]
    Element1,

  /// <summary>
  /// Summary.
  /// </summary>
  Element2,
}

enum Enum2
{
  /// <summary>
  /// Summary.
  /// </summary>
  [My]
Element1,

  /// <summary>
  /// Summary.
  /// </summary>
  Element2,
}

enum Enum3
{
  /// <summary>
  /// Summary.
  /// </summary>
  [My] Element1,

   /// <summary>
   /// Summary.
   /// </summary>
   Element2,
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";
            string fixedCode = @"
using System;
enum Enum1
{
  /// <summary>
  /// Summary.
  /// </summary>
    [My]
    Element1,

  /// <summary>
  /// Summary.
  /// </summary>
    Element2,
}

enum Enum2
{
  /// <summary>
  /// Summary.
  /// </summary>
[My]
Element1,

  /// <summary>
  /// Summary.
  /// </summary>
Element2,
}

enum Enum3
{
  /// <summary>
  /// Summary.
  /// </summary>
   [My] Element1,

   /// <summary>
   /// Summary.
   /// </summary>
   Element2,
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(8, 1),
                this.CSharpDiagnostic().WithLocation(14, 1),
                this.CSharpDiagnostic().WithLocation(22, 1),
                this.CSharpDiagnostic().WithLocation(28, 1),
                this.CSharpDiagnostic().WithLocation(36, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumDeclarationAsync()
        {
            string testCode = @"
using System;
enum Enum1
{
  [My]
    Element1,

  Element2,
}

enum Enum2
{
  [My]
Element1,

  Element2,
}

enum Enum3
{
  [My] Element1,

   Element2,
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";
            string fixedCode = @"
using System;
enum Enum1
{
    [My]
    Element1,

    Element2,
}

enum Enum2
{
[My]
Element1,

Element2,
}

enum Enum3
{
   [My] Element1,

   Element2,
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 1),
                this.CSharpDiagnostic().WithLocation(8, 1),
                this.CSharpDiagnostic().WithLocation(13, 1),
                this.CSharpDiagnostic().WithLocation(16, 1),
                this.CSharpDiagnostic().WithLocation(21, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationAsync()
        {
            string testCode = @"
class Container
{
    void NonGenericType()
    {
    }

    void TypeWithoutConstraints<T>()
    {
    }

    void TypeWithOneConstraint<T>()
        where T : new()
    {
    }

    void TypeWithMultipleConstraints1<T1, T2, T3>() where T1 : new()
        where T2 : new()
         where T3 : new()
    {
    }

    void TypeWithMultipleConstraints2<T1, T2, T3>()
    where T1 : new()
        where T2 : new()
         where T3 : new()
    {
    }
}
";
            string fixedCode = @"
class Container
{
    void NonGenericType()
    {
    }

    void TypeWithoutConstraints<T>()
    {
    }

    void TypeWithOneConstraint<T>()
        where T : new()
    {
    }

    void TypeWithMultipleConstraints1<T1, T2, T3>() where T1 : new()
        where T2 : new()
        where T3 : new()
    {
    }

    void TypeWithMultipleConstraints2<T1, T2, T3>()
    where T1 : new()
    where T2 : new()
    where T3 : new()
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(19, 1),
                this.CSharpDiagnostic().WithLocation(25, 1),
                this.CSharpDiagnostic().WithLocation(26, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyAccessorListAsync()
        {
            string testCode = @"
using System;

class Container
{
    int Property1
    {
      [My]
        get;

      set;
    }

    int Property2
    {
      [My]
get;

      set;
    }

    int Property3
    {
      [My] get;

       set;
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";
            string fixedCode = @"
using System;

class Container
{
    int Property1
    {
        [My]
        get;

        set;
    }

    int Property2
    {
[My]
get;

set;
    }

    int Property3
    {
       [My] get;

       set;
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(8, 1),
                this.CSharpDiagnostic().WithLocation(11, 1),
                this.CSharpDiagnostic().WithLocation(16, 1),
                this.CSharpDiagnostic().WithLocation(19, 1),
                this.CSharpDiagnostic().WithLocation(24, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerAccessorListAsync()
        {
            string testCode = @"
using System;

interface IContainer1
{
    int this[int arg]
    {
      [My]
        get;

      set;
    }
}

interface IContainer2
{
    int this[int arg]
    {
      [My]
get;

      set;
    }
}

interface IContainer3
{
    int this[int arg]
    {
      [My] get;

       set;
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";
            string fixedCode = @"
using System;

interface IContainer1
{
    int this[int arg]
    {
        [My]
        get;

        set;
    }
}

interface IContainer2
{
    int this[int arg]
    {
[My]
get;

set;
    }
}

interface IContainer3
{
    int this[int arg]
    {
       [My] get;

       set;
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(8, 1),
                this.CSharpDiagnostic().WithLocation(11, 1),
                this.CSharpDiagnostic().WithLocation(19, 1),
                this.CSharpDiagnostic().WithLocation(22, 1),
                this.CSharpDiagnostic().WithLocation(30, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventAccessorListAsync()
        {
            string testCode = @"
using System;

class Container
{
    event EventHandler Event1
    {
      [My]
        add { }

      remove { }
    }

    event EventHandler Event2
    {
      [My]
add { }

      remove { }
    }

    event EventHandler Event3
    {
      [My] add { }

       remove { }
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";
            string fixedCode = @"
using System;

class Container
{
    event EventHandler Event1
    {
        [My]
        add { }

        remove { }
    }

    event EventHandler Event2
    {
[My]
add { }

remove { }
    }

    event EventHandler Event3
    {
       [My] add { }

       remove { }
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(8, 1),
                this.CSharpDiagnostic().WithLocation(11, 1),
                this.CSharpDiagnostic().WithLocation(16, 1),
                this.CSharpDiagnostic().WithLocation(19, 1),
                this.CSharpDiagnostic().WithLocation(24, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestValidVariableDeclarationAsync()
        {
            string testCode = @"
using System;
class Container
{
    private int T1;
    private int
        T2;
    private int
        T3, T4;

    private event EventHandler T5;
    private event EventHandler
        T6;
    private event EventHandler
        T7, T8;

    void MethodName()
    {
        int t1;
        int
            t2;
        int
            t3, t4;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableDeclarationAsync()
        {
            string testCode = @"
using System;
class Container
{
    private int
        X1,
      Y1,
Z1;

    private int
X2,
      Y2,
        Z2;

    private event EventHandler
        X3,
      Y3,
Z3;

    private event EventHandler
X4,
      Y4,
        Z4;

    void MethodName()
    {
        int
            X1,
          Y1,
Z1;

        int
X2,
          Y2,
            Z2;
    }
}
";
            string fixedCode = @"
using System;
class Container
{
    private int
        X1,
        Y1,
        Z1;

    private int
X2,
Y2,
Z2;

    private event EventHandler
        X3,
        Y3,
        Z3;

    private event EventHandler
X4,
Y4,
Z4;

    void MethodName()
    {
        int
            X1,
            Y1,
            Z1;

        int
X2,
Y2,
Z2;
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(7, 1),
                this.CSharpDiagnostic().WithLocation(8, 1),
                this.CSharpDiagnostic().WithLocation(12, 1),
                this.CSharpDiagnostic().WithLocation(13, 1),
                this.CSharpDiagnostic().WithLocation(17, 1),
                this.CSharpDiagnostic().WithLocation(18, 1),
                this.CSharpDiagnostic().WithLocation(22, 1),
                this.CSharpDiagnostic().WithLocation(23, 1),
                this.CSharpDiagnostic().WithLocation(29, 1),
                this.CSharpDiagnostic().WithLocation(30, 1),
                this.CSharpDiagnostic().WithLocation(34, 1),
                this.CSharpDiagnostic().WithLocation(35, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class", " { }")]
        [InlineData("void", "() { }")]
        public async Task TestValidTypeParameterListAsync(string prefix, string suffix)
        {
            string testCode = $@"
class Container
{{
    {prefix} ClassName1<T>{suffix}

    {prefix} ClassName2<
        T>{suffix}

    {prefix} ClassName3<
        T1, T2>{suffix}
}}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class", " { }")]
        [InlineData("void", "() { }")]
        public async Task TestTypeParameterListAsync(string prefix, string suffix)
        {
            string testCode = $@"
class Container
{{
    {prefix} NonZeroAlignment<
        X,
      Y,
Z>{suffix}

    {prefix} ZeroAlignment<
X,
      Y,
        Z>{suffix}
}}
";
            string fixedCode = $@"
class Container
{{
    {prefix} NonZeroAlignment<
        X,
        Y,
        Z>{suffix}

    {prefix} ZeroAlignment<
X,
Y,
Z>{suffix}
}}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 1),
                this.CSharpDiagnostic().WithLocation(7, 1),
                this.CSharpDiagnostic().WithLocation(11, 1),
                this.CSharpDiagnostic().WithLocation(12, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterListAsync()
        {
            string testCode = @"
class Container
{
    void NonZeroAlignment(
        int X,
      int Y,
int Z) { }

    void ZeroAlignment(
int X,
      int Y,
        int Z) { }
}
";
            string fixedCode = @"
class Container
{
    void NonZeroAlignment(
        int X,
        int Y,
        int Z) { }

    void ZeroAlignment(
int X,
int Y,
int Z) { }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 1),
                this.CSharpDiagnostic().WithLocation(7, 1),
                this.CSharpDiagnostic().WithLocation(11, 1),
                this.CSharpDiagnostic().WithLocation(12, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestBracketedParameterListAsync()
        {
            string testCode = @"
class Container1
{
    int this[
        int X,
      int Y,
int Z] => 0;
}

class Container2
{
    int this[
int X,
      int Y,
        int Z] => 0;
}
";
            string fixedCode = @"
class Container1
{
    int this[
        int X,
        int Y,
        int Z] => 0;
}

class Container2
{
    int this[
int X,
int Y,
int Z] => 0;
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 1),
                this.CSharpDiagnostic().WithLocation(7, 1),
                this.CSharpDiagnostic().WithLocation(14, 1),
                this.CSharpDiagnostic().WithLocation(15, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArgumentListAsync()
        {
            string testCode = @"
class Container
{
    int NonZeroAlignment(int x, int y, int z) => NonZeroAlignment(
        0,
      0,
0);

    int ZeroAlignment(int x, int y, int z) => ZeroAlignment(
0,
      0,
        0);
}
";
            string fixedCode = @"
class Container
{
    int NonZeroAlignment(int x, int y, int z) => NonZeroAlignment(
        0,
        0,
        0);

    int ZeroAlignment(int x, int y, int z) => ZeroAlignment(
0,
0,
0);
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 1),
                this.CSharpDiagnostic().WithLocation(7, 1),
                this.CSharpDiagnostic().WithLocation(11, 1),
                this.CSharpDiagnostic().WithLocation(12, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestBracketedArgumentListAsync()
        {
            string testCode = @"
class Container1
{
    int this[int x, int y, int z] => this[
        0,
      0,
0];
}

class Container2
{
    int this[int x, int y, int z] => this[
0,
      0,
        0];
}
";
            string fixedCode = @"
class Container1
{
    int this[int x, int y, int z] => this[
        0,
        0,
        0];
}

class Container2
{
    int this[int x, int y, int z] => this[
0,
0,
0];
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 1),
                this.CSharpDiagnostic().WithLocation(7, 1),
                this.CSharpDiagnostic().WithLocation(14, 1),
                this.CSharpDiagnostic().WithLocation(15, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeListAsync()
        {
            string testCode = @"
using System;

[My]
[
    My]
[
    My, My]
class TypeName1
{
}

[
        My,
      My,
My]
[
My,
      My,
        My]
class TypeName2
{
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";
            string fixedCode = @"
using System;

[My]
[
    My]
[
    My, My]
class TypeName1
{
}

[
        My,
        My,
        My]
[
My,
My,
My]
class TypeName2
{
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(15, 1),
                this.CSharpDiagnostic().WithLocation(16, 1),
                this.CSharpDiagnostic().WithLocation(19, 1),
                this.CSharpDiagnostic().WithLocation(20, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeArgumentListAsync()
        {
            string testCode = @"
using System;

[My(0)]
[My(
    0)]
[My(
    0, Y = 2)]
class TypeName1
{
}

[My(
        0,
      Y = 2,
Z = 3)]
[My(
0,
      Y = 2,
        Z = 3)]
class TypeName2
{
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute
{
    public MyAttribute() { }
    public MyAttribute(int value) { }

    public int Y { get; set; }
    public int Z { get; set; }
}
";
            string fixedCode = @"
using System;

[My(0)]
[My(
    0)]
[My(
    0, Y = 2)]
class TypeName1
{
}

[My(
        0,
        Y = 2,
        Z = 3)]
[My(
0,
Y = 2,
Z = 3)]
class TypeName2
{
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute
{
    public MyAttribute() { }
    public MyAttribute(int value) { }

    public int Y { get; set; }
    public int Z { get; set; }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(15, 1),
                this.CSharpDiagnostic().WithLocation(16, 1),
                this.CSharpDiagnostic().WithLocation(19, 1),
                this.CSharpDiagnostic().WithLocation(20, 1),
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
        public async Task TestLeadingCommentAsync()
        {
            string testCode = @"
using System.Collections.Generic;
class ClassName
{
    void MethodName()
    {
        /* var x = */ new List<string>();
        var y = new List<string>();
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

        [Fact]
        public async Task TestValidAnonymousObjectCreationExpressionAsync()
        {
            string testCode = @"
class ClassName
{
    void SingleLineInitializersMethod()
    {
        var obj = new { X = 0 };
    }

    void SingleElementInitializersMethod()
    {
        var obj =
            new
            {
                X = 0,
            };
    }

    void SharedLineInitializersMethod()
    {
        var obj =
            new
            {
                X = 0, Y = 0,
            };
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousObjectCreationExpressionAsync()
        {
            string testCode = @"
class ClassName
{
    void NonZeroAlignmentMethod()
    {
        var obj =
            new
            {
                X = 0,
              Y = 0,
Z = 0,
            };
    }

    void ZeroAlignmentMethod()
    {
        var obj =
            new
            {
X = 0,
              Y = 0,
                Z = 0,
            };
    }
}
";
            string fixedCode = @"
class ClassName
{
    void NonZeroAlignmentMethod()
    {
        var obj =
            new
            {
                X = 0,
                Y = 0,
                Z = 0,
            };
    }

    void ZeroAlignmentMethod()
    {
        var obj =
            new
            {
X = 0,
Y = 0,
Z = 0,
            };
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(10, 1),
                this.CSharpDiagnostic().WithLocation(11, 1),
                this.CSharpDiagnostic().WithLocation(21, 1),
                this.CSharpDiagnostic().WithLocation(22, 1),
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

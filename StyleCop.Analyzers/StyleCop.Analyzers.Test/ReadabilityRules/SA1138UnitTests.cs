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
    /// <remarks>
    /// <para>This set of tests is run with both SA1137 and SA1138 enabled, since they are intended to be used
    /// together.</para>
    /// </remarks>
    public class SA1138UnitTests : CodeFixVerifier
    {
        protected string BlockStatementTestCode => @"
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

        protected virtual string BlockStatementFixedCode => @"
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

        protected virtual DiagnosticResult[] BlockStatementExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(17, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(18, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(19, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(20, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(21, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(24, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(25, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(26, 1),
            };

        protected string SwitchStatementTestCode => @"
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

        protected virtual string SwitchStatementFixedCode => @"
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

        protected virtual DiagnosticResult[] SwitchStatementExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(9, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(10, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(17, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(18, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(19, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(20, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(21, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(23, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(25, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(26, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(27, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(28, 1),
            };

        protected string InitializerExpressionTestCode => @"
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

        protected virtual string InitializerExpressionFixedCode => @"
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

        protected virtual DiagnosticResult[] InitializerExpressionExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(20, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(21, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(29, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(30, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(38, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(39, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(49, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(50, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(58, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(59, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(67, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(68, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(76, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(77, 1),
            };

        protected string AnonymousObjectCreationExpressionTestCode => @"
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

        protected virtual string AnonymousObjectCreationExpressionFixedCode => @"
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

        protected virtual DiagnosticResult[] AnonymousObjectCreationExpressionExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(10, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(21, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(22, 1),
            };

        protected string DestructorDeclarationTestCode => @"
class ClassName
{
~ClassName()
{
return;
}
}
";

        protected virtual string DestructorDeclarationFixedCode => @"
class ClassName
{
    ~ClassName()
    {
        return;
    }
}
";

        protected virtual DiagnosticResult[] DestructorDeclarationExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(4, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(5, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
            };

        protected string OperatorDeclarationTestCode => @"
class ClassName
{
public static bool operator !(ClassName obj)
{
return false;
}
}
";

        protected virtual string OperatorDeclarationFixedCode => @"
class ClassName
{
    public static bool operator !(ClassName obj)
    {
        return false;
    }
}
";

        protected virtual DiagnosticResult[] OperatorDeclarationExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(4, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(5, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
            };

        protected string ConversionOperatorDeclarationTestCode => @"
class ClassName
{
public static explicit operator bool(ClassName obj)
{
return false;
}
}
";

        protected virtual string ConversionOperatorDeclarationFixedCode => @"
class ClassName
{
    public static explicit operator bool(ClassName obj)
    {
        return false;
    }
}
";

        protected virtual DiagnosticResult[] ConversionOperatorExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(4, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(5, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
            };

        protected string CheckedStatementTestCode => @"
class ClassName
{
    void MethodName()
    {
checked
{
int y = 3 + 2;
}

unchecked
{
int y = 3 + 2;
}
    }
}
";

        protected virtual string CheckedStatementFixedCode => @"
class ClassName
{
    void MethodName()
    {
        checked
        {
            int y = 3 + 2;
        }

        unchecked
        {
            int y = 3 + 2;
        }
    }
}
";

        protected virtual DiagnosticResult[] CheckedStatementExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(9, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(13, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(14, 1),
            };

        protected string DoStatementTestCode => @"
class ClassName
{
    void MethodName()
    {
do
{
int y = 3 + 2;
}
while (true);

do
return;
while (true);

do
do
return;
while (true);
while (true);
    }
}
";

        protected virtual string DoStatementFixedCode => @"
class ClassName
{
    void MethodName()
    {
        do
        {
            int y = 3 + 2;
        }
        while (true);

        do
            return;
while (true);

        do
            do
                return;
while (true);
while (true);
    }
}
";

        protected virtual DiagnosticResult[] DoStatementExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(9, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(10, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(13, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(17, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(18, 1),
            };

        protected string FixedStatementTestCode => @"
class ClassName
{
    unsafe void MethodName()
    {
        int[] x = new int[1];
fixed (int* p = &x[0])
{
int y = 3 + 2;
}

fixed (int* p = &x[0])
return;

fixed (int* p = &x[0])
fixed (int* q = &x[0])
return;
    }
}
";

        protected virtual string FixedStatementFixedCode => @"
class ClassName
{
    unsafe void MethodName()
    {
        int[] x = new int[1];
        fixed (int* p = &x[0])
        {
            int y = 3 + 2;
        }

        fixed (int* p = &x[0])
            return;

        fixed (int* p = &x[0])
            fixed (int* q = &x[0])
                return;
    }
}
";

        protected virtual DiagnosticResult[] FixedStatementExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(9, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(10, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(13, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(15, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(17, 1),
            };

        protected string ForEachStatementTestCode => @"
class ClassName
{
    void MethodName()
    {
foreach (int x in new int[3])
{
int y = 3 + 2;
}

foreach (int x in new int[3])
return;

foreach (int x in new int[3])
foreach (int y in new int[3])
return;
    }
}
";

        protected virtual string ForEachStatementFixedCode => @"
class ClassName
{
    void MethodName()
    {
        foreach (int x in new int[3])
        {
            int y = 3 + 2;
        }

        foreach (int x in new int[3])
            return;

        foreach (int x in new int[3])
            foreach (int y in new int[3])
                return;
    }
}
";

        protected virtual DiagnosticResult[] ForEachStatementExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(9, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(14, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(15, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
            };

        protected string ForStatementTestCode => @"
class ClassName
{
    void MethodName()
    {
for (int i = 0; i < 3; i++)
{
int y = 3 + 2;
}

for (int i = 0; i < 3; i++)
return;

for (int i = 0; i < 3; i++)
for (int j = 0; j < 3; j++)
return;
    }
}
";

        protected virtual string ForStatementFixedCode => @"
class ClassName
{
    void MethodName()
    {
        for (int i = 0; i < 3; i++)
        {
            int y = 3 + 2;
        }

        for (int i = 0; i < 3; i++)
            return;

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                return;
    }
}
";

        protected virtual DiagnosticResult[] ForStatementExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(9, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(14, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(15, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
            };

        protected string IfStatementTestCode => @"
class ClassName
{
    void MethodName()
    {
if (1 == 2)
{
int y = 3 + 2;
}
else if (2 == 3)
{
int y = 3 + 2;
}
else
{
int y = 3 + 2;
}

if (1 == 2)
return;
else if (2 == 3)
{
int y = 3 + 2;
}
else
{
int y = 3 + 2;
}

if (1 == 2)
{
int y = 3 + 2;
}
else if (2 == 3)
return;
else
{
int y = 3 + 2;
}

if (1 == 2)
{
int y = 3 + 2;
}
else if (2 == 3)
{
int y = 3 + 2;
}
else
return;

if (1 == 2)
{
int y = 3 + 2;
}
else
{
if (2 == 3)
{
int y = 3 + 2;
}
else
{
return;
}
}
    }
}
";

        protected virtual string IfStatementFixedCode => @"
class ClassName
{
    void MethodName()
    {
        if (1 == 2)
        {
            int y = 3 + 2;
        }
        else if (2 == 3)
        {
                int y = 3 + 2;
        }
        else
        {
                int y = 3 + 2;
        }

        if (1 == 2)
            return;
        else if (2 == 3)
        {
                int y = 3 + 2;
        }
        else
        {
                int y = 3 + 2;
        }

        if (1 == 2)
        {
            int y = 3 + 2;
        }
        else if (2 == 3)
            return;
        else
        {
                int y = 3 + 2;
        }

        if (1 == 2)
        {
            int y = 3 + 2;
        }
        else if (2 == 3)
        {
                int y = 3 + 2;
        }
        else
            return;

        if (1 == 2)
        {
            int y = 3 + 2;
        }
        else
        {
            if (2 == 3)
            {
                int y = 3 + 2;
            }
            else
            {
                return;
            }
        }
    }
}
";

        protected virtual DiagnosticResult[] IfStatementExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(9, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(10, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(13, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(14, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(15, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(17, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(19, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(20, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(21, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(22, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(23, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(24, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(25, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(26, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(27, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(28, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(30, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(31, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(32, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(33, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(34, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(35, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(36, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(37, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(38, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(39, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(41, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(42, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(43, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(44, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(45, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(46, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(47, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(48, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(49, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(50, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(52, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(53, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(54, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(55, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(56, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(57, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(58, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(59, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(60, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(61, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(62, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(63, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(64, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(65, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(66, 1),
            };

        protected string LockStatementTestCode => @"
class ClassName
{
    void MethodName()
    {
lock (new object())
{
int y = 3 + 2;
}

lock (new object())
return;

lock (new object())
lock (new object())
return;
    }
}
";

        protected virtual string LockStatementFixedCode => @"
class ClassName
{
    void MethodName()
    {
        lock (new object())
        {
            int y = 3 + 2;
        }

        lock (new object())
            return;

        lock (new object())
            lock (new object())
                return;
    }
}
";

        protected virtual DiagnosticResult[] LockStatementExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(9, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(14, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(15, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
            };

        protected string UsingStatementTestCode => @"
using System;
class ClassName
{
    void MethodName()
    {
using (default(IDisposable))
{
int y = 3 + 2;
}

using (default(IDisposable))
using (default(IDisposable))
{
int y = 3 + 2;
}

using (default(IDisposable))
{
using (default(IDisposable))
{
int y = 3 + 2;
}
}

using (default(IDisposable))
return;

using (default(IDisposable))
using (default(IDisposable))
return;
    }
}
";

        protected virtual string UsingStatementFixedCode => @"
using System;
class ClassName
{
    void MethodName()
    {
        using (default(IDisposable))
        {
            int y = 3 + 2;
        }

        using (default(IDisposable))
    using (default(IDisposable))
    {
            int y = 3 + 2;
    }

        using (default(IDisposable))
        {
            using (default(IDisposable))
            {
                int y = 3 + 2;
            }
        }

        using (default(IDisposable))
            return;

        using (default(IDisposable))
    using (default(IDisposable))
        return;
    }
}
";

        protected virtual DiagnosticResult[] UsingStatementExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(9, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(10, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(13, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(14, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(15, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(18, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(19, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(20, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(21, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(22, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(23, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(24, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(26, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(27, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(29, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(30, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(31, 1),
            };

        protected string WhileStatementTestCode => @"
class ClassName
{
    void MethodName()
    {
while (true)
{
int y = 3 + 2;
}

while (true)
return;

while (true)
while (true)
return;
    }
}
";

        protected virtual string WhileStatementFixedCode => @"
class ClassName
{
    void MethodName()
    {
        while (true)
        {
            int y = 3 + 2;
        }

        while (true)
            return;

        while (true)
            while (true)
                return;
    }
}
";

        protected virtual DiagnosticResult[] WhileStatementExpectedDiagnostics =>
            new[]
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(9, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(14, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(15, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
            };

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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(17, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(19, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(26, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(33, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(41, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(48, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(17, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(22, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(24, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(4, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(5, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(13, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(14, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(22, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(23, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(28, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(36, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(41, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(5, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(13, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(14, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(21, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(23, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(19, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(24, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(26, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(17, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(19, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(24, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(26, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(19, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(20, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(22, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(30, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(32, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(17, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(19, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(24, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(26, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(13, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(17, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(18, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(22, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(23, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(29, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(30, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(34, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(35, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(10, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(11, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(10, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(11, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(13, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(14, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(12, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(14, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(15, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAssemblyAttributeListAsync()
        {
            string testCode = @"
using System;

[assembly: My]
[assembly:
    My]
[assembly:
    My, My]

[assembly:
        My,
      My,
My]
[assembly:
My,
      My,
        My]

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";
            string fixedCode = @"
using System;

[assembly: My]
[assembly:
    My]
[assembly:
    My, My]

[assembly:
    My,
    My,
    My]
[assembly:
    My,
    My,
    My]

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute { }
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(11, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(12, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(13, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(15, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(17, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(14, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(15, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(16, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(18, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(19, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(20, 1),
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
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(15, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(16, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(19, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId).WithLocation(20, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestBlockStatementAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(this.BlockStatementTestCode, this.BlockStatementExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.BlockStatementFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.BlockStatementTestCode, this.BlockStatementFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSwitchStatementAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(this.SwitchStatementTestCode, this.SwitchStatementExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.SwitchStatementFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.SwitchStatementTestCode, this.SwitchStatementFixedCode, numberOfFixAllIterations: 2, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(this.InitializerExpressionTestCode, this.InitializerExpressionExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.InitializerExpressionFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.InitializerExpressionTestCode, this.InitializerExpressionFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(this.AnonymousObjectCreationExpressionTestCode, this.AnonymousObjectCreationExpressionExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.AnonymousObjectCreationExpressionFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.AnonymousObjectCreationExpressionTestCode, this.AnonymousObjectCreationExpressionFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorDeclarationAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(this.DestructorDeclarationTestCode, this.DestructorDeclarationExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.DestructorDeclarationFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.DestructorDeclarationTestCode, this.DestructorDeclarationFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorDeclarationAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(this.OperatorDeclarationTestCode, this.OperatorDeclarationExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.OperatorDeclarationFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.OperatorDeclarationTestCode, this.OperatorDeclarationFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConversionOperatorDeclarationAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(this.ConversionOperatorDeclarationTestCode, this.ConversionOperatorExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.ConversionOperatorDeclarationFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.ConversionOperatorDeclarationTestCode, this.ConversionOperatorDeclarationFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCheckedStatementAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(this.CheckedStatementTestCode, this.CheckedStatementExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.CheckedStatementFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.CheckedStatementTestCode, this.CheckedStatementFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDoStatementAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(this.DoStatementTestCode, this.DoStatementExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.DoStatementFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.DoStatementTestCode, this.DoStatementFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFixedStatementAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(this.FixedStatementTestCode, this.FixedStatementExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.FixedStatementFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.FixedStatementTestCode, this.FixedStatementFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestForEachStatementAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(this.ForEachStatementTestCode, this.ForEachStatementExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.ForEachStatementFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.ForEachStatementTestCode, this.ForEachStatementFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestForStatementAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(this.ForStatementTestCode, this.ForStatementExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.ForStatementFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.ForStatementTestCode, this.ForStatementFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIfStatementAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(this.IfStatementTestCode, this.IfStatementExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.IfStatementFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.IfStatementTestCode, this.IfStatementFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLockStatementAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(this.LockStatementTestCode, this.LockStatementExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.LockStatementFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.LockStatementTestCode, this.LockStatementFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUsingStatementAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(this.UsingStatementTestCode, this.UsingStatementExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.UsingStatementFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.UsingStatementTestCode, this.UsingStatementFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhileStatementAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(this.WhileStatementTestCode, this.WhileStatementExpectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(this.WhileStatementFixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(this.WhileStatementTestCode, this.WhileStatementFixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedBlockStatementAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
{
{
int y = 3 + 2;
}
}
    }
}
";
            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
        {
            {
                int y = 3 + 2;
            }
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(8, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(9, 1),
                this.CSharpDiagnostic(SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId).WithLocation(10, 1),
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

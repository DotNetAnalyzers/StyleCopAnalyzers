// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1143UseImplicitlyAssignableForeachVariableType>;

    /// <summary>
    /// This class contains the unit tests for SA1141.
    /// </summary>
    /// <seealso cref="SA1143UseImplicitlyAssignableForeachVariableType"/>
    public class SA1143UnitTests
    {
        [Fact]
        public async Task NonGenericIComparableCollectionAsync()
        {
            var test = @"
namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            {|#0:foreach|} (string item in new A())
            {
            }
        }
    }

    struct A
    {
        public Enumerator GetEnumerator() =>  new Enumerator();

        public struct Enumerator
        {
            public System.IComparable Current => 42;

            public bool MoveNext() => true;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, GetResultAt(0).WithArguments("IComparable", "String"), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task GenericObjectCollectionAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<object>();
            {|#0:foreach|} (string item in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, GetResultAt(0).WithArguments("Object", "String"), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task NongenericObjectCollectionAsync()
        {
            var test = @"
using System.Collections;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new ArrayList();
            foreach (string item in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task SameTypeAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<string>();
            foreach (string item in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task CastBaseToChildAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<A>();
            {|#0:foreach|} (B item in x)
            {
            }
        }
    }

    class A { }
    class B : A { }
}";

            await VerifyCSharpDiagnosticAsync(test, GetResultAt(0).WithArguments("A", "B"), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task ImplicitConversionAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<int>();
            foreach (long item in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task UserDefinedImplicitConversionAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<A>();
            foreach (B item in x)
            {
            }
        }
    }

    class A { }
    class B 
    { 
        public static implicit operator B(A a) => new B();
    }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task ExplicitConversionAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<long>();
            {|#0:foreach|} (int item in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, GetResultAt(0).WithArguments("Int64", "Int32"), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task UserDefinedExplicitConversionAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<A>();
            {|#0:foreach|} (B item in x)
            {
            }
        }
    }

    class A { }
    class B 
    { 
        public static explicit operator B(A a) => new B();
    }
}";

            await VerifyCSharpDiagnosticAsync(test, GetResultAt(0).WithArguments("A", "B"), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task CastChildToBaseAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<B>();
            foreach (A item in x)
            {
            }
        }
    }

    class A { }
    class B : A { }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task InterfaceToClassAsync()
        {
            var test = @"
using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<IComparable>();
            {|#0:foreach|} (string s in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, GetResultAt(0).WithArguments("IComparable", "String"), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task ClassToInterfaseAsync()
        {
            var test = @"
using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<string>();
            foreach (IComparable s in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task GenericTypes_UnrelatedAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main<A, B>()
        {
            var x = new List<A>();
            {|#0:foreach|} (B s in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.CompilerError("CS0030").WithLocation(0).WithArguments("A", "B"), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task GenericTypes_Valid_RelationshipAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main<A, B>() where A : B
        {
            var x = new List<A>();
            foreach (B s in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task GenericTypes_Invalid_RelationshipAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main<A, B>() where B : A
        {
            var x = new List<A>();
            {|#0:foreach|} (B s in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, GetResultAt(0).WithArguments("A", "B"), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task CollectionFromMethodResult_InvalidAsync()
        {
            var test = @"
using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            {|#0:foreach|} (string item in GenerateSequence())
            {
            }

            IEnumerable<IComparable> GenerateSequence()
            {
                throw new NotImplementedException();
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, GetResultAt(0).WithArguments("IComparable", "String"), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task CollectionFromMethodResult_ValidAsync()
        {
            var test = @"
using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            foreach (IComparable item in GenerateSequence())
            {
            }

            IEnumerable<IComparable> GenerateSequence()
            {
                throw new NotImplementedException();
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task DynamicSameTypeAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<dynamic>();
            foreach (dynamic s in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task DynamicToObjectAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<dynamic>();
            foreach (object s in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task DynamicToStringAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<dynamic>();
            {|#0:foreach|} (string s in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, GetResultAt(0).WithArguments("dynamic", "String"), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task DynamicToVarAsync()
        {
            var test = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<dynamic>();
            foreach (var s in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TupleToVarTupleAsync()
        {
            var test = @"
using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<(int, IComparable)>();
            foreach (var (i, j) in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TupleToSameTupleAsync()
        {
            var test = @"
using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<(int, IComparable)>();
            foreach ((int i,  IComparable j) in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TupleToChildTupleAsync()
        {
            var test = @"
using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {   
        void Main()
        {
            var x = new List<(int, IComparable)>();
            foreach ((int i,  {|#0:int j|}) in x)
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(test, DiagnosticResult.CompilerError("CS0266").WithLocation(0).WithArguments("System.IComparable", "int"), CancellationToken.None).ConfigureAwait(false);
        }

        private static DiagnosticResult GetResultAt(int markupKey) =>
            Diagnostic().WithLocation(markupKey);
    }
}

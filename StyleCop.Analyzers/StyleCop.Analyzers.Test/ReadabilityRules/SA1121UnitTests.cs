namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1121UseBuiltInTypeAlias"/>
    /// </summary>
    public class SA1121UnitTests : CodeFixVerifier
    {
        private static readonly Tuple<string, string>[] ReferenceTypes = new Tuple<string, string>[]
        {
            new Tuple<string, string>("object", nameof(Object)),
            new Tuple<string, string>("string", nameof(String))
        };

        private static readonly Tuple<string, string>[] ValueTypes = new Tuple<string, string>[]
        {
            new Tuple<string, string>("bool", nameof(Boolean)),
            new Tuple<string, string>("byte", nameof(Byte)),
            new Tuple<string, string>("char", nameof(Char)),
            new Tuple<string, string>("decimal", nameof(Decimal)),
            new Tuple<string, string>("double", nameof(Double)),
            new Tuple<string, string>("short", nameof(Int16)),
            new Tuple<string, string>("int", nameof(Int32)),
            new Tuple<string, string>("long", nameof(Int64)),
            new Tuple<string, string>("sbyte", nameof(SByte)),
            new Tuple<string, string>("float", nameof(Single)),
            new Tuple<string, string>("ushort", nameof(UInt16)),
            new Tuple<string, string>("uint", nameof(UInt32)),
            new Tuple<string, string>("ulong", nameof(UInt64))
        };

        private static readonly Tuple<string, string>[] EnumBaseTypes = new Tuple<string, string>[]
        {
            new Tuple<string, string>("byte", nameof(Byte)),
            new Tuple<string, string>("short", nameof(Int16)),
            new Tuple<string, string>("int", nameof(Int32)),
            new Tuple<string, string>("long", nameof(Int64)),
            new Tuple<string, string>("sbyte", nameof(SByte)),
            new Tuple<string, string>("ushort", nameof(UInt16)),
            new Tuple<string, string>("uint", nameof(UInt32)),
            new Tuple<string, string>("ulong", nameof(UInt64))
        };

        private static readonly Tuple<string, string>[] AllTypes = ReferenceTypes.Concat(ValueTypes).ToArray();

        public string DiagnosticId { get; } = SA1121UseBuiltInTypeAlias.DiagnosticId;

        private async Task TestCases(Func<string, string, Task> func, Tuple<string, string>[] types)
        {
            foreach (var item in types)
            {
                await func(item.Item1, item.Item2);
                await func(item.Item1, "System." + item.Item2);
                await func(item.Item1, "global::System." + item.Item2);
            }
        }

        private async Task VerifyFixes(string testSource, Tuple<string, string>[] types)
        {
            foreach (var item in types)
            {
                await this.VerifyCSharpFixAsync(string.Format(testSource, item.Item2), string.Format(testSource, item.Item1));
                await this.VerifyCSharpFixAsync(string.Format(testSource, "System." + item.Item2), string.Format(testSource, item.Item1));
                await this.VerifyCSharpFixAsync(string.Format(testSource, "global::System." + item.Item2), string.Format(testSource, item.Item1));
            }
        }

        private async Task TestAllCases(Func<string, string, Task> func)
        {
            await this.TestCases(func, AllTypes);
        }

        private async Task VerifyAllFixes(string testSource)
        {
            await this.VerifyFixes(testSource, AllTypes);
        }

        private async Task TestEnumTypeCases(Func<string, string, Task> func)
        {
            await this.TestCases(func, EnumBaseTypes);
        }

        private async Task VerifyEnumTypeFixes(string testSource)
        {
            await this.VerifyFixes(testSource, AllTypes);
        }

        private async Task TestValueTypeCases(Func<string, string, Task> func)
        {
            await this.TestCases(func, ValueTypes);
        }

        private async Task VerifyValueTypeFixes(string testSource)
        {
            await this.VerifyFixes(testSource, AllTypes);
        }

        private async Task TestReferenceTypeCases(Func<string, string, Task> func)
        {
            await this.TestCases(func, ReferenceTypes);
        }

        private async Task VerifyReferenceTypeFixes(string testSource)
        {
            await this.VerifyFixes(testSource, ReferenceTypes);
        }

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestVariableDeclarationImpl(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar()
    {{
        {0} test;
    }}
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, predefined), EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestVariableDeclarationCodeFix()
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar()
    {{
        {0} test;
    }}
}}
}}";

            await this.VerifyAllFixes(testSource);
        }

        [Fact]
        public async Task TestVariableDeclaration()
        {
            await this.TestAllCases(this.TestVariableDeclarationImpl);
        }

        private async Task TestDefaultDeclarationImpl(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar()
    {{
        var test = default({0});
    }}
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 28);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestDefaultDeclaration()
        {
            await this.TestAllCases(this.TestDefaultDeclarationImpl);
        }

        [Fact]
        public async Task TestDefaultDeclarationCodeFix()
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar()
    {{
        var test = default({0});
    }}
}}
}}";

            await this.VerifyAllFixes(testSource);
        }

        private async Task TestTypeOfImpl(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar()
    {{
        var test = typeof({0});
    }}
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 27);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestTypeOf()
        {
            await this.TestAllCases(this.TestTypeOfImpl);
        }

        [Fact]
        public async Task TestTypeOfCodeFix()
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar()
    {{
        var test = typeof({0});
    }}
}}
}}";

            await this.VerifyAllFixes(testSource);
        }

        private async Task TestReturnTypeImpl(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public {0} Bar()
    {{
        return default({0});
    }}
}}";
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 12),
                this.CSharpDiagnostic().WithLocation(6, 24),
            };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestReturnType()
        {
            await this.TestAllCases(this.TestReturnTypeImpl);
        }

        [Fact]
        public async Task TestReturnTypeCodeFix()
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public {0} Bar()
    {{
        return default({0});
    }}
}}
}}";

            await this.VerifyAllFixes(testSource);
        }

        private async Task TestEnumBaseTypeImpl(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public enum Bar : {0}
    {{
    }}
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 23);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestEnumBaseType()
        {
            await this.TestEnumTypeCases(this.TestEnumBaseTypeImpl);
        }

        [Fact]
        public async Task TestEnumBaseTypeCodeFix()
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public enum Bar : {0}
    {{
    }}
}}
}}";

            await this.VerifyEnumTypeFixes(testSource);
        }

        private async Task TestPointerDeclarationImpl(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public unsafe void Bar()
    {{
        {0}* test;
    }}
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestPointerDeclaration()
        {
            await this.TestValueTypeCases(this.TestPointerDeclarationImpl);
        }

        [Fact]
        public async Task TestPointerDeclarationCodeFix()
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public unsafe void Bar()
    {{
        {0}* test;
    }}
}}
}}";

            await this.VerifyValueTypeFixes(testSource);
        }

        private async Task TestArgumentImpl(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar({0} test)
    {{
    }}
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 21);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestArgument()
        {
            await this.TestAllCases(this.TestArgumentImpl);
        }

        [Fact]
        public async Task TestArgumentCodeFix()
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar({0} test)
    {{
    }}
}}
}}";

            await this.VerifyAllFixes(testSource);
        }

        private async Task TestIndexerImpl(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public {0} this
            [{0} test]
    {{
        get {{ return default({0}); }}
    }}
}}";
            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(4, 12),
                    this.CSharpDiagnostic().WithLocation(5, 14),
                    this.CSharpDiagnostic().WithLocation(7, 30),
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexer()
        {
            await this.TestAllCases(this.TestIndexerImpl);
        }

        [Fact]
        public async Task TestIndexerCodeFix()
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public {0} this
            [{0} test]
    {{
        get {{ return default({0}); }}
    }}
}}
}}";

            await this.VerifyAllFixes(testSource);
        }

        private async Task TestGenericAndLambdaImpl(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar()
    {{
        Func<{0}, 
                {0}> f = 
                    ({0} param) => param;
    }}
}}";
            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(6, 14),
                    this.CSharpDiagnostic().WithLocation(7, 17),
                    this.CSharpDiagnostic().WithLocation(8, 22),
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestGenericAndLambda()
        {
            await this.TestAllCases(this.TestGenericAndLambdaImpl);
        }

        [Fact]
        public async Task TestGenericAndLambdaCodeFix()
        {
            string testSource = @"using System;
public class Foo
{{
    public void Bar()
    {{
        Func<{0}, 
                {0}> f = 
                    ({0} param) => param;
    }}
}}";

            await this.VerifyAllFixes(testSource);
        }

        private async Task TestArrayImpl(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar()
    {{
        var array = new {0}[0];
    }}
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 25);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestArray()
        {
            await this.TestAllCases(this.TestArrayImpl);
        }

        [Fact]
        public async Task TestArrayCodeFix()
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar()
    {{
        var array = new {0}[0];
    }}
}}
}}";

            await this.VerifyAllFixes(testSource);
        }

        private async Task TestStackAllocArrayImpl(string predefined, string fullName)
        {
            if (predefined == "object" || predefined == "string")
                return;

            string testCode = @"using System;
public class Foo
{{
    public unsafe void Bar()
    {{
        var array = stackalloc {0}[0];
    }}
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 32);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestStackAllocArray()
        {
            await this.TestAllCases(this.TestStackAllocArrayImpl);
        }

        [Fact]
        public async Task TestStackAllocArrayCodeFix()
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public unsafe void Bar()
    {{
        var array = stackalloc {0}[0];
    }}
}}
}}";

            await this.VerifyValueTypeFixes(testSource);
        }

        private async Task TestImplicitCastImpl(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar()
    {{
        var t = ({0})
                    default({0});
    }}
}}";
            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(6, 18),
                    this.CSharpDiagnostic().WithLocation(7, 29),
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestImplicitCast()
        {
            await this.TestAllCases(this.TestImplicitCastImpl);
        }

        [Fact]
        public async Task TestImplicitCastCodeFix()
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar()
    {{
        var t = ({0})
                    default({0});
    }}
}}
}}";

            await this.VerifyValueTypeFixes(testSource);
        }

        private async Task TestExplicitCastImpl(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar()
    {{
        var t = null as {0};
    }}
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 25);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestExplicitCast()
        {
            await this.TestReferenceTypeCases(this.TestExplicitCastImpl);
        }

        [Fact]
        public async Task TestExplicitCastCodeFix()
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar()
    {{
        var t = ({0})default({0});
    }}
}}
}}";

            await this.VerifyValueTypeFixes(testSource);
        }

        private async Task TestNullableImpl(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar()
    {{
        {0}? t = null;
    }}
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestNullable()
        {
            await this.TestValueTypeCases(this.TestNullableImpl);
        }

        [Fact]
        public async Task TestNullableCodeFix()
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar()
    {{
        {0}? t = null;
    }}
}}
}}";

            await this.VerifyValueTypeFixes(testSource);
        }

        [Fact]
        public async Task TestMissleadingUsing()
        {
            string testCode = @"namespace Foo
{
  using Int32 = System.UInt32;
  class Bar
  {
    Int32 value = 3;
  }
}
";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMissleadingUsingCodeFix()
        {
            string oldSource = @"namespace Foo
{
  using Int32 = System.UInt32;
  class Bar
  {
    Int32 value = 3;
  }
}
";
            string newSource = @"namespace Foo
{
  using Int32 = System.UInt32;
  class Bar
  {
    uint value = 3;
  }
}
";

            await this.VerifyCSharpFixAsync(oldSource, newSource, allowNewCompilerDiagnostics: true);
        }

        [Fact]
        public async Task TestUsingNameChange()
        {
            string testCode = @"namespace Foo
{
  using MyInt = System.UInt32;
  class Bar
  {
    MyInt value = 3;
  }
}
";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestUsingNameChangeCodeFix()
        {
            string oldSource = @"namespace Foo
{
  using MyInt = System.UInt32;
  class Bar
  {
    MyInt value = 3;
  }
}
";
            string newSource = @"namespace Foo
{
  using MyInt = System.UInt32;
  class Bar
  {
    uint value = 3;
  }
}
";

            await this.VerifyCSharpFixAsync(oldSource, newSource, allowNewCompilerDiagnostics: true);
        }

        [Fact]
        public async Task TestWrongType()
        {
            string testCode = @"
public class Foo
{{
    public unsafe void Bar()
    {{
        {0} t = null;
    }}
}}
public class {0} {{}}
";
            foreach (var item in AllTypes)
            {
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "@" + item.Item1), EmptyDiagnosticResults, CancellationToken.None);
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, item.Item2), EmptyDiagnosticResults, CancellationToken.None);
            }
        }

        [Fact]
        public async Task TestUsing()
        {
            string testCode = @"
namespace Foo
{{
    using {0};
    public class Foo
    {{
    }}
}}
namespace {0} 
{{
        public class Bar {{ }}
}}
";
            foreach (var item in AllTypes)
            {
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "@" + item.Item1), EmptyDiagnosticResults, CancellationToken.None);
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, item.Item2), EmptyDiagnosticResults, CancellationToken.None);
            }
        }

        [Fact]
        public async Task TestNameOf()
        {
            string testCode = @"
namespace Foo
{{
    public class Foo
    {{
        public void Bar()
        {{
            string test = nameof({0});
        }}
    }}
}}
";
            foreach (var item in AllTypes)
            {
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "System." + item.Item2), EmptyDiagnosticResults, CancellationToken.None);
            }

            var expected = this.CSharpDiagnostic().WithLocation(8, 41);

            foreach (var item in AllTypes)
            {
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "System." + item.Item2 + ".ToString"), expected, CancellationToken.None);
                await this.VerifyCSharpFixAsync(string.Format(testCode, "System." + item.Item2 + ".ToString"), string.Format(testCode, item.Item1 + ".ToString"));
            }
        }

        [Fact]
        public async Task TestNameOfInnerMethod()
        {
            string testCode = @"
namespace Foo
{{
    public class Foo
    {{
        public void Bar()
        {{
            string test = nameof({0}.ToString);
        }}
    }}
}}
";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 41);
            foreach (var item in AllTypes)
            {
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "System." + item.Item2), expected, CancellationToken.None);
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, item.Item1), EmptyDiagnosticResults, CancellationToken.None);
            }
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1121UseBuiltInTypeAlias();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1121CodeFixProvider();
        }
    }
}

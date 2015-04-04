namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System;
    using System.Collections.Generic;
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
        private static readonly Tuple<string, string>[] ReferenceTypesData = new Tuple<string, string>[]
        {
            new Tuple<string, string>("object", nameof(Object)),
            new Tuple<string, string>("string", nameof(String))
        };

        private static readonly Tuple<string, string>[] ValueTypesData = new Tuple<string, string>[]
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

        private static readonly Tuple<string, string>[] EnumBaseTypesData = new Tuple<string, string>[]
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

        private static readonly Tuple<string, string>[] AllTypesData = ReferenceTypesData.Concat(ValueTypesData).ToArray();

        public static IEnumerable<object[]> ReferenceTypes
        {
            get
            {
                foreach (var pair in ReferenceTypesData)
                {
                    yield return new[] { pair.Item1, pair.Item2 };
                    yield return new[] { pair.Item1, "System." + pair.Item2 };
                    yield return new[] { pair.Item1, "global::System." + pair.Item2 };
                }
            }
        }

        public static IEnumerable<object[]> ValueTypes
        {
            get
            {
                foreach (var pair in ValueTypesData)
                {
                    yield return new[] { pair.Item1, pair.Item2 };
                    yield return new[] { pair.Item1, "System." + pair.Item2 };
                    yield return new[] { pair.Item1, "global::System." + pair.Item2 };
                }
            }
        }

        public static IEnumerable<object[]> EnumBaseTypes
        {
            get
            {
                foreach (var pair in EnumBaseTypesData)
                {
                    yield return new[] { pair.Item1, pair.Item2 };
                    yield return new[] { pair.Item1, "System." + pair.Item2 };
                    yield return new[] { pair.Item1, "global::System." + pair.Item2 };
                }
            }
        }

        public static IEnumerable<object[]> AllTypes
        {
            get
            {
                return ReferenceTypes.Concat(ValueTypes);
            }
        }

        public string DiagnosticId { get; } = SA1121UseBuiltInTypeAlias.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestVariableDeclaration(string predefined, string fullName)
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testSource, predefined), EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(string.Format(testSource, fullName), expected, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestVariableDeclarationCodeFix(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestDefaultDeclaration(string predefined, string fullName)
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

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestDefaultDeclarationCodeFix(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestTypeOf(string predefined, string fullName)
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 27);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testSource, fullName), expected, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestTypeOfCodeFix(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestReturnType(string predefined, string fullName)
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
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 12),
                this.CSharpDiagnostic().WithLocation(6, 24),
            };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testSource, fullName), expected, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestReturnTypeCodeFix(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(EnumBaseTypes))]
        public async Task TestEnumBaseType(string predefined, string fullName)
        {
            string testCode = @"namespace System {{
public class Foo
{{
    public enum Bar : {0}
    {{
    }}
}}
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 23);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(EnumBaseTypes))]
        public async Task TestEnumBaseTypeCodeFix(string predefined, string fullName)
        {
            string testCode = @"namespace System {{
public class Foo
{{
    public enum Bar : {0}
    {{
    }}
}}
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, fullName), string.Format(testCode, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestPointerDeclaration(string predefined, string fullName)
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

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestPointerDeclarationCodeFix(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestArgument(string predefined, string fullName)
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

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestArgumentCodeFix(string predefined, string fullName)
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar({0} test)
    {{
    }}
}}
}}";

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestIndexer(string predefined, string fullName)
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

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestIndexerCodeFix(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestGenericAndLambda(string predefined, string fullName)
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

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestGenericAndLambdaCodeFix(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestArray(string predefined, string fullName)
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

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestArrayCodeFix(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestStackAllocArray(string predefined, string fullName)
        {
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

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestStackAllocArrayCodeFix(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestImplicitCast(string predefined, string fullName)
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

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestImplicitCastCodeFix(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestDocumentationCommentDirectReference(string predefined, string fullName)
        {
            string testCode = @"using System;
/// <seealso cref=""{0}""/>
public class Foo
{{
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 20);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestDocumentationCommentDirectReferenceCodeFix(string predefined, string fullName)
        {
            string testCode = @"using System;
/// <seealso cref=""{0}""/>
public class Foo
{{
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, fullName), string.Format(testCode, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestDocumentationCommentIndirectReference(string predefined, string fullName)
        {
            string testCode = @"using System;
/// <seealso cref=""Convert.ToBoolean({0})""/>
public class Foo
{{
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 38);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestDocumentationCommentIndirectReferenceCodeFix(string predefined, string fullName)
        {
            string testCode = @"using System;
/// <seealso cref=""Convert.ToBoolean({0})""/>
public class Foo
{{
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, fullName), string.Format(testCode, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(ReferenceTypes))]
        public async Task TestExplicitCast(string predefined, string fullName)
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

        [Theory]
        [MemberData(nameof(ReferenceTypes))]
        public async Task TestExplicitCastCodeFix(string predefined, string fullName)
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar()
    {{
        var t = null as {0};
    }}
}}
}}";

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestNullable(string predefined, string fullName)
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

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestNullableCodeFix(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None);
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
            foreach (var item in AllTypesData)
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
            foreach (var item in AllTypesData)
            {
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "@" + item.Item1), EmptyDiagnosticResults, CancellationToken.None);
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, item.Item2), EmptyDiagnosticResults, CancellationToken.None);
            }
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestNameOf(string predefined, string fullName)
        {
            string testCode = @"
namespace System
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestNameOfInnerMethod(string predefined, string fullName)
        {
            string testCode = @"
namespace System
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 34);
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestNameOfInnerMethodCodeFix(string predefined, string fullName)
        {
            string testCode = @"
namespace System
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

            await this.VerifyCSharpFixAsync(string.Format(testCode, fullName), string.Format(testCode, predefined), cancellationToken:  CancellationToken.None);
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

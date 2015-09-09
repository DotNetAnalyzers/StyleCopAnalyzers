// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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

        public static IEnumerable<object[]> AllFullQualifiedTypes
        {
            get
            {
                foreach (var pair in ReferenceTypesData)
                {
                    yield return new[] { pair.Item1, "System." + pair.Item2 };
                    yield return new[] { pair.Item1, "global::System." + pair.Item2 };
                }

                foreach (var pair in ValueTypesData)
                {
                    yield return new[] { pair.Item1, "System." + pair.Item2 };
                    yield return new[] { pair.Item1, "global::System." + pair.Item2 };
                }
            }
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestVariableDeclarationAsync(string predefined, string fullName)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testSource, predefined), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(string.Format(testSource, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestEscapedVariableDeclarationAsync(string predefined, string fullName)
        {
            if (fullName.IndexOf('.') >= 0)
            {
                return;
            }

            string testSource = @"namespace NotSystem {{
public class ClassName
{{
    public void Bar()
    {{
        @{0} test;
    }}

    public struct @{0} {{ }}
}}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testSource, predefined), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(string.Format(testSource, fullName), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestVariableDeclarationCodeFixAsync(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestDefaultDeclarationAsync(string predefined, string fullName)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestDefaultDeclarationCodeFixAsync(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestTypeOfAsync(string predefined, string fullName)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testSource, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestTypeOfCodeFixAsync(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestReturnTypeAsync(string predefined, string fullName)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testSource, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestReturnTypeCodeFixAsync(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(EnumBaseTypes))]
        public async Task TestEnumBaseTypeAsync(string predefined, string fullName)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(EnumBaseTypes))]
        public async Task TestEnumBaseTypeCodeFixAsync(string predefined, string fullName)
        {
            string testCode = @"namespace System {{
public class Foo
{{
    public enum Bar : {0}
    {{
    }}
}}
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, fullName), string.Format(testCode, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestPointerDeclarationAsync(string predefined, string fullName)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestPointerDeclarationCodeFixAsync(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestArgumentAsync(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar({0} test)
    {{
    }}
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 21);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestArgumentCodeFixAsync(string predefined, string fullName)
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar({0} test)
    {{
    }}
}}
}}";

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestIndexerAsync(string predefined, string fullName)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestIndexerCodeFixAsync(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestGenericAndLambdaAsync(string predefined, string fullName)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestGenericAndLambdaCodeFixAsync(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestArrayAsync(string predefined, string fullName)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestArrayCodeFixAsync(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestStackAllocArrayAsync(string predefined, string fullName)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestStackAllocArrayCodeFixAsync(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestImplicitCastAsync(string predefined, string fullName)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestImplicitCastCodeFixAsync(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllFullQualifiedTypes))]
        public async Task TestUsingNameChangeGenericAsync(string predefined, string fullName)
        {
            string testCode = @"using System;
using IntAction = System.Action<{0}>;
public class Foo
{{
}}";
            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(2, 33)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllFullQualifiedTypes))]
        public async Task TestUsingNameChangeGenericCodeFixAsync(string predefined, string fullName)
        {
            string testSource = @"using System;
using IntAction = System.Action<{0}>;
public class Foo
{{
}}";

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllFullQualifiedTypes))]
        public async Task TestUsingStaticGenericAsync(string predefined, string fullName)
        {
            string testCode = @"using System;
using static StaticGenericClass<{0}>;
public class Foo
{{
}}
public static class StaticGenericClass<T> {{ }}";
            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(2, 33)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllFullQualifiedTypes))]
        public async Task TestUsingStaticGenericCodeFixAsync(string predefined, string fullName)
        {
            string testSource = @"using System;
using static StaticGenericClass<{0}>;
public class Foo
{{
}}
public static class StaticGenericClass<T> {{ }}";

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestDocumentationCommentDirectReferenceAsync(string predefined, string fullName)
        {
            string testCode = @"using System;
/// <seealso cref=""{0}""/>
public class Foo
{{
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 20);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestDocumentationCommentDirectReferenceCodeFixAsync(string predefined, string fullName)
        {
            string testCode = @"namespace System {{
/// <seealso cref=""{0}""/>
public class Foo
{{
}}
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, fullName), string.Format(testCode, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestDocumentationCommentIndirectReferenceAsync(string predefined, string fullName)
        {
            string testCode = @"using System;
/// <seealso cref=""Convert.ToBoolean({0})""/>
public class Foo
{{
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 38);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestDocumentationCommentIndirectReferenceCodeFixAsync(string predefined, string fullName)
        {
            string testCode = @"using System;
/// <seealso cref=""Convert.ToBoolean({0})""/>
public class Foo
{{
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, fullName), string.Format(testCode, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(ReferenceTypes))]
        public async Task TestExplicitCastAsync(string predefined, string fullName)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(ReferenceTypes))]
        public async Task TestExplicitCastCodeFixAsync(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestNullableAsync(string predefined, string fullName)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestNullableCodeFixAsync(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testSource, fullName), string.Format(testSource, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissleadingUsingAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissleadingUsingCodeFixAsync()
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

            await this.VerifyCSharpFixAsync(oldSource, newSource, allowNewCompilerDiagnostics: true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUsingNameChangeAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUsingNameChangeCodeFixAsync()
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

            await this.VerifyCSharpFixAsync(oldSource, newSource, allowNewCompilerDiagnostics: true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWrongTypeAsync()
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
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "@" + item.Item1), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, item.Item2), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task TestUsingAsync()
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
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "@" + item.Item1), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, item.Item2), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestNameOfAsync(string predefined, string fullName)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestNameOfInnerMethodAsync(string predefined, string fullName)
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
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestNameOfInnerMethodCodeFixAsync(string predefined, string fullName)
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

            await this.VerifyCSharpFixAsync(string.Format(testCode, fullName), string.Format(testCode, predefined), cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1121UseBuiltInTypeAlias();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1121CodeFixProvider();
        }
    }
}

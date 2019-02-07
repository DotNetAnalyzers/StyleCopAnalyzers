﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

// Several test methods in this file use the same member data, but in some cases the test does not use all of the
// supported parameters. See https://github.com/xunit/xunit/issues/1556.
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1121UseBuiltInTypeAlias,
        StyleCop.Analyzers.ReadabilityRules.SA1121CodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1121UseBuiltInTypeAlias"/>.
    /// </summary>
    public class SA1121UnitTests
    {
        private const string AllowBuiltInTypeAliasesSettings = @"
{
  ""settings"": {
    ""readabilityRules"": {
      ""allowBuiltInTypeAliases"": true
    }
  }
}
";

        private static readonly Tuple<string, string>[] ReferenceTypesData = new Tuple<string, string>[]
        {
            new Tuple<string, string>("object", nameof(Object)),
            new Tuple<string, string>("string", nameof(String)),
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
            new Tuple<string, string>("ulong", nameof(UInt64)),
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
            new Tuple<string, string>("ulong", nameof(UInt64)),
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 9);

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined)).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(string.Format(testSource, predefined), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(string.Format(testSource, fullName), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestDefaultDeclarationAsync(string predefined, string fullName)
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 28);

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined)).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 27);

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined)).ConfigureAwait(false);
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
                Diagnostic().WithLocation(4, 12),
                Diagnostic().WithLocation(6, 24),
            };

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined)).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(4, 23);

            await VerifyCSharpFixAsync(string.Format(testCode, fullName), expected, string.Format(testCode, predefined)).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestPointerDeclarationAsync(string predefined, string fullName)
        {
            string testCode = @"namespace System {{
public class Foo
{{
    public unsafe void Bar()
    {{
        {0}* test;
    }}
}}
}}";
            DiagnosticResult expected = Diagnostic().WithLocation(6, 9);

            await VerifyCSharpFixAsync(string.Format(testCode, fullName), expected, string.Format(testCode, predefined)).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestArgumentAsync(string predefined, string fullName)
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar({0} test)
    {{
    }}
}}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 21);

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined)).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestIndexerAsync(string predefined, string fullName)
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

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(4, 12),
                    Diagnostic().WithLocation(5, 14),
                    Diagnostic().WithLocation(7, 30),
                };

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined)).ConfigureAwait(false);
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
                    Diagnostic().WithLocation(6, 14),
                    Diagnostic().WithLocation(7, 17),
                    Diagnostic().WithLocation(8, 22),
                };

            await VerifyCSharpFixAsync(string.Format(testCode, fullName), expected, string.Format(testCode, predefined)).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestArrayAsync(string predefined, string fullName)
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 25);

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined)).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestStackAllocArrayAsync(string predefined, string fullName)
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 32);

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined)).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestImplicitCastAsync(string predefined, string fullName)
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

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(6, 18),
                    Diagnostic().WithLocation(7, 29),
                };

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined)).ConfigureAwait(false);
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
                    Diagnostic().WithLocation(2, 33),
                };

            await VerifyCSharpFixAsync(string.Format(testCode, fullName), expected, string.Format(testCode, predefined)).ConfigureAwait(false);
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
                    Diagnostic().WithLocation(2, 33),
                };

            await VerifyCSharpFixAsync(string.Format(testCode, fullName), expected, string.Format(testCode, predefined)).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllTypes))]
        public async Task TestDocumentationCommentDirectReferenceAsync(string predefined, string fullName)
        {
            string testCode = @"#pragma warning disable CS0419 // Ambiguous reference in cref attribute
namespace System {{
/// <seealso cref=""{0}""/>
public class Foo
{{
}}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 20);

            await VerifyCSharpFixAsync(string.Format(testCode, fullName), expected, string.Format(testCode, predefined)).ConfigureAwait(false);
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
            DiagnosticResult expected = Diagnostic().WithLocation(2, 38);

            await VerifyCSharpFixAsync(string.Format(testCode, fullName), expected, string.Format(testCode, predefined)).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(ReferenceTypes))]
        public async Task TestExplicitCastAsync(string predefined, string fullName)
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 25);

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined)).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public async Task TestNullableAsync(string predefined, string fullName)
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 9);

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined)).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissleadingUsingAsync()
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

            await new CSharpTest
            {
                TestCode = oldSource,
                ExpectedDiagnostics = { Diagnostic().WithLocation(6, 5) },
                FixedCode = newSource,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUsingNameChangeAsync()
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

            await new CSharpTest
            {
                TestCode = oldSource,
                ExpectedDiagnostics = { Diagnostic().WithLocation(6, 5) },
                FixedCode = newSource,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissleadingUsingAllowAliasesAsync()
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

            await new CSharpTest
            {
                TestCode = oldSource,
                ExpectedDiagnostics = { Diagnostic().WithLocation(6, 5) },
                FixedCode = newSource,
                Settings = AllowBuiltInTypeAliasesSettings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUsingNameChangeAllowAliasesAsync()
        {
            string testSource = @"namespace Foo
{
  using MyInt = System.UInt32;
  class Bar
  {
    MyInt value = 3;
  }
}
";

            await new CSharpTest
            {
                TestCode = testSource,
                Settings = AllowBuiltInTypeAliasesSettings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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
                await VerifyCSharpDiagnosticAsync(string.Format(testCode, "@" + item.Item1), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
                await VerifyCSharpDiagnosticAsync(string.Format(testCode, item.Item2), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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
                await VerifyCSharpDiagnosticAsync(string.Format(testCode, "@" + item.Item1), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
                await VerifyCSharpDiagnosticAsync(string.Format(testCode, item.Item2), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 34);
            await VerifyCSharpFixAsync(string.Format(testCode, fullName), expected, string.Format(testCode, predefined)).ConfigureAwait(false);
        }
    }
}

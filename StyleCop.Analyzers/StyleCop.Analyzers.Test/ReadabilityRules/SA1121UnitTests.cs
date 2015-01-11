namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using System;


    /// <summary>
    /// This class contains unit tests for <see cref="SA1121UseBuiltInTypeAlias"/>
    /// </summary>
    [TestClass]
    public class SA1121UnitTests : CodeFixVerifier
    {
        static readonly Tuple<string, string>[] _referenceTypes = new Tuple<string, string>[]
        {
            new Tuple<string,string>("object", "Object"),
            new Tuple<string,string>("string", "String")
        };
        static readonly Tuple<string, string>[] _valueTypes = new Tuple<string, string>[]
        {
            new Tuple<string,string>("bool", "Boolean"),
            new Tuple<string,string>("byte", "Byte"),
            new Tuple<string,string>("char", "Char"),
            new Tuple<string,string>("decimal", "Decimal"),
            new Tuple<string,string>("double", "Double"),
            new Tuple<string,string>("short", "Int16"),
            new Tuple<string,string>("int", "Int32"),
            new Tuple<string,string>("long", "Int64"),
            new Tuple<string,string>("sbyte", "SByte"),
            new Tuple<string,string>("float", "Single"),
            new Tuple<string,string>("ushort", "UInt16"),
            new Tuple<string,string>("uint", "UInt32"),
            new Tuple<string,string>("ulong", "UInt64")
        };
        static readonly Tuple<string, string>[] _enumBaseTypes = new Tuple<string, string>[]
        {
            new Tuple<string,string>("byte", "Byte"),
            new Tuple<string,string>("short", "Int16"),
            new Tuple<string,string>("int", "Int32"),
            new Tuple<string,string>("long", "Int64"),
            new Tuple<string,string>("sbyte", "SByte"),
            new Tuple<string,string>("ushort", "UInt16"),
            new Tuple<string,string>("uint", "UInt32"),
            new Tuple<string,string>("ulong", "UInt64")
        };
        static readonly Tuple<string, string>[] _allTypes = _referenceTypes.Concat(_valueTypes).ToArray();


        protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };

        public string DiagnosticId { get; } = SA1121UseBuiltInTypeAlias.DiagnosticId;

        private async Task TestCases(Func<string, string, Task> func, Tuple<string, string>[] types)
        {
            foreach (var item in types)
            {
                try
                {
                    await func(item.Item1, item.Item2);
                    await func(item.Item1, "System." + item.Item2);
                    await func(item.Item1, "global::System." + item.Item2);
                }
                catch (Exception ex)
                {
                    throw new AssertFailedException("Type failed: " + item.Item1 + Environment.NewLine + ex.Message, ex);
                }
            }
        }
        private async Task TestAllCases(Func<string, string, Task> func)
        {
            await TestCases(func, _allTypes);
        }
        private async Task TestEnumTypeCases(Func<string, string, Task> func)
        {
            await TestCases(func, _enumBaseTypes);
        }
        private async Task TestValueTypeCases(Func<string, string, Task> func)
        {
            await TestCases(func, _valueTypes);
        }
        private async Task TestReferenceTypeCases(Func<string, string, Task> func)
        {
            await TestCases(func, _referenceTypes);
        }


        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestVariableDeclaration(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar()
    {{
        {0} test;
    }}
}}";
            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 9)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, predefined), EmptyDiagnosticResults, CancellationToken.None);
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestVariableDeclaration()
        {
            await TestAllCases(TestVariableDeclaration);
        }

        private async Task TestDefaultDeclaration(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar()
    {{
        var test = default({0});
    }}
}}";
            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 28)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDefaultDeclaration()
        {
            await TestAllCases(TestDefaultDeclaration);
        }

        private async Task TestTypeOf(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar()
    {{
        var test = typeof({0});
    }}
}}";
            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 27)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestTypeOf()
        {
            await TestAllCases(TestTypeOf);
        }

        private async Task TestReturnType(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public {0} Bar()
    {{
    }}
}}";
            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 4, 12)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestReturnType()
        {
            await TestAllCases(TestReturnType);
        }

        private async Task TestEnumBaseType(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public enum Bar : {0}
    {{
    }}
}}";
            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 4, 23)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEnumBaseType()
        {
            await TestEnumTypeCases(TestEnumBaseType);
        }

        private async Task TestPointerDeclaration(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public unsafe void Bar()
    {{
        {0}* test;
    }}
}}";
            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 9)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPointerDeclaration()
        {
            await TestValueTypeCases(TestPointerDeclaration);
        }

        private async Task TestArgument(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar({0} test)
    {{
    }}
}}";
            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 4, 21)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestArgument()
        {
            await TestAllCases(TestArgument);
        }

        private async Task TestIndexer(string predefined, string fullName)
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
            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 4, 12)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 5, 14)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 30)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexer()
        {
            await TestAllCases(TestIndexer);
        }

        private async Task TestGenericAndLambda(string predefined, string fullName)
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
            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 14)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 17)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 22)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestGenericAndLambda()
        {
            await TestAllCases(TestGenericAndLambda);
        }

        private async Task TestArray(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar()
    {{
        var array = new {0}[0];
    }}
}}";
            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 25)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestArray()
        {
            await TestAllCases(TestArray);
        }

        private async Task TestStackAllocArray(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public unsafe void Bar()
    {{
        var array = stackalloc {0}[0];
    }}
}}";
            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 32)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestStackAllocArray()
        {
            await TestAllCases(TestStackAllocArray);
        }

        private async Task TestImplicitCast(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public unsafe void Bar()
    {{
        var t = ({0})
                    default({0});
    }}
}}";
            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 18)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 29)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestImplicitCast()
        {
            await TestAllCases(TestImplicitCast);
        }

        private async Task TestExplicitCast(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public unsafe void Bar()
    {{
        var t = null as {0};
    }}
}}";
            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 25)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestExplicitCast()
        {
            await TestReferenceTypeCases(TestExplicitCast);
        }

        private async Task TestNullable(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public unsafe void Bar()
    {{
        {0}? t = null;
    }}
}}";
            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Use built-in type alias",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 9)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestNullable()
        {
            await TestValueTypeCases(TestNullable);
        }

        [TestMethod]
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
            foreach (var item in _allTypes)
            {
                await VerifyCSharpDiagnosticAsync(string.Format(testCode, "@" + item.Item1), EmptyDiagnosticResults, CancellationToken.None);
                await VerifyCSharpDiagnosticAsync(string.Format(testCode, item.Item2), EmptyDiagnosticResults, CancellationToken.None);
            }
        }

        [TestMethod]
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
public namespace {0} 
{{
        public class Bar {{ }}
}}
";
            foreach (var item in _allTypes)
            {
                await VerifyCSharpDiagnosticAsync(string.Format(testCode, "@" + item.Item1), EmptyDiagnosticResults, CancellationToken.None);
                await VerifyCSharpDiagnosticAsync(string.Format(testCode, item.Item2), EmptyDiagnosticResults, CancellationToken.None);
            }
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1121UseBuiltInTypeAlias();
        }
    }
}

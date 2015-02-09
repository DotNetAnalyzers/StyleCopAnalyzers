﻿namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;

    [TestClass]
    public class SA1101UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1101PrefixLocalCallsWithThis.DiagnosticId;

        private const string ReferenceCode = @"
        public class BaseTypeName
        {
            public static int BaseStaticFieldName;
            public int BaseInstanceFieldName;

            public static event EventHandler BaseStaticEventName;
            public virtual event EventHandler BaseInstanceEventName;

            public static int BaseStaticPropertyName
            {
                get;
            }

            public virtual int BaseInstancePropertyName
            {
                get;
                set;
            }

            public static void BaseStaticMethodName()
            {
            }

            public virtual void BaseInstanceMethodName()
            {
            }

            public static void BaseMethodGroupName(int x)
            {
            }

            public virtual void BaseMethodGroupName(string y)
            {
            }
        }

        [Obsolete(nameof(BaseInstanceFieldName))]
        public class TypeName : BaseTypeName
        {
            /// <seealso cref='BaseStaticFieldName'/>
            /// <seealso cref='BaseInstanceFieldName'/>
            [Obsolete(nameof(BaseStaticFieldName))]
            public static int N1;
            [Obsolete(nameof(BaseInstanceFieldName))]
            public int N2;

            [Obsolete(nameof(BaseStaticEventName))]
            public static int N3;
            [Obsolete(nameof(BaseInstanceEventName))]
            public static int N4;

            [Obsolete(nameof(BaseStaticPropertyName))]
            public static int N5;
            [Obsolete(nameof(BaseInstancePropertyName))]
            public static int N6;

            [Obsolete(nameof(BaseStaticMethodName))]
            public static int N7;
            [Obsolete(nameof(BaseInstanceMethodName))]
            public static int N8;

            [Obsolete(nameof(N1))]
            public static int N9;
            [Obsolete(nameof(N2))]
            public static int N10;

            public static void StaticMethodName(int ParameterName)
            {
                string LocalName;
                LocalName = nameof(N1);
                LocalName = nameof(N2);
                LocalName = nameof(ParameterName);
                LocalName = nameof(LocalName);
                LocalName = nameof(BaseStaticFieldName);
                LocalName = nameof(BaseInstanceFieldName);
                LocalName = nameof(BaseStaticEventName);
                LocalName = nameof(BaseInstanceEventName);
                LocalName = nameof(BaseStaticPropertyName);
                LocalName = nameof(BaseInstancePropertyName);
                LocalName = nameof(BaseStaticMethodName);
                LocalName = nameof(BaseInstanceMethodName);
                LocalName = nameof(BaseMethodGroupName);
            }

            public void InstanceMethodName(int ParameterName)
            {
                string LocalName;
                LocalName = nameof(N1);
                LocalName = nameof(N2);
                LocalName = nameof(ParameterName);
                LocalName = nameof(LocalName);
                LocalName = nameof(BaseStaticFieldName);
                LocalName = nameof(BaseInstanceFieldName);
                LocalName = nameof(BaseStaticEventName);
                LocalName = nameof(BaseInstanceEventName);
                LocalName = nameof(BaseStaticPropertyName);
                LocalName = nameof(BaseInstancePropertyName);
                LocalName = nameof(BaseStaticMethodName);
                LocalName = nameof(BaseInstanceMethodName);
                LocalName = nameof(BaseMethodGroupName);

                var obj = new BaseTypeName
                {
                    BaseInstanceFieldName = BaseInstanceFieldName,
                    BaseInstancePropertyName = BaseInstancePropertyName,
                }
            }
        }
        ";

        private static string FixedCode = @"
        public class BaseTypeName
        {
            public static int BaseStaticFieldName;
            public int BaseInstanceFieldName;

            public static event EventHandler BaseStaticEventName;
            public virtual event EventHandler BaseInstanceEventName;

            public static int BaseStaticPropertyName
            {
                get;
            }

            public virtual int BaseInstancePropertyName
            {
                get;
                set;
            }

            public static void BaseStaticMethodName()
            {
            }

            public virtual void BaseInstanceMethodName()
            {
            }

            public static void BaseMethodGroupName(int x)
            {
            }

            public virtual void BaseMethodGroupName(string y)
            {
            }
        }

        [Obsolete(nameof(BaseInstanceFieldName))]
        public class TypeName : BaseTypeName
        {
            /// <seealso cref='BaseStaticFieldName'/>
            /// <seealso cref='BaseInstanceFieldName'/>
            [Obsolete(nameof(BaseStaticFieldName))]
            public static int N1;
            [Obsolete(nameof(BaseInstanceFieldName))]
            public int N2;

            [Obsolete(nameof(BaseStaticEventName))]
            public static int N3;
            [Obsolete(nameof(BaseInstanceEventName))]
            public static int N4;

            [Obsolete(nameof(BaseStaticPropertyName))]
            public static int N5;
            [Obsolete(nameof(BaseInstancePropertyName))]
            public static int N6;

            [Obsolete(nameof(BaseStaticMethodName))]
            public static int N7;
            [Obsolete(nameof(BaseInstanceMethodName))]
            public static int N8;

            [Obsolete(nameof(N1))]
            public static int N9;
            [Obsolete(nameof(N2))]
            public static int N10;

            public static void StaticMethodName(int ParameterName)
            {
                string LocalName;
                LocalName = nameof(N1);
                LocalName = nameof(N2);
                LocalName = nameof(ParameterName);
                LocalName = nameof(LocalName);
                LocalName = nameof(BaseStaticFieldName);
                LocalName = nameof(BaseInstanceFieldName);
                LocalName = nameof(BaseStaticEventName);
                LocalName = nameof(BaseInstanceEventName);
                LocalName = nameof(BaseStaticPropertyName);
                LocalName = nameof(BaseInstancePropertyName);
                LocalName = nameof(BaseStaticMethodName);
                LocalName = nameof(BaseInstanceMethodName);
                LocalName = nameof(BaseMethodGroupName);
            }

            public void InstanceMethodName(int ParameterName)
            {
                string LocalName;
                LocalName = nameof(N1);
                LocalName = nameof(this.N2);
                LocalName = nameof(ParameterName);
                LocalName = nameof(LocalName);
                LocalName = nameof(BaseStaticFieldName);
                LocalName = nameof(this.BaseInstanceFieldName);
                LocalName = nameof(BaseStaticEventName);
                LocalName = nameof(this.BaseInstanceEventName);
                LocalName = nameof(BaseStaticPropertyName);
                LocalName = nameof(this.BaseInstancePropertyName);
                LocalName = nameof(BaseStaticMethodName);
                LocalName = nameof(this.BaseInstanceMethodName);
                LocalName = nameof(BaseMethodGroupName);

                var obj = new BaseTypeName
                {
                    BaseInstanceFieldName = this.BaseInstanceFieldName,
                    BaseInstancePropertyName = this.BaseInstancePropertyName,
                }
            }
        }
        ";

        private DiagnosticResult CreateDiagnosticResult(int line, int column)
        {
            return new DiagnosticResult
            {
                Id = DiagnosticId,
                Message = "Prefix local calls with this",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", line, column)
                    }
            };
        }

        [TestMethod]
        public async Task TestPrefixLocalCallsWithThisDiagnostics()
        {
            var expected = new[]
            {
                CreateDiagnosticResult(90, 36),
                CreateDiagnosticResult(94, 36),
                CreateDiagnosticResult(96, 36),
                CreateDiagnosticResult(98, 36),
                CreateDiagnosticResult(100, 36),
                CreateDiagnosticResult(105, 45),
                CreateDiagnosticResult(106, 48),
            };

            await VerifyCSharpDiagnosticAsync(ReferenceCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMemberAccessExpressions()
        {
            string code = @"class Foo
{
    public string Bar {get; set; }
    void Main()
    {
        var foo = new Foo();
        string bar = foo?.Bar;
    }
}";

            await VerifyCSharpDiagnosticAsync(code, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPrefixLocalCallsWithThisCodeFix()
        {
            await VerifyCSharpFixAsync(ReferenceCode, FixedCode, cancellationToken: CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1101PrefixLocalCallsWithThis();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1101CodeFixProvider();
        }
    }
}

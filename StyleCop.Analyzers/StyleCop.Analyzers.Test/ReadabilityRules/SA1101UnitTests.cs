// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1101PrefixLocalCallsWithThis,
        StyleCop.Analyzers.ReadabilityRules.SA1101CodeFixProvider>;

    public class SA1101UnitTests
    {
        [Fact]
        public async Task TestPrefixLocalCallsWithThisAsync()
        {
            string testCode = @"
        using System;
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
                };

                // the following line is a regression test for #464
                // https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/464
                var conditional = this?.BaseInstanceFieldName;
            }
        }
        ";

            string fixedCode = @"
        using System;
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
                };

                // the following line is a regression test for #464
                // https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/464
                var conditional = this?.BaseInstanceFieldName;
            }
        }
        ";

            var expected = new[]
            {
                Diagnostic().WithLocation(91, 36),
                Diagnostic().WithLocation(95, 36),
                Diagnostic().WithLocation(97, 36),
                Diagnostic().WithLocation(99, 36),
                Diagnostic().WithLocation(101, 36),
                Diagnostic().WithLocation(106, 45),
                Diagnostic().WithLocation(107, 48),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPrefixLocalCallsWithThisWithGenericArgumentsAsync()
        {
            string testCode = @"public class Test_SA1101
{
    public void Foo()
    {
        ConvertAll(42); // SA1101
        this.ConvertAll(42); // no SA1101
        ConvertAll<int>(42); // SA1101
        this.ConvertAll<int>(42); // no SA1101
    }
    public void ConvertAll<T>(T value) { }
}";
            string fixedCode = @"public class Test_SA1101
{
    public void Foo()
    {
        this.ConvertAll(42); // SA1101
        this.ConvertAll(42); // no SA1101
        this.ConvertAll<int>(42); // SA1101
        this.ConvertAll<int>(42); // no SA1101
    }
    public void ConvertAll<T>(T value) { }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(5, 9),
                Diagnostic().WithLocation(7, 9),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a collision between a member and a static member is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2093, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2093")]
        public async Task TestNameCollisionForStaticMethodAsync()
        {
            var testCode = @"
using System;

public class TestClass
{
    public DateTime DateTime => DateTime.FromFileTime(0);
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a collision between a member and a static property is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNameCollisionForStaticPropertyAsync()
        {
            var testCode = @"
using System;

public class TestClass
{
    public DateTime DateTime => DateTime.UtcNow;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2211, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2211")]
        public async Task TestStaticMemberAliasesPropertyAsync()
        {
            var testCode = @"
using System;

public class Foo
{
    public Array Array { get; } = new int[0];

    public int IndexOf(object value) => Array.IndexOf(this.Array, value);
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2656, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2656")]
        public async Task TestStaticMemberNameOfAsync()
        {
            var testCode = @"
public class Foo
{
    public string Array { get; } = nameof(Array);
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2799, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2799")]
        public async Task TestNameofInConstructorCallAsync()
        {
            var testCode = @"
public class TestClass
{
    public TestClass()
        : this(nameof(P))
    {
    }

    public TestClass(string p)
    {
        this.P = p;
    }

    public string P { get; }
}

public class DerivedTestClass : TestClass
{
    public DerivedTestClass()
        : base(nameof(Q))
    {
        this.Q = string.Empty;
    }

    public string Q { get; }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
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

    public class SA1101UnitTests : CodeFixVerifier
    {
        private const string ReferenceCode = @"
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

        private static string fixedCode = @"
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

        [Fact]
        public async Task TestPrefixLocalCallsWithThisDiagnosticsAsync()
        {
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(91, 36),
                this.CSharpDiagnostic().WithLocation(95, 36),
                this.CSharpDiagnostic().WithLocation(97, 36),
                this.CSharpDiagnostic().WithLocation(99, 36),
                this.CSharpDiagnostic().WithLocation(101, 36),
                this.CSharpDiagnostic().WithLocation(106, 45),
                this.CSharpDiagnostic().WithLocation(107, 48),
            };

            await this.VerifyCSharpDiagnosticAsync(ReferenceCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(5, 9),
                this.CSharpDiagnostic().WithLocation(7, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPrefixLocalCallsWithThisCodeFixAsync()
        {
            await this.VerifyCSharpFixAsync(ReferenceCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1101PrefixLocalCallsWithThis();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1101CodeFixProvider();
        }
    }
}

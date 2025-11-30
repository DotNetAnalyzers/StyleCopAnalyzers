// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Text;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1412StoreFilesAsUtf8,
        StyleCop.Analyzers.MaintainabilityRules.SA1412CodeFixProvider>;

    public class SA1412UnitTests
    {
#if NET
        static SA1412UnitTests()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
#endif

        public static IEnumerable<object[]> NonUtf8Encodings
        {
            get
            {
                yield return new object[] { Encoding.ASCII.CodePage };
                yield return new object[] { Encoding.BigEndianUnicode.CodePage };
#if NETFRAMEWORK
                yield return new object[] { Encoding.Default.CodePage };
#else
                yield return new object[] { 1252 };
#endif
                yield return new object[] { Encoding.Unicode.CodePage };
                yield return new object[] { Encoding.UTF32.CodePage };
#pragma warning disable SYSLIB0001 // Type or member is obsolete
                yield return new object[] { Encoding.UTF7.CodePage };
#pragma warning restore SYSLIB0001 // Type or member is obsolete
            }
        }

        [Theory]
        [MemberData(nameof(NonUtf8Encodings))]
        public async Task TestFileWithWrongEncodingAsync(int codepage)
        {
            var testCode = SourceText.From("class TypeName { }", GetEncoding(codepage));
            var fixedCode = SourceText.From(testCode.ToString(), Encoding.UTF8);

            var expected = Diagnostic().WithLocation(1, 1);

            var test = new CSharpTest
            {
                TestSources = { testCode },
                ExpectedDiagnostics = { expected },
                FixedSources = { fixedCode },
            };

            test.TestBehaviors |= TestBehaviors.SkipSuppressionCheck;
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFileWithUtf8EncodingWithoutBOMAsync()
        {
            var testCode = SourceText.From("class TypeName { }", new UTF8Encoding(false));
            var fixedCode = SourceText.From(testCode.ToString(), Encoding.UTF8);

            var expected = Diagnostic().WithLocation(1, 1);

            var test = new CSharpTest
            {
                TestSources = { testCode },
                ExpectedDiagnostics = { expected },
                FixedSources = { fixedCode },
            };

            test.TestBehaviors |= TestBehaviors.SkipSuppressionCheck;
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(NonUtf8Encodings))]
        public async Task TestFixAllAsync(int codepage)
        {
            await this.TestFixAllExecuterAsync(codepage, FixAllScope.Project).ConfigureAwait(false);
            await this.TestFixAllExecuterAsync(codepage, FixAllScope.Solution).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFixAllWithMultipleEncodingsAsync()
        {
            var test = new CSharpTest
            {
                TestSources =
                {
                    SourceText.From("class Foo { }", Encoding.Unicode),
                    SourceText.From("class Bar { }", Encoding.Unicode),
#pragma warning disable SYSLIB0001 // Type or member is obsolete
                    SourceText.From("class FooBar { }", Encoding.UTF7),
#pragma warning restore SYSLIB0001 // Type or member is obsolete
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation("/0/Test0.cs", 1, 1),
                    Diagnostic().WithLocation("/0/Test1.cs", 1, 1),
                    Diagnostic().WithLocation("/0/Test2.cs", 1, 1),
                },
                FixedSources =
                {
                    SourceText.From("class Foo { }", Encoding.UTF8),
                    SourceText.From("class Bar { }", Encoding.UTF8),
                    SourceText.From("class FooBar { }", Encoding.UTF8),
                },
                NumberOfFixAllIterations = 2,
                NumberOfFixAllInDocumentIterations = 3,
            };

            test.TestBehaviors |= TestBehaviors.SkipSuppressionCheck;
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        private static Encoding GetEncoding(int codepage)
        {
#pragma warning disable SYSLIB0001 // Type or member is obsolete
            if (codepage == Encoding.UTF7.CodePage)
            {
                return Encoding.UTF7;
            }
#pragma warning restore SYSLIB0001 // Type or member is obsolete

            return Encoding.GetEncoding(codepage);
        }

        private async Task TestFixAllExecuterAsync(int codepage, FixAllScope scope)
        {
            // Currently unused
            _ = scope;

            var test = new CSharpTest
            {
                TestSources =
                {
                    SourceText.From("class Foo { }", GetEncoding(codepage)),
                    SourceText.From("class Bar { }", GetEncoding(codepage)),
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation("/0/Test0.cs", 1, 1),
                    Diagnostic().WithLocation("/0/Test1.cs", 1, 1),
                },
                FixedSources =
                {
                    SourceText.From("class Foo { }", Encoding.UTF8),
                    SourceText.From("class Bar { }", Encoding.UTF8),
                },
                NumberOfFixAllIterations = 1,
                NumberOfFixAllInDocumentIterations = 2,
            };

            test.TestBehaviors |= TestBehaviors.SkipSuppressionCheck;
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
        public static IEnumerable<object[]> NonUtf8Encodings
        {
            get
            {
                yield return new object[] { Encoding.ASCII.CodePage };
                yield return new object[] { Encoding.BigEndianUnicode.CodePage };
                yield return new object[] { Encoding.Default.CodePage };
                yield return new object[] { Encoding.Unicode.CodePage };
                yield return new object[] { Encoding.UTF32.CodePage };
                yield return new object[] { Encoding.UTF7.CodePage };
            }
        }

        [Theory]
        [MemberData(nameof(NonUtf8Encodings))]
        public async Task TestFileWithWrongEncodingAsync(int codepage)
        {
            var testCode = SourceText.From("class TypeName { }", Encoding.GetEncoding(codepage));
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
                    SourceText.From("class FooBar { }", Encoding.UTF7),
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation("Test0.cs", 1, 1),
                    Diagnostic().WithLocation("Test1.cs", 1, 1),
                    Diagnostic().WithLocation("Test2.cs", 1, 1),
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

        private async Task TestFixAllExecuterAsync(int codepage, FixAllScope scope)
        {
            // Currently unused
            _ = scope;

            var test = new CSharpTest
            {
                TestSources =
                {
                    SourceText.From("class Foo { }", Encoding.GetEncoding(codepage)),
                    SourceText.From("class Bar { }", Encoding.GetEncoding(codepage)),
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation("Test0.cs", 1, 1),
                    Diagnostic().WithLocation("Test1.cs", 1, 1),
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

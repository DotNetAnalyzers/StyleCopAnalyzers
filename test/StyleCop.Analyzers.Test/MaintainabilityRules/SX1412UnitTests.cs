// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using StyleCop.Analyzers.Test.Helpers;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.MaintainabilityRules.SX1412StoreFilesAsUtf8WithoutByteOrderMark,
    StyleCop.Analyzers.MaintainabilityRules.SX1412CodeFixProvider>;

public class SX1412UnitTests
{
    [Fact]
    public async Task TestFileWithUtf8EncodingWithoutByteOrderMarkAsync()
    {
        SourceText source = SourceText.From("class TypeName { }", new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        var test = new CSharpTest
        {
            TestSources = { source },
            FixedSources = { source },
        };

        test.TestBehaviors |= TestBehaviors.SkipSuppressionCheck;
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestFileWithStrictUtf8EncodingWithoutByteOrderMarkAsync()
    {
        SourceText source = SourceText.From("class TypeName { }", new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true));
        var test = new CSharpTest
        {
            TestSources = { source },
            FixedSources = { source },
        };

        test.TestBehaviors |= TestBehaviors.SkipSuppressionCheck;
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestFileWithUtf8EncodingWithByteOrderMarkAsync()
    {
        const string source = "class TypeName { }";
        var test = new CSharpTest
        {
            TestSources = { SourceText.From(source, Encoding.UTF8) },
            ExpectedDiagnostics = { Diagnostic().WithLocation(1, 1) },
            FixedSources = { SourceText.From(source, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)) },
        };

        test.TestBehaviors |= TestBehaviors.SkipSuppressionCheck;
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestFileWithNonUtf8EncodingAsync()
    {
        const string source = "class TypeName { }";
        var test = new CSharpTest
        {
            TestSources = { SourceText.From(source, Encoding.Unicode) },
            ExpectedDiagnostics = { Diagnostic().WithLocation(1, 1) },
            FixedSources = { SourceText.From(source, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)) },
        };

        test.TestBehaviors |= TestBehaviors.SkipSuppressionCheck;
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestFileWithUnknownEncodingAsync()
    {
        const string source = "class TypeName { }";
        var test = new CSharpTest
        {
            TestSources = { source },
            ExpectedDiagnostics = { Diagnostic().WithLocation(1, 1) },
            FixedSources = { SourceText.From(source, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)) },
        };

        test.TestBehaviors |= TestBehaviors.SkipSuppressionCheck;
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestFixAllWithMultipleFilesAsync()
    {
        const string firstSource = "class FirstType { }";
        const string secondSource = "class SecondType { }";
        var test = new CSharpTest
        {
            TestSources =
            {
                SourceText.From(firstSource, Encoding.UTF8),
                SourceText.From(secondSource, Encoding.UTF8),
            },
            ExpectedDiagnostics =
            {
                Diagnostic().WithLocation("/0/Test0.cs", 1, 1),
                Diagnostic().WithLocation("/0/Test1.cs", 1, 1),
            },
            FixedSources =
            {
                SourceText.From(firstSource, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)),
                SourceText.From(secondSource, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)),
            },
            NumberOfFixAllIterations = 1,
            NumberOfFixAllInDocumentIterations = 2,
        };

        test.TestBehaviors |= TestBehaviors.SkipSuppressionCheck;
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }
}

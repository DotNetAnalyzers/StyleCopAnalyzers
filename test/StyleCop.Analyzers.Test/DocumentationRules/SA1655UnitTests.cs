// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.DocumentationRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1655DocumentationElementTagsMustBeConsistentlyPlaced>;

/// <summary>
/// Unit tests for <see cref="SA1655DocumentationElementTagsMustBeConsistentlyPlaced"/>.
/// </summary>
public class SA1655UnitTests
{
    [Fact]
    public async Task TestBothTagsOnOneLineAsync()
    {
        var testCode = @"/// <summary>Summary text.</summary>
public class TestClass
{
}";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestBothTagsAloneAsync()
    {
        var testCode = @"/// <summary>
/// Summary text.
/// </summary>
public class TestClass
{
}";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestNeitherTagAloneWithTextSpanningLinesAsync()
    {
        var testCode = @"/// <summary>
/// <p>paragraph text
/// more text</p>.
/// </summary>
public class TestClass
{
}";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestEndTagAloneButStartTagNotAloneAsync()
    {
        var testCode = @"/// {|#0:<summary>|}Summary text
/// </summary>
public class TestClass
{
}";

        var expected = Diagnostic().WithLocation(0).WithArguments("summary");
        await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestStartTagAloneButEndTagNotAloneAsync()
    {
        var testCode = @"/// <summary>
/// Summary text.{|#0:</summary>|}
public class TestClass
{
}";

        var expected = Diagnostic().WithLocation(0).WithArguments("summary");
        await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestParaWithStartTagAloneButEndTagNotAloneAsync()
    {
        var testCode = @"/// <summary>
/// <para>
/// Paragraph text.{|#0:</para>|}
/// </summary>
public class TestClass
{
}";

        var expected = Diagnostic().WithLocation(0).WithArguments("para");
        await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestParaWithNeitherTagAloneAsync()
    {
        var testCode = @"/// <summary>
/// <para>Paragraph text
/// more text.</para>
/// </summary>
public class TestClass
{
}";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestStartTagFollowedByTextAndPrecededByTextAsync()
    {
        var testCode = @"/// <summary>
/// Some text {|#0:<b>|}
/// bold
/// </b>
/// </summary>
public class TestClass
{
}";

        var expected = Diagnostic().WithLocation(0).WithArguments("b");
        await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMultiLineStartTagWithAttributesAsync()
    {
        var testCode = @"/// <summary>
/// Summary text.
/// </summary>
/// <param
///   name=""value"">
/// The value.
/// </param>
public class TestClass
{
    /// <param name=""value"">The value.</param>
    public void Method(int value)
    {
    }
}";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestIncompleteElementIsIgnoredAsync()
    {
        var testCode = @"/// <summary>
/// Summary text.
public class TestClass
{
}";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestSlashInTextIsNotCommentExteriorAsync()
    {
        var testCode = @"/// <summary>
/// / {|#0:<b>|}
/// bold
/// </b>
/// </summary>
public class TestClass
{
}";

        var expected = Diagnostic().WithLocation(0).WithArguments("b");
        await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestSlashInTextBeforeBothTagsIsAcceptedAsync()
    {
        var testCode = @"/// <summary>
/// / <b>
/// bold
/// / </b>
/// </summary>
public class TestClass
{
}";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestBlockCommentTerminatorIsNotTextAfterClosingTagAsync()
    {
        var testCode = @"/**
 * <summary>
 * Summary text.
 * </summary> */
public class TestClass
{
}";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestBlockCommentWithMisplacedClosingTagAsync()
    {
        var testCode = @"/**
 * <summary>
 * Summary text.{|#0:</summary>|} */
public class TestClass
{
}";

        var expected = Diagnostic().WithLocation(0).WithArguments("summary");
        await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
    }
}

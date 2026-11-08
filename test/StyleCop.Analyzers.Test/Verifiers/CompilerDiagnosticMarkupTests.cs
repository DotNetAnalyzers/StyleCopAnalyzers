// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers;

using Xunit;

/// <summary>
/// This class verifies <see cref="CompilerDiagnosticMarkup"/>.
/// </summary>
public class CompilerDiagnosticMarkupTests
{
    [Theory]
    [InlineData("a{|CS1001:|}b", "ab")]
    [InlineData("{|CS1003:{|CS1026:;|}|}", ";")]
    [InlineData("{|CS1001:|}{|CS1002:x|}", "x")]
    [InlineData("{|CS1001:{|}", "{")]
    [InlineData("{|#0:{|CS1001:x|}|}", "{|#0:x|}")]
    [InlineData("{|CS1001:{|#0:x|}|}", "{|#0:x|}")]
    [InlineData("[|x|]{|#0:y|}{|SA1002:z|}", "[|x|]{|#0:y|}{|SA1002:z|}")]
    [InlineData("a|}b", "a|}b")]
    [InlineData("{|CS:x|}{|CSX1:y|}", "{|CS:x|}{|CSX1:y|}")]
    public void TestRemove(string source, string expected)
    {
        Assert.Equal(expected, CompilerDiagnosticMarkup.Remove(source));
    }
}

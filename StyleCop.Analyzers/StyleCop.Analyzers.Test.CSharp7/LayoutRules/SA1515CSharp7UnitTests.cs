// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.LayoutRules;
    using Xunit;

    public class SA1515CSharp7UnitTests : SA1515UnitTests
    {
        [Fact]
        public async Task TestCommentAfterCasePatternSwitchLabelAsync()
        {
            var testCode = @"
public class ClassName
{
    public void Method()
    {
        switch (new object())
        {
            case int x:
                // Single line comment after pattern-matching case statement is valid
                break;
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

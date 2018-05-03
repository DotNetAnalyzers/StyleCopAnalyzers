// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.SpacingRules;
    using Xunit;

    public class SA1011CSharp7UnitTests : SA1011UnitTests
    {
        /// <summary>
        /// Verifies spacing around a <c>]</c> character in tuple types and expressions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1001CSharp7UnitTests.TestBracketsInTupleTypesNotFollowedBySpaceAsync"/>
        /// <seealso cref="SA1009CSharp7UnitTests.TestBracketsInTupleTypesNotFollowedBySpaceAsync"/>
        [Fact]
        public async Task TestBracketsInTupleTypesNotFollowedBySpaceAsync()
        {
            // No diagnostics reported because the offending tokens are followed by comma (SA1001) or a closing
            // parenthesis (SA1009)
            const string testCode = @"using System;

public class Foo
{
    public (int[] , int[] ) TestMethod((int[] , int[] ) a)
    {
        (int[] , int[] ) ints = (new int[][] { new[] { 3 } }[0] , new int[][] { new[] { 3 } }[0] );
        return ints;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

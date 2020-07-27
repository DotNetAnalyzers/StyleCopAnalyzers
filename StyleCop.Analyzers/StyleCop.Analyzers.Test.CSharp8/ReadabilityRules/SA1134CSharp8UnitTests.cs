// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.ReadabilityRules
{
    using System.Threading.Tasks;

    using StyleCop.Analyzers.Test.CSharp7.ReadabilityRules;

    public class SA1134CSharp8UnitTests : SA1134CSharp7UnitTests
    {
        /// <inheritdoc/>
        public override Task VerifyInvalidMemberSyntaxInCodeFixAsync()
        {
            // Making this test a dummy, as the 3.6.0 compiler actually parses the invalid syntax
            // into a valid AttributeSyntaxList, with an attribute named ';' (which is an invalid name).
            // Because of this, the code fix no longer fails, but it ofcourse produces garbage.
            return Task.CompletedTask;
        }
    }
}

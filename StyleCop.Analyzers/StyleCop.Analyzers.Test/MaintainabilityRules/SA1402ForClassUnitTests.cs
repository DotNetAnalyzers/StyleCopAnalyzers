// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public class SA1402ForClassUnitTests : SA1402ForBlockDeclarationUnitTestsBase
    {
        public override string Keyword => "class";

        protected override bool IsConfiguredAsTopLevelTypeByDefault => true;
    }
}

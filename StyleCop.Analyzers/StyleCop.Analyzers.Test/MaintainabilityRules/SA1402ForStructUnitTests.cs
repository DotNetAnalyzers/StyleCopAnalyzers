// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    internal class SA1402ForStructUnitTests : SA1402ForBlockDeclarationUnitTestsBase
    {
        public override string Keyword => "struct";

        protected override bool IsConfiguredAsTopLevelTypeByDefault => false;
    }
}

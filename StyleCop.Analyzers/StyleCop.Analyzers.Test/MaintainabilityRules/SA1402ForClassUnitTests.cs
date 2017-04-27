﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public class SA1402ForClassUnitTests : SA1402ForBlockDeclarationUnitTestsBase
    {
        public override string Keyword => "class";

        protected override bool IsConfiguredAsTopLevelTypeByDefault => true;
    }
}

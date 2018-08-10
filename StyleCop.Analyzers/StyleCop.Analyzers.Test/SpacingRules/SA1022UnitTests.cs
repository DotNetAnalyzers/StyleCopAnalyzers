// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using StyleCop.Analyzers.SpacingRules;

    internal class SA1022UnitTests : NumberSignSpacingTestBase<SA1022PositiveSignsMustBeSpacedCorrectly, TokenSpacingCodeFixProvider>
    {
        protected override string Sign
        {
            get
            {
                return "+";
            }
        }
    }
}

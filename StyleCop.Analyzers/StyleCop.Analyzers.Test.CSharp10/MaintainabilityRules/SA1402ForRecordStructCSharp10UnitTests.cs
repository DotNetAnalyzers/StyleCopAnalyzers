// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules
{
    using StyleCop.Analyzers.Test.MaintainabilityRules;

    public class SA1402ForRecordStructCSharp10UnitTests : SA1402ForBlockDeclarationUnitTestsBase
    {
        public override string Keyword => "record struct";

        protected override string SettingKeyword => "struct";

        protected override bool IsConfiguredAsTopLevelTypeByDefault => false;
    }
}

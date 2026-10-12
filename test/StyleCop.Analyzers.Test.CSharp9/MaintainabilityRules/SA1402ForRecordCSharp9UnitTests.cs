// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.MaintainabilityRules
{
    using StyleCop.Analyzers.Test.MaintainabilityRules;

    public class SA1402ForRecordCSharp9UnitTests : SA1402ForBlockDeclarationUnitTestsBase
    {
        public override string Keyword => "record";

        protected override string SettingKeyword => "class";

        protected override bool IsConfiguredAsTopLevelTypeByDefault => true;
    }
}

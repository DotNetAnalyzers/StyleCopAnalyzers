// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp11.MaintainabilityRules
{
    using StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules;

    public partial class SA1119CSharp11UnitTests : SA1119CSharp10UnitTests
    {
        // In earlier Roslyn versions, we ended up with an extra space between the opening brace
        // and the identifier. Does not happen anymore.
        protected override string GetFixedCodeTestParenthesisInInterpolatedStringThatShouldBeRemoved()
        {
            return @"class Foo
{
    public void Bar()
    {
        bool flag = false;
        string data = $""{flag}"";
    }
}";
        }
    }
}

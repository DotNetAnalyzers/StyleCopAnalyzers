// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.Lightup
{
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.CSharp8.Lightup;
    using StyleCop.Analyzers.Test.Lightup;

    /// <summary>
    /// This class tests edge case behavior of <see cref="LightupHelpers"/> in Roslyn 3.8+. It extends
    /// <see cref="LightupHelpersUnitTests"/> since the tests defined there are valid in both environments without
    /// alteration.
    /// </summary>
    public partial class LightupHelpersCSharp9UnitTests : LightupHelpersCSharp8UnitTests
    {
    }
}

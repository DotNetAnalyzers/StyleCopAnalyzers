// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using StyleCop.Analyzers.LayoutRules;

    /// <summary>
    /// Unit tests for <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    /// <remarks>
    /// <para>The test cases can be found in the SA1502 subfolder.</para>
    /// </remarks>
    public partial class SA1502UnitTests
    {
        protected static string FormatTestCode(string testCode, string placeHolderReplacement)
        {
            return testCode.Replace("##PH##", placeHolderReplacement);
        }
    }
}

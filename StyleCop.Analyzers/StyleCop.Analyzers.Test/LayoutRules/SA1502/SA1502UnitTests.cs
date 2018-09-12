// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using StyleCop.Analyzers.LayoutRules;

    /// <summary>
    /// Unit tests for <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    /// <remarks>
    /// The test cases can be found in the SA1502 subfolder.
    /// </remarks>
    public partial class SA1502UnitTests
    {
        protected static string FormatTestCode(string testCode, string placeHolderReplacement)
        {
            return testCode.Replace("##PH##", placeHolderReplacement);
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    internal static class HelpLinkHelper
    {
        internal static string GetHelpLink(string diagnosticId)
        {
            return $"https://dotnetanalyzers.github.io/StyleCopAnalyzers/{diagnosticId}.html";
        }
    }
}

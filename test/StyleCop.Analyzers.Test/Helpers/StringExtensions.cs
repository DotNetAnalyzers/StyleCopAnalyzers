// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    public static class StringExtensions
    {
        public static string ReplaceLineEndings(this string input, string replacementText)
        {
            // First normalize to LF
            var lineFeedInput = input
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\f", "\n")
                .Replace("\x0085", "\n")
                .Replace("\x2028", "\n")
                .Replace("\x2029", "\n");

            // Then normalize to the replacement text
            return lineFeedInput.Replace("\n", replacementText);
        }
    }
}

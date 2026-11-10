// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers;

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Helpers for markup that marks compiler diagnostics, such as <c>{|CS1001:|}</c>.
/// </summary>
internal static class CompilerDiagnosticMarkup
{
    private static readonly Regex OpeningMarker = new Regex(@"\G\{\|([^:|{}]*):");
    private static readonly Regex CompilerDiagnosticId = new Regex(@"^CS[0-9]+$");

    /// <summary>
    /// Removes the markup for compiler diagnostics from a test source, keeping the code inside it and all other
    /// markup, such as <c>[|...|]</c>, <c>{|#0:...|}</c> and markup for analyzer diagnostics.
    /// </summary>
    /// <param name="source">The test source with markup.</param>
    /// <returns>The test source without markup for compiler diagnostics.</returns>
    internal static string Remove(string source)
    {
        var result = new StringBuilder(source.Length);

        // One entry per open {|...: marker, telling whether it marks a compiler diagnostic.
        var openMarkers = new Stack<bool>();

        int i = 0;
        while (i < source.Length)
        {
            var opening = OpeningMarker.Match(source, i);
            if (opening.Success)
            {
                bool isCompilerDiagnostic = CompilerDiagnosticId.IsMatch(opening.Groups[1].Value);
                openMarkers.Push(isCompilerDiagnostic);
                if (!isCompilerDiagnostic)
                {
                    result.Append(opening.Value);
                }

                i += opening.Length;
            }
            else if (openMarkers.Count > 0 && string.CompareOrdinal(source, i, "|}", 0, 2) == 0)
            {
                if (!openMarkers.Pop())
                {
                    result.Append("|}");
                }

                i += 2;
            }
            else
            {
                result.Append(source[i]);
                i++;
            }
        }

        return result.ToString();
    }
}

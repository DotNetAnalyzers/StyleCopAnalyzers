// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

/* Contributor: Tomasz Maczyński */

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class LiteralExpressionHelpers
    {
        private static readonly IDictionary<string, SyntaxKind> IntegerLiteralSuffixToLiteralSyntaxKind =
            new Dictionary<string, SyntaxKind>(StringComparer.OrdinalIgnoreCase)
            {
                { string.Empty, SyntaxKind.IntKeyword },
                { "L", SyntaxKind.LongKeyword },
                { "UL", SyntaxKind.ULongKeyword },
                { "LU", SyntaxKind.ULongKeyword },
                { "U", SyntaxKind.UIntKeyword },
                { "D", SyntaxKind.DoubleKeyword },
            };

        private static readonly IDictionary<string, SyntaxKind> RealLiteralSuffixToLiteralSyntaxKind =
            new Dictionary<string, SyntaxKind>(StringComparer.OrdinalIgnoreCase)
            {
                { "F", SyntaxKind.FloatKeyword },
                { "D", SyntaxKind.DoubleKeyword },
                { "M", SyntaxKind.DecimalKeyword }
            };

        private static readonly char[] LettersAllowedInIntegerLiteralSuffix =
            GetCharsFromKeysLowerAndUpperCase(IntegerLiteralSuffixToLiteralSyntaxKind);

        private static readonly char[] LettersAllowedInRealLiteralSuffix =
            GetCharsFromKeysLowerAndUpperCase(RealLiteralSuffixToLiteralSyntaxKind);

        private static readonly RegexOptions LiteralRegexOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
        private static readonly Regex IntegerBase10Regex = new Regex("^([0-9]*)(|u|l|ul|lu)$", LiteralRegexOptions, Regex.InfiniteMatchTimeout);
        private static readonly Regex IntegerBase16Regex = new Regex("^(0x)([0-9a-f]*)(|u|l|ul|lu)$", LiteralRegexOptions, Regex.InfiniteMatchTimeout);
        private static readonly Regex RealRegex = new Regex("^([0-9]*)(m|f|d)|([0-9]*)[.[0-9]*[e[0-9{1,2}]]([|m|f|d])]$", LiteralRegexOptions, Regex.InfiniteMatchTimeout);

        internal static SyntaxKind GetCorrespondingSyntaxKind(this LiteralExpressionSyntax literalExprssionSyntax)
        {
            var literalText = literalExprssionSyntax.Token.Text;
            var suffixStartIndex = SuffixStartIndex(literalText);

            var suffix = suffixStartIndex == -1 ?
                string.Empty :
                literalText.Substring(suffixStartIndex, length: literalText.Length - suffixStartIndex);
            return GetLiteralSyntaxKindBySuffix(suffix);
        }

        internal static string StripLiteralSuffix(this LiteralExpressionSyntax literalExpressionSyntax)
        {
            var literalText = literalExpressionSyntax.Token.Text;
            int suffixStartIndex = SuffixStartIndex(literalText);
            return suffixStartIndex == -1 ? literalText : literalText.Substring(0, suffixStartIndex);
        }

        private static SyntaxKind GetLiteralSyntaxKindBySuffix(string suffix)
        {
            SyntaxKind syntaxKind;
            if (IntegerLiteralSuffixToLiteralSyntaxKind.TryGetValue(suffix, out syntaxKind))
            {
                return syntaxKind;
            }
            else if (RealLiteralSuffixToLiteralSyntaxKind.TryGetValue(suffix, out syntaxKind))
            {
                return syntaxKind;
            }

            throw new ArgumentException($"There is no integer nor real numeric literal with suffix '{suffix}'.");
        }

        private static int SuffixStartIndex(string literalText)
        {
            int suffixStartIndex = -1;
            if (IsIntegerLiteral(literalText))
            {
                suffixStartIndex = literalText.IndexOfAny(LettersAllowedInIntegerLiteralSuffix);
            }
            else if (IsRealLiteral(literalText))
            {
                suffixStartIndex = literalText.IndexOfAny(LettersAllowedInRealLiteralSuffix);
            }

            return suffixStartIndex;
        }

        private static bool IsIntegerLiteral(string literal) =>
            IntegerBase10Regex.IsMatch(literal) || IntegerBase16Regex.IsMatch(literal);

        private static bool IsRealLiteral(string literal) =>
            RealRegex.IsMatch(literal);

        private static char[] GetCharsFromKeysLowerAndUpperCase(IDictionary<string, SyntaxKind> dict)
        {
            return dict.Keys
                    .SelectMany(s => s.ToCharArray()).Distinct()
                    .SelectMany(c => new[] { char.ToLowerInvariant(c), char.ToUpperInvariant(c) })
                    .ToArray();
        }
    }
}

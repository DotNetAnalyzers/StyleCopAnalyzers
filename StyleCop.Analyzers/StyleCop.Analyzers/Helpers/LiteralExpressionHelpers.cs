// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class LiteralExpressionHelpers
    {
        internal static string StripLiteralSuffix(this LiteralExpressionSyntax literalExpressionSyntax)
        {
            var literalText = literalExpressionSyntax.Token.Text;

            bool isBase16 = literalText.Length > 2 && (literalText[1] == 'x' || literalText[1] == 'X');

            for (int i = literalText.Length - 1; i >= 0; i--)
            {
                switch (literalText[i])
                {
                case 'L':
                case 'U':
                case 'M':
                case 'l':
                case 'u':
                case 'm':
                    continue;
                case 'D':
                case 'F':
                case 'd':
                case 'f':
                    if (isBase16)
                    {
                        goto default;
                    }
                    else
                    {
                        continue;
                    }

                default:
                    return literalText.Substring(0, i + 1);
                }
            }

            // If this is reached literalText does not contain a literal
            return string.Empty;
        }

        internal static LiteralExpressionSyntax WithLiteralSuffix(this LiteralExpressionSyntax literalExpression, SyntaxKind syntaxKindKeyword)
        {
            string textWithoutSuffix = literalExpression.StripLiteralSuffix();

            string suffix;
            switch (syntaxKindKeyword)
            {
            case SyntaxKind.UIntKeyword:
                suffix = "U";
                break;

            case SyntaxKind.ULongKeyword:
                suffix = "UL";
                break;

            case SyntaxKind.LongKeyword:
                suffix = "L";
                break;

            case SyntaxKind.FloatKeyword:
                suffix = "F";
                break;

            case SyntaxKind.DoubleKeyword:
                suffix = "D";
                break;

            case SyntaxKind.DecimalKeyword:
                suffix = "M";
                break;

            default:
                suffix = string.Empty;
                break;
            }

            return literalExpression.WithToken(SyntaxFactory.ParseToken(textWithoutSuffix + suffix).WithTriviaFrom(literalExpression.Token));
        }
    }
}

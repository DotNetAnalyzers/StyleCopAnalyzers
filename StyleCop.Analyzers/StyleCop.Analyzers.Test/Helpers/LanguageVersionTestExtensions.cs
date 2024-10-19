// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;

    internal static class LanguageVersionTestExtensions
    {
        public static LanguageVersion? OrLaterDefault(this LanguageVersion input)
        {
            // Use the default version instead, if that would be a later version than the one specified
            switch (input)
            {
            case LanguageVersionEx.CSharp7_1:
            case LanguageVersionEx.CSharp7_2:
            case LanguageVersionEx.CSharp7_3:
                return LightupHelpers.SupportsCSharp8 ? null : input;

            default:
                throw new ArgumentException($"Unexpected value {input}", nameof(input));
            }
        }
    }
}

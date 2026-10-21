// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;

    internal static class TestLanguageVersion
    {
        /// <summary>
        /// Gets the language version that tests use when they do not specify one, or <see langword="null"/> to use the
        /// compiler's default language version.
        /// </summary>
        /// <value>
        /// <see cref="LanguageVersionEx.Preview"/> for the C# 15 test project; otherwise, <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// <para>If needed, this property can be temporarily updated to default to a preview version.</para>
        /// </remarks>
        public static LanguageVersion? Default
        {
            get
            {
                if (LightupHelpers.SupportsCSharp15)
                {
                    // C# 15 is still in preview, so the C# 15 test project runs every test with the preview language
                    // version. Remove this once C# 15 is the default language version of the referenced compiler.
                    return LanguageVersionEx.Preview;
                }

                return null;
            }
        }
    }
}

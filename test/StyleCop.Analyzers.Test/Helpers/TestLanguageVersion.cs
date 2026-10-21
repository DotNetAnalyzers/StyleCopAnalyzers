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
                    // Some C# 15 features (e.g. the safe modifier) are still gated behind the preview language version
                    // in the referenced compiler, so the C# 15 test project runs every test with it.
                    // Remove this once an official C# 15 compiler is referenced.
                    return LanguageVersionEx.Preview;
                }

                return null;
            }
        }
    }
}

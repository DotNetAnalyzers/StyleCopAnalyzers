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
        /// Currently always <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// <para>If needed, this property can be temporarily updated to default to a preview version, for example
        /// <see cref="LanguageVersionEx.Preview"/> when <see cref="LightupHelpers"/> reports support for a language
        /// version that the referenced compiler still treats as preview.</para>
        /// </remarks>
        public static LanguageVersion? Default => null;
    }
}

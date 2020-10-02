// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using Xunit;

    public class SA1649CSharp9UnitTests : SA1649CSharp8UnitTests
    {
        public static IEnumerable<object[]> CSharp9TypeKeywords
        {
            get
            {
                foreach (var keyword in TypeKeywords)
                {
                    yield return keyword;
                }

                yield return new object[] { "record", LanguageVersion.CSharp9 };
            }
        }

        [Theory]
        [MemberData(nameof(CSharp9TypeKeywords))]
        public override async Task VerifyWrongFileNameAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            await base.VerifyWrongFileNameAsync(typeKeyword, languageVersion).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CSharp9TypeKeywords))]
        public override async Task VerifyWrongFileNameMultipleExtensionsAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            await base.VerifyWrongFileNameMultipleExtensionsAsync(typeKeyword, languageVersion).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CSharp9TypeKeywords))]
        public override async Task VerifyWrongFileNameNoExtensionAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            await base.VerifyWrongFileNameNoExtensionAsync(typeKeyword, languageVersion).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CSharp9TypeKeywords))]
        public override async Task VerifyCaseInsensitivityAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            await base.VerifyCaseInsensitivityAsync(typeKeyword, languageVersion).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CSharp9TypeKeywords))]
        public override async Task VerifyFirstTypeIsUsedAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            await base.VerifyFirstTypeIsUsedAsync(typeKeyword, languageVersion).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CSharp9TypeKeywords))]
        public override async Task VerifyThatPartialTypesAreIgnoredAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            await base.VerifyThatPartialTypesAreIgnoredAsync(typeKeyword, languageVersion).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CSharp9TypeKeywords))]
        public override async Task VerifyStyleCopNamingConventionForGenericTypeAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            await base.VerifyStyleCopNamingConventionForGenericTypeAsync(typeKeyword, languageVersion).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CSharp9TypeKeywords))]
        public override async Task VerifyMetadataNamingConventionForGenericTypeAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            await base.VerifyMetadataNamingConventionForGenericTypeAsync(typeKeyword, languageVersion).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CSharp9TypeKeywords))]
        public override async Task VerifyMetadataNamingConventionForGenericTypeMultipleExtensionsAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            await base.VerifyMetadataNamingConventionForGenericTypeMultipleExtensionsAsync(typeKeyword, languageVersion).ConfigureAwait(false);
        }
    }
}

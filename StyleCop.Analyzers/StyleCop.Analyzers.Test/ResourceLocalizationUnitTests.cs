// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Resources;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;

    public class ResourceLocalizationUnitTests
    {
        [Fact]
        public void TestRussianResourcesAreComplete()
        {
            var assembly = typeof(SA1000KeywordsMustBeSpacedCorrectly).Assembly;
            var culture = CultureInfo.GetCultureInfo("ru-RU");
            var resourceNames = assembly.GetManifestResourceNames()
                .Where(name => name.EndsWith(".resources", StringComparison.Ordinal))
                .ToArray();
            Assert.NotEmpty(resourceNames);

            foreach (var name in resourceNames)
            {
                var manager = new ResourceManager(name.Substring(0, name.Length - ".resources".Length), assembly);
                using (var neutral = manager.GetResourceSet(CultureInfo.InvariantCulture, true, false))
                using (var russian = manager.GetResourceSet(culture, true, false))
                {
                    Assert.NotNull(neutral);
                    Assert.NotNull(russian);
                    var neutralKeys = neutral.Cast<DictionaryEntry>().Select(entry => (string)entry.Key).OrderBy(key => key).ToArray();
                    var russianKeys = russian.Cast<DictionaryEntry>().Select(entry => (string)entry.Key).OrderBy(key => key).ToArray();
                    Assert.Equal(neutralKeys, russianKeys);
                    Assert.All(russian.Cast<DictionaryEntry>(), entry => Assert.IsType<string>(entry.Value));
                    Assert.Contains(russian.Cast<DictionaryEntry>(), entry => Assert.IsType<string>(entry.Value).Any(character => character >= '\u0400' && character <= '\u04ff'));
                }
            }
        }
    }
}

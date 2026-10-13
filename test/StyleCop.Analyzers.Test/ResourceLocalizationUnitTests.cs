// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test
{
    using System;
    using System.Collections;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Linq;
    using System.Resources;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;

    public class ResourceLocalizationUnitTests
    {
        [Theory]
        [InlineData(
            "int a , int b",
            "++ i",
            "Commas should not be preceded by whitespace",
            "逗号的前方 不应有空白字符",
            "Пробельные символы не должны находиться слева от запятой",
            "Increment symbol '++' should not be followed by a space",
            "递增符号“++”的后方不应有空格",
            "Пробел не должен находиться справа от символа инкремента '++'")]
        [InlineData(
            "int a,int b",
            "i --",
            "Commas should be followed by whitespace",
            "逗号的后方应有空白字符",
            "Пробельные символы должны находиться справа от запятой",
            "Decrement symbol '--' should not be preceded by a space",
            "递减符号“--”的前方不应有空格",
            "Пробел не должен находиться слева от символа декремента '--'")]
        [WorkItem(3792, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3792")]
        public async Task TestLocalizedSpacingMessageArgumentsAsync(
            string parameters,
            string expression,
            string englishComma,
            string chineseComma,
            string russianComma,
            string englishSymbol,
            string chineseSymbol,
            string russianSymbol)
        {
            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText($"class C {{ void M({parameters}) {{ int i = 0; {expression}; }} }}") },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
            var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(
                new SA1001CommasMustBeSpacedCorrectly(),
                new SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly());
            var diagnostics = await compilation.WithAnalyzers(analyzers).GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);
            var comma = Assert.Single(diagnostics, diagnostic => diagnostic.Id == "SA1001");
            var symbol = Assert.Single(diagnostics, diagnostic => diagnostic.Id == "SA1020");
            Assert.Equal(englishComma, comma.GetMessage(CultureInfo.GetCultureInfo("en-US")));
            Assert.Equal(chineseComma, comma.GetMessage(CultureInfo.GetCultureInfo("zh-CN")));
            Assert.Equal(russianComma, comma.GetMessage(CultureInfo.GetCultureInfo("ru-RU")));
            Assert.Equal(englishSymbol, symbol.GetMessage(CultureInfo.GetCultureInfo("en-US")));
            Assert.Equal(chineseSymbol, symbol.GetMessage(CultureInfo.GetCultureInfo("zh-CN")));
            Assert.Equal(russianSymbol, symbol.GetMessage(CultureInfo.GetCultureInfo("ru-RU")));
        }

        [Theory]
        [InlineData("ru-RU", '\u0400', '\u04ff')]
        [InlineData("zh-CN", '\u3400', '\u9fff')]
        public void TestLocalizedResourcesAreComplete(string cultureName, char firstCharacter, char lastCharacter)
        {
            var assembly = typeof(SA1000KeywordsMustBeSpacedCorrectly).Assembly;
            var culture = CultureInfo.GetCultureInfo(cultureName);
            var resourceNames = assembly.GetManifestResourceNames()
                .Where(name => name.EndsWith(".resources", StringComparison.Ordinal))
                .ToArray();
            Assert.NotEmpty(resourceNames);

            foreach (var name in resourceNames)
            {
                var manager = new ResourceManager(name.Substring(0, name.Length - ".resources".Length), assembly);
                using (var neutral = manager.GetResourceSet(CultureInfo.InvariantCulture, true, false))
                using (var localized = manager.GetResourceSet(culture, true, false))
                {
                    Assert.NotNull(neutral);
                    Assert.NotNull(localized);
                    var neutralKeys = neutral.Cast<DictionaryEntry>().Select(entry => (string)entry.Key).OrderBy(key => key).ToArray();
                    var localizedKeys = localized.Cast<DictionaryEntry>().Select(entry => (string)entry.Key).OrderBy(key => key).ToArray();
                    Assert.Equal(neutralKeys, localizedKeys);
                    Assert.All(localized.Cast<DictionaryEntry>(), entry => Assert.IsType<string>(entry.Value));
                    Assert.Contains(localized.Cast<DictionaryEntry>(), entry => Assert.IsType<string>(entry.Value).Any(character => character >= firstCharacter && character <= lastCharacter));
                }
            }
        }
    }
}

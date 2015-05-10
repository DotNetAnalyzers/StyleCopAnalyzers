namespace StyleCop.Analyzers.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using StyleCop.Analyzers.ReadabilityRules;
    using Xunit;

    public class ExportCodeFixProviderAttributeNameTest
    {
        public static IEnumerable<object[]> CodeFixProviderTypeData
        {
            get
            {
                var codeFixProviders = typeof(SA1110OpeningParenthesisMustBeOnDeclarationLine)
                    .Assembly
                    .GetTypes()
                    .Where(t => typeof(CodeFixProvider).IsAssignableFrom(t));

                return codeFixProviders.Select(x => new[] { x });
            }
        }

        [Theory]
        [MemberData(nameof(CodeFixProviderTypeData))]
        public void Test(Type codeFixProvider)
        {
            var exportCodeFixProviderAttribute = codeFixProvider.GetCustomAttributes<ExportCodeFixProviderAttribute>(false).FirstOrDefault();

            Assert.NotNull(exportCodeFixProviderAttribute);
            Assert.Equal(codeFixProvider.Name, exportCodeFixProviderAttribute.Name);
            Assert.Equal(1, exportCodeFixProviderAttribute.Languages.Length);
            Assert.Equal(LanguageNames.CSharp, exportCodeFixProviderAttribute.Languages[0]);
        }
    }
}
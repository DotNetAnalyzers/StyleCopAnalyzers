// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;

    public class ExportCodeFixProviderAttributeNameTest
    {
        public static IEnumerable<object[]> CodeFixProviderTypeData
        {
            get
            {
                var codeFixProviders = typeof(TokenSpacingCodeFixProvider)
                    .Assembly
                    .GetTypes()
                    .Where(t => typeof(CodeFixProvider).IsAssignableFrom(t));

                return codeFixProviders.Select(x => new[] { x });
            }
        }

        [Fact]
        public void TestSA1612CodeFixProviderIsRegistered()
        {
            var codeFixProviderType = typeof(TokenSpacingCodeFixProvider)
                .Assembly
                .GetTypes()
                .FirstOrDefault(t => t.Name == "SA1612CodeFixProvider");

            Assert.NotNull(codeFixProviderType);

            var codeFixProvider = (CodeFixProvider)Activator.CreateInstance(codeFixProviderType, nonPublic: true)!;
            Assert.Contains("SA1612", codeFixProvider.FixableDiagnosticIds);
        }

        [Fact]
        public void TestElementOrderCodeFixProviderIsExported()
        {
            var codeFixProviderType = typeof(StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider);

            Assert.Null(codeFixProviderType.GetCustomAttributes<NoCodeFixAttribute>(false).FirstOrDefault());
            Assert.NotNull(codeFixProviderType.GetCustomAttributes<ExportCodeFixProviderAttribute>(false).FirstOrDefault());
        }

        [Theory]
        [MemberData(nameof(CodeFixProviderTypeData))]
        public void TestExportCodeFixProviderAttribute(Type codeFixProvider)
        {
            var exportCodeFixProviderAttribute = codeFixProvider.GetCustomAttributes<ExportCodeFixProviderAttribute>(false).FirstOrDefault();
            var noCodeFixAttribute = codeFixProvider.GetCustomAttributes<NoCodeFixAttribute>(false).FirstOrDefault();

            if (noCodeFixAttribute != null)
            {
                Assert.Null(exportCodeFixProviderAttribute);

                return;
            }

            Assert.NotNull(exportCodeFixProviderAttribute);
            Assert.Equal(codeFixProvider.Name, exportCodeFixProviderAttribute.Name);
            Assert.Single(exportCodeFixProviderAttribute.Languages);
            Assert.Equal(LanguageNames.CSharp, exportCodeFixProviderAttribute.Languages[0]);
        }
    }
}

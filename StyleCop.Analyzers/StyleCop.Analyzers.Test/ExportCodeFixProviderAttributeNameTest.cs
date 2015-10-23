// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Analyzers.SpacingRules;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
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

        [Theory]
        [MemberData(nameof(CodeFixProviderTypeData))]
        public void TestExportCodeFixProviderAttribute(Type codeFixProvider)
        {
            var exportCodeFixProviderAttribute = codeFixProvider.GetCustomAttributes<ExportCodeFixProviderAttribute>(false).FirstOrDefault();

            Assert.NotNull(exportCodeFixProviderAttribute);
            Assert.Equal(codeFixProvider.Name, exportCodeFixProviderAttribute.Name);
            Assert.Equal(1, exportCodeFixProviderAttribute.Languages.Length);
            Assert.Equal(LanguageNames.CSharp, exportCodeFixProviderAttribute.Languages[0]);
        }
    }
}
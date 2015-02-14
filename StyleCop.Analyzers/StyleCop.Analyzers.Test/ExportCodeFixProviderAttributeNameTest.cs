using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.ReadabilityRules;

namespace StyleCop.Analyzers.Test
{
    [TestClass]
    public class ExportCodeFixProviderAttributeNameTest
    {
        [TestMethod]
        public void Test()
        {
            var issues = new List<string>();

            var codeFixProviders = typeof(SA1110OpeningParenthesisMustBeOnDeclarationLine)
                .Assembly
                .GetExportedTypes()
                .Where(t => typeof(CodeFixProvider).IsAssignableFrom(t))
                .ToList();

            foreach (var codeFixProvider in codeFixProviders)
            {
                var exportCodeFixProviderAttribute = codeFixProvider.GetCustomAttributes(false)
                    .OfType<ExportCodeFixProviderAttribute>()
                    .FirstOrDefault();
                if (exportCodeFixProviderAttribute == null)
                {
                    issues.Add(string.Format("{0} should have ExportCodeFixProviderAttribute attribute", codeFixProvider.Name));
                    continue;
                }

                if (!string.Equals(exportCodeFixProviderAttribute.Name, codeFixProvider.Name,
                        StringComparison.InvariantCulture))
                {
                    issues.Add(string.Format("Name parameter of ExportCodeFixProviderAttribute applied on {0} should be set to {0}", codeFixProvider.Name));
                }
            }

            if (issues.Any())
            {
                Assert.Fail(string.Join("\r\t", issues));
            }
        }
    }
}
// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Lightup
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class LanguageVersionExUnitTests
    {
        private static readonly Dictionary<LanguageVersion, string> LanguageVersionToName;
        private static readonly Dictionary<string, LanguageVersion> NameToLanguageVersion;

        static LanguageVersionExUnitTests()
        {
            LanguageVersionToName = new Dictionary<LanguageVersion, string>();
            NameToLanguageVersion = new Dictionary<string, LanguageVersion>();

            foreach (var field in typeof(LanguageVersion).GetTypeInfo().DeclaredFields)
            {
                if (!field.IsStatic)
                {
                    continue;
                }

                var value = (LanguageVersion)field.GetRawConstantValue();
                var name = field.Name;
                LanguageVersionToName.Add(value, name);
                NameToLanguageVersion.Add(name, value);
            }
        }

        public static IEnumerable<object[]> LanguageVersions
        {
            get
            {
                foreach (var field in typeof(LanguageVersionEx).GetTypeInfo().DeclaredFields)
                {
                    yield return new object[] { field.Name, (LanguageVersion)field.GetRawConstantValue() };
                }
            }
        }

        [Theory]
        [MemberData(nameof(LanguageVersions))]
        public void TestLanguageVersion(string name, LanguageVersion languageVersion)
        {
            if (LanguageVersionToName.TryGetValue(languageVersion, out var expectedName))
            {
                Assert.Equal(expectedName, name);
            }
            else
            {
                Assert.False(NameToLanguageVersion.TryGetValue(name, out _));
            }
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class SyntaxKindExTests
    {
        private static readonly Dictionary<SyntaxKind, string> SyntaxKindToName;
        private static readonly Dictionary<string, SyntaxKind> NameToSyntaxKind;

        static SyntaxKindExTests()
        {
            SyntaxKindToName = new Dictionary<SyntaxKind, string>();
            NameToSyntaxKind = new Dictionary<string, SyntaxKind>();

            foreach (var field in typeof(SyntaxKind).GetTypeInfo().DeclaredFields)
            {
                if (!field.IsStatic)
                {
                    continue;
                }

                var value = (SyntaxKind)field.GetRawConstantValue();
                var name = field.Name;
                SyntaxKindToName.Add(value, name);
                NameToSyntaxKind.Add(name, value);
            }
        }

        public static IEnumerable<object[]> SyntaxKinds
        {
            get
            {
                foreach (var field in typeof(SyntaxKindEx).GetTypeInfo().DeclaredFields)
                {
                    yield return new object[] { field.Name, (SyntaxKind)field.GetRawConstantValue() };
                }
            }
        }

        [Theory]
        [MemberData(nameof(SyntaxKinds))]
        public void TestSyntaxKind(string name, SyntaxKind syntaxKind)
        {
            if (SyntaxKindToName.TryGetValue(syntaxKind, out var expectedName))
            {
                Assert.Equal(expectedName, name);
            }
            else
            {
                Assert.False(NameToSyntaxKind.TryGetValue(name, out _));
            }
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class MethodKindExUnitTests
    {
        private static readonly Dictionary<MethodKind, string> MethodKindToName;
        private static readonly Dictionary<string, MethodKind> NameToMethodKind;

        static MethodKindExUnitTests()
        {
            MethodKindToName = new Dictionary<MethodKind, string>();
            NameToMethodKind = new Dictionary<string, MethodKind>();

            foreach (var field in typeof(MethodKind).GetTypeInfo().DeclaredFields)
            {
                if (!field.IsStatic)
                {
                    continue;
                }

                var rawValue = field.GetRawConstantValue();
                Assert.NotNull(rawValue);
                var value = (MethodKind)rawValue;
                var name = field.Name;
                if (!MethodKindToName.ContainsKey(value))
                {
                    MethodKindToName[value] = name;
                }

                NameToMethodKind.Add(name, value);
            }
        }

        public static IEnumerable<object[]> MethodKinds
        {
            get
            {
                foreach (var fieldInfo in typeof(MethodKindEx).GetTypeInfo().DeclaredFields)
                {
                    var rawValue = fieldInfo.GetRawConstantValue();
                    Assert.NotNull(rawValue);
                    yield return new object[] { fieldInfo.Name, (MethodKind)rawValue };
                }
            }
        }

        [Theory]
        [MemberData(nameof(MethodKinds))]
        public void TestMethodKind(string name, MethodKind methodKind)
        {
            if (MethodKindToName.TryGetValue(methodKind, out var expectedName))
            {
                Assert.Equal(expectedName, name);
            }
            else
            {
                Assert.False(NameToMethodKind.TryGetValue(name, out _));
            }
        }
    }
}

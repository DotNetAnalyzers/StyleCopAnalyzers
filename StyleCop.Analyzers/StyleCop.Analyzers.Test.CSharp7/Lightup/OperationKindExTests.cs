// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class OperationKindExTests
    {
        private static readonly Dictionary<OperationKind, string> OperationKindToName;
        private static readonly Dictionary<string, OperationKind> NameToOperationKind;

        static OperationKindExTests()
        {
            var renamedOperations =
                new Dictionary<string, string>()
                {
                    { "BinaryOperator", "Binary" },
                    { "ConstructorBodyOperation", "ConstructorBody" },
                    { "MethodBodyOperation", "MethodBody" },
                    { "TupleBinaryOperator", "TupleBinary" },
                    { "UnaryOperator", "Unary" },
                };

            OperationKindToName = new Dictionary<OperationKind, string>();
            NameToOperationKind = new Dictionary<string, OperationKind>();

            foreach (var field in typeof(OperationKind).GetTypeInfo().DeclaredFields)
            {
                if (!field.IsStatic)
                {
                    continue;
                }

                var value = (OperationKind)field.GetRawConstantValue();
                var name = field.Name;
                if (renamedOperations.TryGetValue(name, out var newName))
                {
                    name = newName;
                }

                if (!OperationKindToName.ContainsKey(value))
                {
                    OperationKindToName[value] = name;
                }

                if (!NameToOperationKind.ContainsKey(name))
                {
                    NameToOperationKind.Add(name, value);
                }
            }
        }

        public static IEnumerable<object[]> OperationKinds
        {
            get
            {
                foreach (var field in typeof(OperationKindEx).GetTypeInfo().DeclaredFields)
                {
                    yield return new object[] { field.Name, (OperationKind)field.GetRawConstantValue() };
                }
            }
        }

        [Theory]
        [MemberData(nameof(OperationKinds))]
        public void TestOperationKind(string name, OperationKind operationKind)
        {
            if (OperationKindToName.TryGetValue(operationKind, out var expectedName))
            {
                Assert.Equal(expectedName, name);
            }
            else
            {
                Assert.False(NameToOperationKind.TryGetValue(name, out _));
            }
        }
    }
}

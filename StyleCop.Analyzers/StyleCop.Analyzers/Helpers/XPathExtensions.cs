// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml.Linq;

    internal static class XPathExtensions
    {
        // This class borrows heavily from src/Compilers/Core/Portable/PortableShim.cs of Roslyn
        private static readonly Type XPathExtensionsType = GetTypeFromEither(
            contractName: "System.Xml.XPath.Extensions, System.Xml.XPath.XDocument, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = b03f5f7f11d50a3a",
            desktopName: "System.Xml.XPath.Extensions, System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

        internal static IEnumerable<XElement> XPathSelectElements(this XNode node, string expression)
        {
            var xpathSelectElements = XPathExtensionsType
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(XPathSelectElements), new[] { typeof(XNode), typeof(string) });

            return xpathSelectElements.Invoke(null, new object[] { node, expression }) as IEnumerable<XElement>;
        }

        private static Type GetTypeFromEither(string contractName, string desktopName)
        {
            var type = TryGetType(contractName);

            if (type == null)
            {
                type = TryGetType(desktopName);
            }

            return type;
        }

        private static Type TryGetType(string assemblyQualifiedName)
        {
            try
            {
                // Note that throwOnError=false only suppresses some exceptions, not all.
                return Type.GetType(assemblyQualifiedName, throwOnError: false);
            }
            catch
            {
                return null;
            }
        }

        private static MethodInfo GetDeclaredMethod(this TypeInfo typeInfo, string name, params Type[] paramTypes)
        {
            return FindItem(typeInfo.GetDeclaredMethods(name), paramTypes);
        }

        private static T FindItem<T>(IEnumerable<T> collection, params Type[] paramTypes)
             where T : MethodBase
        {
            foreach (var current in collection)
            {
                var p = current.GetParameters();
                if (p.Length != paramTypes.Length)
                {
                    continue;
                }

                bool allMatch = true;
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    if (p[i].ParameterType != paramTypes[i])
                    {
                        allMatch = false;
                        break;
                    }
                }

                if (allMatch)
                {
                    return current;
                }
            }

            return null;
        }
    }
}

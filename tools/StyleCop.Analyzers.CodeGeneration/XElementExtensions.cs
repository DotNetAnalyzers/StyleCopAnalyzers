// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.CodeGeneration
{
    using System;
    using System.Xml.Linq;

    internal static class XElementExtensions
    {
        public static XAttribute RequiredAttribute(this XElement element, XName name)
            => element.Attribute(name) ?? throw new InvalidOperationException($"Expected attribute '{name}'");
    }
}

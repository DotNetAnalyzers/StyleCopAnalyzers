// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Microsoft.CodeAnalysis;

    internal class TestXmlReferenceResolver : XmlReferenceResolver
    {
        public Dictionary<string, string> XmlReferences { get; } =
            new Dictionary<string, string>();

        public override bool Equals(object other)
        {
            return ReferenceEquals(this, other);
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }

        public override Stream OpenRead(string resolvedPath)
        {
            string content;
            if (!this.XmlReferences.TryGetValue(resolvedPath, out content))
            {
                return null;
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }

        public override string ResolveReference(string path, string baseFilePath)
        {
            return path;
        }
    }
}

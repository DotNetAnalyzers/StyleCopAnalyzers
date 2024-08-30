// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    internal class XmlComment
    {
        internal XmlComment(string[] documentedParameterNames, bool hasInheritdoc)
        {
            this.DocumentedParameterNames = documentedParameterNames;
            this.HasInheritdoc = hasInheritdoc;
        }

        internal XmlComment()
        {
        }

        internal static XmlComment MissingSummary => new() { IsMissing = true };

        internal bool IsMissing { get; private set; }

        internal bool HasInheritdoc { get; private set; }

        internal IEnumerable<string> DocumentedParameterNames { get; private set; } = Enumerable.Empty<string>();
    }
}

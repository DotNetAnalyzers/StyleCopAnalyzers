// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using System.Collections.Generic;
    using StyleCop.Analyzers.Lightup;

    public static class CommonMemberData
    {
        public static IEnumerable<object[]> TypeDeclarationKeywords
        {
            get
            {
                yield return new[] { "class" };
                yield return new[] { "struct" };
                yield return new[] { "interface" };
                if (LightupHelpers.SupportsCSharp9)
                {
                    yield return new[] { "record" };
                }
            }
        }

        public static IEnumerable<object[]> BaseTypeDeclarationKeywords
        {
            get
            {
                foreach (var keyword in TypeDeclarationKeywords)
                {
                    yield return keyword;
                }

                yield return new[] { "enum" };
            }
        }
    }
}

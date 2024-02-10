// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using StyleCop.Analyzers.Lightup;

    public static class CommonMemberData
    {
        public static IEnumerable<object[]> DataTypeDeclarationKeywords
        {
            get
            {
                yield return new[] { "class" };
                yield return new[] { "struct" };

                if (LightupHelpers.SupportsCSharp9)
                {
                    yield return new[] { "record" };
                }

                if (LightupHelpers.SupportsCSharp10)
                {
                    yield return new[] { "record class" };
                    yield return new[] { "record struct" };
                }
            }
        }

        public static IEnumerable<object[]> ReferenceTypeDeclarationKeywords
        {
            get
            {
                yield return new[] { "class" };

                if (LightupHelpers.SupportsCSharp9)
                {
                    yield return new[] { "record" };
                }

                if (LightupHelpers.SupportsCSharp10)
                {
                    yield return new[] { "record class" };
                }
            }
        }

        public static IEnumerable<object[]> ValueTypeDeclarationKeywords
        {
            get
            {
                yield return new[] { "struct" };

                if (LightupHelpers.SupportsCSharp10)
                {
                    yield return new[] { "record struct" };
                }
            }
        }

        public static IEnumerable<object[]> RecordTypeDeclarationKeywords
        {
            get
            {
                if (LightupHelpers.SupportsCSharp9)
                {
                    yield return new[] { "record" };
                }

                if (LightupHelpers.SupportsCSharp10)
                {
                    yield return new[] { "record class" };
                    yield return new[] { "record struct" };
                }
            }
        }

        public static IEnumerable<object[]> TypeDeclarationKeywords
        {
            get
            {
                return DataTypeDeclarationKeywords
                    .Concat(new[] { new[] { "interface" } });
            }
        }

        public static IEnumerable<object[]> BaseTypeDeclarationKeywords
        {
            get
            {
                return TypeDeclarationKeywords
                    .Concat(new[] { new[] { "enum" } });
            }
        }

        public static IEnumerable<object[]> AllTypeDeclarationKeywords
        {
            get
            {
                return BaseTypeDeclarationKeywords
                    .Concat(new[] { new[] { "delegate" } });
            }
        }

        public static IEnumerable<object[]> GenericTypeDeclarationKeywords
        {
            get
            {
                return TypeDeclarationKeywords
                    .Concat(new[] { new[] { "delegate" } });
            }
        }

        public static IEnumerable<object[]> TypeKeywordsWhichSupportPrimaryConstructors
        {
            get
            {
                if (LightupHelpers.SupportsCSharp9)
                {
                    yield return new[] { "record" };
                }

                if (LightupHelpers.SupportsCSharp10)
                {
                    yield return new[] { "record class" };
                    yield return new[] { "record struct" };
                }

                if (LightupHelpers.SupportsCSharp12)
                {
                    yield return new[] { "class" };
                    yield return new[] { "struct" };
                }
            }
        }
    }
}

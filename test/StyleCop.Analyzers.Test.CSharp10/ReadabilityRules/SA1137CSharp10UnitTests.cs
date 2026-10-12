// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1137ElementsShouldHaveTheSameIndentation,
        StyleCop.Analyzers.ReadabilityRules.IndentationCodeFixProvider>;

    public partial class SA1137CSharp10UnitTests : SA1137CSharp9UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestFileScopedNamespaceDeclarationAsync(string baseTypeKind)
        {
            await new CSharpTest
            {
                TestSources =
                {
                    $@"
using System;

namespace Namespace0;

    [My] [My] {baseTypeKind} TypeName {{ }}
",
                    $@"
using System;

namespace Namespace1;

    [My]
[|  |][My] {baseTypeKind} TypeName {{ }}
",
                    $@"
using System;

namespace Namespace2;

  [My]
[|    |][My]
  {baseTypeKind} TypeName {{ }}
",
                    $@"
using System;

namespace Namespace3;

[|    |][My]
[|    |][My]
  {baseTypeKind} TypeName {{ }}
",
                    $@"
using System;

namespace Namespace4;

    {baseTypeKind} TypeName1 {{ }}

[|  |][My] {baseTypeKind} TypeName2 {{ }}
",
                    $@"
using System;

namespace Namespace5;

    {baseTypeKind} TypeName1 {{ }}

    [My]
[|  |][My] {baseTypeKind} TypeName2 {{ }}
",
                    $@"
using System;

namespace Namespace6;

    {baseTypeKind} TypeName1 {{ }}

[|  |][My]
    [My] {baseTypeKind} TypeName2 {{ }}
",
                    $@"
using System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute {{ }}
",
                },
                FixedSources =
                {
                    $@"
using System;

namespace Namespace0;

    [My] [My] {baseTypeKind} TypeName {{ }}
",
                    $@"
using System;

namespace Namespace1;

    [My]
    [My] {baseTypeKind} TypeName {{ }}
",
                    $@"
using System;

namespace Namespace2;

  [My]
  [My]
  {baseTypeKind} TypeName {{ }}
",
                    $@"
using System;

namespace Namespace3;

  [My]
  [My]
  {baseTypeKind} TypeName {{ }}
",
                    $@"
using System;

namespace Namespace4;

    {baseTypeKind} TypeName1 {{ }}

    [My] {baseTypeKind} TypeName2 {{ }}
",
                    $@"
using System;

namespace Namespace5;

    {baseTypeKind} TypeName1 {{ }}

    [My]
    [My] {baseTypeKind} TypeName2 {{ }}
",
                    $@"
using System;

namespace Namespace6;

    {baseTypeKind} TypeName1 {{ }}

    [My]
    [My] {baseTypeKind} TypeName2 {{ }}
",
                    $@"
using System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
class MyAttribute : Attribute {{ }}
",
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
using StyleCop.Analyzers.Test.Helpers;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.ReadabilityRules.SA1143ConditionalExpressionsMustBeFormattedConsistently,
    StyleCop.Analyzers.ReadabilityRules.SA1143CodeFixProvider>;

/// <summary>
/// Tests formatting of conditional expressions with C# 9 syntax.
/// </summary>
public partial class SA1143CSharp9UnitTests : SA1143CSharp8UnitTests
{
    /// <summary>
    /// Verifies that target-typed object creation can span multiple lines in a conditional branch.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestMultilineTargetTypedObjectCreationAsync()
    {
        const string source = """
            #nullable enable
            class C
            {
                Product? Value { get; set; }

                void M(bool condition)
                {
                    Value = condition
                        ? null
                        : new()
                        {
                            First = 1,
                            Second = 2,
                            Third = 3,
                            Fourth = 4,
                        };
                }
            }

            class Product
            {
                public int First { get; set; }

                public int Second { get; set; }

                public int Third { get; set; }

                public int Fourth { get; set; }
            }
            """;

        await VerifyCSharpDiagnosticAsync(source, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.OrderingRules;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.CSharp10.OrderingRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.OrderingRules.SA1200UsingDirectivesMustBePlacedCorrectly,
    StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

public partial class SA1200CSharp12ExternAliasUnitTests : SA1200CSharp10UnitTests
{
    [Fact]
    [WorkItem(1931, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1931")]
    public async Task TestExternAliasUsedByGlobalUsingWithTypeTargetRemainsOutsideNamespaceAsync()
    {
        var testCode = @"extern alias corlib;
global using Items = corlib::System.String[];
global using Pair = (corlib::System.Int32, corlib::System.String);

namespace TestNamespace
{
}
";

        var test = new CSharpTest
        {
            TestState =
            {
                Sources = { testCode },
            },
        };
        test.SolutionTransforms.Add((solution, projectId) =>
        {
            var project = solution.GetProject(projectId) ?? throw new InvalidOperationException();
            var systemRuntime = project.MetadataReferences.OfType<PortableExecutableReference>()
                .Single(reference => Path.GetFileName(reference.FilePath) == "System.Runtime.dll");

            return solution
                .RemoveMetadataReference(projectId, systemRuntime)
                .AddMetadataReference(projectId, systemRuntime.WithAliases(new[] { "global", "corlib" }));
        });

        await test.RunAsync().ConfigureAwait(false);
    }
}

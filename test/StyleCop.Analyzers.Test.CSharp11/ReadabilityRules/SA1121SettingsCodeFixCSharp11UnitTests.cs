// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules;

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.ReadabilityRules.SA1121UseBuiltInTypeAlias,
    StyleCop.Analyzers.ReadabilityRules.SA1121MultiTargetsLegacyFrameworksCodeFixProvider>;

public class SA1121SettingsCodeFixCSharp11UnitTests
{
    private const string Source = @"
using System;

class TestClass
{
    private static int Size => [|IntPtr|].Size;
}";

    private const string LinkedSource = @"
using System;

class TestClass
{
    private static int Size => IntPtr.Size;
}";

    [Fact]
    public async Task TestStyleCopJsonIsUpdatedAsync()
    {
        var sharedPath = Path.Combine(Path.GetTempPath(), "SA1121.Settings.Shared.cs");
        var test = CreateTest(sharedPath);
        test.Settings = "{ \"settings\": { } }";
        test.FixedAdditionalFiles.Add(("stylecop.json", "{ \"settings\": {\n  \"compatibility\": {\n    \"multiTargetsLegacyFrameworks\": true\n  }\n} }"));
        await test.RunAsync().ConfigureAwait(false);
    }

    private static CSharpTest CreateTest(string sharedPath)
    {
        var test = new CSharpTest
        {
            TestBehaviors = TestBehaviors.SkipSuppressionCheck,
            TestState =
            {
                Sources = { (sharedPath, Source) },
                AdditionalProjects =
                {
                    ["Linked"] =
                    {
                        ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
                        Sources = { (sharedPath, LinkedSource) },
                    },
                },
            },
        };
        test.FixedState.Sources.Add((sharedPath, Source.Replace("[|", string.Empty).Replace("|]", string.Empty)));
        return test;
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.OrderingRules;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.SpecialRules;
using StyleCop.Analyzers.Test.CSharp7.OrderingRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder,
    StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

public partial class SA1201CSharp8UnitTests : SA1201CSharp7UnitTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task TestMemberKindOrderFromEditorConfigAsync(bool jsonOverrides)
    {
        var test = new CSharpTest
        {
            TestCode = """
                public class C
                {
                    public int P { get; set; }

                    public void {|#0:M|}() { }
                }
                """,
            FixedCode = """
                public class C
                {
                    public void M() { }

                    public int P { get; set; }
                }
                """,
            Settings = jsonOverrides
                ? """{ "settings": { "orderingRules": { "memberKindOrder": ["method", "property"] } } }"""
                : null,
        };
        var kinds = jsonOverrides ? "property, method" : "method, property";
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", $"""
            root = true

            [*]
            stylecop.ordering.memberKindOrder = {kinds}
            """));
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithArguments("A method", "a property"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestEmptyMemberKindOrderFromEditorConfigAsync()
    {
        var test = new CSharpTest
        {
            TestCode = """
                public class C
                {
                    public void M() { }
                    public int P { get; set; }
                    public int F;
                }
                """,
        };
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", """
            root = true

            [*]
            stylecop.ordering.memberKindOrder =
            """));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestInvalidAccessibilityOrderFromEditorConfigAsync()
    {
        var test = new InvalidConfigurationTest { TestCode = "public class C { }" };
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", """
            root = true

            [*]
            stylecop.ordering.accessibilityOrder = private, privateProtected, protected, protectedInternal, internal, public, typo
            """));
        test.ExpectedDiagnostics.Add(new DiagnosticResult(new SA0002InvalidSettingsFile().SupportedDiagnostics[0])
            .WithArguments("accessibilityOrder cannot contain value 'typo'."));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestInvalidAccessibilityOrderInLaterFileAsync()
    {
        var test = new InvalidConfigurationTest();
        test.TestState.Sources.Add(("/0/First.cs", "public class First { }"));
        test.TestState.Sources.Add(("/0/Second.cs", "public class Second { }"));
        test.TestState.Sources.Add(("/0/Third.cs", "public class Third { }"));
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", """
            root = true

            [First.cs]
            stylecop.ordering.memberKindOrder = method
            stylecop.ordering.accessibilityOrder = public, internal, protectedInternal, protected, privateProtected, private

            [Second.cs]
            stylecop.ordering.memberKindOrder = method
            stylecop.ordering.accessibilityOrder = public, internal, protectedInternal, protected, privateProtected, private

            [Third.cs]
            stylecop.ordering.memberKindOrder = method
            stylecop.ordering.accessibilityOrder = public, internal, protectedInternal, protected
            """));
        test.ExpectedDiagnostics.Add(new DiagnosticResult(new SA0002InvalidSettingsFile().SupportedDiagnostics[0])
            .WithArguments("accessibilityOrder must contain each access level exactly once."));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Theory]
    [InlineData("unknown", "memberKindOrder cannot contain kind 'unknown'")]
    [InlineData("property, PROPERTY", "memberKindOrder cannot contain duplicate kind 'PROPERTY'")]
    public async Task TestInvalidMemberKindOrderFromEditorConfigAsync(string value, string error)
    {
        var test = new InvalidConfigurationTest { TestCode = "public class C { }" };
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", $"""
            root = true

            [*]
            stylecop.ordering.memberKindOrder = {value}
            """));
        test.ExpectedDiagnostics.Add(new DiagnosticResult(new SA0002InvalidSettingsFile().SupportedDiagnostics[0]).WithArguments(error));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Theory]
    [InlineData("unknown", "memberKindOrder cannot contain kind 'unknown'")]
    [InlineData("property, PROPERTY", "memberKindOrder cannot contain duplicate kind 'PROPERTY'")]
    public async Task TestInvalidMemberKindOrderInLaterFileAsync(string value, string error)
    {
        var test = new InvalidConfigurationTest();
        test.TestState.Sources.Add(("/0/First.cs", "public class First { }"));
        test.TestState.Sources.Add(("/0/Second.cs", "public class Second { }"));
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", $"""
            root = true

            [Second.cs]
            stylecop.ordering.memberKindOrder = {value}
            """));
        test.ExpectedDiagnostics.Add(new DiagnosticResult(new SA0002InvalidSettingsFile().SupportedDiagnostics[0])
            .WithArguments(error));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Theory]
    [InlineData("unset", false)]
    [InlineData("", false)]
    [InlineData("method, property", false)]
    [InlineData("unknown", true)]
    public async Task TestValidMemberKindOrderInLaterFileAsync(string value, bool jsonOverrides)
    {
        var test = new InvalidConfigurationTest
        {
            Settings = jsonOverrides ? """{ "settings": { "orderingRules": { "memberKindOrder": ["property"] } } }""" : null,
        };
        test.TestState.Sources.Add(("/0/First.cs", "public class First { }"));
        test.TestState.Sources.Add(("/0/Second.cs", "public class Second { }"));
        test.TestState.Sources.Add(("/0/Third.cs", "public class Third { }"));
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", $"""
            root = true

            [Second.cs]
            stylecop.ordering.memberKindOrder = {value}

            [Third.cs]
            stylecop.ordering.memberKindOrder = {value}
            """));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    private sealed class InvalidConfigurationTest : CSharpTest
    {
        protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
        {
            return new DiagnosticAnalyzer[]
            {
                new StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder(),
                new SA0002InvalidSettingsFile(),
            };
        }
    }
}

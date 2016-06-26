// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.ReadabilityRules;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.Helpers;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for attributes part of <see cref="SA1117ParametersMustBeOnSameLineOrSeparateLines"/>.
    /// </summary>
    public partial class SA1117UnitTests
    {
        private AttributeArgumentSplitting? attributeArgumentSplitting;

        public static IEnumerable GetAttributeTestScenarios()
        {
            return new AttributeTestScenarios().GetTestData();
        }

        [Theory]
        [MemberData(nameof(GetAttributeTestScenarios))]
        internal async Task TestAttributeScenarioAsync(
            AttributeArgumentSplitting? splitting,
            TestScenario<AttributeArgumentSplitting?> scenario)
        {
            this.attributeArgumentSplitting = splitting;

            var expectedViolations = scenario.ExpectedViolations.Where(s => s.Setting == (splitting ?? AttributeArgumentSplitting.Default)).ToList();
            DiagnosticResult[] expected = (expectedViolations.Count == 0) ?
                EmptyDiagnosticResults :
                expectedViolations.Select(v => this.CSharpDiagnostic().WithLocation(v.Line, v.Column)).ToArray();

            await this.VerifyCSharpDiagnosticAsync(scenario.TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override string GetSettings()
        {
            if (this.attributeArgumentSplitting == null)
            {
                return base.GetSettings();
            }

            return $@"
{{
  ""settings"": {{
    ""readabilityRules"": {{
      ""attributeArgumentSplitting"": ""{this.attributeArgumentSplitting.ToString().ToLowerInvariant()}""
    }}
  }}
}}";
        }

        private sealed class AttributeTestScenarios : TestScenarios<AttributeArgumentSplitting?>
        {
            private readonly string attributeDeclaration = @"
[System.AttributeUsage(System.AttributeTargets.Class)]
public class MyAttribute : System.Attribute
{
    public MyAttribute(int a, int b, int c)
    {
    }

    public int D { get; set; }

    public int E { get; set; }
}
";

            private readonly Func<string, string> displayNameFunction;

            internal AttributeTestScenarios()
            {
                this.displayNameFunction = c => c.Replace(this.attributeDeclaration, string.Empty);
            }

            protected override IEnumerable<TestScenario<AttributeArgumentSplitting?>> Scenarios
            {
                get
                {
                    yield return this.CreateCustomAttributeScenario(@"
[MyAttribute(1, 2, 3)]
class Foo
{
}");

                    yield return new TestScenario<AttributeArgumentSplitting?>(@"
// This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1211
[System.Obsolete]
class ObsoleteType
{
}");

                    yield return this.CreateCustomAttributeScenario(
@"
[MyAttribute(
    1,
    2, 3)]
class Foo
{
}",
                        new ExpectedViolation<AttributeArgumentSplitting?>(AttributeArgumentSplitting.Default, 16, 8),
                        new ExpectedViolation<AttributeArgumentSplitting?>(AttributeArgumentSplitting.PositionalParametersMayShareFirstLine, 16, 8));

                    yield return this.CreateCustomAttributeScenario(
@"
[MyAttribute(1, 2, 3, D = 4, E = 5)]
class Foo
{
}");

                    yield return this.CreateCustomAttributeScenario(
@"
[MyAttribute(
    1,
    2,
    3,
    D = 4,
    E = 5)]
class Foo
{
}");

                yield return this.CreateCustomAttributeScenario(
@"
[MyAttribute(1, 2,
    3,
    D = 4,
    E = 5)]
class Foo
{
}",
                        new ExpectedViolation<AttributeArgumentSplitting?>(AttributeArgumentSplitting.Default, 14, 17),
                        new ExpectedViolation<AttributeArgumentSplitting?>(AttributeArgumentSplitting.PositionalParametersMayShareFirstLine, 14, 17));

                yield return this.CreateCustomAttributeScenario(
@"
[MyAttribute(1, 2, 3,
    D = 4,
    E = 5)]
class Foo
{
}",
                        new ExpectedViolation<AttributeArgumentSplitting?>(AttributeArgumentSplitting.Default, 14, 17));

                    yield return this.CreateCustomAttributeScenario(
@"
[MyAttribute(1, 2, 3,
    D = 4, E = 5)]
class Foo
{
}",
                        new ExpectedViolation<AttributeArgumentSplitting?>(AttributeArgumentSplitting.Default, 14, 17),
                        new ExpectedViolation<AttributeArgumentSplitting?>(AttributeArgumentSplitting.PositionalParametersMayShareFirstLine, 15, 12));
                }
    }

            protected override IEnumerable<AttributeArgumentSplitting?> SettingsValues
            {
                get
                {
                    return new AttributeArgumentSplitting?[]
                        {
                            null,
                            AttributeArgumentSplitting.Default,
                            AttributeArgumentSplitting.Ignore,
                            AttributeArgumentSplitting.PositionalParametersMayShareFirstLine
                        };
                }
            }

            private TestScenario<AttributeArgumentSplitting?> CreateCustomAttributeScenario(
                string testCode,
                params ExpectedViolation<AttributeArgumentSplitting?>[] expectedViolations)
            {
                return new TestScenario<AttributeArgumentSplitting?>(
                    this.attributeDeclaration + testCode,
                    this.displayNameFunction,
                    expectedViolations);
            }
        }
    }
}

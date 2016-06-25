// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
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
        private AttributeParameterSplitting? attributeParameterSplitting;

        public static IEnumerable GetAttributeTestScenarios()
        {
            return new AttributeTestScenarios().GetTestData();
        }

        [Theory]
        [MemberData(nameof(GetAttributeTestScenarios))]
        internal async Task TestAttributeScenarioAsync(
            AttributeParameterSplitting? settingValue,
            TestScenario<AttributeParameterSplitting?> scenario)
        {
            this.attributeParameterSplitting = settingValue;

            var expectedViolations = scenario.ExpectedViolations.Where(s => s.Setting == (settingValue ?? AttributeParameterSplitting.Default)).ToList();
            DiagnosticResult[] expected = (expectedViolations.Count == 0) ?
                EmptyDiagnosticResults :
                expectedViolations.Select(v => this.CSharpDiagnostic().WithLocation(v.Line, v.Column)).ToArray();

            await this.VerifyCSharpDiagnosticAsync(scenario.TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override string GetSettings()
        {
            if (this.attributeParameterSplitting == null)
            {
                return base.GetSettings();
            }

            return $@"
{{
  ""settings"": {{
    ""readabilityRules"": {{
      ""attributeParameterSplitting"": ""{this.attributeParameterSplitting.ToString().ToLowerInvariant()}""
    }}
  }}
}}";
        }

        private sealed class AttributeTestScenarios : TestScenarios<AttributeParameterSplitting?>
        {
            internal AttributeTestScenarios()
            {
            }

            protected override IEnumerable<TestScenario<AttributeParameterSplitting?>> Scenarios
            {
                get
                {
                    var attributeDeclaration = @"
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

                    yield return new TestScenario<AttributeParameterSplitting?>(attributeDeclaration + @"
[MyAttribute(1, 2, 3)]
class Foo
{
}");

                    yield return new TestScenario<AttributeParameterSplitting?>(@"
// This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1211
[System.Obsolete]
class ObsoleteType
{
}");

                    yield return new TestScenario<AttributeParameterSplitting?>(
                        attributeDeclaration + @"
[MyAttribute(
    1,
    2, 3)]
class Foo
{
}",
                        new ExpectedViolation<AttributeParameterSplitting?>(AttributeParameterSplitting.Default, 16, 8),
                        new ExpectedViolation<AttributeParameterSplitting?>(AttributeParameterSplitting.PositionalParametersMayShareFirstLine, 16, 8));

                    yield return new TestScenario<AttributeParameterSplitting?>(
                        attributeDeclaration + @"
[MyAttribute(1, 2, 3, D = 4, E = 5)]
class Foo
{
}");

                    yield return new TestScenario<AttributeParameterSplitting?>(
                        attributeDeclaration + @"
[MyAttribute(
    1,
    2,
    3,
    D = 4,
    E = 5)]
class Foo
{
}");

                yield return new TestScenario<AttributeParameterSplitting?>(
                        attributeDeclaration + @"
[MyAttribute(1, 2,
    3,
    D = 4,
    E = 5)]
class Foo
{
}",
                        new ExpectedViolation<AttributeParameterSplitting?>(AttributeParameterSplitting.Default, 15, 5),
                        new ExpectedViolation<AttributeParameterSplitting?>(AttributeParameterSplitting.PositionalParametersMayShareFirstLine, 15, 5));

                yield return new TestScenario<AttributeParameterSplitting?>(
                        attributeDeclaration + @"
[MyAttribute(1, 2, 3,
    D = 4,
    E = 5)]
class Foo
{
}",
                        new ExpectedViolation<AttributeParameterSplitting?>(AttributeParameterSplitting.Default, 15, 5));

                    yield return new TestScenario<AttributeParameterSplitting?>(
                        attributeDeclaration + @"
[MyAttribute(1, 2, 3,
    D = 4, E = 5)]
class Foo
{
}",
                        new ExpectedViolation<AttributeParameterSplitting?>(AttributeParameterSplitting.Default, 15, 5),
                        new ExpectedViolation<AttributeParameterSplitting?>(AttributeParameterSplitting.PositionalParametersMayShareFirstLine, 15, 12));
                }
    }

            protected override IEnumerable<AttributeParameterSplitting?> SettingsValues
            {
                get
                {
                    return new AttributeParameterSplitting?[]
                        {
                            null,
                            AttributeParameterSplitting.Default,
                            AttributeParameterSplitting.Ignore,
                            AttributeParameterSplitting.PositionalParametersMayShareFirstLine
                        };
                }
            }
        }
    }
}

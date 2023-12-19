// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp7.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1202ElementsMustBeOrderedByAccess,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public partial class SA1202CSharp8UnitTests : SA1202CSharp7UnitTests
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle property access levels.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertiesOfInterfaceAsync()
        {
            var testCode = @"public interface TestClass
{
    private string TestProperty1 { get { return """"; } set { } }
    protected string {|#0:TestProperty2|} { get; set; }
    protected internal string {|#1:TestProperty3|} { get; set; }
    internal string {|#2:TestProperty4|} { get; set; }
    public string {|#3:TestProperty5|} { get; set; }
    string TestProperty0 { get; set; }
}
";

            var fixedCode = @"public interface TestClass
{
    public string TestProperty5 { get; set; }
    string TestProperty0 { get; set; }
    internal string TestProperty4 { get; set; }
    protected internal string TestProperty3 { get; set; }
    protected string TestProperty2 { get; set; }
    private string TestProperty1 { get { return """"; } set { } }
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0).WithArguments("protected", "private"),
                    Diagnostic().WithLocation(1).WithArguments("protected internal", "protected"),
                    Diagnostic().WithLocation(2).WithArguments("internal", "protected internal"),
                    Diagnostic().WithLocation(3).WithArguments("public", "internal"),
                },
                FixedCode = fixedCode,
                NumberOfIncrementalIterations = 5,
                NumberOfFixAllIterations = 2,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}

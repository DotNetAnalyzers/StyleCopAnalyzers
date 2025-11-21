// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.DocumentationRules;
    using Xunit;

    public partial class SA1648CSharp11UnitTests : SA1648CSharp10UnitTests
    {
        [WorkItem(3595, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3595")]
        [Theory]
        [InlineData("abstract void TestMethod();", "public void TestMethod() {}")]
        [InlineData("abstract void TestMethod();", "void TestInterface.TestMethod() {}")]
        [InlineData("virtual void TestMethod() {}", "public void TestMethod() {}")]
        [InlineData("virtual void TestMethod() {}", "void TestInterface.TestMethod() {}")]
        [InlineData("abstract int TestProperty { get; set; }", "public int TestProperty { get; set; }")]
        [InlineData("abstract int TestProperty { get; set; }", "int TestInterface.TestProperty { get; set; }")]
        [InlineData("virtual int TestProperty { get; set; }", "public int TestProperty { get; set; }")]
        [InlineData("virtual int TestProperty { get; set; }", "int TestInterface.TestProperty { get; set; }")]
        [InlineData("abstract event System.Action TestEvent;", "public event System.Action TestEvent;")]
        [InlineData("abstract event System.Action TestEvent;", "event System.Action TestInterface.TestEvent { add {} remove {} }")]
        [InlineData("virtual event System.Action TestEvent;", "public event System.Action TestEvent;")]
        [InlineData("virtual event System.Action TestEvent;", "event System.Action TestInterface.TestEvent { add {} remove {} }")]
        public async Task TestCorrectMemberInheritDocFromStaticAbstractOrVirtualMemberInInterfaceAsync(string interfaceMember, string classMember)
        {
            var testCode = $@"
public interface TestInterface
{{
    /// <summary>
    /// A summary text.
    /// </summary>
    static {interfaceMember}
}}

public class TestClass : TestInterface
{{
    /// <inheritdoc />
    static {classMember}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.DocumentationRules
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.DocumentationRules;
    using Xunit;

    public partial class SA1648CSharp8UnitTests : SA1648CSharp7UnitTests
    {
        [WorkItem(3595, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3595")]
        [Theory]
        [InlineData("void TestMethod() {}")]
        [InlineData("int TestProperty { get; set; }")]
        [InlineData("event System.Action TestEvent;")]
        public async Task TestIncorrectMemberInheritDocFromStaticMemberInInterfaceAsync(string member)
        {
            var testCode = $@"
public interface TestInterface
{{
    /// <summary>
    /// A summary text.
    /// </summary>
    static {member}
}}

public class TestClass : TestInterface
{{
    /// [|<inheritdoc />|]
    public static {member}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceStaticConstructorAsync()
        {
            var testCode = @"
/// <summary>
/// An interface.
/// </summary>
public interface ITest
{
    /// <summary>
    /// Initializes static members of the <see cref=""ITest""/> interface.
    /// </summary>
    static ITest()
    {
    }
}

/// <summary>
/// A generic interface.
/// </summary>
/// <typeparam name=""T"">The type.</typeparam>
public interface ITest<T>
{
    /// [|<inheritdoc/>|]
    static ITest()
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

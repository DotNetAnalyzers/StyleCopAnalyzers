// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1600ElementsMustBeDocumented,
        StyleCop.Analyzers.DocumentationRules.SA1600CodeFixProvider>;

    public partial class SA1600CSharp8UnitTests : SA1600CSharp7UnitTests
    {
        // Using 'Default' here makes sure that later test projects also run these tests with their own language version, without having to override this property
        protected override LanguageVersion LanguageVersion => LanguageVersion.Default;

        [Theory]
        [InlineData("")]
        [InlineData("out ")]
        [InlineData("in ")]
        public async Task TestGenericInterfaceStaticConstructorCodeFixAsync(string variance)
        {
            var testCode = @"/// <summary>
/// Test interface.
/// </summary>
/// <typeparam name=""T"">The type.</typeparam>
public interface ITest<" + variance + @"T>
{
    static {|#0:ITest|}()
    {
    }
}
";
            var fixedCode = @"/// <summary>
/// Test interface.
/// </summary>
/// <typeparam name=""T"">The type.</typeparam>
public interface ITest<" + variance + @"T>
{
    /// <summary>
    /// Initializes static members of the <see cref=""ITest{T}""/> class.
    /// </summary>
    static ITest()
    {
    }
}
";

            await new CSharpTest(this.LanguageVersion)
            {
                TestCode = testCode,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0) },
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// The editorconfig interface setting does not disable general documentation requirements.
        /// </summary>
        /// <param name="mode">The interface documentation mode.</param>
        /// <param name="documentExposed">Whether exposed elements require documentation.</param>
        /// <param name="documentInternal">Whether internal elements require documentation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Theory]
        [CombinatorialData]
        [WorkItem(4345, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4345")]
        public async Task TestInterfaceDocumentationSettingsFromEditorConfigAreAdditiveAsync(
            [CombinatorialValues("false", "none", "exposed")] string mode,
            bool documentExposed,
            bool documentInternal)
        {
            var testCode = """
                internal interface {|#0:IInternal|}
                {
                    void {|#1:Method|}() { }
                    int {|#2:Property|} => 0;
                    private void PrivateMethod() { }
                }

                public interface {|#3:IPublic|}
                {
                    void {|#4:Method|}() { }
                    int {|#5:Property|} => 0;
                    private void PrivateMethod() { }
                }
                """;
            var test = new CSharpTest(this.LanguageVersion)
            {
                TestCode = testCode,
                FixedCode = testCode,
                DisabledDiagnostics = { "CS1591" },
            };
            test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", $$"""
                root = true

                [*.cs]
                stylecop.documentation.documentInterfaces = {{mode}}
                stylecop.documentation.documentExposedElements = {{documentExposed.ToString().ToLowerInvariant()}}
                stylecop.documentation.documentInternalElements = {{documentInternal.ToString().ToLowerInvariant()}}
                """));
            if (documentInternal)
            {
                test.ExpectedDiagnostics.AddRange(new[] { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1), Diagnostic().WithLocation(2) });
            }

            if (documentExposed || mode == "exposed")
            {
                test.ExpectedDiagnostics.AddRange(new[] { Diagnostic().WithLocation(3), Diagnostic().WithLocation(4), Diagnostic().WithLocation(5) });
            }

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Non-public interface members follow general settings without making private methods require documentation by default.
        /// </summary>
        /// <param name="mode">The interface documentation mode.</param>
        /// <param name="documentExposed">Whether exposed elements require documentation.</param>
        /// <param name="documentInternal">Whether internal elements require documentation.</param>
        /// <param name="documentPrivate">Whether private elements require documentation.</param>
        /// <param name="documentPrivateFields">Whether private fields require documentation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Theory]
        [CombinatorialData]
        [WorkItem(4345, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4345")]
        public async Task TestNonPublicInterfaceMembersFollowDocumentationSettingsAsync(
            [CombinatorialValues("all", "exposed", "none")] string mode,
            bool documentExposed,
            bool documentInternal,
            bool documentPrivate,
            bool documentPrivateFields)
        {
            var testCode = """
                /// <summary>Interface summary.</summary>
                public interface ITest
                {
                    private void {|#0:PrivateMethod|}() { }
                    internal void {|#1:InternalMethod|}() { }
                    protected void {|#2:ProtectedMethod|}() { }
                    private static int {|#3:PrivateField|};
                    internal static int {|#4:InternalField|};
                    private event System.Action {|#5:PrivateEvent|} { add { } remove { } }
                }
                """;
            var test = new CSharpTest(this.LanguageVersion)
            {
                TestCode = testCode,
                FixedCode = testCode,
                DisabledDiagnostics = { "CS1591" },
                Settings = $$"""
                    {
                      "settings": {
                        "documentationRules": {
                          "documentInterfaces": "{{mode}}",
                          "documentExposedElements": {{documentExposed.ToString().ToLowerInvariant()}},
                          "documentInternalElements": {{documentInternal.ToString().ToLowerInvariant()}},
                          "documentPrivateElements": {{documentPrivate.ToString().ToLowerInvariant()}},
                          "documentPrivateFields": {{documentPrivateFields.ToString().ToLowerInvariant()}}
                        }
                      }
                    }
                    """,
            };
            if (documentPrivate)
            {
                test.ExpectedDiagnostics.AddRange(new[] { Diagnostic().WithLocation(0), Diagnostic().WithLocation(5) });
            }

            if (documentPrivate || documentInternal)
            {
                test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(1));
            }

            if (documentPrivate || documentExposed)
            {
                test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(2));
            }

            if (documentPrivateFields)
            {
                test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(3));
            }

            if (documentPrivate || documentInternal || documentPrivateFields)
            {
                test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(4));
            }

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// The editorconfig exposed mode exempts internal interface members, including default implementations.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        [WorkItem(4342, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4342")]
        public async Task TestExposedInterfaceDocumentationFromEditorConfigAsync()
        {
            var testCode = """
                internal interface IInternal
                {
                    public void ExplicitPublicMethod();
                    void DefaultMethod() { }
                    private void PrivateMethod() { }
                    public int Property => 0;
                    event System.Action Changed { add { } remove { } }
                    public static int Field;
                    public class Nested { }
                    interface INested { void Method(); }
                }

                /// <summary>Exposed interface summary.</summary>
                public interface IExposed
                {
                    public void {|#0:RequiredMethod|}();
                    private void PrivateMethod() { }
                }
                """;
            var test = new CSharpTest(this.LanguageVersion)
            {
                TestCode = testCode,
                FixedCode = testCode,
                DisabledDiagnostics = { "CS1591" },
                ExpectedDiagnostics = { Diagnostic().WithLocation(0) },
            };
            test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", """
                root = true

                [*.cs]
                stylecop.documentation.documentInterfaces = exposed
                stylecop.documentation.documentInternalElements = false
                stylecop.documentation.documentExposedElements = false
                """));

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3002, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3002")]
        public async Task TestDefaultInterfaceMemberRequiresDocumentationAsync()
        {
            var testCode = @"using System;
/// <summary>Summary.</summary>
public interface ITest
{
    public static int [|field1|];
    static int [|field2|];

    public int [|Prop1|] { get => 0; }
    int [|Prop2|] { get => 0; }

    int [|this|][int index] { get => 0; }

    public event EventHandler [|Event1|];
    event EventHandler [|Event2|];

    public event EventHandler [|Event3|] { add { } remove { } }
    event EventHandler [|Event4|] { add { } remove { } }

    public void [|Method1|]() { }
    void [|Method2|]() { }

    public delegate void [|Del1|]();
    delegate void [|Del2|]();

    public class [|Class1|] { }
    class [|Class2|] { }

    public struct [|Struct1|] { }
    struct [|Struct2|] { }

    public interface [|Interface1|] { }
    interface [|Interface2|] { }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3002, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3002")]
        public async Task TestDefaultInterfaceFieldRequiresDocumentationAsync()
        {
            var testCode = @"
/// <summary>Summary.</summary>
public interface ITest
{
    public static int [|field1|];
    static int [|field2|];
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3002, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3002")]
        public async Task TestPrivateDefaultInterfaceMethodDoesNotRequireDocumentationByDefaultAsync()
        {
            var testCode = @"
/// <summary>Summary.</summary>
public interface ITest
{
    private void M()
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3002, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3002")]
        public async Task TestPrivateDefaultInterfaceMethodHonorsDocumentPrivateElementsAsync()
        {
            var testCode = @"
/// <summary>Summary.</summary>
public interface ITest
{
    private void [|M|]()
    {
    }
}
";

            var settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentPrivateElements"": true
    }
  }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

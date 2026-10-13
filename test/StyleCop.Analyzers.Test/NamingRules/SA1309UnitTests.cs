// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1309FieldNamesMustNotBeginWithUnderscore,
        StyleCop.Analyzers.NamingRules.SA1309CodeFixProvider>;

    public class SA1309UnitTests
    {
        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task TestFieldStartingWithAnUnderscoreAsync(string lineEnding)
        {
            var testCode = @"public class Foo
{
    public string {|#0:_bar|} = ""baz"";
}".ReplaceLineEndings(lineEnding);

            DiagnosticResult expected = Diagnostic().WithArguments("_bar").WithLocation(0);

            var fixedCode = @"public class Foo
{
    public string bar = ""baz"";
}".ReplaceLineEndings(lineEnding);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingWithLetterAsync()
        {
            var testCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that renaming a field preserves references when an out variable has the new name.
        /// </summary>
        /// <param name="isStatic">Whether the field is static.</param>
        /// <param name="localName">The name of the out variable.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(false, "documentIndex")]
        [InlineData(true, "documentIndex")]
        [InlineData(false, "index")]
        [InlineData(true, "index")]
        [WorkItem(2939, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2939")]
        public async Task TestFieldRenameWithOutVariableCollisionAsync(bool isStatic, string localName)
        {
            var localDeclaration = LightupHelpers.SupportsCSharp7 ? string.Empty : "        int " + localName + ";\r\n";
            var outArgument = (LightupHelpers.SupportsCSharp7 ? "out int " : "out ") + localName;
            var fieldModifier = isStatic ? "static " : string.Empty;
            var receiver = localName == "documentIndex" ? (isStatic ? "PdbWriter." : "this.") : string.Empty;
            var testCode = @"using System.Collections.Generic;

public class PdbWriter
{
    private " + fieldModifier + @"Dictionary<string, int> {|#0:_documentIndex|} = new Dictionary<string, int>();

    public int GetDocumentIndex(string document)
    {
" + localDeclaration + @"        if (_documentIndex.TryGetValue(document, " + outArgument + @"))
        {
            return " + localName + @";
        }

        return -1;
    }
}";
            var fixedCode = @"using System.Collections.Generic;

public class PdbWriter
{
    private " + fieldModifier + @"Dictionary<string, int> documentIndex = new Dictionary<string, int>();

    public int GetDocumentIndex(string document)
    {
" + localDeclaration + @"        if (" + receiver + @"documentIndex.TryGetValue(document, " + outArgument + @"))
        {
            return " + localName + @";
        }

        return -1;
    }
}";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithArguments("_documentIndex").WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that collisions in another partial declaration are qualified without changing comments or existing receivers.
        /// </summary>
        /// <param name="isStatic">Whether the field is static.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [WorkItem(2939, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2939")]
        public async Task TestFieldRenameWithCollisionInAnotherDocumentAsync(bool isStatic)
        {
            var fieldModifier = isStatic ? "static " : string.Empty;
            var receiver = isStatic ? "PdbWriter<T>." : "this.";
            var declaration = @"using System.Collections.Generic;

public partial class PdbWriter<T>
{
    private " + fieldModifier + @"Dictionary<T, int> {|#0:_documentIndex|} = new Dictionary<T, int>();
}";
            var references = @"public partial class PdbWriter<T>
{
    public int GetDocumentIndex(T documentIndex)
    {
        int result;
        if (_documentIndex /* field */ .TryGetValue(documentIndex, out result))
        {
            return result;
        }

        return " + receiver + @"_documentIndex.Count;
    }
}";
            var fixedDeclaration = declaration.Replace("{|#0:_documentIndex|}", "documentIndex");
            var fixedReferences = references
                .Replace("if (_documentIndex", "if (" + receiver + "documentIndex")
                .Replace(receiver + "_documentIndex.Count", receiver + "documentIndex.Count");
            var test = new CSharpTest();
            test.TestState.Sources.Add(declaration);
            test.TestState.Sources.Add(references);
            test.FixedState.Sources.Add(fixedDeclaration);
            test.FixedState.Sources.Add(fixedReferences);
            test.ExpectedDiagnostics.Add(Diagnostic().WithArguments("_documentIndex").WithLocation(0));

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2939, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2939")]
        public async Task TestFieldRenameInObjectInitializerAsync()
        {
            var testCode = @"public class C
{
    private int {|#0:_value|};

    public C Create(int value)
    {
        return new C { _value = _value + value };
    }
}";
            var fixedCode = @"public class C
{
    private int value;

    public C Create(int value)
    {
        return new C { value = this.value + value };
    }
}";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithArguments("_value").WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2939, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2939")]
        public async Task TestInheritedStaticFieldRenameWithCollisionAsync()
        {
            var testCode = @"public class Base<T>
{
    protected static int {|#0:_value|};
}

public class Derived : Base<int>
{
    public int GetValue(int value)
    {
        return _value + value;
    }
}";
            var fixedCode = @"public class Base<T>
{
    protected static int value;
}

public class Derived : Base<int>
{
    public int GetValue(int value)
    {
        return Base<int>.value + value;
    }
}";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithArguments("_value").WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("int[]", "new int[]", false)]
        [InlineData("int[]", "new int[]", true)]
        [InlineData("System.Collections.Generic.List<int>", "new System.Collections.Generic.List<int>", true)]
        [WorkItem(2939, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2939")]
        public async Task TestFieldAssignmentInInitializerAsync(string returnType, string creation, bool parenthesized)
        {
            var open = parenthesized ? "(" : string.Empty;
            var close = parenthesized ? ")" : string.Empty;
            var testCode = @"public class C
{
    private int {|#0:_value|};

    public " + returnType + @" Create(int value)
    {
        return " + creation + @" { " + open + "_value = value" + close + @" };
    }
}";
            var fixedCode = @"public class C
{
    private int value;

    public " + returnType + @" Create(int value)
    {
        return " + creation + @" { " + open + "this.value = value" + close + @" };
    }
}";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithArguments("_value").WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingAnUnderscorePlacedInsideNativeMethodsClassAsync()
        {
            var testCode = @"public class FooNativeMethods
{
    internal string _bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingAnUnderscorePlacedInsideNativeMethodsClassWithIncorrectNameAsync()
        {
            var testCode = @"public class FooNativeMethodsClass
{
    internal string _bar = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithArguments("_bar").WithLocation(3, 21);

            var fixedCode = @"public class FooNativeMethodsClass
{
    internal string bar = ""baz"";
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingAnUnderscorePlacedInsideOuterNativeMethodsClassAsync()
        {
            var testCode = @"public class FooNativeMethods
{
    public class Foo
    {
        public string _bar = ""baz"";
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnderscoreOnlyVariableNameAsync()
        {
            var testCode = @"public class FooNativeMethodsClass
{
    internal string _ = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithArguments("_").WithLocation(3, 21);

            // no changes will be made
            var fixedCode = testCode;
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#627.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/627">#627: Code Fixes For Naming
        /// Rules SA1308 and SA1309 Do Not Always Fix The Name Entirely</seealso>
        [Fact]
        public async Task TestFieldStartingWithMultipleUnderscoresAsync()
        {
            var testCode = @"public class Foo
{
    public string ____bar = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithArguments("____bar").WithLocation(3, 19);

            var fixedCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        // This a regression test for issue #2360.
        [Fact]
        public async Task VerifyThatAFieldStartingWithAnUnderscoreAndADigitIsNotAffectedByCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    private string _1bar = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithArguments("_1bar").WithLocation(3, 20);

            // no changes will be made
            var fixedCode = testCode;
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyThatAFieldStartingWithUnderscoreAndFollowedByKeywordTriggersDiagnosticAndIsCorrectedByCodefixAsync()
        {
            var testCode = @"public class Foo
{
    private string _int = ""baz"";
}";

            var fixedCode = @"public class Foo
{
    private string @int = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithArguments("_int").WithLocation(3, 20);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

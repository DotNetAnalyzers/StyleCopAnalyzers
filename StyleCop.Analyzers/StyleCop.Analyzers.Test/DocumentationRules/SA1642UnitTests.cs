// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.DocumentationRules.SA1642ConstructorSummaryDocumentationMustBeginWithStandardText;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1642ConstructorSummaryDocumentationMustBeginWithStandardText"/>-
    /// </summary>
    public class SA1642UnitTests : CodeFixVerifier
    {
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNoDocumentationAsync(string typeKind)
        {
            var testCode = $@"namespace FooNamespace
{{
    public {typeKind} Foo<TFoo, TBar>
    {{
        public Foo(int arg)
        {{
        }}
    }}
}}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestEmptyPublicConstructorAsync(string typeKind)
        {
            await this.TestEmptyConstructorAsync(typeKind, "public").ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestEmptyNonPublicConstructorAsync(string typeKind)
        {
            await this.TestEmptyConstructorAsync(typeKind, "private").ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestEmptyStaticConstructorAsync(string typeKind)
        {
            await this.TestEmptyConstructorAsync(typeKind, "static").ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorCorrectDocumentationSimpleAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationSimpleAsync(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorCorrectDocumentationCustomizedAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationCustomizedAsync(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericSimpleAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationSimpleAsync(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericCustomizedAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationCustomizedAsync(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentationAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationAsync(typeKind, "private", PrivateConstructorStandardText[0], $" {typeKind} from being created.", string.Empty, false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectExtendedDocumentationAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationAsync(typeKind, "private", PrivateConstructorStandardText[0], $" {typeKind} from being created externally.", string.Empty, false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentation_NonPrivateSimpleAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationSimpleAsync(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentation_NonPrivateCustomizedAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationCustomizedAsync(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentationGenericAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationAsync(typeKind, "private", PrivateConstructorStandardText[0], $" {typeKind} from being created.", string.Empty, true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectExtendedDocumentationGenericAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationAsync(typeKind, "private", PrivateConstructorStandardText[0], $" {typeKind} from being created externally.", string.Empty, true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentationGeneric_NonPrivateSimpleAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationSimpleAsync(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentationGeneric_NonPrivateCustomizedAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationCustomizedAsync(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorCorrectDocumentationAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationAsync(typeKind, "static", StaticConstructorStandardText, $" {typeKind}.", string.Empty, false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorCorrectDocumentationGenericAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationAsync(typeKind, "static", StaticConstructorStandardText, $" {typeKind}.", string.Empty, true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorMissingDocumentationAsync(string typeKind)
        {
            await this.TestConstructorMissingDocumentationAsync(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorMissingDocumentationGenericAsync(string typeKind)
        {
            await this.TestConstructorMissingDocumentationAsync(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorMissingDocumentationAsync(string typeKind)
        {
            await this.TestConstructorMissingDocumentationAsync(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorMissingDocumentationGenericAsync(string typeKind)
        {
            await this.TestConstructorMissingDocumentationAsync(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorMissingDocumentationAsync(string typeKind)
        {
            await this.TestConstructorMissingDocumentationAsync(typeKind, "static", StaticConstructorStandardText, $" {typeKind}.", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorMissingDocumentationGenericAsync(string typeKind)
        {
            await this.TestConstructorMissingDocumentationAsync(typeKind, "static", StaticConstructorStandardText, $" {typeKind}.", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorSimpleDocumentationAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationAsync(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorSimpleDocumentationGenericAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationAsync(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorSimpleDocumentationAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationAsync(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorSimpleDocumentationGenericAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationAsync(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorSimpleDocumentationAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationAsync(typeKind, "static", StaticConstructorStandardText, $" {typeKind}.", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorSimpleDocumentationGenericAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationAsync(typeKind, "static", StaticConstructorStandardText, $" {typeKind}.", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorSimpleDocumentationWrongTypeNameAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationWrongTypeNameAsync(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorSimpleDocumentationGenericWrongTypeNameAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationWrongTypeNameAsync(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorSimpleDocumentationWrongTypeNameAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationWrongTypeNameAsync(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorSimpleDocumentationGenericWrongTypeNameAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationWrongTypeNameAsync(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorSimpleDocumentationWrongTypeNameAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationWrongTypeNameAsync(typeKind, "static", StaticConstructorStandardText, $" {typeKind}.", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorSimpleDocumentationGenericWrongTypeNameAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationWrongTypeNameAsync(typeKind, "static", StaticConstructorStandardText, $" {typeKind}.", true).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#676 "SA1642 misfires on nested structs,
        /// requiring text describing the outer type"
        /// https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/676
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestStructNestedInClassAsync()
        {
            string testCode = @"
class ClassName
{
    struct StructName
    {
        /// <summary>
        /// </summary>
        StructName(int argument)
        {
        }
    }
}
";
            string fixedCode = @"
class ClassName
{
    struct StructName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""StructName""/> struct.
        /// </summary>
        StructName(int argument)
        {
        }
    }
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 13);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class", "class", "class")]
        [InlineData("class", "struct", "struct")]
        [InlineData("struct", "class", "class")]
        [InlineData("struct", "struct", "struct")]
        public async Task TestAllowedOuterQualifiedNamesAsync(string outerTypeKind, string nestedTypeKind, string describedTypeKind)
        {
            string testCode = $@"
{outerTypeKind} OuterName
{{
    {nestedTypeKind} NestedName
    {{
        /// <summary>
        /// Initializes a new instance of the <see cref=""OuterName.NestedName""/> {describedTypeKind}.
        /// </summary>
        NestedName(int argument)
        {{
        }}
    }}
}}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for
        /// <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/780">DotNetAnalyzers/StyleCopAnalyzers#780</see>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFailureIssue780Async()
        {
            string testCode = @"
internal abstract class CustomizableBlockSubscriberBase<TSource, TTarget, TSubscribedElement>
    where TSource : class
    where TTarget : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""ProjectBuildSnapshotService""/> class.
    /// </summary>
    protected CustomizableBlockSubscriberBase()
    {
    }
}
";
            string fixedCode = @"
internal abstract class CustomizableBlockSubscriberBase<TSource, TTarget, TSubscribedElement>
    where TSource : class
    where TTarget : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""CustomizableBlockSubscriberBase{TSource, TTarget, TSubscribedElement}""/> class.
    /// </summary>
    protected CustomizableBlockSubscriberBase()
    {
    }
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 43);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1642ConstructorSummaryDocumentationMustBeginWithStandardText();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1642SA1643CodeFixProvider();
        }

        private async Task TestEmptyConstructorAsync(string typeKind, string modifiers)
        {
            var testCode = @"namespace FooNamespace
{{
    public {0} Foo<TFoo, TBar>
    {{
        /// 
        /// 
        /// 
        {1} 
        Foo({2})
        {{

        }}
    }}
}}";

            string arguments = typeKind == "struct" && modifiers != "static" ? "int argument" : null;
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKind, modifiers, arguments), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestConstructorCorrectDocumentationAsync(string typeKind, string modifiers, string part1, string part2, string part3, bool generic)
        {
            // First test it all on one line
            var testCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}<see cref=""Foo{2}""/>{4}{5}
        /// </summary>
        {6} Foo({7})
        {{

        }}
    }}
}}";

            string arguments = typeKind == "struct" && modifiers != "static" ? "int argument" : null;
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            // Then test splitting after the <see> element
            testCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}<see cref=""Foo{2}""/>
        /// {4}{5}
        /// </summary>
        {6} Foo({7})
        {{

        }}
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            // Then test splitting before the <see> element
            testCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}
        /// <see cref=""Foo{2}""/>{4}{5}
        /// </summary>
        {6} Foo({7})
        {{

        }}
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestConstructorCorrectDocumentationSimpleAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            await this.TestConstructorCorrectDocumentationAsync(typeKind, modifiers, part1, part2, ".", generic).ConfigureAwait(false);
        }

        private async Task TestConstructorCorrectDocumentationCustomizedAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            await this.TestConstructorCorrectDocumentationAsync(typeKind, modifiers, part1, part2, " with A and B.", generic).ConfigureAwait(false);
        }

        private async Task TestConstructorMissingDocumentationAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            var testCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// </summary>
        {2}
        Foo({3})
        {{

        }}
    }}
}}";
            string arguments = typeKind == "struct" && modifiers != "static" ? "int argument" : null;
            testCode = string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, modifiers, arguments);

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}<see cref=""Foo{2}""/>{4}{5}
        /// </summary>
        {6}
        Foo({7})
        {{

        }}
    }}
}}";

            string part3 = part2.EndsWith(".") ? string.Empty : ".";
            fixedCode = string.Format(fixedCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        private async Task TestConstructorSimpleDocumentationAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            string part3 = part2.EndsWith(".") ? string.Empty : ".";

            var testCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}Foo{4}{5}
        /// </summary>
        {6}
        Foo({7})
        {{

        }}
    }}
}}";
            string arguments = typeKind == "struct" && modifiers != "static" ? "int argument" : null;
            testCode = string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments);

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 13);

            await this.VerifyCSharpDiagnosticAsync(
                testCode,
                expected,
                CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}<see cref=""Foo{2}""/>{4}{5}
        /// </summary>
        {6}
        Foo({7})
        {{

        }}
    }}
}}";
            fixedCode = string.Format(fixedCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        private async Task TestConstructorSimpleDocumentationWrongTypeNameAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            string part3 = part2.EndsWith(".") ? string.Empty : ".";

            var testCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}Bar{4}{5}
        /// </summary>
        {6}
        Foo({7})
        {{

        }}
    }}
}}";
            string arguments = typeKind == "struct" && modifiers != "static" ? "int argument" : null;
            testCode = string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments);

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 13);

            await this.VerifyCSharpDiagnosticAsync(
                testCode,
                expected,
                CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}<see cref=""Foo{2}""/>{4}{5}
        /// </summary>
        {6}
        Foo({7})
        {{

        }}
    }}
}}";
            fixedCode = string.Format(fixedCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<Analyzers.ReadabilityRules.SA1125UseShorthandForNullableTypes>;

    public class SA1125CSharp7UnitTests : SA1125UnitTests
    {
        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#386.
        /// <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/386">SA1125
        /// UseShorthandForNullableTypes incorrectly reported in typeof()</see>.
        /// </summary>
        /// <param name="longForm">The source code for the long form of a <c>cref</c> attribute referencing
        /// an instantiation of <see cref="Nullable{T}"/> in a <c>typeof</c> expression.</param>
        /// <param name="shortForm">The source code for the shorthand form of a <c>cref</c> attribute referencing
        /// an instantiation of <see cref="Nullable{T}"/> in a <c>typeof</c> expression. If no shorthand form is
        /// available, this argument should be the same as <paramref name="longForm"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]

        [InlineData("Nullable<(int, int)>", "(int, int)?")]
        [InlineData("System.Nullable<(int, int)>", "(int, int)?")]
        [InlineData("global::System.Nullable<(int, int)>", "(int, int)?")]

        [InlineData("Nullable<(T, T)>", "(T, T)?")]
        [InlineData("System.Nullable<(T, T)>", "(T, T)?")]
        [InlineData("global::System.Nullable<(T, T)>", "(T, T)?")]
        public async Task TestTypeOfNullableValueTupleAsync(string longForm, string shortForm)
        {
            string template = @"
namespace System
{{
    class ClassName<T>
        where T : struct
    {{
        Type nullableType = typeof({0});
    }}
}}
";

            string testCode = string.Format(template, longForm);
            string fixedCode = string.Format(template, shortForm);

            DiagnosticResult expected = Diagnostic().WithLocation(7, 36);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(fixedCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]

        [InlineData("Nullable<(int, int)>", "(int, int)?")]
        [InlineData("System.Nullable<(int, int)>", "(int, int)?")]
        [InlineData("global::System.Nullable<(int, int)>", "(int, int)?")]

        [InlineData("Nullable<(T, T)>", "(T, T)?")]
        [InlineData("System.Nullable<(T, T)>", "(T, T)?")]
        [InlineData("global::System.Nullable<(T, T)>", "(T, T)?")]
        public async Task TestNullableValueTupleFieldAsync(string longForm, string shortForm)
        {
            string template = @"
namespace System
{{
    class ClassName<T>
        where T : struct
    {{
        {0} nullableField;
    }}
}}
";
            string testCode = string.Format(template, longForm);
            string fixedCode = string.Format(template, shortForm);

            DiagnosticResult expected = Diagnostic().WithLocation(7, 9);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(fixedCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]

        [InlineData("Nullable<(int, int)>", "(int, int)?")]
        [InlineData("System.Nullable<(int, int)>", "(int, int)?")]
        [InlineData("global::System.Nullable<(int, int)>", "(int, int)?")]

        [InlineData("Nullable<(T, T)>", "(T, T)?")]
        [InlineData("System.Nullable<(T, T)>", "(T, T)?")]
        [InlineData("global::System.Nullable<(T, T)>", "(T, T)?")]
        public async Task TestDefaultNullableValueTupleAsync(string longForm, string shortForm)
        {
            string template = @"
namespace System
{{
    class ClassName<T>
        where T : struct
    {{
        void MethodName()
        {{
            var nullableValue = default({0});
        }}
    }}
}}
";
            string testCode = string.Format(template, longForm);
            string fixedCode = string.Format(template, shortForm);

            DiagnosticResult expected = Diagnostic().WithLocation(9, 41);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(fixedCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#637.
        /// <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/637">SA1125
        /// UseShorthandForNullableTypes incorrectly reported in <c>nameof</c> expression</see>.
        /// </summary>
        /// <param name="form">The source code for the content of a <c>nameof</c> expression referencing
        /// <see cref="Nullable{T}"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("Nullable<(int, int)>")]
        [InlineData("System.Nullable<(int, int)>")]
        [InlineData("global::System.Nullable<(int, int)>")]
        public async Task TestNameOfNullableValueTupleAsync(string form)
        {
            string template = @"
namespace System
{{
    class ClassName<T>
        where T : struct
    {{
        string nullableName = nameof({0});
    }}
}}
";
            string testCode = string.Format(template, form);
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#636.
        /// <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/636">SA1125
        /// UseShorthandForNullableTypes incorrectly reported for static access through Nullable&lt;int&gt;</see>.
        /// </summary>
        /// <remarks>
        /// <para>This special case of instance access through <c>Nullable&lt;int&gt;</c> was mentioned in a
        /// comment.</para>
        /// </remarks>
        /// <param name="form">The source code for the content of a <c>nameof</c> expression referencing
        /// <see cref="Nullable{T}"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("Nullable<(int, int)>")]
        [InlineData("System.Nullable<(int, int)>")]
        [InlineData("global::System.Nullable<(int, int)>")]
        public async Task TestNameOfNullableValueTupleValueAsync(string form)
        {
            string template = @"
namespace System
{{
    class ClassName<T>
        where T : struct
    {{
        string nullableName = nameof({0}.Value);
    }}
}}
";
            string testCode = string.Format(template, form);
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#636.
        /// <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/636">SA1125
        /// UseShorthandForNullableTypes incorrectly reported for static access through Nullable&lt;int&gt;</see>.
        /// </summary>
        /// <param name="form">The source code for an instantiation of <see cref="Nullable{T}"/> which does not use the
        /// shorthand syntax.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("Nullable<(int, int)>")]
        [InlineData("System.Nullable<(int, int)>")]
        [InlineData("global::System.Nullable<(int, int)>")]
        public async Task TestAccessObjectEqualThroughNullableValueTupleAsync(string form)
        {
            string template = @"
namespace System
{{
    class ClassName<T>
        where T : struct
    {{
        bool equal = {0}.Equals(null, null);
    }}
}}
";
            string testCode = string.Format(template, form);
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]

        [InlineData("Nullable<(int, int)>", "(int, int)?")]
        [InlineData("System.Nullable<(int, int)>", "(int, int)?")]
        [InlineData("global::System.Nullable<(int, int)>", "(int, int)?")]

        [InlineData("Nullable<(T, T)>", "(T, T)?")]
        [InlineData("System.Nullable<(T, T)>", "(T, T)?")]
        [InlineData("global::System.Nullable<(T, T)>", "(T, T)?")]
        public async Task TestNameOfListOfNullableValueTupleAsync(string longForm, string shortForm)
        {
            string template = @"
using System.Collections.Generic;
namespace System
{{
    class ClassName<T>
        where T : struct
    {{
        string nullableName = nameof(List<{0}>);
    }}
}}
";
            string testCode = string.Format(template, longForm);
            string fixedCode = string.Format(template, shortForm);

            DiagnosticResult expected = Diagnostic().WithLocation(8, 43);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(fixedCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        // This is a regression test for issue 2284.
        [Theory]
        [InlineData("@Nullable<(int, int)>", "(int, int)?")]
        [InlineData("System.@Nullable<(int, int)>", "(int, int)?")]
        [InlineData("global::System.@Nullable<(int, int)>", "(int, int)?")]
        public async Task TestNullableValueTupleFieldWithAtSignPrefixInTypeAsync(string longForm, string shortForm)
        {
            string template = @"
namespace System
{{
    class ClassName<T>
        where T : struct
    {{
        {0} nullableField;
    }}
}}
";
            string testCode = string.Format(template, longForm);
            string fixedCode = string.Format(template, shortForm);

            DiagnosticResult expected = Diagnostic().WithLocation(7, 9);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(fixedCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

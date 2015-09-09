// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1125UseShorthandForNullableTypes"/>.
    /// </summary>
    public class SA1125UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#385.
        /// <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/385">SA1125
        /// UseShorthandForNullableTypes incorrectly reported in XML comment</see>
        /// </summary>
        /// <param name="form">The source code for the content of a <c>cref</c> attribute referencing
        /// <see cref="Nullable{T}"/> in an XML documentation comment.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("Nullable{T}")]
        [InlineData("System.Nullable{T}")]
        [InlineData("global::System.Nullable{T}")]
        public async Task TestSeeAlsoNullableAsync(string form)
        {
            string template = @"
namespace System
{{
    /// <seealso cref=""{0}""/>
    class ClassName
    {{
    }}
}}
";
            string testCode = string.Format(template, form);
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#638.
        /// <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/638">SA1125
        /// UseShorthandForNullableTypes incorrectly reported for member in XML comment</see>
        /// </summary>
        /// <param name="form">The source code for the content of a <c>cref</c> attribute referencing
        /// <see cref="Nullable{T}"/> in an XML documentation comment.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("Nullable{T}")]
        [InlineData("System.Nullable{T}")]
        [InlineData("global::System.Nullable{T}")]
        public async Task TestSeeAlsoNullableValueAsync(string form)
        {
            string template = @"
namespace System
{{
    /// <seealso cref=""{0}.Value""/>
    class ClassName
    {{
    }}
}}
";
            string testCode = string.Format(template, form);
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#385.
        /// <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/385">SA1125
        /// UseShorthandForNullableTypes incorrectly reported in XML comment</see>
        /// </summary>
        /// <param name="longForm">The source code for the long form of a <c>cref</c> attribute referencing
        /// an instantiation of <see cref="Nullable{T}"/> in an XML documentation comment (e.g. for the parameter type
        /// in a reference to <see cref="Enumerable.Average(IEnumerable{int?})"/>.</param>
        /// <param name="shortForm">The source code for the shorthand form of a <c>cref</c> attribute referencing
        /// an instantiation of <see cref="Nullable{T}"/> in an XML documentation comment (e.g. for the parameter type
        /// in a reference to <see cref="Enumerable.Average(IEnumerable{int?})"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("Nullable{int}", "int?")]
        [InlineData("System.Nullable{int}", "int?")]
        [InlineData("global::System.Nullable{int}", "int?")]
        public async Task TestSeeAlsoNullableShorthandAsync(string longForm, string shortForm)
        {
            string template = @"
using System.Collections.Generic;
using System.Linq;
namespace System
{{
    /// <seealso cref=""Enumerable.Average(IEnumerable{{{0}}})""/>
    class ClassName
    {{
    }}
}}
";
            string testCode = string.Format(template, longForm);
            string fixedCode = string.Format(template, shortForm);

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 55);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#386.
        /// <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/386">SA1125
        /// UseShorthandForNullableTypes incorrectly reported in typeof()</see>
        /// </summary>
        /// <param name="longForm">The source code for the long form of a <c>cref</c> attribute referencing
        /// an instantiation of <see cref="Nullable{T}"/> in a <c>typeof</c> expression.</param>
        /// <param name="shortForm">The source code for the shorthand form of a <c>cref</c> attribute referencing
        /// an instantiation of <see cref="Nullable{T}"/> in a <c>typeof</c> expression. If no shorthand form is
        /// available, this argument should be the same as <paramref name="longForm"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]

        [InlineData("Nullable<int>", "int?")]
        [InlineData("System.Nullable<int>", "int?")]
        [InlineData("global::System.Nullable<int>", "int?")]

        [InlineData("Nullable<T>", "T?")]
        [InlineData("System.Nullable<T>", "T?")]
        [InlineData("global::System.Nullable<T>", "T?")]

        [InlineData("Nullable<>", "Nullable<>")]
        [InlineData("System.Nullable<>", "System.Nullable<>")]
        [InlineData("global::System.Nullable<>", "global::System.Nullable<>")]
        public async Task TestTypeOfNullableAsync(string longForm, string shortForm)
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

            if (testCode != fixedCode)
            {
                DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 36);
                await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            }

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]

        [InlineData("Nullable<int>", "int?")]
        [InlineData("System.Nullable<int>", "int?")]
        [InlineData("global::System.Nullable<int>", "int?")]

        [InlineData("Nullable<T>", "T?")]
        [InlineData("System.Nullable<T>", "T?")]
        [InlineData("global::System.Nullable<T>", "T?")]
        public async Task TestNullableFieldAsync(string longForm, string shortForm)
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]

        [InlineData("Nullable<int>", "int?")]
        [InlineData("System.Nullable<int>", "int?")]
        [InlineData("global::System.Nullable<int>", "int?")]

        [InlineData("Nullable<T>", "T?")]
        [InlineData("System.Nullable<T>", "T?")]
        [InlineData("global::System.Nullable<T>", "T?")]
        public async Task TestDefaultNullableValueAsync(string longForm, string shortForm)
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 41);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#637.
        /// <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/637">SA1125
        /// UseShorthandForNullableTypes incorrectly reported in <c>nameof</c> expression</see>
        /// </summary>
        /// <param name="form">The source code for the content of a <c>nameof</c> expression referencing
        /// <see cref="Nullable{T}"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("Nullable<int>")]
        [InlineData("System.Nullable<int>")]
        [InlineData("global::System.Nullable<int>")]
        public async Task TestNameOfNullableAsync(string form)
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#636.
        /// <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/636">SA1125
        /// UseShorthandForNullableTypes incorrectly reported for static access through Nullable&lt;int&gt;</see>
        /// </summary>
        /// <remarks>
        /// <para>This special case of instance access through <c>Nullable&lt;int&gt;</c> was mentioned in a
        /// comment.</para>
        /// </remarks>
        /// <param name="form">The source code for the content of a <c>nameof</c> expression referencing
        /// <see cref="Nullable{T}"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("Nullable<int>")]
        [InlineData("System.Nullable<int>")]
        [InlineData("global::System.Nullable<int>")]
        public async Task TestNameOfNullableValueAsync(string form)
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#636.
        /// <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/636">SA1125
        /// UseShorthandForNullableTypes incorrectly reported for static access through Nullable&lt;int&gt;</see>
        /// </summary>
        /// <param name="form">The source code for an instantiation of <see cref="Nullable{T}"/> which does not use the
        /// shorthand syntax.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("Nullable<int>")]
        [InlineData("System.Nullable<int>")]
        [InlineData("global::System.Nullable<int>")]
        public async Task TestAccessObjectEqualThroughNullableAsync(string form)
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]

        [InlineData("Nullable<int>", "int?")]
        [InlineData("System.Nullable<int>", "int?")]
        [InlineData("global::System.Nullable<int>", "int?")]

        [InlineData("Nullable<T>", "T?")]
        [InlineData("System.Nullable<T>", "T?")]
        [InlineData("global::System.Nullable<T>", "T?")]
        public async Task TestNameOfListOfNullableAsync(string longForm, string shortForm)
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 43);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1125UseShorthandForNullableTypes();
        }
    }
}

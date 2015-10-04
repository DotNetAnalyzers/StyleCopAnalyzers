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
    using static StyleCop.Analyzers.DocumentationRules.SA1643DestructorSummaryDocumentationMustBeginWithStandardText;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1643DestructorSummaryDocumentationMustBeginWithStandardText"/>-
    /// </summary>
    public class SA1643UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestNoDocumentationAsync()
        {
            var testCode = @"namespace FooNamespace
{
    public class Foo<TFoo, TBar>
    {                                                                                                 
        ~Foo()
        {

        }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorCorrectDocumentationSimpleAsync()
        {
            await this.TestDestructorCorrectDocumentationSimpleImplAsync(DestructorStandardText[0], DestructorStandardText[1], false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorCorrectDocumentationCustomizedAsync()
        {
            await this.TestDestructorCorrectDocumentationCustomizedImplAsync(DestructorStandardText[0], DestructorStandardText[1], false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericSimpleAsync()
        {
            await this.TestDestructorCorrectDocumentationSimpleImplAsync(DestructorStandardText[0], DestructorStandardText[1], true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorCorrectDocumentationGenericCustomizedAsync()
        {
            await this.TestDestructorCorrectDocumentationCustomizedImplAsync(DestructorStandardText[0], DestructorStandardText[1], true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorMissingDocumentationAsync()
        {
            await this.TestDestructorMissingDocumentationImplAsync(DestructorStandardText[0], DestructorStandardText[1], false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorMissingDocumentationGenericAsync()
        {
            await this.TestDestructorMissingDocumentationImplAsync(DestructorStandardText[0], DestructorStandardText[1], true).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1643DestructorSummaryDocumentationMustBeginWithStandardText();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1642SA1643CodeFixProvider();
        }

        [Fact]
        private async Task TestEmptyDestructorAsync()
        {
            var testCode = @"namespace FooNamespace
{
    public class Foo<TFoo, TBar>
    {
        /// 
        /// 
        /// 
        ~Foo()
        {

        }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestDestructorCorrectDocumentationAsync(string part1, string part2, string part3, bool generic)
        {
            // First test it all on one line
            var testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}<see cref=""Foo{1}""/>{3}{4}
        /// </summary>
        ~Foo()
        {{

        }}
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            // Then test splitting after the <see> element
            testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}<see cref=""Foo{1}""/>
        /// {3}{4}
        /// </summary>
        ~Foo()
        {{

        }}
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            // Then test splitting before the <see> element
            testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}
        /// <see cref=""Foo{1}""/>{3}{4}
        /// </summary>
        Foo()
        {{

        }}
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestDestructorCorrectDocumentationSimpleImplAsync(string part1, string part2, bool generic)
        {
            await this.TestDestructorCorrectDocumentationAsync(part1, part2, ".", generic).ConfigureAwait(false);
        }

        private async Task TestDestructorCorrectDocumentationCustomizedImplAsync(string part1, string part2, bool generic)
        {
            await this.TestDestructorCorrectDocumentationAsync(part1, part2, " with A and B.", generic).ConfigureAwait(false);
        }

        private async Task TestDestructorMissingDocumentationImplAsync(string part1, string part2, bool generic)
        {
            var testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// </summary>
        ~Foo()
        {{

        }}
    }}
}}";
            testCode = string.Format(testCode, generic ? "<T1, T2>" : string.Empty);

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}<see cref=""Foo{1}""/>{3}{4}
        /// </summary>
        ~Foo()
        {{

        }}
    }}
}}";

            string part3 = part2.EndsWith(".") ? string.Empty : ".";
            fixedCode = string.Format(fixedCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }
    }
}

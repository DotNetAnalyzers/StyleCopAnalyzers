// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        StyleCop.Analyzers.LayoutRules.SA1500CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1500BracesForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid data types defined in this test.
        /// </summary>
        /// <remarks>
        /// <para>These are valid for SA1500 only, some will report other diagnostics.</para>
        /// </remarks>
        /// <param name="keyword">The data type keyword.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestDataTypeValidAsync(string keyword)
        {
            var testCode = $@"public class Foo
{{
    public {keyword} ValidStruct1
    {{
    }}

    public {keyword} ValidStruct2
    {{
        public int Field;
    }}

    public {keyword} ValidStruct3 {{ }} /* Valid only for SA1500 */

    public {keyword} ValidStruct4 {{ public int Field; }}  /* Valid only for SA1500 */

    public {keyword} ValidStruct5 /* Valid only for SA1500 */
    {{ public int Field; }}  
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid data type definitions.
        /// </summary>
        /// <remarks>
        /// <para>These will normally also report SA1401, but not in the unit test.</para>
        /// </remarks>
        /// <param name="keyword">The data type keyword.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestDataTypeInvalidAsync(string keyword)
        {
            var testCode = $@"public class Foo
{{
    public {keyword} InvalidStruct1 {{|#0:{{|}}
    }}

    public {keyword} InvalidStruct2 {{|#1:{{|}}
        public int Field;
    }}

    public {keyword} InvalidStruct3 {{|#2:{{|}}
        public int Field; {{|#3:}}|}}

    public {keyword} InvalidStruct4 {{|#4:{{|}} public int Field;
    }}

    public {keyword} InvalidStruct5
    {{
        public int Field; {{|#5:}}|}}

    public {keyword} InvalidStruct6
    {{|#6:{{|}} public int Field;
    }}
}}";

            var fixedTestCode = $@"public class Foo
{{
    public {keyword} InvalidStruct1
    {{
    }}

    public {keyword} InvalidStruct2
    {{
        public int Field;
    }}

    public {keyword} InvalidStruct3
    {{
        public int Field;
    }}

    public {keyword} InvalidStruct4
    {{
        public int Field;
    }}

    public {keyword} InvalidStruct5
    {{
        public int Field;
    }}

    public {keyword} InvalidStruct6
    {{
        public int Field;
    }}
}}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // InvalidStruct1
                Diagnostic().WithLocation(0),

                // InvalidStruct2
                Diagnostic().WithLocation(1),

                // InvalidStruct3
                Diagnostic().WithLocation(2),
                Diagnostic().WithLocation(3),

                // InvalidStruct4
                Diagnostic().WithLocation(4),

                // InvalidStruct5
                Diagnostic().WithLocation(5),

                // InvalidStruct6
                Diagnostic().WithLocation(6),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

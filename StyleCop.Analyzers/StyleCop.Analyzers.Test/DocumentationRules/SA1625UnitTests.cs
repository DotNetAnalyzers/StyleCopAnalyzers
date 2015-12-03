﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains the unit tests for SA1625.
    /// </summary>
    public class SA1625UnitTests : DiagnosticVerifier
    {
        public static IEnumerable<object[]> Members
        {
            get
            {
                yield return new[] { "public void Test() { }" };
                yield return new[] { "public string Test { get; set; }" };
                yield return new[] { "public string Test;" };
                yield return new[] { "public class Test { }" };
                yield return new[] { "public struct Test { }" };
                yield return new[] { "public enum Test { }" };
                yield return new[] { "public delegate void Test();" };
            }
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatCorrectDocumentationDoesNotReportADiagnosticAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <remark>Some remark.</remark>
    {member}
}}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatCorrectDocumentationWithEmptyElementsDoesNotReportADiagnosticAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// Some documentation <see cref=""TestClass""/>.
    /// </summary>
    /// <summary>
    /// Some documentation <see cref=""TestClass2""/>.
    /// </summary>
    /// <remark>Some remark.</remark>
    {member}
}}
public class TestClass2 {{ }}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatTheAnalyzerDoesNotCrashOnInheritDocAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <inheritdoc/>
    {member}
}}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatWhitespacesAreNormalizedForEmptyXmlElementsAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// Some documentation <see cref=""TestClass""/>.
    /// </summary>
    /// <summary>
    /// Some documentation <see       cref  =   ""TestClass""     />.
    /// </summary>
    /// <remark>Some remark.</remark>
    {member}
}}
";
            var expected = this.CSharpDiagnostic().WithLocation(7, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatDublicatedDocumentationDoesReportADiagnosticAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>Some documentation.</summary>
    /// <remark>Some documentation.</remark>
    {member}
}}
";
            var expected = this.CSharpDiagnostic().WithLocation(5, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatAnalyzerIgnoresLeadingAndTrailingWhitespaceAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    ///                                             Some documentation.
    ///
    ///
    /// </summary>
    /// <remark>    Some documentation.      </remark>
    {member}
}}
";
            var expected = this.CSharpDiagnostic().WithLocation(9, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatAnalysisIgnoresUnusedParametersAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>The parameter is not used.</summary>
    /// <remark>Documentation</remark>
    /// <remark>The parameter is not used.</remark>
    /// <remark>Documentation</remark>
    {member}
}}
";
            var expected = this.CSharpDiagnostic().WithLocation(7, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatAnalysisIgnoresEmptyElementsAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary></summary>
    /// <remark>Documentation</remark>
    /// <remark></remark>
    /// <remark>Documentation</remark>
    {member}
}}
";
            var expected = this.CSharpDiagnostic().WithLocation(7, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatCorrectDocumentationDoesNotReportADiagnosticMultiLineAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /** <summary>
    * Some documentation.
    * </summary>
    * <remark>Some remark.</remark>
    **/
    {member}
}}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatDublicatedDocumentationDoesReportADiagnosticMultiLineAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /** <summary>Some documentation.</summary>
    * <remark>Some documentation.</remark>
    **/
    {member}
}}
";
            var expected = this.CSharpDiagnostic().WithLocation(5, 7);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatAnalyzerIgnoresLeadingAndTrailingWhitespaceMultiLineAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /** <summary>
    *                                             Some documentation.
    *
    *
    * </summary>
    * <remark>    Some documentation.      </remark>
    **/
    {member}
}}
";
            var expected = this.CSharpDiagnostic().WithLocation(9, 7);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatAnalysisIgnoresUnusedParametersMultiLineAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /** <summary>The parameter is not used.</summary>
    * <remark>Documentation</remark>
    * <remark>The parameter is not used.</remark>
    * <remark>Documentation</remark>
    **/
    {member}
}}
";
            var expected = this.CSharpDiagnostic().WithLocation(7, 7);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatAnalysisIgnoresEmptyElementsMultiLineAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /** <summary></summary>
    * <remark>Documentation</remark>
    * <remark></remark>
    * <remark>Documentation</remark>
    **/
    {member}
}}
";
            var expected = this.CSharpDiagnostic().WithLocation(7, 7);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1625ElementDocumentationMustNotBeCopiedAndPasted();
        }
    }
}

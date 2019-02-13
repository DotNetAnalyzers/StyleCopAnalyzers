// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<Analyzers.DocumentationRules.SA1613ElementParameterDocumentationMustDeclareParameterName>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1613ElementParameterDocumentationMustDeclareParameterName"/>.
    /// </summary>
    public class SA1613UnitTests
    {
        public static IEnumerable<object[]> Declarations
        {
            get
            {
                yield return new[] { "    public ClassName Method(string foo, string bar) { return null; }" };
                yield return new[] { "    public delegate ClassName Method(string foo, string bar);" };
                yield return new[] { "    public ClassName this[string foo, string bar] { get { return null; } set { } }" };
            }
        }

        [Fact]
        public async Task TestMemberWithoutDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ClassName Method() { return null; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithoutParamsAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
$$
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithValidParamsAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    ///<param name=""foo"">Test</param>
    ///<param name=""bar"">Test</param>
$$
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithInvalidParamsAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    ///<param>Test</param>
    ///<param/>
    ///<param name="""">Test</param>
    ///<param name=""    "">Test</param>
$$
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(10, 8),
                Diagnostic().WithLocation(11, 8),
                Diagnostic().WithLocation(12, 15),
                Diagnostic().WithLocation(13, 15),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithInvalidParamsAndInheritDocAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    ///<param>Test</param>
    ///<param/>
    ///<param name="""">Test</param>
    ///<param name=""    "">Test</param>
$$
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(8, 8),
                Diagnostic().WithLocation(9, 8),
                Diagnostic().WithLocation(10, 15),
                Diagnostic().WithLocation(11, 15),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberWithoutParamsAndIncludedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='MissingParamDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberWithValidParamsAndIncludedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithParamDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberWithInvalidParamsAndIncludedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithInvalidParamDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(8, 22),
                Diagnostic().WithLocation(8, 22),
                Diagnostic().WithLocation(8, 22),
                Diagnostic().WithLocation(8, 22),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberWithInvalidParamsAndInheritDocAndIncludedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithInheritedDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(8, 22),
                Diagnostic().WithLocation(8, 22),
                Diagnostic().WithLocation(8, 22),
                Diagnostic().WithLocation(8, 22),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            string contentWithoutParamDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
    </Method>
</ClassName>
";
            string contentWithParamDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
        <param name=""foo"">Param 1</param>
        <param name=""bar"">Param 2</param>
    </Method>
</ClassName>
";
            string contentWithInvalidParamDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
        <param>Test</param>
        <param/>
        <param name="""">Test</param>
        <param name=""    "">Test</param>
    </Method>
</ClassName>
";
            string contentWithInheritedDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
 <ClassName>
    <Method>
        <inheritdoc />
        <param>Test</param>
        <param/>
        <param name="""">Test</param>
        <param name=""    "">Test</param>
    </Method>
 </ClassName>
 ";

            var test = new StyleCopDiagnosticVerifier<SA1613ElementParameterDocumentationMustDeclareParameterName>.CSharpTest
            {
                TestCode = source,
                XmlReferences =
                {
                    { "MissingParamDocumentation.xml", contentWithoutParamDocumentation },
                    { "WithParamDocumentation.xml", contentWithParamDocumentation },
                    { "WithInvalidParamDocumentation.xml", contentWithInvalidParamDocumentation },
                    { "WithInheritedDocumentation.xml", contentWithInheritedDocumentation },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}

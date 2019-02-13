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
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<Analyzers.DocumentationRules.SA1614ElementParameterDocumentationMustHaveText>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1614ElementParameterDocumentationMustHaveText"/>.
    /// </summary>
    public class SA1614UnitTests
    {
        public static IEnumerable<object[]> Declarations
        {
            get
            {
                yield return new[] { "    public ClassName Method(string foo, string bar) { return null; }" };
                yield return new[] { "    public ClassName this[string foo, string bar] { get { return null; } set { } }" };
            }
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberNoDocumentationAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
$$
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
        public async Task TestMemberWithEmptyParamsAsync(string declaration)
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
    ///<param name=""foo""></param>
    ///<param name=""bar"">   

    ///</param>
$$
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(10, 8),
                Diagnostic().WithLocation(11, 8),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithEmptyParams2Async(string declaration)
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
    ///<param name=""foo""/>
    ///<param name=""bar"">
    ///<para>
    ///     
    ///</para>
    ///</param>
$$
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(10, 8),
                Diagnostic().WithLocation(11, 8),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberIncludedDocumentationNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='NoDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberIncludedDocumentationWithoutParamsAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='NoParamDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberIncludedDocumentationWithValidParamsAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='ParamDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberIncludedDocumentationWithEmptyParamsAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='EmptyParamDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(8, 22),
                Diagnostic().WithLocation(8, 22),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberIncludedDocumentationWithEmptyParams2Async()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='EmptyParamDocumentation2.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(8, 22),
                Diagnostic().WithLocation(8, 22),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            string contentWithoutDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
    </Method>
</ClassName>
";
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
        <param name=""foo"">Test</param>
        <param name=""bar"">Test</param>
    </Method>
</ClassName>
";
            string contentWithEmptyParamDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
        <param name=""foo""></param>
        <param name=""bar"">   
            
        </param>
    </Method>
</ClassName>
";
            string contentWithEmptyParamDocumentation2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
        <param name=""foo""/>
        <param name=""bar"">
            <para>
                 
            </para>
        </param>
    </Method>
</ClassName>
";

            var test = new StyleCopDiagnosticVerifier<SA1614ElementParameterDocumentationMustHaveText>.CSharpTest
            {
                TestCode = source,
                XmlReferences =
                {
                    { "NoDocumentation.xml", contentWithoutDocumentation },
                    { "NoParamDocumentation.xml", contentWithoutParamDocumentation },
                    { "ParamDocumentation.xml", contentWithParamDocumentation },
                    { "EmptyParamDocumentation.xml", contentWithEmptyParamDocumentation },
                    { "EmptyParamDocumentation2.xml", contentWithEmptyParamDocumentation2 },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1614ElementParameterDocumentationMustHaveText"/>.
    /// </summary>
    public class SA1614UnitTests : DiagnosticVerifier
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(10, 8),
                this.CSharpDiagnostic().WithLocation(11, 8),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(10, 8),
                this.CSharpDiagnostic().WithLocation(11, 8),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(8, 22),
                this.CSharpDiagnostic().WithLocation(8, 22),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(8, 22),
                this.CSharpDiagnostic().WithLocation(8, 22),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();

            string contentWithoutDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
    </Method>
</ClassName>
";
            resolver.XmlReferences.Add("NoDocumentation.xml", contentWithoutDocumentation);

            string contentWithoutParamDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
    </Method>
</ClassName>
";
            resolver.XmlReferences.Add("NoParamDocumentation.xml", contentWithoutParamDocumentation);

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
            resolver.XmlReferences.Add("ParamDocumentation.xml", contentWithParamDocumentation);

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
            resolver.XmlReferences.Add("EmptyParamDocumentation.xml", contentWithEmptyParamDocumentation);

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
            resolver.XmlReferences.Add("EmptyParamDocumentation2.xml", contentWithEmptyParamDocumentation2);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1614ElementParameterDocumentationMustHaveText();
        }
    }
}

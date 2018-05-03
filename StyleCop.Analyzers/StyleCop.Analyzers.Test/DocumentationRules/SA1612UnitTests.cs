// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.DocumentationRules;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1612ElementParameterDocumentationMustMatchElementParameters"/>.
    /// </summary>
    public class SA1612UnitTests : DiagnosticVerifier
    {
        private string currentTestSettings;

        public static IEnumerable<object[]> Declarations
        {
            get
            {
                yield return new[] { "    public ClassName Method(string foo, string bar, string @new) { return null; }" };
                yield return new[] { "    public delegate ClassName Method(string foo, string bar, string @new);" };
                yield return new[] { "    public ClassName this[string foo, string bar, string @new] { get { return null; } set { } }" };
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    ///<param name=""new"">Test</param>
$$
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        [WorkItem(2452, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2452")]
        public async Task TestMemberWithMissingNotRequiredParamAsync(string declaration)
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
    ///<param name=""new"">Test</param>
$$
}";

            this.currentTestSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentExposedElements"": false
    }
  }
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        [WorkItem(2452, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2452")]
        public async Task TestMemberWithMissingNotRequiredReorderedParamAsync(string declaration)
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
    ///<param name=""new"">Test</param>
    ///<param name=""foo"">Test</param>
$$
}";

            var diagnostic = this.CSharpDiagnostic()
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}.");

            var expected = new[]
            {
                diagnostic.WithLocation(10, 21).WithArguments("new", 3),
                diagnostic.WithLocation(11, 21).WithArguments("foo", 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
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
    ///<param name=""boo"">Test</param>
    ///<param name=""far"">Test</param>
    ///<param name=""foe"">Test</param>
$$
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 21).WithArguments("boo"),
                this.CSharpDiagnostic().WithLocation(11, 21).WithArguments("far"),
                this.CSharpDiagnostic().WithLocation(12, 21).WithArguments("foe"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithInvalidParamsThatShouldBeHandledBySA1613Async(string declaration)
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
    ///<param name=""  "">Test</param>
$$
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMembersWithAllDocumentationWrongOrderAsync(string p)
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
    /// <param name=""bar"">Param 2</param>
    /// <param name=""new"">Param 3</param>
    /// <param name=""foo"">Param 1</param>
    $$
}";

            var diagnostic = this.CSharpDiagnostic()
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}.");

            var expected = new[]
            {
                diagnostic.WithLocation(10, 22).WithArguments("bar", 2),
                diagnostic.WithLocation(11, 22).WithArguments("new", 3),
                diagnostic.WithLocation(12, 22).WithArguments("foo", 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMembersWithTooManyDocumentationAsync(string p)
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
    /// <param name=""foo"">Param 1</param>
    /// <param name=""bar"">Param 2</param>
    /// <param name=""new"">Param 3</param>
    /// <param name=""bar"">Param 4</param>
    $$
}";

            var diagnostic = this.CSharpDiagnostic()
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}.");

            var expected = diagnostic.WithLocation(13, 22).WithArguments("bar", 2);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task VerifyInheritedDocumentationReportsNoDiagnosticsAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    $$
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyIncludedMemberWithoutParamsIsNotReportedAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='MissingParamDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method() { return null; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyIncludedMemberWithValidParamsIsNotReportedAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithParamDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar, string @new) { return null; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyIncludedMemberWithInvalidParamsIsReportedAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithInvalidParamDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar, string @new) { return null; }
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(8, 22).WithArguments("boo"),
                this.CSharpDiagnostic().WithLocation(8, 22).WithArguments("far"),
                this.CSharpDiagnostic().WithLocation(8, 22).WithArguments("foe"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyIncludedMemberWithInvalidParamsThatShouldBeHandledBySA1613IsNotReportedAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithSA1613ParamDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar, string @new) { return null; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyIncludedMemberWithAllDocumentationWrongOrderIsReportedAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithParamDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string bar, string @new, string foo) { return null; }
}";

            var diagnostic = this.CSharpDiagnostic()
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}.");

            var expected = new[]
            {
                diagnostic.WithLocation(8, 22).WithArguments("foo", 3),
                diagnostic.WithLocation(8, 22).WithArguments("bar", 1),
                diagnostic.WithLocation(8, 22).WithArguments("new", 2),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            // This is even reported if the documentation is not required, except that no warning is reported for the
            // first param element (which is actually the last parameter) since it would otherwise be allowed to skip
            // the documentation for the first two parameters.
            this.currentTestSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentExposedElements"": false
    }
  }
}
";

            expected = new[]
            {
                diagnostic.WithLocation(8, 22).WithArguments("bar", 1),
                diagnostic.WithLocation(8, 22).WithArguments("new", 2),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyIncludedMemberWithTooManyDocumentationIsReportedAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithTooManyParamDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar, string @new) { return null; }
}";

            var diagnostic = this.CSharpDiagnostic()
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}.");

            var expected = diagnostic.WithLocation(8, 22).WithArguments("bar", 2);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyIncludedInheritedDocumentationIsNotReportedAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithInheritedDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar, string @new) { return null; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();

            string contentWithoutParamDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
    </Method>
</ClassName>
";
            resolver.XmlReferences.Add("MissingParamDocumentation.xml", contentWithoutParamDocumentation);

            string contentWithParamDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
        <param name=""foo"">Param 1</param>
        <param name=""bar"">Param 2</param>
        <param name=""new"">Param 3</param>
    </Method>
</ClassName>
";
            resolver.XmlReferences.Add("WithParamDocumentation.xml", contentWithParamDocumentation);

            string contentWithInvalidParamDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
        <param name=""boo"">Param 1</param>
        <param name=""far"">Param 2</param>
        <param name=""foe"">Param 3</param>
    </Method>
</ClassName>
";
            resolver.XmlReferences.Add("WithInvalidParamDocumentation.xml", contentWithInvalidParamDocumentation);

            string contentWithSA1613ParamDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
        <param>Test</param>
        <param/>
        <param name="""">Test</param>
        <param name=""    "">Test</param>
        <param name=""  "">Test</param>
    </Method>
</ClassName>
";
            resolver.XmlReferences.Add("WithSA1613ParamDocumentation.xml", contentWithSA1613ParamDocumentation);

            string contentWithTooManyParamDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
        <param name=""foo"">Param 1</param>
        <param name=""bar"">Param 2</param>
        <param name=""new"">Param 3</param>
        <param name=""bar"">Param 4</param>
    </Method>
</ClassName>
";
            resolver.XmlReferences.Add("WithTooManyParamDocumentation.xml", contentWithTooManyParamDocumentation);

            string contentWithInheritedDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
 <ClassName>
    <Method>
        <inheritdoc />
    </Method>
 </ClassName>
 ";
            resolver.XmlReferences.Add("WithInheritedDocumentation.xml", contentWithInheritedDocumentation);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        protected override string GetSettings()
        {
            return this.currentTestSettings ?? base.GetSettings();
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1612ElementParameterDocumentationMustMatchElementParameters();
        }
    }
}

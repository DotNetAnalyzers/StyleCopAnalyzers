// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1612ElementParameterDocumentationMustMatchElementParameters>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1612ElementParameterDocumentationMustMatchElementParameters"/>.
    /// </summary>
    public class SA1612UnitTests
    {
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
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var testSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentExposedElements"": false
    }
  }
}
";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), testSettings, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var diagnostic = Diagnostic()
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}.");

            var expected = new[]
            {
                diagnostic.WithLocation(10, 21).WithArguments("new", 3),
                diagnostic.WithLocation(11, 21).WithArguments("foo", 1),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(10, 21).WithArguments("boo"),
                Diagnostic().WithLocation(11, 21).WithArguments("far"),
                Diagnostic().WithLocation(12, 21).WithArguments("foe"),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var diagnostic = Diagnostic()
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}.");

            var expected = new[]
            {
                diagnostic.WithLocation(10, 22).WithArguments("bar", 2),
                diagnostic.WithLocation(11, 22).WithArguments("new", 3),
                diagnostic.WithLocation(12, 22).WithArguments("foo", 1),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", p), expected, CancellationToken.None).ConfigureAwait(false);
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

            var diagnostic = Diagnostic()
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}.");

            var expected = diagnostic.WithLocation(13, 22).WithArguments("bar", 2);

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", p), expected, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(8, 22).WithArguments("boo"),
                Diagnostic().WithLocation(8, 22).WithArguments("far"),
                Diagnostic().WithLocation(8, 22).WithArguments("foe"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var diagnostic = Diagnostic()
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}.");

            var expected = new[]
            {
                diagnostic.WithLocation(8, 22).WithArguments("foo", 3),
                diagnostic.WithLocation(8, 22).WithArguments("bar", 1),
                diagnostic.WithLocation(8, 22).WithArguments("new", 2),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            // This is even reported if the documentation is not required, except that no warning is reported for the
            // first param element (which is actually the last parameter) since it would otherwise be allowed to skip
            // the documentation for the first two parameters.
            var testSettings = @"
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

            await VerifyCSharpDiagnosticAsync(testCode, testSettings, expected, CancellationToken.None).ConfigureAwait(false);
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

            var diagnostic = Diagnostic()
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}.");

            var expected = diagnostic.WithLocation(8, 22).WithArguments("bar", 2);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, testSettings: null, new[] { expected }, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, testSettings: null, expected, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, string testSettings, DiagnosticResult[] expected, CancellationToken cancellationToken)
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
        <param name=""new"">Param 3</param>
    </Method>
</ClassName>
";
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
            string contentWithInheritedDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
 <ClassName>
    <Method>
        <inheritdoc />
    </Method>
 </ClassName>
 ";

            var test = new StyleCopDiagnosticVerifier<SA1612ElementParameterDocumentationMustMatchElementParameters>.CSharpTest
            {
                TestCode = source,
                Settings = testSettings,
                XmlReferences =
                {
                    { "MissingParamDocumentation.xml", contentWithoutParamDocumentation },
                    { "WithParamDocumentation.xml", contentWithParamDocumentation },
                    { "WithInvalidParamDocumentation.xml", contentWithInvalidParamDocumentation },
                    { "WithSA1613ParamDocumentation.xml", contentWithSA1613ParamDocumentation },
                    { "WithTooManyParamDocumentation.xml", contentWithTooManyParamDocumentation },
                    { "WithInheritedDocumentation.xml", contentWithInheritedDocumentation },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}

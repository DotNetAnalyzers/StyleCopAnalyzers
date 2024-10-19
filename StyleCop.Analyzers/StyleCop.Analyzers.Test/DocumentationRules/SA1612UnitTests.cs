﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.Verifiers;
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
                yield return new[] { "    public ClassName {|#0:Method|}(string foo, string bar, string @new) { return null; }" };
                yield return new[] { "    public delegate ClassName {|#0:Method|}(string foo, string bar, string @new);" };
                yield return new[] { "    public ClassName {|#0:this|}[string foo, string bar, string @new] { get { return null; } set { } }" };

                if (LightupHelpers.SupportsCSharp9)
                {
                    yield return new[] { "    public record {|#0:TestType|}(string foo, string bar, string @new) {}" };
                }

                if (LightupHelpers.SupportsCSharp10)
                {
                    yield return new[] { "    public record struct {|#0:TestType|}(string foo, string bar, string @new) {}" };
                    yield return new[] { "    public record class {|#0:TestType|}(string foo, string bar, string @new) {}" };
                }

                if (LightupHelpers.SupportsCSharp12)
                {
                    yield return new[] { "    public struct {|#0:TestType|}(string foo, string bar, string @new) {}" };
                    yield return new[] { "    public class {|#0:TestType|}(string foo, string bar, string @new) {}" };
                }
            }
        }

        [Fact]
        public async Task VerifyClassIsNotReportedAsync()
        {
            var testCode = @"
    /// <summary>
    /// Foo
    /// </summary>
public class ClassName
{
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMethodWithNoParametersIsNotReportedAsync()
        {
            var testCode = @"
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public void Method() { }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    ///<param name=""new"">Test</param>
$$
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), testSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}");

            var normallyExpected = new[]
            {
                diagnostic.WithLocation(10, 21).WithArguments("new", 3),
                diagnostic.WithLocation(11, 21).WithArguments("foo", 1),
            };
            var expected = GetExpectedDiagnostics(normallyExpected, declaration);

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

            var normallyExpected = new[]
            {
                Diagnostic().WithLocation(10, 21).WithArguments("boo"),
                Diagnostic().WithLocation(11, 21).WithArguments("far"),
                Diagnostic().WithLocation(12, 21).WithArguments("foe"),
            };
            var expected = GetExpectedDiagnostics(normallyExpected, declaration);

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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMembersWithAllDocumentationWrongOrderAsync(string declaration)
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
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}");

            var normallyExpected = new[]
            {
                diagnostic.WithLocation(10, 22).WithArguments("bar", 2),
                diagnostic.WithLocation(11, 22).WithArguments("new", 3),
                diagnostic.WithLocation(12, 22).WithArguments("foo", 1),
            };
            var expected = GetExpectedDiagnostics(normallyExpected, declaration);

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);

            var testSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentExposedElements"": false
    }
  }
}
";

            normallyExpected = new[]
            {
                diagnostic.WithLocation(12, 22).WithArguments("foo", 1),
            };
            expected = GetExpectedDiagnostics(normallyExpected, declaration);

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), testSettings, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMembersWithTooManyDocumentationAsync(string declaration)
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
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}");

            var normallyExpected = diagnostic.WithLocation(13, 22).WithArguments("bar", 2);
            var expected = GetExpectedDiagnostics(normallyExpected, declaration);

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateWithoutIdentifierWithTooManyDocumentationIsNotReportedAsync()
        {
            var testCode = @"
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    /// <param name=""foo"">Param 1</param>
    /// <param name=""bar"">Param 2</param>
    public delegate void (int foo);
}";

            await VerifyCSharpDiagnosticAsync(testCode, testSettings: null, DiagnosticResult.EmptyDiagnosticResults, ignoreCompilerDiagnostics: true, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyIncludedClassIsNotReportedAsync()
        {
            var testCode = @"
/// <include file='MissingParamDocumentation.xml' path='/ClassName/Method/*' />
public class ClassName
{
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3150, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3150")]
        public async Task VerifyIncludedMissingFileIsNotReportedAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='MissingFile.xml' path='/ClassName/Method/*' />
    public ClassName Method() { return null; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task VerifyIncludedMemberWithValidParamsIsNotReportedAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithParamDocumentation.xml' path='/ClassName/Method/*' />
$$
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task VerifyIncludedMemberWithInvalidParamsIsReportedAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithInvalidParamDocumentation.xml' path='/ClassName/Method/*' />
$$
}";

            var normallyExpected = new[]
            {
                Diagnostic().WithLocation(0).WithArguments("boo"),
                Diagnostic().WithLocation(0).WithArguments("far"),
                Diagnostic().WithLocation(0).WithArguments("foe"),
            };
            var expected = GetExpectedDiagnostics(normallyExpected, declaration);

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task VerifyIncludedMemberWithAllDocumentationWrongOrderIsReportedAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithWrongOrderParamDocumentation.xml' path='/ClassName/Method/*' />
$$
}";

            var diagnostic = Diagnostic()
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}");

            var normallyExpected = new[]
            {
                diagnostic.WithLocation(0).WithArguments("new", 3),
                diagnostic.WithLocation(0).WithArguments("foo", 1),
                diagnostic.WithLocation(0).WithArguments("bar", 2),
            };
            var expected = GetExpectedDiagnostics(normallyExpected, declaration);

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);

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

            normallyExpected = new[]
            {
                diagnostic.WithLocation(0).WithArguments("foo", 1),
                diagnostic.WithLocation(0).WithArguments("bar", 2),
            };
            expected = GetExpectedDiagnostics(normallyExpected, declaration);

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), testSettings, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task VerifyIncludedMemberWithTooManyDocumentationIsReportedAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithTooManyParamDocumentation.xml' path='/ClassName/Method/*' />
$$
}";

            var diagnostic = Diagnostic()
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}");

            var normallyExpected = diagnostic.WithLocation(0).WithArguments("bar", 2);
            var expected = GetExpectedDiagnostics(normallyExpected, declaration);

            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyIncludedDelegateWithoutIdentifierWithTooManyDocumentationIsNotReportedAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithTooManyParamDocumentation.xml' path='/ClassName/Method/*' />
    public delegate void (int foo);
}";

            await VerifyCSharpDiagnosticAsync(testCode, testSettings: null, DiagnosticResult.EmptyDiagnosticResults, ignoreCompilerDiagnostics: true, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static DiagnosticResult[] GetExpectedDiagnostics(DiagnosticResult normallyExpected, string declaration)
        {
            return GetExpectedDiagnostics(new[] { normallyExpected }, declaration);
        }

        // Syntax node actions for type declarations with a primary constructor were called twice
        // before support for c# 11 was added.
        private static DiagnosticResult[] GetExpectedDiagnostics(DiagnosticResult[] normallyExpected, string declaration)
        {
            var isPrimaryConstructor = declaration.Contains("record") || declaration.Contains("class") || declaration.Contains("struct");

            if (isPrimaryConstructor && !LightupHelpers.SupportsCSharp11)
            {
                // Diagnostic issued twice because of https://github.com/dotnet/roslyn/issues/53136 and https://github.com/dotnet/roslyn/issues/70488
                return normallyExpected.Concat(normallyExpected).ToArray();
            }
            else
            {
                return normallyExpected;
            }
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, testSettings: null, expected, ignoreCompilerDiagnostics: false, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, string testSettings, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, testSettings, expected, ignoreCompilerDiagnostics: false, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, string testSettings, DiagnosticResult[] expected, bool ignoreCompilerDiagnostics, CancellationToken cancellationToken)
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
            string contentWithWrongOrderParamDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
        <param name=""new"">Param 3</param>
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
                    { "WithWrongOrderParamDocumentation.xml", contentWithWrongOrderParamDocumentation },
                    { "WithInvalidParamDocumentation.xml", contentWithInvalidParamDocumentation },
                    { "WithSA1613ParamDocumentation.xml", contentWithSA1613ParamDocumentation },
                    { "WithTooManyParamDocumentation.xml", contentWithTooManyParamDocumentation },
                    { "WithInheritedDocumentation.xml", contentWithInheritedDocumentation },
                },
            };

            if (ignoreCompilerDiagnostics)
            {
                test.CompilerDiagnostics = CompilerDiagnostics.None;
            }

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}

﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1600ElementsMustBeDocumented,
        StyleCop.Analyzers.DocumentationRules.SA1600CodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1600ElementsMustBeDocumented"/>.
    /// </summary>
    public class SA1600UnitTests
    {
        protected virtual LanguageVersion LanguageVersion => LanguageVersion.CSharp6;

        [Theory]
        [InlineData("public string {|#0:TestMember|};")]
        [InlineData("public string {|#0:TestMember|} { get; set; }")]
        [InlineData("public void {|#0:TestMember|}() { }")]
        [InlineData("public string {|#0:this|}[int a] { get { return \"a\"; } set { } }")]
        [InlineData("public event EventHandler {|#0:TestMember|} { add { } remove { } }")]
        public async Task TestRegressionMethodGlobalNamespaceAsync(string code)
        {
            // This test is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1416
            var testCode = $@"
using System;

{code}";

            var expected = this.GetExpectedResultTestRegressionMethodGlobalNamespace(code);
            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestBaseTypeWithoutDocumentationAsync(string type)
        {
            var isInterface = type == "interface";
            await this.TestTypeWithoutDocumentationAsync(type, isInterface).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestBaseTypeWithDocumentationAsync(string type)
        {
            await this.TestTypeWithDocumentationAsync(type).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestPartialTypeWithoutDocumentationAsync(string type)
        {
            await this.TestTypeDeclarationDocumentationAsync(type, "partial", false, false).ConfigureAwait(false);
            await this.TestTypeDeclarationDocumentationAsync(type, "internal partial", false, false).ConfigureAwait(false);
            await this.TestTypeDeclarationDocumentationAsync(type, "public partial", false, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateWithoutDocumentationAsync()
        {
            await this.TestDelegateDeclarationDocumentationAsync(string.Empty, true, false).ConfigureAwait(false);
            await this.TestDelegateDeclarationDocumentationAsync("internal", true, false).ConfigureAwait(false);
            await this.TestDelegateDeclarationDocumentationAsync("public", true, false).ConfigureAwait(false);

            await this.TestNestedDelegateDeclarationDocumentationAsync(string.Empty, false, false).ConfigureAwait(false);
            await this.TestNestedDelegateDeclarationDocumentationAsync("private", false, false).ConfigureAwait(false);
            await this.TestNestedDelegateDeclarationDocumentationAsync("protected", true, false).ConfigureAwait(false);
            await this.TestNestedDelegateDeclarationDocumentationAsync("internal", true, false).ConfigureAwait(false);
            await this.TestNestedDelegateDeclarationDocumentationAsync("protected internal", true, false).ConfigureAwait(false);
            await this.TestNestedDelegateDeclarationDocumentationAsync("public", true, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateWithDocumentationAsync()
        {
            await this.TestDelegateDeclarationDocumentationAsync(string.Empty, false, true).ConfigureAwait(false);
            await this.TestDelegateDeclarationDocumentationAsync("internal", false, true).ConfigureAwait(false);
            await this.TestDelegateDeclarationDocumentationAsync("public", false, true).ConfigureAwait(false);

            await this.TestNestedDelegateDeclarationDocumentationAsync(string.Empty, false, true).ConfigureAwait(false);
            await this.TestNestedDelegateDeclarationDocumentationAsync("private", false, true).ConfigureAwait(false);
            await this.TestNestedDelegateDeclarationDocumentationAsync("protected", false, true).ConfigureAwait(false);
            await this.TestNestedDelegateDeclarationDocumentationAsync("internal", false, true).ConfigureAwait(false);
            await this.TestNestedDelegateDeclarationDocumentationAsync("protected internal", false, true).ConfigureAwait(false);
            await this.TestNestedDelegateDeclarationDocumentationAsync("public", false, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithoutDocumentationAsync()
        {
            await this.TestMethodDeclarationDocumentationAsync(string.Empty, false, false, false).ConfigureAwait(false);
            await this.TestMethodDeclarationDocumentationAsync(string.Empty, true, true, false).ConfigureAwait(false);
            await this.TestMethodDeclarationDocumentationAsync("private", false, false, false).ConfigureAwait(false);
            await this.TestMethodDeclarationDocumentationAsync("protected", false, true, false).ConfigureAwait(false);
            await this.TestMethodDeclarationDocumentationAsync("internal", false, true, false).ConfigureAwait(false);
            await this.TestMethodDeclarationDocumentationAsync("protected internal", false, true, false).ConfigureAwait(false);
            await this.TestMethodDeclarationDocumentationAsync("public", false, true, false).ConfigureAwait(false);

            await this.TestInterfaceMethodDeclarationDocumentationAsync(false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithDocumentationAsync()
        {
            await this.TestMethodDeclarationDocumentationAsync(string.Empty, false, false, true).ConfigureAwait(false);
            await this.TestMethodDeclarationDocumentationAsync(string.Empty, true, false, true).ConfigureAwait(false);
            await this.TestMethodDeclarationDocumentationAsync("private", false, false, true).ConfigureAwait(false);
            await this.TestMethodDeclarationDocumentationAsync("protected", false, false, true).ConfigureAwait(false);
            await this.TestMethodDeclarationDocumentationAsync("internal", false, false, true).ConfigureAwait(false);
            await this.TestMethodDeclarationDocumentationAsync("protected internal", false, false, true).ConfigureAwait(false);
            await this.TestMethodDeclarationDocumentationAsync("public", false, false, true).ConfigureAwait(false);

            await this.TestInterfaceMethodDeclarationDocumentationAsync(true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorWithoutDocumentationAsync()
        {
            await this.TestConstructorDeclarationDocumentationAsync(string.Empty, false, false).ConfigureAwait(false);
            await this.TestConstructorDeclarationDocumentationAsync("private", false, false).ConfigureAwait(false);
            await this.TestConstructorDeclarationDocumentationAsync("protected", true, false).ConfigureAwait(false);
            await this.TestConstructorDeclarationDocumentationAsync("internal", true, false).ConfigureAwait(false);
            await this.TestConstructorDeclarationDocumentationAsync("protected internal", true, false).ConfigureAwait(false);
            await this.TestConstructorDeclarationDocumentationAsync("public", true, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorWithDocumentationAsync()
        {
            await this.TestConstructorDeclarationDocumentationAsync(string.Empty, false, true).ConfigureAwait(false);
            await this.TestConstructorDeclarationDocumentationAsync("private", false, true).ConfigureAwait(false);
            await this.TestConstructorDeclarationDocumentationAsync("protected", false, true).ConfigureAwait(false);
            await this.TestConstructorDeclarationDocumentationAsync("internal", false, true).ConfigureAwait(false);
            await this.TestConstructorDeclarationDocumentationAsync("protected internal", false, true).ConfigureAwait(false);
            await this.TestConstructorDeclarationDocumentationAsync("public", false, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorWithoutDocumentationAsync()
        {
            await this.TestDestructorDeclarationDocumentationAsync(true, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorWithDocumentationAsync()
        {
            await this.TestDestructorDeclarationDocumentationAsync(false, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWithoutDocumentationAsync()
        {
            await this.TestFieldDeclarationDocumentationAsync(testSettings: null, string.Empty, false, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings: null, "private", false, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings: null, "protected", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings: null, "internal", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings: null, "protected internal", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings: null, "public", true, false).ConfigureAwait(false);

            // Re-test with the 'documentPrivateElements' setting enabled (doesn't impact fields)
            var testSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentPrivateElements"": true
    }
  }
}
";

            await this.TestFieldDeclarationDocumentationAsync(testSettings, string.Empty, false, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "private", false, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "protected", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "internal", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "protected internal", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "public", true, false).ConfigureAwait(false);

            // Re-test with the 'documentInternalElements' setting disabled (does impact fields)
            testSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentInternalElements"": false
    }
  }
}
";

            await this.TestFieldDeclarationDocumentationAsync(testSettings, string.Empty, false, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "private", false, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "protected", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "internal", false, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "protected internal", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "public", true, false).ConfigureAwait(false);

            // Re-test with the 'documentPrivateFields' setting enabled (does impact fields)
            testSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentPrivateFields"": true
    }
  }
}
";

            await this.TestFieldDeclarationDocumentationAsync(testSettings, string.Empty, true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "private", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "protected", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "internal", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "protected internal", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "public", true, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWithDocumentationAsync()
        {
            await this.TestFieldDeclarationDocumentationAsync(testSettings: null, string.Empty, false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings: null, "private", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings: null, "protected", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings: null, "internal", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings: null, "protected internal", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings: null, "public", false, true).ConfigureAwait(false);

            // Re-test with the 'documentPrivateElements' setting enabled (doesn't impact fields)
            var testSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentPrivateElements"": true
    }
  }
}
";

            await this.TestFieldDeclarationDocumentationAsync(testSettings, string.Empty, false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "private", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "protected", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "internal", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "protected internal", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "public", false, true).ConfigureAwait(false);

            // Re-test with the 'documentInternalElements' setting disabled (does impact fields)
            testSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentInternalElements"": false
    }
  }
}
";

            await this.TestFieldDeclarationDocumentationAsync(testSettings, string.Empty, false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "private", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "protected", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "internal", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "protected internal", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "public", false, true).ConfigureAwait(false);

            // Re-test with the 'documentPrivateFields' setting enabled (does impact fields)
            testSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentPrivateFields"": true
    }
  }
}
";

            await this.TestFieldDeclarationDocumentationAsync(testSettings, string.Empty, false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "private", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "protected", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "internal", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "protected internal", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync(testSettings, "public", false, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithoutDocumentationAsync()
        {
            await this.TestPropertyDeclarationDocumentationAsync(string.Empty, false, false, false).ConfigureAwait(false);
            await this.TestPropertyDeclarationDocumentationAsync(string.Empty, true, true, false).ConfigureAwait(false);
            await this.TestPropertyDeclarationDocumentationAsync("private", false, false, false).ConfigureAwait(false);
            await this.TestPropertyDeclarationDocumentationAsync("protected", false, true, false).ConfigureAwait(false);
            await this.TestPropertyDeclarationDocumentationAsync("internal", false, true, false).ConfigureAwait(false);
            await this.TestPropertyDeclarationDocumentationAsync("protected internal", false, true, false).ConfigureAwait(false);
            await this.TestPropertyDeclarationDocumentationAsync("public", false, true, false).ConfigureAwait(false);

            await this.TestInterfacePropertyDeclarationDocumentationAsync(false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithDocumentationAsync()
        {
            await this.TestPropertyDeclarationDocumentationAsync(string.Empty, false, false, true).ConfigureAwait(false);
            await this.TestPropertyDeclarationDocumentationAsync(string.Empty, true, false, true).ConfigureAwait(false);
            await this.TestPropertyDeclarationDocumentationAsync("private", false, false, true).ConfigureAwait(false);
            await this.TestPropertyDeclarationDocumentationAsync("protected", false, false, true).ConfigureAwait(false);
            await this.TestPropertyDeclarationDocumentationAsync("internal", false, false, true).ConfigureAwait(false);
            await this.TestPropertyDeclarationDocumentationAsync("protected internal", false, false, true).ConfigureAwait(false);
            await this.TestPropertyDeclarationDocumentationAsync("public", false, false, true).ConfigureAwait(false);

            await this.TestInterfacePropertyDeclarationDocumentationAsync(true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerWithoutDocumentationAsync()
        {
            await this.TestIndexerDeclarationDocumentationAsync(string.Empty, false, false, false).ConfigureAwait(false);
            await this.TestIndexerDeclarationDocumentationAsync(string.Empty, true, true, false).ConfigureAwait(false);
            await this.TestIndexerDeclarationDocumentationAsync("private", false, false, false).ConfigureAwait(false);
            await this.TestIndexerDeclarationDocumentationAsync("protected", false, true, false).ConfigureAwait(false);
            await this.TestIndexerDeclarationDocumentationAsync("internal", false, true, false).ConfigureAwait(false);
            await this.TestIndexerDeclarationDocumentationAsync("protected internal", false, true, false).ConfigureAwait(false);
            await this.TestIndexerDeclarationDocumentationAsync("public", false, true, false).ConfigureAwait(false);

            await this.TestInterfaceIndexerDeclarationDocumentationAsync(false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerWithDocumentationAsync()
        {
            await this.TestIndexerDeclarationDocumentationAsync(string.Empty, false, false, true).ConfigureAwait(false);
            await this.TestIndexerDeclarationDocumentationAsync(string.Empty, true, false, true).ConfigureAwait(false);
            await this.TestIndexerDeclarationDocumentationAsync("private", false, false, true).ConfigureAwait(false);
            await this.TestIndexerDeclarationDocumentationAsync("protected", false, false, true).ConfigureAwait(false);
            await this.TestIndexerDeclarationDocumentationAsync("internal", false, false, true).ConfigureAwait(false);
            await this.TestIndexerDeclarationDocumentationAsync("protected internal", false, false, true).ConfigureAwait(false);
            await this.TestIndexerDeclarationDocumentationAsync("public", false, false, true).ConfigureAwait(false);

            await this.TestInterfaceIndexerDeclarationDocumentationAsync(true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventWithoutDocumentationAsync()
        {
            await this.TestEventDeclarationDocumentationAsync(string.Empty, false, false, false).ConfigureAwait(false);
            await this.TestEventDeclarationDocumentationAsync(string.Empty, true, true, false).ConfigureAwait(false);
            await this.TestEventDeclarationDocumentationAsync("private", false, false, false).ConfigureAwait(false);
            await this.TestEventDeclarationDocumentationAsync("protected", false, true, false).ConfigureAwait(false);
            await this.TestEventDeclarationDocumentationAsync("internal", false, true, false).ConfigureAwait(false);
            await this.TestEventDeclarationDocumentationAsync("protected internal", false, true, false).ConfigureAwait(false);
            await this.TestEventDeclarationDocumentationAsync("public", false, true, false).ConfigureAwait(false);

            await this.TestInterfaceEventDeclarationDocumentationAsync(false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventWithDocumentationAsync()
        {
            await this.TestEventDeclarationDocumentationAsync(string.Empty, false, false, true).ConfigureAwait(false);
            await this.TestEventDeclarationDocumentationAsync(string.Empty, true, false, true).ConfigureAwait(false);
            await this.TestEventDeclarationDocumentationAsync("private", false, false, true).ConfigureAwait(false);
            await this.TestEventDeclarationDocumentationAsync("protected", false, false, true).ConfigureAwait(false);
            await this.TestEventDeclarationDocumentationAsync("internal", false, false, true).ConfigureAwait(false);
            await this.TestEventDeclarationDocumentationAsync("protected internal", false, false, true).ConfigureAwait(false);
            await this.TestEventDeclarationDocumentationAsync("public", false, false, true).ConfigureAwait(false);

            await this.TestInterfaceEventDeclarationDocumentationAsync(true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventFieldWithoutDocumentationAsync()
        {
            await this.TestEventFieldDeclarationDocumentationAsync(string.Empty, false, false).ConfigureAwait(false);
            await this.TestEventFieldDeclarationDocumentationAsync("private", false, false).ConfigureAwait(false);
            await this.TestEventFieldDeclarationDocumentationAsync("protected", true, false).ConfigureAwait(false);
            await this.TestEventFieldDeclarationDocumentationAsync("internal", true, false).ConfigureAwait(false);
            await this.TestEventFieldDeclarationDocumentationAsync("protected internal", true, false).ConfigureAwait(false);
            await this.TestEventFieldDeclarationDocumentationAsync("public", true, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventFieldWithDocumentationAsync()
        {
            await this.TestEventFieldDeclarationDocumentationAsync(string.Empty, false, true).ConfigureAwait(false);
            await this.TestEventFieldDeclarationDocumentationAsync("private", false, true).ConfigureAwait(false);
            await this.TestEventFieldDeclarationDocumentationAsync("protected", false, true).ConfigureAwait(false);
            await this.TestEventFieldDeclarationDocumentationAsync("internal", false, true).ConfigureAwait(false);
            await this.TestEventFieldDeclarationDocumentationAsync("protected internal", false, true).ConfigureAwait(false);
            await this.TestEventFieldDeclarationDocumentationAsync("public", false, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyXmlCommentsAsync()
        {
            var testCodeWithEmptyDocumentation = @"    /// <summary>
    /// </summary>
public class OuterClass
{
}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass
{
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 14);

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, testCodeWithDocumentation, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, testCodeWithEmptyDocumentation, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCDataXmlCommentsAsync()
        {
            var testCodeWithEmptyDocumentation = @"/// <summary>
    /// <![CDATA[]]>
    /// </summary>
public class OuterClass
{
}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// <![CDATA[A summary.]]>
    /// </summary>
public class OuterClass
{
}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 14);

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, testCodeWithDocumentation, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, testCodeWithEmptyDocumentation, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyElementXmlCommentsAsync()
        {
            var testCodeWithDocumentation = @"/// <inheritdoc/>
public class OuterClass
{
}";

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, testCodeWithDocumentation, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultiLineDocumentationAsync()
        {
            var testCodeWithDocumentation = @"
/**
 * <summary>This is a documentation comment summary.</summary>
 */
public class OuterClass
{
    /**
     * <summary>This is a documentation comment summary.</summary>
     */
    public void SomeMethod() { }
}";

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, testCodeWithDocumentation, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that we recognize the auto-generated xml element.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSkipAutoGeneratedCodeAsync()
        {
            var testCode = @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

public class OuterClass
{
    public void SomeMethod() { }
}";

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that we recognize the autogenerated xml element.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSkipAutoGeneratedCode2Async()
        {
            var testCode = @"//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

public class OuterClass
{
    public void SomeMethod() { }
}";

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a code fix is offered for a constructor without documentation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConstructorCodeFixAsync()
        {
            var testCode = @"
/// <summary>
/// Test type #1.
/// </summary>
public class Test1
{
    public Test1()
    {
    }

    public Test1(int param1, bool param2)
    {
    }
}

/// <summary>
/// Test type #2.
/// </summary>
public struct Test2
{
    public Test2(int param1, bool param2)
    {
    }
}
";

            var fixedTestCode = @"
/// <summary>
/// Test type #1.
/// </summary>
public class Test1
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""Test1""/> class.
    /// </summary>
    public Test1()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref=""Test1""/> class.
    /// </summary>
    /// <param name=""param1""></param>
    /// <param name=""param2""></param>
    public Test1(int param1, bool param2)
    {
    }
}

/// <summary>
/// Test type #2.
/// </summary>
public struct Test2
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""Test2""/> struct.
    /// </summary>
    /// <param name=""param1""></param>
    /// <param name=""param2""></param>
    public Test2(int param1, bool param2)
    {
    }
}
";

            await new CSharpTest(this.LanguageVersion)
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(7, 12),
                    Diagnostic().WithLocation(11, 12),
                    Diagnostic().WithLocation(21, 12),
                },
                FixedCode = fixedTestCode,
                DisabledDiagnostics = { "CS1591" },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a code fix is offered for a destructor without documentation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDestructorCodeFixAsync()
        {
            var testCode = @"
/// <summary>
/// Test class.
/// </summary>
public class TestClass
{
    ~TestClass()
    {
    }
}
";

            var fixedTestCode = @"
/// <summary>
/// Test class.
/// </summary>
public class TestClass
{
    /// <summary>
    /// Finalizes an instance of the <see cref=""TestClass""/> class.
    /// </summary>
    ~TestClass()
    {
    }
}
";

            await new CSharpTest(this.LanguageVersion)
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(7, 6),
                },
                FixedCode = fixedTestCode,
                DisabledDiagnostics = { "CS1591" },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a code fix is not offered for normal methods without documentation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoFixForNormalMethodsAsync()
        {
            var testCode = @"
/// <summary>
/// Test class.
/// </summary>
public class TestClass
{
    public void DoSomething()
    {
    }

    public int DoSomething2()
    {
        return 0;
    }
}
";

            await new CSharpTest(this.LanguageVersion)
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(7, 17),
                    Diagnostic().WithLocation(11, 16),
                },
                FixedCode = testCode,
                DisabledDiagnostics = { "CS1591" },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a code fix is offered for methods returning a task.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestMethodReturningTaskCodeFixAsync(string typeKeyword)
        {
            var testCode = $@"
using System.Threading.Tasks;

/// <summary>
/// Test type.
/// </summary>
public {typeKeyword} Test
{{
    public Task TestMethod1()
    {{
        return Task.Delay(0);
    }}

    public Task<int> TestMethod2()
    {{
        return Task.FromResult(0);
    }}

    public Task<T> TestMethod3<T>()
    {{
        return Task.FromResult(default(T));
    }}

    public Task TestMethod4(int param1, int param2)
    {{
        return Task.Delay(0);
    }}

    public Task<int> TestMethod5(int param1, int param2)
    {{
        return Task.FromResult(param1);
    }}

    public Task<T> TestMethod6<T>(T param1, int param2)
    {{
        return Task.FromResult(param1);
    }}
}}
";

            var fixedTestCode = $@"
using System.Threading.Tasks;

/// <summary>
/// Test type.
/// </summary>
public {typeKeyword} Test
{{
    /// <summary>
    ///
    /// </summary>
    /// <returns>A <see cref=""Task""/> representing the result of the asynchronous operation.</returns>
    public Task TestMethod1()
    {{
        return Task.Delay(0);
    }}

    /// <summary>
    ///
    /// </summary>
    /// <returns>A <see cref=""Task{{TResult}}""/> representing the result of the asynchronous operation.</returns>
    public Task<int> TestMethod2()
    {{
        return Task.FromResult(0);
    }}

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name=""T""></typeparam>
    /// <returns>A <see cref=""Task{{TResult}}""/> representing the result of the asynchronous operation.</returns>
    public Task<T> TestMethod3<T>()
    {{
        return Task.FromResult(default(T));
    }}

    /// <summary>
    ///
    /// </summary>
    /// <param name=""param1""></param>
    /// <param name=""param2""></param>
    /// <returns>A <see cref=""Task""/> representing the result of the asynchronous operation.</returns>
    public Task TestMethod4(int param1, int param2)
    {{
        return Task.Delay(0);
    }}

    /// <summary>
    ///
    /// </summary>
    /// <param name=""param1""></param>
    /// <param name=""param2""></param>
    /// <returns>A <see cref=""Task{{TResult}}""/> representing the result of the asynchronous operation.</returns>
    public Task<int> TestMethod5(int param1, int param2)
    {{
        return Task.FromResult(param1);
    }}

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name=""T""></typeparam>
    /// <param name=""param1""></param>
    /// <param name=""param2""></param>
    /// <returns>A <see cref=""Task{{TResult}}""/> representing the result of the asynchronous operation.</returns>
    public Task<T> TestMethod6<T>(T param1, int param2)
    {{
        return Task.FromResult(param1);
    }}
}}
";

            await new CSharpTest(this.LanguageVersion)
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(9, 17),
                    Diagnostic().WithLocation(14, 22),
                    Diagnostic().WithLocation(19, 20),
                    Diagnostic().WithLocation(24, 17),
                    Diagnostic().WithLocation(29, 22),
                    Diagnostic().WithLocation(34, 20),
                },
                FixedCode = fixedTestCode,
                DisabledDiagnostics = { "CS1591" },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestTypeDeclarationDocumentationAsync(string type, string modifiers, bool requiresDiagnostic, bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"
{0} {1}
TypeName
{{
}}";
            var testCodeWithDocumentation = @"/// <summary> A summary. </summary>
{0} {1}
TypeName
{{
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(3, 1),
                };

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, type), requiresDiagnostic ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestNestedTypeDeclarationDocumentationAsync(string type, string modifiers, bool requiresDiagnostic, bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass
{{

    {0} {1}
    TypeName
    {{
    }}
}}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass
{{
    /// <summary>A summary.</summary>
    {0} {1}
    TypeName
    {{
    }}
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(8, 5),
                };

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, type), requiresDiagnostic ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestDelegateDeclarationDocumentationAsync(string modifiers, bool requiresDiagnostic, bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"
{0} delegate void
DelegateName();";
            var testCodeWithDocumentation = @"/// <summary> A summary. </summary>
{0} delegate void
DelegateName();";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(3, 1),
                };

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers), requiresDiagnostic ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestNestedDelegateDeclarationDocumentationAsync(string modifiers, bool requiresDiagnostic, bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass
{{

    {0} delegate void
    DelegateName();
}}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass
{{
    /// <summary>A summary.</summary>
    {0} delegate void
    DelegateName();
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(8, 5),
                };

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers), requiresDiagnostic ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestMethodDeclarationDocumentationAsync(string modifiers, bool isExplicitInterfaceMethod, bool requiresDiagnostic, bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass : BaseClass, IInterface
{{

    {0} void{1}
    MemberName()
    {{
    }}
}}
#pragma warning disable SA1600 // the following code is used for ensuring the above code compiles
public class BaseClass : IInterface {{ public void MemberName() {{ }} }}
public interface IInterface {{ void MemberName(); }}
";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass : BaseClass, IInterface
{{
    /// <summary>A summary.</summary>
    {0} void{1}
    MemberName()
    {{
    }}
}}
#pragma warning disable SA1600 // the following code is used for ensuring the above code compiles
public class BaseClass : IInterface {{ public void MemberName() {{ }} }}
public interface IInterface {{ void MemberName(); }}
";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(8, 5),
                };

            string explicitInterfaceText = isExplicitInterfaceMethod ? " IInterface." : string.Empty;
            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, explicitInterfaceText), requiresDiagnostic ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestInterfaceMethodDeclarationDocumentationAsync(bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public interface InterfaceName
{

    void
    MemberName();
}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public interface InterfaceName
{
    /// <summary>A summary.</summary>
    void
    MemberName();
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(8, 5),
                };

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, !hasDocumentation ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestInterfacePropertyDeclarationDocumentationAsync(bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public interface InterfaceName
{

    
    string MemberName
    {
        get; set;
    }
}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public interface InterfaceName
{
    /// <summary>A summary.</summary>
    
    string MemberName
    {
        get; set;
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(8, 12),
                };

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, !hasDocumentation ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestInterfaceEventDeclarationDocumentationAsync(bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public interface InterfaceName
{

    
    event System.Action MemberName;
}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public interface InterfaceName
{
    /// <summary>A summary.</summary>
    
    event System.Action MemberName;
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(8, 25),
                };

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, !hasDocumentation ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestInterfaceIndexerDeclarationDocumentationAsync(bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public interface InterfaceName
{

    string
    this[string key] { get; set; }
}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public interface InterfaceName
{
    /// <summary>A summary.</summary>
    string
    this[string key] { get; set; }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(8, 5),
                };

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, !hasDocumentation ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestConstructorDeclarationDocumentationAsync(string modifiers, bool requiresDiagnostic, bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass
{{

    {0}
    OuterClass()
    {{
    }}
}}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass
{{
    /// <summary>A summary.</summary>
    {0}
    OuterClass()
    {{
    }}
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(8, 5),
                };

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers), requiresDiagnostic ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestDestructorDeclarationDocumentationAsync(bool requiresDiagnostic, bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass
{{

    ~OuterClass()
    {{
    }}
}}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass
{{
    /// <summary>A summary.</summary>
    ~OuterClass()
    {{
    }}
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(7, 6),
                };

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation), requiresDiagnostic ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestPropertyDeclarationDocumentationAsync(string modifiers, bool isExplicitInterfaceProperty, bool requiresDiagnostic, bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass : BaseClass, IInterface
{{

    {0}
    string{1}
    MemberName {{ get; set; }}
}}
#pragma warning disable SA1600 // the following code is used for ensuring the above code compiles
public class BaseClass : IInterface {{ public string MemberName {{ get; set; }} }}
public interface IInterface {{ string MemberName {{ get; set; }} }}
";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass : BaseClass, IInterface
{{
    /// <summary>A summary.</summary>
    {0}
    string{1}
    MemberName {{ get; set; }}
}}
#pragma warning disable SA1600 // the following code is used for ensuring the above code compiles
public class BaseClass : IInterface {{ public string MemberName {{ get; set; }} }}
public interface IInterface {{ string MemberName {{ get; set; }} }}
";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(9, 5),
                };

            string explicitInterfaceText = isExplicitInterfaceProperty ? " IInterface." : string.Empty;
            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, explicitInterfaceText), requiresDiagnostic ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestIndexerDeclarationDocumentationAsync(string modifiers, bool isExplicitInterfaceIndexer, bool requiresDiagnostic, bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass : BaseClass, IInterface
{{

    {0}
    string{1}
    this[string key] {{ get {{ return """"; }} set {{ }} }}
}}
#pragma warning disable SA1600 // the following code is used for ensuring the above code compiles
public class BaseClass : IInterface {{ public string this[string key] {{ get {{ return """"; }} set {{ }} }} }}
public interface IInterface {{ string this[string key] {{ get; set; }} }}
";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass : BaseClass, IInterface
{{
    /// <summary>A summary.</summary>
    {0}
    string{1}
    this[string key] {{ get {{ return """"; }} set {{ }} }}
}}
#pragma warning disable SA1600 // the following code is used for ensuring the above code compiles
public class BaseClass : IInterface {{ public string this[string key] {{ get {{ return """"; }} set {{ }} }} }}
public interface IInterface {{ string this[string key] {{ get; set; }} }}
";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(9, 5),
                };

            string explicitInterfaceText = isExplicitInterfaceIndexer ? " IInterface." : string.Empty;
            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, explicitInterfaceText), requiresDiagnostic ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestEventDeclarationDocumentationAsync(string modifiers, bool isExplicitInterfaceEvent, bool requiresDiagnostic, bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass : BaseClass, IInterface
{{
    System.Action _myEvent;

    {0}
    event System.Action{1}
    MyEvent
    {{
        add
        {{
            _myEvent += value;
        }}
        remove
        {{
            _myEvent -= value;
        }}
    }}
}}
#pragma warning disable SA1600 // the following code is used for ensuring the above code compiles
public class BaseClass : IInterface {{ public event System.Action MyEvent; }}
public interface IInterface {{ event System.Action MyEvent; }}
";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass : BaseClass, IInterface
{{
    System.Action _myEvent;
    /// <summary>A summary.</summary>
    {0}
    event System.Action{1}
    MyEvent
    {{
        add
        {{
            _myEvent += value;
        }}
        remove
        {{
            _myEvent -= value;
        }}
    }}
}}
#pragma warning disable SA1600 // the following code is used for ensuring the above code compiles
public class BaseClass : IInterface {{ public event System.Action MyEvent; }}
public interface IInterface {{ event System.Action MyEvent; }}
";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(10, 5),
                };

            string explicitInterfaceText = isExplicitInterfaceEvent ? " IInterface." : string.Empty;
            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, explicitInterfaceText), requiresDiagnostic ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestFieldDeclarationDocumentationAsync(string testSettings, string modifiers, bool requiresDiagnostic, bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass
{{

    {0}
    System.Action Action;
}}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass
{{
    /// <summary>A summary.</summary>
    {0}
    System.Action Action;
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(8, 19),
                };

            var test = new CSharpTest(this.LanguageVersion)
            {
                TestCode = string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers),
                Settings = testSettings,
                DisabledDiagnostics = { "CS1591" },
            };

            if (requiresDiagnostic)
            {
                test.ExpectedDiagnostics.AddRange(expected);
            }

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task TestEventFieldDeclarationDocumentationAsync(string modifiers, bool requiresDiagnostic, bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass
{{

    {0} event
    System.Action Action;
}}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass
{{
    /// <summary>A summary.</summary>
    {0} event
    System.Action Action;
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(8, 19),
                };

            await VerifyCSharpDiagnosticAsync(this.LanguageVersion, string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers), requiresDiagnostic ? expected : DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestRegressionMethodGlobalNamespace(string code)
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS0116").WithMessage("A namespace cannot directly contain members such as fields or methods").WithLocation(0),
                Diagnostic().WithLocation(0),
            };
        }

        protected virtual async Task TestTypeWithoutDocumentationAsync(string type, bool isInterface)
        {
            await this.TestTypeDeclarationDocumentationAsync(type, string.Empty, true, false).ConfigureAwait(false);
            await this.TestTypeDeclarationDocumentationAsync(type, "internal", true, false).ConfigureAwait(false);
            await this.TestTypeDeclarationDocumentationAsync(type, "public", true, false).ConfigureAwait(false);

            await this.TestNestedTypeDeclarationDocumentationAsync(type, string.Empty, isInterface, false).ConfigureAwait(false);
            await this.TestNestedTypeDeclarationDocumentationAsync(type, "private", isInterface, false).ConfigureAwait(false);
            await this.TestNestedTypeDeclarationDocumentationAsync(type, "protected", true, false).ConfigureAwait(false);
            await this.TestNestedTypeDeclarationDocumentationAsync(type, "internal", true, false).ConfigureAwait(false);
            await this.TestNestedTypeDeclarationDocumentationAsync(type, "protected internal", true, false).ConfigureAwait(false);
            await this.TestNestedTypeDeclarationDocumentationAsync(type, "public", true, false).ConfigureAwait(false);
        }

        protected virtual async Task TestTypeWithDocumentationAsync(string type)
        {
            await this.TestTypeDeclarationDocumentationAsync(type, string.Empty, false, true).ConfigureAwait(false);
            await this.TestTypeDeclarationDocumentationAsync(type, "internal", false, true).ConfigureAwait(false);
            await this.TestTypeDeclarationDocumentationAsync(type, "public", false, true).ConfigureAwait(false);

            await this.TestNestedTypeDeclarationDocumentationAsync(type, string.Empty, false, true).ConfigureAwait(false);
            await this.TestNestedTypeDeclarationDocumentationAsync(type, "private", false, true).ConfigureAwait(false);
            await this.TestNestedTypeDeclarationDocumentationAsync(type, "protected", false, true).ConfigureAwait(false);
            await this.TestNestedTypeDeclarationDocumentationAsync(type, "internal", false, true).ConfigureAwait(false);
            await this.TestNestedTypeDeclarationDocumentationAsync(type, "protected internal", false, true).ConfigureAwait(false);
            await this.TestNestedTypeDeclarationDocumentationAsync(type, "public", false, true).ConfigureAwait(false);
        }
    }
}

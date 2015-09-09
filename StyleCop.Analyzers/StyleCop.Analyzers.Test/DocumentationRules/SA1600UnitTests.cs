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
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1600ElementsMustBeDocumented"/>.
    /// </summary>
    public class SA1600UnitTests : DiagnosticVerifier
    {
        [Theory]
        [InlineData("public string TestMember;", 15)]
        [InlineData("public string TestMember { get; set; }", 15)]
        [InlineData("public void TestMember() { }", 13)]
        [InlineData("public string this[int a] { get { return \"a\"; } set { } }", 15)]
        [InlineData("public event EventHandler TestMember { add { } remove { } }", 27)]
        public async Task TestRegressionMethodGlobalNamespaceAsync(string code, int column)
        {
            // This test is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1416
            var testCode = $@"
using System;

{code}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(4, column),
                new DiagnosticResult
                {
                    Id = "CS0116",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, column) },
                    Message = "A namespace cannot directly contain members such as fields or methods"
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithoutDocumentationAsync()
        {
            await this.TestTypeWithoutDocumentationAsync("class", false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructWithoutDocumentationAsync()
        {
            await this.TestTypeWithoutDocumentationAsync("struct", false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumWithoutDocumentationAsync()
        {
            await this.TestTypeWithoutDocumentationAsync("enum", false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceWithoutDocumentationAsync()
        {
            await this.TestTypeWithoutDocumentationAsync("interface", true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithDocumentationAsync()
        {
            await this.TestTypeWithDocumentationAsync("class").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructWithDocumentationAsync()
        {
            await this.TestTypeWithDocumentationAsync("struct").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumWithDocumentationAsync()
        {
            await this.TestTypeWithDocumentationAsync("enum").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceWithDocumentationAsync()
        {
            await this.TestTypeWithDocumentationAsync("interface").ConfigureAwait(false);
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
            await this.TestFieldDeclarationDocumentationAsync(string.Empty, false, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync("private", false, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync("protected", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync("internal", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync("protected internal", true, false).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync("public", true, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWithDocumentationAsync()
        {
            await this.TestFieldDeclarationDocumentationAsync(string.Empty, false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync("private", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync("protected", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync("internal", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync("protected internal", false, true).ConfigureAwait(false);
            await this.TestFieldDeclarationDocumentationAsync("public", false, true).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 14);

            await this.VerifyCSharpDiagnosticAsync(testCodeWithDocumentation, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(testCodeWithEmptyDocumentation, expected, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 14);

            await this.VerifyCSharpDiagnosticAsync(testCodeWithDocumentation, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(testCodeWithEmptyDocumentation, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyElementXmlCommentsAsync()
        {
            var testCodeWithDocumentation = @"/// <inheritdoc/>
public class OuterClass
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCodeWithDocumentation, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCodeWithDocumentation, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1600ElementsMustBeDocumented();
        }

        private async Task TestTypeDeclarationDocumentationAsync(string type, string modifiers, bool requiresDiagnostic, bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(3, 1)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, type), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestNestedTypeDeclarationDocumentationAsync(string type, string modifiers, bool requiresDiagnostic, bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(8, 5)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, type), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestDelegateDeclarationDocumentationAsync(string modifiers, bool requiresDiagnostic, bool hasDocumentation)
        {
            var testCodeWithoutDocumentation = @"
{0} delegate void
DelegateName();";
            var testCodeWithDocumentation = @"/// <summary> A summary. </summary>
{0} delegate void
DelegateName();";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(3, 1)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestNestedDelegateDeclarationDocumentationAsync(string modifiers, bool requiresDiagnostic, bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(8, 5)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestMethodDeclarationDocumentationAsync(string modifiers, bool isExplicitInterfaceMethod, bool requiresDiagnostic, bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(8, 5)
                };

            string explicitInterfaceText = isExplicitInterfaceMethod ? " IInterface." : string.Empty;
            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, explicitInterfaceText), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestInterfaceMethodDeclarationDocumentationAsync(bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(8, 5)
                };

            await this.VerifyCSharpDiagnosticAsync(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, !hasDocumentation ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestInterfacePropertyDeclarationDocumentationAsync(bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(8, 12)
                };

            await this.VerifyCSharpDiagnosticAsync(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, !hasDocumentation ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestInterfaceEventDeclarationDocumentationAsync(bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(8, 25)
                };

            await this.VerifyCSharpDiagnosticAsync(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, !hasDocumentation ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestInterfaceIndexerDeclarationDocumentationAsync(bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(8, 5)
                };

            await this.VerifyCSharpDiagnosticAsync(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, !hasDocumentation ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestConstructorDeclarationDocumentationAsync(string modifiers, bool requiresDiagnostic, bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(8, 5)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestDestructorDeclarationDocumentationAsync(bool requiresDiagnostic, bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(7, 6)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestPropertyDeclarationDocumentationAsync(string modifiers, bool isExplicitInterfaceProperty, bool requiresDiagnostic, bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(9, 5)
                };

            string explicitInterfaceText = isExplicitInterfaceProperty ? " IInterface." : string.Empty;
            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, explicitInterfaceText), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestIndexerDeclarationDocumentationAsync(string modifiers, bool isExplicitInterfaceIndexer, bool requiresDiagnostic, bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(9, 5)
                };

            string explicitInterfaceText = isExplicitInterfaceIndexer ? " IInterface." : string.Empty;
            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, explicitInterfaceText), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestEventDeclarationDocumentationAsync(string modifiers, bool isExplicitInterfaceEvent, bool requiresDiagnostic, bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(10, 5)
                };

            string explicitInterfaceText = isExplicitInterfaceEvent ? " IInterface." : string.Empty;
            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, explicitInterfaceText), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestFieldDeclarationDocumentationAsync(string modifiers, bool requiresDiagnostic, bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(8, 19)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestEventFieldDeclarationDocumentationAsync(string modifiers, bool requiresDiagnostic, bool hasDocumentation)
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
                    this.CSharpDiagnostic().WithLocation(8, 19)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestTypeWithoutDocumentationAsync(string type, bool isInterface)
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

        private async Task TestTypeWithDocumentationAsync(string type)
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

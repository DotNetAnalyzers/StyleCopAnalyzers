// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1400AccessModifierMustBeDeclared,
        StyleCop.Analyzers.MaintainabilityRules.SA1400CodeFixProvider>;

    public class SA1400UnitTests
    {
        private const string Tab = "\t";

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestTypeDeclarationAsync(string typeName)
        {
            await this.TestTypeDeclarationImplAsync(typeName).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestPartialTypeDeclarationAsync(string typeName)
        {
            await this.TestTypeDeclarationImplAsync($"partial {typeName}", warning: false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestTypeDeclarationWithAttributesAsync(string typeName)
        {
            await this.TestTypeDeclarationWithAttributesImplAsync(typeName).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestPartialTypeDeclarationWithAttributesAsync(string typeName)
        {
            await this.TestTypeDeclarationWithAttributesImplAsync($"partial {typeName}", warning: false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestTypeDeclarationWithDirectivesAsync(string typeName)
        {
            await this.TestTypeDeclarationWithDirectivesImplAsync(typeName).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestPartialTypeDeclarationWithDirectivesAsync(string typeName)
        {
            await this.TestTypeDeclarationWithDirectivesImplAsync($"partial {typeName}", warning: false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNestedTypeDeclarationAsync(string typeName)
        {
            await this.TestNestedTypeDeclarationImplAsync(typeName).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNestedTypeDeclarationWithAttributesAsync(string typeName)
        {
            await this.TestNestedTypeDeclarationWithAttributesImplAsync(typeName).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNestedTypeDeclarationWithDirectivesAsync(string typeName)
        {
            await this.TestNestedTypeDeclarationWithDirectivesImplAsync(typeName).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationAsync()
        {
            await this.TestDeclarationAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationWithAttributesAsync()
        {
            await this.TestDeclarationWithAttributesAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationWithDirectivesAsync()
        {
            await this.TestDeclarationWithDirectivesAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedDelegateDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedDelegateDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedDelegateDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticEventDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticEventDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticEventDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceEventDeclarationAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { event EventHandler MemberName; }";
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceEventDeclarationWithAttributesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { event EventHandler MemberName; }";
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceEventDeclarationWithDirectivesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { event EventHandler MemberName; }";
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceMethodDeclarationAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { void MemberName(int parameter); }";
            await this.TestNestedDeclarationAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceMethodDeclarationWithAttributesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { void MemberName(int parameter); }";
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceMethodDeclarationWithDirectivesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { void MemberName(int parameter); }";
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n );", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n );", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n );", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "partial void MemberName", "  ( int\n parameter\n );", containingType: "partial class", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "partial void MemberName", "  ( int\n parameter\n );", containingType: "partial class", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "partial void MemberName", "  ( int\n parameter\n );", containingType: "partial class", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticPropertyDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticPropertyDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticPropertyDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfacePropertyDeclarationAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { EventHandler MemberName { get; set; } }";
            await this.TestNestedDeclarationAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfacePropertyDeclarationWithAttributesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { EventHandler MemberName { get; set; } }";
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfacePropertyDeclarationWithDirectivesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { EventHandler MemberName { get; set; } }";
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfacePropertyDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfacePropertyDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfacePropertyDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticEventFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticEventFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticEventFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceEventFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceEventFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceEventFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceIndexerDeclarationAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { EventHandler this[int index] { get; set; } }";
            await this.TestNestedDeclarationAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceIndexerDeclarationWithAttributesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { EventHandler this[int index] { get; set; } }";
            await this.TestNestedDeclarationWithAttributesAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceIndexerDeclarationWithDirectivesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { EventHandler this[int index] { get; set; } }";
            await this.TestNestedDeclarationWithDirectivesAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticConstructorDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "OuterTypeName", "static OuterTypeName(", " ) { }", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticConstructorDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "OuterTypeName", "static OuterTypeName(", " ) { }", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticConstructorDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "OuterTypeName", "static OuterTypeName(", " ) { }", warning: false).ConfigureAwait(false);
        }

        private async Task TestTypeDeclarationImplAsync(string keyword, bool warning = true)
        {
            await this.TestDeclarationAsync("internal", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning).ConfigureAwait(false);
        }

        private async Task TestTypeDeclarationWithAttributesImplAsync(string keyword, bool warning = true)
        {
            await this.TestDeclarationWithAttributesAsync("internal", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning).ConfigureAwait(false);
        }

        private async Task TestTypeDeclarationWithDirectivesImplAsync(string keyword, bool warning = true)
        {
            await this.TestDeclarationWithDirectivesAsync("internal", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning).ConfigureAwait(false);
        }

        private async Task TestNestedTypeDeclarationImplAsync(string keyword, bool warning = true)
        {
            await this.TestNestedDeclarationAsync("private", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning).ConfigureAwait(false);
        }

        private async Task TestNestedTypeDeclarationWithAttributesImplAsync(string keyword, bool warning = true)
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning).ConfigureAwait(false);
        }

        private async Task TestNestedTypeDeclarationWithDirectivesImplAsync(string keyword, bool warning = true)
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning).ConfigureAwait(false);
        }

        private async Task TestDeclarationAsync(string modifier, string identifier, string keywordLine, string linesAfter, string elementName = null, bool warning = true)
        {
            var testCode = $@"
 {Tab} {keywordLine}
{linesAfter}";

            if (!warning)
            {
                await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            DiagnosticResult expected = Diagnostic().WithArguments(elementName ?? identifier).WithLocation(2, 4 + keywordLine.IndexOf(identifier));

            var fixedTestCode = $@"
 {Tab} {modifier} {keywordLine}
{linesAfter}";

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestDeclarationWithAttributesAsync(string modifier, string identifier, string keywordLine, string linesAfter, string elementName = null, bool warning = true)
        {
            var testCode = $@"using System;
  [Obsolete]
 {Tab} {keywordLine}
{linesAfter}";

            if (!warning)
            {
                await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            DiagnosticResult expected = Diagnostic().WithArguments(elementName ?? identifier).WithLocation(3, 4 + keywordLine.IndexOf(identifier));

            var fixedTestCode = $@"using System;
  [Obsolete]
 {Tab} {modifier} {keywordLine}
{linesAfter}";

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestDeclarationWithDirectivesAsync(string modifier, string identifier, string keywordLine, string linesAfter, string elementName = null, bool warning = true)
        {
            var testCode = $@"
 #  if true
 {Tab} {keywordLine}
# endif
{linesAfter}";

            if (!warning)
            {
                await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            DiagnosticResult expected = Diagnostic().WithArguments(elementName ?? identifier).WithLocation(3, 4 + keywordLine.IndexOf(identifier));

            var fixedTestCode = $@"
 #  if true
 {Tab} {modifier} {keywordLine}
# endif
{linesAfter}";

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestNestedDeclarationAsync(string modifier, string identifier, string keywordLine, string linesAfter, string containingType = "class", string baseTypeList = "", string baseTypeDeclarations = "", string elementName = null, bool warning = true)
        {
            var testCode = $@"using System;
public {containingType} OuterTypeName {baseTypeList} {{
 {Tab} {keywordLine}
{linesAfter} }}
{baseTypeDeclarations}";

            if (!warning)
            {
                await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            DiagnosticResult expected = Diagnostic().WithArguments(elementName ?? identifier).WithLocation(3, 4 + keywordLine.IndexOf(identifier));

            var fixedTestCode = $@"using System;
public {containingType} OuterTypeName {baseTypeList} {{
 {Tab} {modifier} {keywordLine}
{linesAfter} }}
{baseTypeDeclarations}";

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestNestedDeclarationWithAttributesAsync(string modifier, string identifier, string keywordLine, string linesAfter, string containingType = "class", string baseTypeList = "", string baseTypeDeclarations = "", string elementName = null, bool warning = true)
        {
            var testCode = $@"using System;
public {containingType} OuterTypeName {baseTypeList} {{
  [Obsolete]
 {Tab} {keywordLine}
{linesAfter} }}
{baseTypeDeclarations}";

            if (!warning)
            {
                await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            DiagnosticResult expected = Diagnostic().WithArguments(elementName ?? identifier).WithLocation(4, 4 + keywordLine.IndexOf(identifier));

            var fixedTestCode = $@"using System;
public {containingType} OuterTypeName {baseTypeList} {{
  [Obsolete]
 {Tab} {modifier} {keywordLine}
{linesAfter} }}
{baseTypeDeclarations}";

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestNestedDeclarationWithDirectivesAsync(string modifier, string identifier, string keywordLine, string linesAfter, string containingType = "class", string baseTypeList = "", string baseTypeDeclarations = "", string elementName = null, bool warning = true)
        {
            var testCode = $@"using System;
public {containingType} OuterTypeName {baseTypeList} {{
 #  if true
 {Tab} {keywordLine}
# endif
{linesAfter} }}
{baseTypeDeclarations}";

            if (!warning)
            {
                await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            DiagnosticResult expected = Diagnostic().WithArguments(elementName ?? identifier).WithLocation(4, 4 + keywordLine.IndexOf(identifier));

            var fixedTestCode = $@"using System;
public {containingType} OuterTypeName {baseTypeList} {{
 #  if true
 {Tab} {modifier} {keywordLine}
# endif
{linesAfter} }}
{baseTypeDeclarations}";

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

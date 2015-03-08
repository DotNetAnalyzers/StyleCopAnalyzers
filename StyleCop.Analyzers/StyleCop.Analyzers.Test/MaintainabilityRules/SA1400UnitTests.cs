namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.MaintainabilityRules;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;
    using TestHelper;

    public class SA1400UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1400AccessModifierMustBeDeclared.DiagnosticId;

        private const string Tab = "\t";

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestClassDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("class");
        }

        [Fact]
        public async Task TestClassDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("class");
        }

        [Fact]
        public async Task TestClassDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("class");
        }

        [Fact]
        public async Task TestNestedClassDeclarationAsync()
        {
            await this.TestNestedTypeDeclarationAsync("class");
        }

        [Fact]
        public async Task TestNestedClassDeclarationWithAttributesAsync()
        {
            await this.TestNestedTypeDeclarationWithAttributesAsync("class");
        }

        [Fact]
        public async Task TestNestedClassDeclarationWithDirectivesAsync()
        {
            await this.TestNestedTypeDeclarationWithDirectivesAsync("class");
        }

        [Fact]
        public async Task TestPartialClassDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("partial class", warning: false);
        }

        [Fact]
        public async Task TestPartialClassDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("partial class", warning: false);
        }

        [Fact]
        public async Task TestPartialClassDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("partial class", warning: false);
        }

        [Fact]
        public async Task TestInterfaceDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("interface");
        }

        [Fact]
        public async Task TestInterfaceDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("interface");
        }

        [Fact]
        public async Task TestInterfaceDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("interface");
        }

        [Fact]
        public async Task TestNestedInterfaceDeclarationAsync()
        {
            await this.TestNestedTypeDeclarationAsync("interface");
        }

        [Fact]
        public async Task TestNestedInterfaceDeclarationWithAttributesAsync()
        {
            await this.TestNestedTypeDeclarationWithAttributesAsync("interface");
        }

        [Fact]
        public async Task TestNestedInterfaceDeclarationWithDirectivesAsync()
        {
            await this.TestNestedTypeDeclarationWithDirectivesAsync("interface");
        }

        [Fact]
        public async Task TestPartialInterfaceDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("partial interface", warning: false);
        }

        [Fact]
        public async Task TestPartialInterfaceDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("partial interface", warning: false);
        }

        [Fact]
        public async Task TestPartialInterfaceDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("partial interface", warning: false);
        }

        [Fact]
        public async Task TestStructDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("struct");
        }

        [Fact]
        public async Task TestStructDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("struct");
        }

        [Fact]
        public async Task TestStructDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("struct");
        }

        [Fact]
        public async Task TestNestedStructDeclarationAsync()
        {
            await this.TestNestedTypeDeclarationAsync("struct");
        }

        [Fact]
        public async Task TestNestedStructDeclarationWithAttributesAsync()
        {
            await this.TestNestedTypeDeclarationWithAttributesAsync("struct");
        }

        [Fact]
        public async Task TestNestedStructDeclarationWithDirectivesAsync()
        {
            await this.TestNestedTypeDeclarationWithDirectivesAsync("struct");
        }

        [Fact]
        public async Task TestPartialStructDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("partial struct", warning: false);
        }

        [Fact]
        public async Task TestPartialStructDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("partial struct", warning: false);
        }

        [Fact]
        public async Task TestPartialStructDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("partial struct", warning: false);
        }

        [Fact]
        public async Task TestEnumDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("enum");
        }

        [Fact]
        public async Task TestEnumDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("enum");
        }

        [Fact]
        public async Task TestEnumDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("enum");
        }

        [Fact]
        public async Task TestNestedEnumDeclarationAsync()
        {
            await this.TestNestedTypeDeclarationAsync("enum");
        }

        [Fact]
        public async Task TestNestedEnumDeclarationWithAttributesAsync()
        {
            await this.TestNestedTypeDeclarationWithAttributesAsync("enum");
        }

        [Fact]
        public async Task TestNestedEnumDeclarationWithDirectivesAsync()
        {
            await this.TestNestedTypeDeclarationWithDirectivesAsync("enum");
        }

        [Fact]
        public async Task TestDelegateDeclarationAsync()
        {
            await this.TestDeclarationAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [Fact]
        public async Task TestDelegateDeclarationWithAttributesAsync()
        {
            await this.TestDeclarationWithAttributesAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [Fact]
        public async Task TestDelegateDeclarationWithDirectivesAsync()
        {
            await this.TestDeclarationWithDirectivesAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [Fact]
        public async Task TestNestedDelegateDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [Fact]
        public async Task TestNestedDelegateDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [Fact]
        public async Task TestNestedDelegateDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        #region EventDeclarationSyntax

        [Fact]
        public async Task TestEventDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }");
        }

        [Fact]
        public async Task TestEventDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }");
        }

        [Fact]
        public async Task TestEventDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }");
        }

        [Fact]
        public async Task TestStaticEventDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }");
        }

        [Fact]
        public async Task TestStaticEventDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }");
        }

        [Fact]
        public async Task TestStaticEventDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }");
        }

        [Fact]
        public async Task TestExplicitInterfaceEventDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", warning: false);
        }

        [Fact]
        public async Task TestExplicitInterfaceEventDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", warning: false);
        }

        [Fact]
        public async Task TestExplicitInterfaceEventDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", warning: false);
        }

        #endregion

        #region MethodDeclarationSyntax

        [Fact]
        public async Task TestMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }");
        }

        [Fact]
        public async Task TestMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }");
        }

        [Fact]
        public async Task TestMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }");
        }

        [Fact]
        public async Task TestStaticMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }");
        }

        [Fact]
        public async Task TestStaticMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }");
        }

        [Fact]
        public async Task TestStaticMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }");
        }

        [Fact]
        public async Task TestExplicitInterfaceMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", warning: false);
        }

        [Fact]
        public async Task TestExplicitInterfaceMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", warning: false);
        }

        [Fact]
        public async Task TestExplicitInterfaceMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", warning: false);
        }

        [Fact]
        public async Task TestInterfaceMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n );", containingType: "interface", warning: false);
        }

        [Fact]
        public async Task TestInterfaceMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n );", containingType: "interface", warning: false);
        }

        [Fact]
        public async Task TestInterfaceMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n );", containingType: "interface", warning: false);
        }

        [Fact]
        public async Task TestPartialMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "partial void MemberName", "  ( int\n parameter\n );", containingType: "partial class", warning: false);
        }

        [Fact]
        public async Task TestPartialMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "partial void MemberName", "  ( int\n parameter\n );", containingType: "partial class", warning: false);
        }

        [Fact]
        public async Task TestPartialMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "partial void MemberName", "  ( int\n parameter\n );", containingType: "partial class", warning: false);
        }

        #endregion

        #region PropertyDeclarationSyntax

        [Fact]
        public async Task TestPropertyDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }");
        }

        [Fact]
        public async Task TestPropertyDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }");
        }

        [Fact]
        public async Task TestPropertyDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }");
        }

        [Fact]
        public async Task TestStaticPropertyDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }");
        }

        [Fact]
        public async Task TestStaticPropertyDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }");
        }

        [Fact]
        public async Task TestStaticPropertyDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }");
        }

        [Fact]
        public async Task TestExplicitInterfacePropertyDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", warning: false);
        }

        [Fact]
        public async Task TestExplicitInterfacePropertyDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", warning: false);
        }

        [Fact]
        public async Task TestExplicitInterfacePropertyDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", warning: false);
        }

        [Fact]
        public async Task TestInterfacePropertyDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false);
        }

        [Fact]
        public async Task TestInterfacePropertyDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false);
        }

        [Fact]
        public async Task TestInterfacePropertyDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false);
        }

        #endregion

        #region EventFieldDeclarationSyntax

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestEventFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;");
        }

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestEventFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;");
        }

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestEventFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;");
        }

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestStaticEventFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;");
        }

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestStaticEventFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;");
        }

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestStaticEventFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;");
        }

        [Fact]
        public async Task TestInterfaceEventFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false);
        }

        [Fact]
        public async Task TestInterfaceEventFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false);
        }

        [Fact]
        public async Task TestInterfaceEventFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false);
        }

        #endregion

        #region FieldDeclarationSyntax

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestStaticFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestStaticFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestStaticFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;");
        }

        #endregion

        #region OperatorDeclarationSyntax

        [Fact]
        public async Task TestOperatorDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("public", "+", "static OuterTypeName operator +", "  ( OuterTypeName x,OuterTypeName  y ) { throw new System.Exception(); }", elementName: "op_Addition");
        }

        [Fact]
        public async Task TestOperatorDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("public", "+", "static OuterTypeName operator +", "  ( OuterTypeName x,OuterTypeName  y ) { throw new System.Exception(); }", elementName: "op_Addition");
        }

        [Fact]
        public async Task TestOperatorDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("public", "+", "static OuterTypeName operator +", "  ( OuterTypeName x,OuterTypeName  y ) { throw new System.Exception(); }", elementName: "op_Addition");
        }

        #endregion

        #region ConversionOperatorDeclarationSyntax

        [Fact]
        public async Task TestConversionOperatorDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("public", "bool", "static explicit operator bool", "  ( OuterTypeName x ) { throw new System.Exception(); }", elementName: "op_Explicit");
        }

        [Fact]
        public async Task TestConversionOperatorDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("public", "bool", "static explicit operator bool", "  ( OuterTypeName x ) { throw new System.Exception(); }", elementName: "op_Explicit");
        }

        [Fact]
        public async Task TestConversionOperatorDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("public", "bool", "static explicit operator bool", "  ( OuterTypeName x ) { throw new System.Exception(); }", elementName: "op_Explicit");
        }

        #endregion

        #region IndexerDeclarationSyntax

        [Fact]
        public async Task TestIndexerDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]");
        }

        [Fact]
        public async Task TestIndexerDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]");
        }

        [Fact]
        public async Task TestIndexerDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]");
        }

        [Fact]
        public async Task TestExplicitInterfaceIndexerDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", warning: false);
        }

        [Fact]
        public async Task TestExplicitInterfaceIndexerDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", warning: false);
        }

        [Fact]
        public async Task TestExplicitInterfaceIndexerDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", warning: false);
        }

        #endregion

        #region ConstructorDeclarationSyntax

        [Fact]
        public async Task TestConstructorDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor");
        }

        [Fact]
        public async Task TestConstructorDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor");
        }

        [Fact]
        public async Task TestConstructorDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor");
        }

        [Fact]
        public async Task TestStaticConstructorDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "OuterTypeName", "static OuterTypeName(", " ) { }", warning: false);
        }

        [Fact]
        public async Task TestStaticConstructorDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "OuterTypeName", "static OuterTypeName(", " ) { }", warning: false);
        }

        [Fact]
        public async Task TestStaticConstructorDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "OuterTypeName", "static OuterTypeName(", " ) { }", warning: false);
        }

        #endregion

        private async Task TestTypeDeclarationAsync(string keyword, bool warning = true)
        {
            await this.TestDeclarationAsync("internal", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning);
        }

        private async Task TestTypeDeclarationWithAttributesAsync(string keyword, bool warning = true)
        {
            await this.TestDeclarationWithAttributesAsync("internal", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning);
        }

        private async Task TestTypeDeclarationWithDirectivesAsync(string keyword, bool warning = true)
        {
            await this.TestDeclarationWithDirectivesAsync("internal", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning);
        }

        private async Task TestNestedTypeDeclarationAsync(string keyword, bool warning = true)
        {
            await this.TestNestedDeclarationAsync("private", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning);
        }

        private async Task TestNestedTypeDeclarationWithAttributesAsync(string keyword, bool warning = true)
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning);
        }

        private async Task TestNestedTypeDeclarationWithDirectivesAsync(string keyword, bool warning = true)
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning);
        }

        private async Task TestDeclarationAsync(string modifier, string identifier, string keywordLine, string linesAfter, string elementName = null, bool warning = true)
        {
            var testCode = $@"
 {Tab} {keywordLine}
{linesAfter}";

            if (!warning)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
                return;
            }

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = $"Element '{elementName ?? identifier}' must declare an access modifier",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 2, 4 + keywordLine.IndexOf(identifier))
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedTestCode = $@"
 {Tab} {modifier} {keywordLine}
{linesAfter}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        private async Task TestDeclarationWithAttributesAsync(string modifier, string identifier, string keywordLine, string linesAfter, string elementName = null, bool warning = true)
        {
            var testCode = $@"
  [Serializable]
 {Tab} {keywordLine}
{linesAfter}";

            if (!warning)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
                return;
            }

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = $"Element '{elementName ?? identifier}' must declare an access modifier",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 4 + keywordLine.IndexOf(identifier))
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedTestCode = $@"
  [Serializable]
 {Tab} {modifier} {keywordLine}
{linesAfter}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
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
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
                return;
            }

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = $"Element '{elementName ?? identifier}' must declare an access modifier",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 4 + keywordLine.IndexOf(identifier))
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedTestCode = $@"
 #  if true
 {Tab} {modifier} {keywordLine}
# endif
{linesAfter}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        private async Task TestNestedDeclarationAsync(string modifier, string identifier, string keywordLine, string linesAfter, string containingType = "class", string elementName = null, bool warning = true)
        {
            var testCode = $@"
public {containingType} OuterTypeName {{
 {Tab} {keywordLine}
{linesAfter} }}";

            if (!warning)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
                return;
            }

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = $"Element '{elementName ?? identifier}' must declare an access modifier",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 4 + keywordLine.IndexOf(identifier))
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedTestCode = $@"
public {containingType} OuterTypeName {{
 {Tab} {modifier} {keywordLine}
{linesAfter} }}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        private async Task TestNestedDeclarationWithAttributesAsync(string modifier, string identifier, string keywordLine, string linesAfter, string containingType = "class", string elementName = null, bool warning = true)
        {
            var testCode = $@"
public {containingType} OuterTypeName {{
  [Serializable]
 {Tab} {keywordLine}
{linesAfter} }}";

            if (!warning)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
                return;
            }

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = $"Element '{elementName ?? identifier}' must declare an access modifier",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 4 + keywordLine.IndexOf(identifier))
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedTestCode = $@"
public {containingType} OuterTypeName {{
  [Serializable]
 {Tab} {modifier} {keywordLine}
{linesAfter} }}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        private async Task TestNestedDeclarationWithDirectivesAsync(string modifier, string identifier, string keywordLine, string linesAfter, string containingType = "class", string elementName = null, bool warning = true)
        {
            var testCode = $@"
public {containingType} OuterTypeName {{
 #  if true
 {Tab} {keywordLine}
# endif
{linesAfter} }}";

            if (!warning)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
                return;
            }

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = $"Element '{elementName ?? identifier}' must declare an access modifier",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 4 + keywordLine.IndexOf(identifier))
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedTestCode = $@"
public {containingType} OuterTypeName {{
 #  if true
 {Tab} {modifier} {keywordLine}
# endif
{linesAfter} }}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1400AccessModifierMustBeDeclared();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1400CodeFixProvider();
        }
    }
}

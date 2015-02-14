namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;

    [TestClass]
    public class SA1400UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1400AccessModifierMustBeDeclared.DiagnosticId;

        private const string Tab = "\t";

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestClassDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("class");
        }

        [TestMethod]
        public async Task TestClassDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("class");
        }

        [TestMethod]
        public async Task TestClassDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("class");
        }

        [TestMethod]
        public async Task TestNestedClassDeclarationAsync()
        {
            await this.TestNestedTypeDeclarationAsync("class");
        }

        [TestMethod]
        public async Task TestNestedClassDeclarationWithAttributesAsync()
        {
            await this.TestNestedTypeDeclarationWithAttributesAsync("class");
        }

        [TestMethod]
        public async Task TestNestedClassDeclarationWithDirectivesAsync()
        {
            await this.TestNestedTypeDeclarationWithDirectivesAsync("class");
        }

        [TestMethod]
        public async Task TestPartialClassDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("partial class", warning: false);
        }

        [TestMethod]
        public async Task TestPartialClassDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("partial class", warning: false);
        }

        [TestMethod]
        public async Task TestPartialClassDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("partial class", warning: false);
        }

        [TestMethod]
        public async Task TestInterfaceDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("interface");
        }

        [TestMethod]
        public async Task TestInterfaceDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("interface");
        }

        [TestMethod]
        public async Task TestInterfaceDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("interface");
        }

        [TestMethod]
        public async Task TestNestedInterfaceDeclarationAsync()
        {
            await this.TestNestedTypeDeclarationAsync("interface");
        }

        [TestMethod]
        public async Task TestNestedInterfaceDeclarationWithAttributesAsync()
        {
            await this.TestNestedTypeDeclarationWithAttributesAsync("interface");
        }

        [TestMethod]
        public async Task TestNestedInterfaceDeclarationWithDirectivesAsync()
        {
            await this.TestNestedTypeDeclarationWithDirectivesAsync("interface");
        }

        [TestMethod]
        public async Task TestPartialInterfaceDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("partial interface", warning: false);
        }

        [TestMethod]
        public async Task TestPartialInterfaceDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("partial interface", warning: false);
        }

        [TestMethod]
        public async Task TestPartialInterfaceDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("partial interface", warning: false);
        }

        [TestMethod]
        public async Task TestStructDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("struct");
        }

        [TestMethod]
        public async Task TestStructDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("struct");
        }

        [TestMethod]
        public async Task TestStructDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("struct");
        }

        [TestMethod]
        public async Task TestNestedStructDeclarationAsync()
        {
            await this.TestNestedTypeDeclarationAsync("struct");
        }

        [TestMethod]
        public async Task TestNestedStructDeclarationWithAttributesAsync()
        {
            await this.TestNestedTypeDeclarationWithAttributesAsync("struct");
        }

        [TestMethod]
        public async Task TestNestedStructDeclarationWithDirectivesAsync()
        {
            await this.TestNestedTypeDeclarationWithDirectivesAsync("struct");
        }

        [TestMethod]
        public async Task TestPartialStructDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("partial struct", warning: false);
        }

        [TestMethod]
        public async Task TestPartialStructDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("partial struct", warning: false);
        }

        [TestMethod]
        public async Task TestPartialStructDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("partial struct", warning: false);
        }

        [TestMethod]
        public async Task TestEnumDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("enum");
        }

        [TestMethod]
        public async Task TestEnumDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("enum");
        }

        [TestMethod]
        public async Task TestEnumDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("enum");
        }

        [TestMethod]
        public async Task TestNestedEnumDeclarationAsync()
        {
            await this.TestNestedTypeDeclarationAsync("enum");
        }

        [TestMethod]
        public async Task TestNestedEnumDeclarationWithAttributesAsync()
        {
            await this.TestNestedTypeDeclarationWithAttributesAsync("enum");
        }

        [TestMethod]
        public async Task TestNestedEnumDeclarationWithDirectivesAsync()
        {
            await this.TestNestedTypeDeclarationWithDirectivesAsync("enum");
        }

        [TestMethod]
        public async Task TestDelegateDeclarationAsync()
        {
            await this.TestDeclarationAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [TestMethod]
        public async Task TestDelegateDeclarationWithAttributesAsync()
        {
            await this.TestDeclarationWithAttributesAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [TestMethod]
        public async Task TestDelegateDeclarationWithDirectivesAsync()
        {
            await this.TestDeclarationWithDirectivesAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [TestMethod]
        public async Task TestNestedDelegateDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [TestMethod]
        public async Task TestNestedDelegateDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [TestMethod]
        public async Task TestNestedDelegateDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        #region EventDeclarationSyntax

        [TestMethod]
        public async Task TestEventDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }");
        }

        [TestMethod]
        public async Task TestEventDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }");
        }

        [TestMethod]
        public async Task TestEventDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }");
        }

        [TestMethod]
        public async Task TestStaticEventDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }");
        }

        [TestMethod]
        public async Task TestStaticEventDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }");
        }

        [TestMethod]
        public async Task TestStaticEventDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }");
        }

        [TestMethod]
        public async Task TestExplicitInterfaceEventDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfaceEventDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfaceEventDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", warning: false);
        }

        #endregion

        #region MethodDeclarationSyntax

        [TestMethod]
        public async Task TestMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }");
        }

        [TestMethod]
        public async Task TestMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }");
        }

        [TestMethod]
        public async Task TestMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }");
        }

        [TestMethod]
        public async Task TestStaticMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }");
        }

        [TestMethod]
        public async Task TestStaticMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }");
        }

        [TestMethod]
        public async Task TestStaticMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }");
        }

        [TestMethod]
        public async Task TestExplicitInterfaceMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfaceMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfaceMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", warning: false);
        }

        [TestMethod]
        public async Task TestInterfaceMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n );", containingType: "interface", warning: false);
        }

        [TestMethod]
        public async Task TestInterfaceMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n );", containingType: "interface", warning: false);
        }

        [TestMethod]
        public async Task TestInterfaceMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n );", containingType: "interface", warning: false);
        }

        [TestMethod]
        public async Task TestPartialMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "partial void MemberName", "  ( int\n parameter\n );", containingType: "partial class", warning: false);
        }

        [TestMethod]
        public async Task TestPartialMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "partial void MemberName", "  ( int\n parameter\n );", containingType: "partial class", warning: false);
        }

        [TestMethod]
        public async Task TestPartialMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "partial void MemberName", "  ( int\n parameter\n );", containingType: "partial class", warning: false);
        }

        #endregion

        #region PropertyDeclarationSyntax

        [TestMethod]
        public async Task TestPropertyDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }");
        }

        [TestMethod]
        public async Task TestPropertyDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }");
        }

        [TestMethod]
        public async Task TestPropertyDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }");
        }

        [TestMethod]
        public async Task TestStaticPropertyDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }");
        }

        [TestMethod]
        public async Task TestStaticPropertyDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }");
        }

        [TestMethod]
        public async Task TestStaticPropertyDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }");
        }

        [TestMethod]
        public async Task TestExplicitInterfacePropertyDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfacePropertyDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfacePropertyDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", warning: false);
        }

        [TestMethod]
        public async Task TestInterfacePropertyDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false);
        }

        [TestMethod]
        public async Task TestInterfacePropertyDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false);
        }

        [TestMethod]
        public async Task TestInterfacePropertyDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false);
        }

        #endregion

        #region EventFieldDeclarationSyntax

        [TestMethod]
        public async Task TestEventFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestEventFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestEventFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestStaticEventFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestStaticEventFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestStaticEventFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestInterfaceEventFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false);
        }

        [TestMethod]
        public async Task TestInterfaceEventFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false);
        }

        [TestMethod]
        public async Task TestInterfaceEventFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false);
        }

        #endregion

        #region FieldDeclarationSyntax

        [TestMethod]
        public async Task TestFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestStaticFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestStaticFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestStaticFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;");
        }

        #endregion

        #region OperatorDeclarationSyntax

        [TestMethod]
        public async Task TestOperatorDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("public", "+", "static OuterTypeName operator +", "  ( OuterTypeName x,OuterTypeName  y ) { throw new System.Exception(); }", elementName: "op_Addition");
        }

        [TestMethod]
        public async Task TestOperatorDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("public", "+", "static OuterTypeName operator +", "  ( OuterTypeName x,OuterTypeName  y ) { throw new System.Exception(); }", elementName: "op_Addition");
        }

        [TestMethod]
        public async Task TestOperatorDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("public", "+", "static OuterTypeName operator +", "  ( OuterTypeName x,OuterTypeName  y ) { throw new System.Exception(); }", elementName: "op_Addition");
        }

        #endregion

        #region ConversionOperatorDeclarationSyntax

        [TestMethod]
        public async Task TestConversionOperatorDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("public", "bool", "static explicit operator bool", "  ( OuterTypeName x ) { throw new System.Exception(); }", elementName: "op_Explicit");
        }

        [TestMethod]
        public async Task TestConversionOperatorDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("public", "bool", "static explicit operator bool", "  ( OuterTypeName x ) { throw new System.Exception(); }", elementName: "op_Explicit");
        }

        [TestMethod]
        public async Task TestConversionOperatorDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("public", "bool", "static explicit operator bool", "  ( OuterTypeName x ) { throw new System.Exception(); }", elementName: "op_Explicit");
        }

        #endregion

        #region IndexerDeclarationSyntax

        [TestMethod]
        public async Task TestIndexerDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]");
        }

        [TestMethod]
        public async Task TestIndexerDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]");
        }

        [TestMethod]
        public async Task TestIndexerDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]");
        }

        [TestMethod]
        public async Task TestExplicitInterfaceIndexerDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfaceIndexerDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfaceIndexerDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", warning: false);
        }

        #endregion

        #region ConstructorDeclarationSyntax

        [TestMethod]
        public async Task TestConstructorDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor");
        }

        [TestMethod]
        public async Task TestConstructorDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor");
        }

        [TestMethod]
        public async Task TestConstructorDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor");
        }

        [TestMethod]
        public async Task TestStaticConstructorDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "OuterTypeName", "static OuterTypeName(", " ) { }", warning: false);
        }

        [TestMethod]
        public async Task TestStaticConstructorDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "OuterTypeName", "static OuterTypeName(", " ) { }", warning: false);
        }

        [TestMethod]
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

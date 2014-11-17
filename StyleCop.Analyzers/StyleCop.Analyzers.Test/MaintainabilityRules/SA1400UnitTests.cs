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
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestClassDeclarationAsync()
        {
            await TestTypeDeclarationAsync("class");
        }

        [TestMethod]
        public async Task TestClassDeclarationWithAttributesAsync()
        {
            await TestTypeDeclarationWithAttributesAsync("class");
        }

        [TestMethod]
        public async Task TestClassDeclarationWithDirectivesAsync()
        {
            await TestTypeDeclarationWithDirectivesAsync("class");
        }

        [TestMethod]
        public async Task TestNestedClassDeclarationAsync()
        {
            await TestNestedTypeDeclarationAsync("class");
        }

        [TestMethod]
        public async Task TestNestedClassDeclarationWithAttributesAsync()
        {
            await TestNestedTypeDeclarationWithAttributesAsync("class");
        }

        [TestMethod]
        public async Task TestNestedClassDeclarationWithDirectivesAsync()
        {
            await TestNestedTypeDeclarationWithDirectivesAsync("class");
        }

        [TestMethod]
        public async Task TestInterfaceDeclarationAsync()
        {
            await TestTypeDeclarationAsync("interface");
        }

        [TestMethod]
        public async Task TestInterfaceDeclarationWithAttributesAsync()
        {
            await TestTypeDeclarationWithAttributesAsync("interface");
        }

        [TestMethod]
        public async Task TestInterfaceDeclarationWithDirectivesAsync()
        {
            await TestTypeDeclarationWithDirectivesAsync("interface");
        }

        [TestMethod]
        public async Task TestNestedInterfaceDeclarationAsync()
        {
            await TestNestedTypeDeclarationAsync("interface");
        }

        [TestMethod]
        public async Task TestNestedInterfaceDeclarationWithAttributesAsync()
        {
            await TestNestedTypeDeclarationWithAttributesAsync("interface");
        }

        [TestMethod]
        public async Task TestNestedInterfaceDeclarationWithDirectivesAsync()
        {
            await TestNestedTypeDeclarationWithDirectivesAsync("interface");
        }

        [TestMethod]
        public async Task TestStructDeclarationAsync()
        {
            await TestTypeDeclarationAsync("struct");
        }

        [TestMethod]
        public async Task TestStructDeclarationWithAttributesAsync()
        {
            await TestTypeDeclarationWithAttributesAsync("struct");
        }

        [TestMethod]
        public async Task TestStructDeclarationWithDirectivesAsync()
        {
            await TestTypeDeclarationWithDirectivesAsync("struct");
        }

        [TestMethod]
        public async Task TestNestedStructDeclarationAsync()
        {
            await TestNestedTypeDeclarationAsync("struct");
        }

        [TestMethod]
        public async Task TestNestedStructDeclarationWithAttributesAsync()
        {
            await TestNestedTypeDeclarationWithAttributesAsync("struct");
        }

        [TestMethod]
        public async Task TestNestedStructDeclarationWithDirectivesAsync()
        {
            await TestNestedTypeDeclarationWithDirectivesAsync("struct");
        }

        [TestMethod]
        public async Task TestEnumDeclarationAsync()
        {
            await TestTypeDeclarationAsync("enum");
        }

        [TestMethod]
        public async Task TestEnumDeclarationWithAttributesAsync()
        {
            await TestTypeDeclarationWithAttributesAsync("enum");
        }

        [TestMethod]
        public async Task TestEnumDeclarationWithDirectivesAsync()
        {
            await TestTypeDeclarationWithDirectivesAsync("enum");
        }

        [TestMethod]
        public async Task TestNestedEnumDeclarationAsync()
        {
            await TestNestedTypeDeclarationAsync("enum");
        }

        [TestMethod]
        public async Task TestNestedEnumDeclarationWithAttributesAsync()
        {
            await TestNestedTypeDeclarationWithAttributesAsync("enum");
        }

        [TestMethod]
        public async Task TestNestedEnumDeclarationWithDirectivesAsync()
        {
            await TestNestedTypeDeclarationWithDirectivesAsync("enum");
        }

        [TestMethod]
        public async Task TestDelegateDeclarationAsync()
        {
            await TestDeclarationAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [TestMethod]
        public async Task TestDelegateDeclarationWithAttributesAsync()
        {
            await TestDeclarationWithAttributesAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [TestMethod]
        public async Task TestDelegateDeclarationWithDirectivesAsync()
        {
            await TestDeclarationWithDirectivesAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [TestMethod]
        public async Task TestNestedDelegateDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [TestMethod]
        public async Task TestNestedDelegateDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        [TestMethod]
        public async Task TestNestedDelegateDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );");
        }

        #region EventDeclarationSyntax

        [TestMethod]
        public async Task TestEventDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }");
        }

        [TestMethod]
        public async Task TestEventDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }");
        }

        [TestMethod]
        public async Task TestEventDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }");
        }

        [TestMethod]
        public async Task TestStaticEventDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }");
        }

        [TestMethod]
        public async Task TestStaticEventDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }");
        }

        [TestMethod]
        public async Task TestStaticEventDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }");
        }

        [TestMethod]
        public async Task TestExplicitInterfaceEventDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfaceEventDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfaceEventDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", warning: false);
        }

        #endregion

        #region MethodDeclarationSyntax

        [TestMethod]
        public async Task TestMethodDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }");
        }

        [TestMethod]
        public async Task TestMethodDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }");
        }

        [TestMethod]
        public async Task TestMethodDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }");
        }

        [TestMethod]
        public async Task TestStaticMethodDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }");
        }

        [TestMethod]
        public async Task TestStaticMethodDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }");
        }

        [TestMethod]
        public async Task TestStaticMethodDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }");
        }

        [TestMethod]
        public async Task TestExplicitInterfaceMethodDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfaceMethodDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfaceMethodDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", warning: false);
        }

        [TestMethod]
        public async Task TestInterfaceMethodDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }", containingType: "interface", warning: false);
        }

        [TestMethod]
        public async Task TestInterfaceMethodDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }", containingType: "interface", warning: false);
        }

        [TestMethod]
        public async Task TestInterfaceMethodDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }", containingType: "interface", warning: false);
        }

        #endregion

        #region PropertyDeclarationSyntax

        [TestMethod]
        public async Task TestPropertyDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }");
        }

        [TestMethod]
        public async Task TestPropertyDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }");
        }

        [TestMethod]
        public async Task TestPropertyDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }");
        }

        [TestMethod]
        public async Task TestStaticPropertyDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }");
        }

        [TestMethod]
        public async Task TestStaticPropertyDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }");
        }

        [TestMethod]
        public async Task TestStaticPropertyDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }");
        }

        [TestMethod]
        public async Task TestExplicitInterfacePropertyDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfacePropertyDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfacePropertyDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", warning: false);
        }

        [TestMethod]
        public async Task TestInterfacePropertyDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false);
        }

        [TestMethod]
        public async Task TestInterfacePropertyDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false);
        }

        [TestMethod]
        public async Task TestInterfacePropertyDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false);
        }

        #endregion

        #region EventFieldDeclarationSyntax

        [TestMethod]
        public async Task TestEventFieldDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestEventFieldDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestEventFieldDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestStaticEventFieldDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestStaticEventFieldDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestStaticEventFieldDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestInterfaceEventFieldDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false);
        }

        [TestMethod]
        public async Task TestInterfaceEventFieldDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false);
        }

        [TestMethod]
        public async Task TestInterfaceEventFieldDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false);
        }

        #endregion

        #region FieldDeclarationSyntax

        [TestMethod]
        public async Task TestFieldDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestFieldDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestFieldDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestStaticFieldDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestStaticFieldDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;");
        }

        [TestMethod]
        public async Task TestStaticFieldDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;");
        }

        #endregion

        #region OperatorDeclarationSyntax

        [TestMethod]
        public async Task TestOperatorDeclarationAsync()
        {
            await TestNestedDeclarationAsync("public", "+", "static OuterTypeName operator +", "  ( OuterTypeName x,OuterTypeName  y ) { throw new System.Exception(); }", elementName: "op_Addition");
        }

        [TestMethod]
        public async Task TestOperatorDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("public", "+", "static OuterTypeName operator +", "  ( OuterTypeName x,OuterTypeName  y ) { throw new System.Exception(); }", elementName: "op_Addition");
        }

        [TestMethod]
        public async Task TestOperatorDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("public", "+", "static OuterTypeName operator +", "  ( OuterTypeName x,OuterTypeName  y ) { throw new System.Exception(); }", elementName: "op_Addition");
        }

        #endregion

        #region ConversionOperatorDeclarationSyntax

        [TestMethod]
        public async Task TestConversionOperatorDeclarationAsync()
        {
            await TestNestedDeclarationAsync("public", "bool", "static explicit operator bool", "  ( OuterTypeName x ) { throw new System.Exception(); }", elementName: "op_Explicit");
        }

        [TestMethod]
        public async Task TestConversionOperatorDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("public", "bool", "static explicit operator bool", "  ( OuterTypeName x ) { throw new System.Exception(); }", elementName: "op_Explicit");
        }

        [TestMethod]
        public async Task TestConversionOperatorDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("public", "bool", "static explicit operator bool", "  ( OuterTypeName x ) { throw new System.Exception(); }", elementName: "op_Explicit");
        }

        #endregion

        #region IndexerDeclarationSyntax

        [TestMethod]
        public async Task TestIndexerDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]");
        }

        [TestMethod]
        public async Task TestIndexerDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]");
        }

        [TestMethod]
        public async Task TestIndexerDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]");
        }

        [TestMethod]
        public async Task TestExplicitInterfaceIndexerDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfaceIndexerDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", warning: false);
        }

        [TestMethod]
        public async Task TestExplicitInterfaceIndexerDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", warning: false);
        }

        #endregion

        #region ConstructorDeclarationSyntax

        [TestMethod]
        public async Task TestConstructorDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor");
        }

        [TestMethod]
        public async Task TestConstructorDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor");
        }

        [TestMethod]
        public async Task TestConstructorDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor");
        }

        [TestMethod]
        public async Task TestStaticConstructorDeclarationAsync()
        {
            await TestNestedDeclarationAsync("private", "OuterTypeName", "static OuterTypeName(", " ) { }", warning: false);
        }

        [TestMethod]
        public async Task TestStaticConstructorDeclarationWithAttributesAsync()
        {
            await TestNestedDeclarationWithAttributesAsync("private", "OuterTypeName", "static OuterTypeName(", " ) { }", warning: false);
        }

        [TestMethod]
        public async Task TestStaticConstructorDeclarationWithDirectivesAsync()
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "OuterTypeName", "static OuterTypeName(", " ) { }", warning: false);
        }

        #endregion

        private async Task TestTypeDeclarationAsync(string keyword)
        {
            await TestDeclarationAsync("internal", "TypeName", $"{keyword} TypeName", "{\n}");
        }

        private async Task TestTypeDeclarationWithAttributesAsync(string keyword)
        {
            await TestDeclarationWithAttributesAsync("internal", "TypeName", $"{keyword} TypeName", "{\n}");
        }

        private async Task TestTypeDeclarationWithDirectivesAsync(string keyword)
        {
            await TestDeclarationWithDirectivesAsync("internal", "TypeName", $"{keyword} TypeName", "{\n}");
        }

        private async Task TestNestedTypeDeclarationAsync(string keyword)
        {
            await TestNestedDeclarationAsync("private", "TypeName", $"{keyword} TypeName", "{\n}");
        }

        private async Task TestNestedTypeDeclarationWithAttributesAsync(string keyword)
        {
            await TestNestedDeclarationWithAttributesAsync("private", "TypeName", $"{keyword} TypeName", "{\n}");
        }

        private async Task TestNestedTypeDeclarationWithDirectivesAsync(string keyword)
        {
            await TestNestedDeclarationWithDirectivesAsync("private", "TypeName", $"{keyword} TypeName", "{\n}");
        }

        private async Task TestDeclarationAsync(string modifier, string identifier, string keywordLine, string linesAfter, string elementName = null, bool warning = true)
        {
            var testCode = $@"
 {Tab} {keywordLine}
{linesAfter}";

            if (!warning)
            {
                await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedTestCode = $@"
 {Tab} {modifier} {keywordLine}
{linesAfter}";

            await VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        private async Task TestDeclarationWithAttributesAsync(string modifier, string identifier, string keywordLine, string linesAfter, string elementName = null, bool warning = true)
        {
            var testCode = $@"
  [Serializable]
 {Tab} {keywordLine}
{linesAfter}";

            if (!warning)
            {
                await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedTestCode = $@"
  [Serializable]
 {Tab} {modifier} {keywordLine}
{linesAfter}";

            await VerifyCSharpFixAsync(testCode, fixedTestCode);
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
                await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedTestCode = $@"
 #  if true
 {Tab} {modifier} {keywordLine}
# endif
{linesAfter}";

            await VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        private async Task TestNestedDeclarationAsync(string modifier, string identifier, string keywordLine, string linesAfter, string containingType = "class", string elementName = null, bool warning = true)
        {
            var testCode = $@"
public {containingType} OuterTypeName {{
 {Tab} {keywordLine}
{linesAfter} }}";

            if (!warning)
            {
                await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedTestCode = $@"
public {containingType} OuterTypeName {{
 {Tab} {modifier} {keywordLine}
{linesAfter} }}";

            await VerifyCSharpFixAsync(testCode, fixedTestCode);
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
                await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedTestCode = $@"
public {containingType} OuterTypeName {{
  [Serializable]
 {Tab} {modifier} {keywordLine}
{linesAfter} }}";

            await VerifyCSharpFixAsync(testCode, fixedTestCode);
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
                await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedTestCode = $@"
public {containingType} OuterTypeName {{
 #  if true
 {Tab} {modifier} {keywordLine}
# endif
{linesAfter} }}";

            await VerifyCSharpFixAsync(testCode, fixedTestCode);
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

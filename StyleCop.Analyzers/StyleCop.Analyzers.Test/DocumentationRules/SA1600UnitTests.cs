namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1600ElementsMustBeDocumented"/>-
    /// </summary>
    public class SA1600UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1600ElementsMustBeDocumented.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeDeclarationDocumentation(string type, string modifiers, bool requiresDiagnostic, bool hasDocumentation)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, type), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestNestedTypeDeclarationDocumentation(string type, string modifiers, bool requiresDiagnostic, bool hasDocumentation)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, type), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestDelegateDeclarationDocumentation(string modifiers, bool requiresDiagnostic, bool hasDocumentation)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestNestedDelegateDeclarationDocumentation(string modifiers, bool requiresDiagnostic, bool hasDocumentation)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestMethodDeclarationDocumentation(string modifiers, bool isExplicitInterfaceMethod, bool requiresDiagnostic, bool hasDocumentation)
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
            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, explicitInterfaceText), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestInterfaceMethodDeclarationDocumentation(bool hasDocumentation)
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

            await this.VerifyCSharpDiagnosticAsync(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, !hasDocumentation ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestInterfacePropertyDeclarationDocumentation(bool hasDocumentation)
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

            await this.VerifyCSharpDiagnosticAsync(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, !hasDocumentation ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestInterfaceEventDeclarationDocumentation(bool hasDocumentation)
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

            await this.VerifyCSharpDiagnosticAsync(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, !hasDocumentation ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestInterfaceIndexerDeclarationDocumentation(bool hasDocumentation)
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

            await this.VerifyCSharpDiagnosticAsync(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, !hasDocumentation ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestConstructorDeclarationDocumentation(string modifiers, bool requiresDiagnostic, bool hasDocumentation)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestDestructorDeclarationDocumentation(bool requiresDiagnostic, bool hasDocumentation)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestPropertyDeclarationDocumentation(string modifiers, bool isExplicitInterfaceProperty, bool requiresDiagnostic, bool hasDocumentation)
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
            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, explicitInterfaceText), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestIndexerDeclarationDocumentation(string modifiers, bool isExplicitInterfaceIndexer, bool requiresDiagnostic, bool hasDocumentation)
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
            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, explicitInterfaceText), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestEventDeclarationDocumentation(string modifiers, bool isExplicitInterfaceEvent, bool requiresDiagnostic, bool hasDocumentation)
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
            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers, explicitInterfaceText), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestFieldDeclarationDocumentation(string modifiers, bool requiresDiagnostic, bool hasDocumentation)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestEventFieldDeclarationDocumentation(string modifiers, bool requiresDiagnostic, bool hasDocumentation)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(hasDocumentation ? testCodeWithDocumentation : testCodeWithoutDocumentation, modifiers), requiresDiagnostic ? expected : EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeWithoutDocumentation(string type)
        {
            await this.TestTypeDeclarationDocumentation(type, string.Empty, true, false);
            await this.TestTypeDeclarationDocumentation(type, "internal", true, false);
            await this.TestTypeDeclarationDocumentation(type, "public", true, false);

            await this.TestNestedTypeDeclarationDocumentation(type, string.Empty, false, false);
            await this.TestNestedTypeDeclarationDocumentation(type, "private", false, false);
            await this.TestNestedTypeDeclarationDocumentation(type, "protected", true, false);
            await this.TestNestedTypeDeclarationDocumentation(type, "internal", true, false);
            await this.TestNestedTypeDeclarationDocumentation(type, "protected internal", true, false);
            await this.TestNestedTypeDeclarationDocumentation(type, "public", true, false);
        }

        private async Task TestTypeWithDocumentation(string type)
        {
            await this.TestTypeDeclarationDocumentation(type, string.Empty, false, true);
            await this.TestTypeDeclarationDocumentation(type, "internal", false, true);
            await this.TestTypeDeclarationDocumentation(type, "public", false, true);

            await this.TestNestedTypeDeclarationDocumentation(type, string.Empty, false, true);
            await this.TestNestedTypeDeclarationDocumentation(type, "private", false, true);
            await this.TestNestedTypeDeclarationDocumentation(type, "protected", false, true);
            await this.TestNestedTypeDeclarationDocumentation(type, "internal", false, true);
            await this.TestNestedTypeDeclarationDocumentation(type, "protected internal", false, true);
            await this.TestNestedTypeDeclarationDocumentation(type, "public", false, true);
        }

        [Fact]
        public async Task TestClassWithoutDocumentation()
        {
            await this.TestTypeWithoutDocumentation("class");
        }

        [Fact]
        public async Task TestStructWithoutDocumentation()
        {
            await this.TestTypeWithoutDocumentation("struct");
        }

        [Fact]
        public async Task TestEnumWithoutDocumentation()
        {
            await this.TestTypeWithoutDocumentation("enum");
        }

        [Fact]
        public async Task TestInterfaceWithoutDocumentation()
        {
            await this.TestTypeWithoutDocumentation("interface");
        }

        [Fact]
        public async Task TestClassWithDocumentation()
        {
            await this.TestTypeWithDocumentation("class");
        }

        [Fact]
        public async Task TestStructWithDocumentation()
        {
            await this.TestTypeWithDocumentation("struct");
        }

        [Fact]
        public async Task TestEnumWithDocumentation()
        {
            await this.TestTypeWithoutDocumentation("enum");
        }

        [Fact]
        public async Task TestInterfaceWithDocumentation()
        {
            await this.TestTypeWithoutDocumentation("interface");
        }

        [Fact]
        public async Task TestDelegateWithoutDocumentation()
        {
            await this.TestDelegateDeclarationDocumentation(string.Empty, true, false);
            await this.TestDelegateDeclarationDocumentation("internal", true, false);
            await this.TestDelegateDeclarationDocumentation("public", true, false);

            await this.TestNestedDelegateDeclarationDocumentation(string.Empty, false, false);
            await this.TestNestedDelegateDeclarationDocumentation("private", false, false);
            await this.TestNestedDelegateDeclarationDocumentation("protected", true, false);
            await this.TestNestedDelegateDeclarationDocumentation("internal", true, false);
            await this.TestNestedDelegateDeclarationDocumentation("protected internal", true, false);
            await this.TestNestedDelegateDeclarationDocumentation("public", true, false);
        }

        [Fact]
        public async Task TestDelegateWithDocumentation()
        {
            await this.TestDelegateDeclarationDocumentation(string.Empty, false, true);
            await this.TestDelegateDeclarationDocumentation("internal", false, true);
            await this.TestDelegateDeclarationDocumentation("public", false, true);

            await this.TestNestedDelegateDeclarationDocumentation(string.Empty, false, true);
            await this.TestNestedDelegateDeclarationDocumentation("private", false, true);
            await this.TestNestedDelegateDeclarationDocumentation("protected", false, true);
            await this.TestNestedDelegateDeclarationDocumentation("internal", false, true);
            await this.TestNestedDelegateDeclarationDocumentation("protected internal", false, true);
            await this.TestNestedDelegateDeclarationDocumentation("public", false, true);
        }

        [Fact]
        public async Task TestMethodWithoutDocumentation()
        {
            await this.TestMethodDeclarationDocumentation(string.Empty, false, false, false);
            await this.TestMethodDeclarationDocumentation(string.Empty, true, true, false);
            await this.TestMethodDeclarationDocumentation("private", false, false, false);
            await this.TestMethodDeclarationDocumentation("protected", false, true, false);
            await this.TestMethodDeclarationDocumentation("internal", false, true, false);
            await this.TestMethodDeclarationDocumentation("protected internal", false, true, false);
            await this.TestMethodDeclarationDocumentation("public", false, true, false);

            await this.TestInterfaceMethodDeclarationDocumentation(false);
        }

        [Fact]
        public async Task TestMethodWithDocumentation()
        {
            await this.TestMethodDeclarationDocumentation(string.Empty, false, false, true);
            await this.TestMethodDeclarationDocumentation(string.Empty, true, false, true);
            await this.TestMethodDeclarationDocumentation("private", false, false, true);
            await this.TestMethodDeclarationDocumentation("protected", false, false, true);
            await this.TestMethodDeclarationDocumentation("internal", false, false, true);
            await this.TestMethodDeclarationDocumentation("protected internal", false, false, true);
            await this.TestMethodDeclarationDocumentation("public", false, false, true);

            await this.TestInterfaceMethodDeclarationDocumentation(true);
        }

        [Fact]
        public async Task TestConstructorWithoutDocumentation()
        {
            await this.TestConstructorDeclarationDocumentation(string.Empty, false, false);
            await this.TestConstructorDeclarationDocumentation("private", false, false);
            await this.TestConstructorDeclarationDocumentation("protected", true, false);
            await this.TestConstructorDeclarationDocumentation("internal", true, false);
            await this.TestConstructorDeclarationDocumentation("protected internal", true, false);
            await this.TestConstructorDeclarationDocumentation("public", true, false);
        }

        [Fact]
        public async Task TestConstructorWithDocumentation()
        {
            await this.TestConstructorDeclarationDocumentation(string.Empty, false, true);
            await this.TestConstructorDeclarationDocumentation("private", false, true);
            await this.TestConstructorDeclarationDocumentation("protected", false, true);
            await this.TestConstructorDeclarationDocumentation("internal", false, true);
            await this.TestConstructorDeclarationDocumentation("protected internal", false, true);
            await this.TestConstructorDeclarationDocumentation("public", false, true);
        }

        [Fact]
        public async Task TestDestructorWithoutDocumentation()
        {
            await this.TestDestructorDeclarationDocumentation(true, false);
        }

        [Fact]
        public async Task TestDestructorWithDocumentation()
        {
            await this.TestDestructorDeclarationDocumentation(false, true);
        }

        [Fact]
        public async Task TestFieldWithoutDocumentation()
        {
            await this.TestFieldDeclarationDocumentation(string.Empty, false, false);
            await this.TestFieldDeclarationDocumentation("private", false, false);
            await this.TestFieldDeclarationDocumentation("protected", true, false);
            await this.TestFieldDeclarationDocumentation("internal", true, false);
            await this.TestFieldDeclarationDocumentation("protected internal", true, false);
            await this.TestFieldDeclarationDocumentation("public", true, false);
        }

        [Fact]
        public async Task TestFieldWithDocumentation()
        {
            await this.TestFieldDeclarationDocumentation(string.Empty, false, true);
            await this.TestFieldDeclarationDocumentation("private", false, true);
            await this.TestFieldDeclarationDocumentation("protected", false, true);
            await this.TestFieldDeclarationDocumentation("internal", false, true);
            await this.TestFieldDeclarationDocumentation("protected internal", false, true);
            await this.TestFieldDeclarationDocumentation("public", false, true);
        }

        [Fact]
        public async Task TestPropertyWithoutDocumentation()
        {
            await this.TestPropertyDeclarationDocumentation(string.Empty, false, false, false);
            await this.TestPropertyDeclarationDocumentation(string.Empty, true, true, false);
            await this.TestPropertyDeclarationDocumentation("private", false, false, false);
            await this.TestPropertyDeclarationDocumentation("protected", false, true, false);
            await this.TestPropertyDeclarationDocumentation("internal", false, true, false);
            await this.TestPropertyDeclarationDocumentation("protected internal", false, true, false);
            await this.TestPropertyDeclarationDocumentation("public", false, true, false);

            await this.TestInterfacePropertyDeclarationDocumentation(false);
        }

        [Fact]
        public async Task TestPropertyWithDocumentation()
        {
            await this.TestPropertyDeclarationDocumentation(string.Empty, false, false, true);
            await this.TestPropertyDeclarationDocumentation(string.Empty, true, false, true);
            await this.TestPropertyDeclarationDocumentation("private", false, false, true);
            await this.TestPropertyDeclarationDocumentation("protected", false, false, true);
            await this.TestPropertyDeclarationDocumentation("internal", false, false, true);
            await this.TestPropertyDeclarationDocumentation("protected internal", false, false, true);
            await this.TestPropertyDeclarationDocumentation("public", false, false, true);

            await this.TestInterfacePropertyDeclarationDocumentation(true);
        }

        [Fact]
        public async Task TestIndexerWithoutDocumentation()
        {
            await this.TestIndexerDeclarationDocumentation(string.Empty, false, false, false);
            await this.TestIndexerDeclarationDocumentation(string.Empty, true, true, false);
            await this.TestIndexerDeclarationDocumentation("private", false, false, false);
            await this.TestIndexerDeclarationDocumentation("protected", false, true, false);
            await this.TestIndexerDeclarationDocumentation("internal", false, true, false);
            await this.TestIndexerDeclarationDocumentation("protected internal", false, true, false);
            await this.TestIndexerDeclarationDocumentation("public", false, true, false);

            await this.TestInterfaceIndexerDeclarationDocumentation(false);
        }

        [Fact]
        public async Task TestIndexerWithDocumentation()
        {
            await this.TestIndexerDeclarationDocumentation(string.Empty, false, false, true);
            await this.TestIndexerDeclarationDocumentation(string.Empty, true, false, true);
            await this.TestIndexerDeclarationDocumentation("private", false, false, true);
            await this.TestIndexerDeclarationDocumentation("protected", false, false, true);
            await this.TestIndexerDeclarationDocumentation("internal", false, false, true);
            await this.TestIndexerDeclarationDocumentation("protected internal", false, false, true);
            await this.TestIndexerDeclarationDocumentation("public", false, false, true);

            await this.TestInterfaceIndexerDeclarationDocumentation(true);
        }

        [Fact]
        public async Task TestEventWithoutDocumentation()
        {
            await this.TestEventDeclarationDocumentation(string.Empty, false, false, false);
            await this.TestEventDeclarationDocumentation(string.Empty, true, true, false);
            await this.TestEventDeclarationDocumentation("private", false, false, false);
            await this.TestEventDeclarationDocumentation("protected", false, true, false);
            await this.TestEventDeclarationDocumentation("internal", false, true, false);
            await this.TestEventDeclarationDocumentation("protected internal", false, true, false);
            await this.TestEventDeclarationDocumentation("public", false, true, false);

            await this.TestInterfaceEventDeclarationDocumentation(false);
        }

        [Fact]
        public async Task TestEventWithDocumentation()
        {
            await this.TestEventDeclarationDocumentation(string.Empty, false, false, true);
            await this.TestEventDeclarationDocumentation(string.Empty, true, false, true);
            await this.TestEventDeclarationDocumentation("private", false, false, true);
            await this.TestEventDeclarationDocumentation("protected", false, false, true);
            await this.TestEventDeclarationDocumentation("internal", false, false, true);
            await this.TestEventDeclarationDocumentation("protected internal", false, false, true);
            await this.TestEventDeclarationDocumentation("public", false, false, true);

            await this.TestInterfaceEventDeclarationDocumentation(true);
        }

        [Fact]
        public async Task TestEventFieldWithoutDocumentation()
        {
            await this.TestEventFieldDeclarationDocumentation(string.Empty, false, false);
            await this.TestEventFieldDeclarationDocumentation("private", false, false);
            await this.TestEventFieldDeclarationDocumentation("protected", true, false);
            await this.TestEventFieldDeclarationDocumentation("internal", true, false);
            await this.TestEventFieldDeclarationDocumentation("protected internal", true, false);
            await this.TestEventFieldDeclarationDocumentation("public", true, false);
        }

        [Fact]
        public async Task TestEventFieldWithDocumentation()
        {
            await this.TestEventFieldDeclarationDocumentation(string.Empty, false, true);
            await this.TestEventFieldDeclarationDocumentation("private", false, true);
            await this.TestEventFieldDeclarationDocumentation("protected", false, true);
            await this.TestEventFieldDeclarationDocumentation("internal", false, true);
            await this.TestEventFieldDeclarationDocumentation("protected internal", false, true);
            await this.TestEventFieldDeclarationDocumentation("public", false, true);
        }

        [Fact]
        public async Task TestEmptyXmlComments()
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

            await this.VerifyCSharpDiagnosticAsync(testCodeWithDocumentation, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(testCodeWithEmptyDocumentation, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestCDataXmlComments()
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

            await this.VerifyCSharpDiagnosticAsync(testCodeWithDocumentation, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(testCodeWithEmptyDocumentation, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestEmptyElementXmlComments()
        {
            var testCodeWithDocumentation = @"/// <inheritdoc/>
public class OuterClass
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCodeWithDocumentation, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMultiLineDocumentation()
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

            await this.VerifyCSharpDiagnosticAsync(testCodeWithDocumentation, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1600ElementsMustBeDocumented();
        }
    }
}
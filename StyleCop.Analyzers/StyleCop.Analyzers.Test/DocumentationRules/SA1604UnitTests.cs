namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1604ElementDocumentationMustHaveSummary"/>-
    /// </summary>
    public class SA1604UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1604ElementDocumentationMustHaveSummary.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeNoDocumentation(string typeName)
        {
            var testCode = @"
{0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeWithDocumentation(string typeName)
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
{0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeWithInheritedDocumentation(string typeName)
        {
            var testCode = @"
/// <inheritdoc/>
{0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeWithoutDocumentation(string typeName)
        {
            var testCode = @"
///
{0}
TypeName
{{
}}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(4, 1)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestEnumWithDocumentation()
        {
            await this.TestTypeWithDocumentation("enum");
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
        public async Task TestInterfaceWithDocumentation()
        {
            await this.TestTypeWithDocumentation("interface");
        }

        [Fact]
        public async Task TestEnumWithInheritedDocumentation()
        {
            await this.TestTypeWithInheritedDocumentation("enum");
        }

        [Fact]
        public async Task TestClassWithInheritedDocumentation()
        {
            await this.TestTypeWithInheritedDocumentation("class");
        }

        [Fact]
        public async Task TestStructWithInheritedDocumentation()
        {
            await this.TestTypeWithInheritedDocumentation("struct");
        }

        [Fact]
        public async Task TestInterfaceWithInheritedDocumentation()
        {
            await this.TestTypeWithInheritedDocumentation("interface");
        }

        [Fact]
        public async Task TestEnumWithoutDocumentation()
        {
            await this.TestTypeWithoutDocumentation("enum");
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
        public async Task TestInterfaceWithoutDocumentation()
        {
            await this.TestTypeWithoutDocumentation("interface");
        }

        [Fact]
        public async Task TestEnumNoDocumentation()
        {
            await this.TestTypeNoDocumentation("enum");
        }

        [Fact]
        public async Task TestClassNoDocumentation()
        {
            await this.TestTypeNoDocumentation("class");
        }

        [Fact]
        public async Task TestStructNoDocumentation()
        {
            await this.TestTypeNoDocumentation("struct");
        }

        [Fact]
        public async Task TestInterfaceNoDocumentation()
        {
            await this.TestTypeNoDocumentation("interface");
        }

        [Fact]
        public async Task TestDelegateNoDocumentation()
        {
            var testCode = @"
public delegate 
TypeName();";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestDelegateWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public delegate 
TypeName();";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestDelegateWithInheritedDocumentation()
        {
            var testCode = @"
/// <inheritdoc/>
public delegate 
TypeName();";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestDelegateWithoutDocumentation()
        {
            var testCode = @"
///
public delegate 
void TypeName();";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(4, 6)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public void Test() { }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    ///
    /// </summary>
    public void Test() { }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public void Test() { }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    ///
    public void Test() { }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(8, 17)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public ClassName() { }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    ///
    /// </summary>
    public ClassName() { }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ClassName() { }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    ///
    public ClassName() { }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(8, 12)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestDestructorNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    ~ClassName() { }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestDestructorWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    ///
    /// </summary>
    ~ClassName() { }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestDestructorWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    ~ClassName() { }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestDestructorWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    ///
    ~ClassName() { }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(8, 6)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    ///
    /// </summary>
    public ClassName Property { get; set; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ClassName Property { get; set; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public ClassName Property { get; set; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    ///
    public ClassName Property { get; set; }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(8, 22)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    ///
    /// </summary>
    public ClassName this[string t] { get { return null; } }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ClassName this[string t] { get { return null; } }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public ClassName this[string t] { get { return null; } }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    ///
    public ClassName this[string t] { get { return null; } }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(8, 22)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestFieldWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    ///
    /// </summary>
    public ClassName Foo;
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestFieldWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ClassName Foo;
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestFieldNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public ClassName Foo;
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestFieldWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    ///
    public ClassName Foo;
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(8, 22)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    ///
    /// </summary>
    public event System.Action Foo;
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public event System.Action Foo;
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public ClassName Foo;
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    ///
    public event System.Action Foo;
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(8, 32)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventPropertyWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public interface InterfaceName
{
    /// <summary>
    ///
    /// </summary>
    event System.Action Foo { add; remove; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventPropertyWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public interface InterfaceName
{
    /// <inheritdoc/>
    event System.Action Foo { add; remove; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventPropertyNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public interface InterfaceName
{
    event System.Action Foo { add; remove; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventPropertyWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public interface InterfaceName
{
    ///
    event System.Action Foo { add; remove; }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(8, 25)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1604ElementDocumentationMustHaveSummary();
        }
    }
}

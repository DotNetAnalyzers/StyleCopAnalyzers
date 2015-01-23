namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1606ElementDocumentationMustHaveSummaryText"/>-
    /// </summary>
    [TestClass]
    public class SA1606UnitTests : CodeFixVerifier
    {
        protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };

        public string DiagnosticId { get; } = SA1606ElementDocumentationMustHaveSummaryText.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeNoDocumentation(string typeName)
        {
            var testCode = @"
{0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeWithDocumentation(string typeName)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
{0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeWithInheritedDocumentation(string typeName)
        {
            var testCode = @"
/// <inheritdoc/>
{0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeWithoutDocumentation(string typeName)
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
{0}
TypeName
{{
}}";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Element documentation must have summary text",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 1)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEnumWithDocumentation()
        {
            await TestTypeWithDocumentation("enum");
        }

        [TestMethod]
        public async Task TestClassWithDocumentation()
        {
            await TestTypeWithDocumentation("class");
        }

        [TestMethod]
        public async Task TestStructWithDocumentation()
        {
            await TestTypeWithDocumentation("struct");
        }

        [TestMethod]
        public async Task TestInterfaceWithDocumentation()
        {
            await TestTypeWithDocumentation("interface");
        }

        [TestMethod]
        public async Task TestEnumWithInheritedDocumentation()
        {
            await TestTypeWithInheritedDocumentation("enum");
        }

        [TestMethod]
        public async Task TestClassWithInheritedDocumentation()
        {
            await TestTypeWithInheritedDocumentation("class");
        }

        [TestMethod]
        public async Task TestStructWithInheritedDocumentation()
        {
            await TestTypeWithInheritedDocumentation("struct");
        }

        [TestMethod]
        public async Task TestInterfaceWithInheritedDocumentation()
        {
            await TestTypeWithInheritedDocumentation("interface");
        }

        [TestMethod]
        public async Task TestEnumWithoutDocumentation()
        {
            await TestTypeWithoutDocumentation("enum");
        }

        [TestMethod]
        public async Task TestClassWithoutDocumentation()
        {
            await TestTypeWithoutDocumentation("class");
        }

        [TestMethod]
        public async Task TestStructWithoutDocumentation()
        {
            await TestTypeWithoutDocumentation("struct");
        }

        [TestMethod]
        public async Task TestInterfaceWithoutDocumentation()
        {
            await TestTypeWithoutDocumentation("interface");
        }

        [TestMethod]
        public async Task TestEnumNoDocumentation()
        {
            await TestTypeNoDocumentation("enum");
        }

        [TestMethod]
        public async Task TestClassNoDocumentation()
        {
            await TestTypeNoDocumentation("class");
        }

        [TestMethod]
        public async Task TestStructNoDocumentation()
        {
            await TestTypeNoDocumentation("struct");
        }

        [TestMethod]
        public async Task TestInterfaceNoDocumentation()
        {
            await TestTypeNoDocumentation("interface");
        }

        [TestMethod]
        public async Task TestDelegateNoDocumentation()
        {
            var testCode = @"
public delegate 
TypeName();";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDelegateWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public delegate 
TypeName();";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDelegateWithInheritedDocumentation()
        {
            var testCode = @"
/// <inheritdoc/>
public delegate 
TypeName();";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDelegateWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public delegate 
void TypeName();";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Element documentation must have summary text",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 6)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public void Test() { }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodWithDocumentation()
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
    public void Test() { }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public void Test() { }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// 
    /// </summary>
    public void Test() { }
}";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Element documentation must have summary text",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 10, 17)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ClassName() { }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorWithDocumentation()
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
    public ClassName() { }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ClassName() { }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// 
    /// </summary>
    public ClassName() { }
}";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Element documentation must have summary text",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 10, 12)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDestructorNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    ~ClassName() { }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDestructorWithDocumentation()
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
    ~ClassName() { }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDestructorWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    ~ClassName() { }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDestructorWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// 
    /// </summary>
    ~ClassName() { }
}";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Element documentation must have summary text",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 10, 6)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPropertyWithDocumentation()
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
    public ClassName Property { get; set; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPropertyWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ClassName Property { get; set; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPropertyNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ClassName Property { get; set; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPropertyWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// 
    /// </summary>
    public ClassName Property { get; set; }
}";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Element documentation must have summary text",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 10, 22)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexerWithDocumentation()
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
    public ClassName this[string t] { get { return null; } }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexerWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ClassName this[string t] { get { return null; } }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexerNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ClassName this[string t] { get { return null; } }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexerWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// 
    /// </summary>
    public ClassName this[string t] { get { return null; } }
}";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Element documentation must have summary text",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 10, 22)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestFieldWithDocumentation()
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
    public ClassName Foo;
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestFieldWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ClassName Foo;
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestFieldNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ClassName Foo;
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestFieldWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// 
    /// </summary>
    public ClassName Foo;
}";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Element documentation must have summary text",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 10, 22)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEventWithDocumentation()
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
    public event System.Action Foo;
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEventWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public event System.Action Foo;
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEventNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ClassName Foo;
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEventWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// 
    /// </summary>
    public event System.Action Foo;
}";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Element documentation must have summary text",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 10, 32)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEventPropertyWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public interface InterfaceName
{
    /// <summary>
    /// Foo
    /// </summary>
    event System.Action Foo { add; remove; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEventPropertyWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public interface InterfaceName
{
    /// <inheritdoc/>
    event System.Action Foo { add; remove; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEventPropertyNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public interface InterfaceName
{
    event System.Action Foo { add; remove; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEventPropertyWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public interface InterfaceName
{
    /// <summary>
    /// 
    /// </summary>
    event System.Action Foo { add; remove; }
}";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Element documentation must have summary text",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 10, 25)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1606ElementDocumentationMustHaveSummaryText();
        }
    }
}

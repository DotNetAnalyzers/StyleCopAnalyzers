// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1604ElementDocumentationMustHaveSummary>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1604ElementDocumentationMustHaveSummary"/>.
    /// </summary>
    public class SA1604UnitTests
    {
        /// <summary>
        /// Verifies that an excluded public class does not require a summary by default.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        [WorkItem(3094, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3094")]
        public async Task TestExcludedClassWithoutSummaryAsync()
        {
            var testCode = @"/// <exclude/>
public class Class1 { }";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that excluded declarations follow private documentation settings.
        /// </summary>
        /// <param name="declaration">The declaration to test.</param>
        /// <param name="documentPrivate">Whether private declarations require documentation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Theory]
        [CombinatorialData]
        [WorkItem(3094, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3094")]
        public async Task TestExcludedDeclarationAsync(
            [CombinatorialValues(
                "public class {|#0:NestedClass|} { }",
                "public struct {|#0:Struct1|} { }",
                "public interface {|#0:Interface1|} { }",
                "public enum {|#0:Enum1|} { }",
                "public delegate void {|#0:Delegate1|}();",
                "public void {|#0:Method1|}() { }",
                "public {|#0:Class1|}() { }",
                "public int {|#0:Property1|} { get; set; }",
                "public int {|#0:this|}[int index] => index;",
                "public int {|#0:Field1|}, {|#1:Field2|};",
                "public event System.Action {|#0:Event1|}, {|#1:Event2|};",
                "public event System.Action {|#0:Event1|} { add { } remove { } }")] string declaration,
            bool documentPrivate)
        {
            var testCode = @"public class Class1
{
    /// <exclude/>
    " + declaration + @"
}";
            var test = new StyleCopDiagnosticVerifier<SA1604ElementDocumentationMustHaveSummary>.CSharpTest
            {
                TestCode = testCode,
                Settings = @"{
    ""settings"": {
        ""documentationRules"": {
            ""documentPrivateElements"": " + (documentPrivate ? "true" : "false") + @",
            ""documentPrivateFields"": " + (documentPrivate ? "true" : "false") + @"
        }
    }
}",
            };

            if (documentPrivate)
            {
                test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
                if (declaration.Contains("{|#1:"))
                {
                    test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(1));
                }
            }

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that only a top-level exclude tag relaxes the summary requirement.
        /// </summary>
        /// <param name="documentation">The documentation to test.</param>
        /// <param name="expectDiagnostic">Whether a summary is still required.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Theory]
        [InlineData("<exclude></exclude>", false)]
        [InlineData("<remarks><exclude/></remarks>", true)]
        [InlineData("<Exclude/>", true)]
        [InlineData("<remarks/>", true)]
        [InlineData("<exclude/><summary>Summary.</summary>", false)]
        [WorkItem(3094, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3094")]
        public async Task TestExcludeTagWithoutSummaryAsync(string documentation, bool expectDiagnostic)
        {
            var testCode = "/// " + documentation + @"
public class {|#0:Class1|} { }";
            var expected = expectDiagnostic ? new[] { Diagnostic().WithLocation(0) } : DiagnosticResult.EmptyDiagnosticResults;
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that excluded fields use the private-field setting independently of other private elements.
        /// </summary>
        /// <param name="documentPrivateElements">Whether private elements require documentation.</param>
        /// <param name="documentPrivateFields">Whether private fields require documentation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Theory]
        [CombinatorialData]
        [WorkItem(3094, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3094")]
        public async Task TestExcludedFieldSettingsAsync(bool documentPrivateElements, bool documentPrivateFields)
        {
            var test = new StyleCopDiagnosticVerifier<SA1604ElementDocumentationMustHaveSummary>.CSharpTest
            {
                TestCode = @"public class Class1
{
    /// <exclude/>
    public int {|#0:Field1|};

    /// <exclude/>
    public int {|#1:Property1|} { get; set; }
}",
                Settings = @"{
    ""settings"": {
        ""documentationRules"": {
            ""documentPrivateElements"": " + (documentPrivateElements ? "true" : "false") + @",
            ""documentPrivateFields"": " + (documentPrivateFields ? "true" : "false") + @"
        }
    }
}",
            };

            if (documentPrivateFields)
            {
                test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
            }

            if (documentPrivateElements)
            {
                test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(1));
            }

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that exclude tags in included documentation follow private documentation settings.
        /// </summary>
        /// <param name="documentPrivate">Whether private declarations require documentation.</param>
        /// <param name="hasSummary">Whether the included documentation contains a summary.</param>
        /// <param name="inlineExclude">Whether the exclude tag appears outside the included documentation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Theory]
        [CombinatorialData]
        [WorkItem(3094, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3094")]
        public async Task TestIncludedExcludedDocumentationAsync(bool documentPrivate, bool hasSummary, bool inlineExclude)
        {
            var test = new StyleCopDiagnosticVerifier<SA1604ElementDocumentationMustHaveSummary>.CSharpTest
            {
                TestCode = (inlineExclude ? "/// <exclude/>\r\n" : string.Empty) + @"/// <include file='Excluded.xml' path='/doc/*'/>
public class {|#0:Class1|} { }",
                Settings = @"{ ""settings"": { ""documentationRules"": { ""documentPrivateElements"": " + (documentPrivate ? "true" : "false") + @" } } }",
                XmlReferences =
                {
                    { "Excluded.xml", "<doc>" + (inlineExclude ? string.Empty : "<exclude/>") + (hasSummary ? "<summary>Summary.</summary>" : string.Empty) + "</doc>" },
                },
            };

            if (documentPrivate && !hasSummary)
            {
                test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
            }

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestTypeNoDocumentationAsync(string typeName)
        {
            var testCode = @"
{0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestTypeWithDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
{0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestTypeWithInheritedDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <inheritdoc/>
{0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestTypeWithoutDocumentationAsync(string typeName)
        {
            var testCode = @"
///
{0}
TypeName
{{
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 1);

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestPartialTypeWithoutDocumentationAsync(string typeName)
        {
            var testCode = @"
///
partial {0}
TypeName
{{
}}";

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateNoDocumentationAsync()
        {
            var testCode = @"
public delegate
void TypeName();";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateWithDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public delegate
void TypeName();";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateWithInheritedDocumentationAsync()
        {
            var testCode = @"
/// <inheritdoc/>
public delegate
void TypeName();";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateWithoutDocumentationAsync()
        {
            var testCode = @"
///
public delegate
void TypeName();";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 6);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public void Test() { }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithDocumentationAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithInheritedDocumentationAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithoutDocumentationAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 17);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2443, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2443")]
        public async Task TestPrivateMethodWithoutSummaryAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
internal class ClassName
{
    ///
    private void Test1() { }

    /**
     *
     */
    private void Test2() { }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialMethodWithoutDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public partial class ClassName
{
    ///
    partial void Test();
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public ClassName() { }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorWithDocumentationAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorWithInheritedDocumentationAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorWithoutDocumentationAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 12);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    ~ClassName() { }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorWithDocumentationAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorWithInheritedDocumentationAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorWithoutDocumentationAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 6);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithDocumentationAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithInheritedDocumentationAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public ClassName Property { get; set; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithoutDocumentationAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 22);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerWithDocumentationAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerWithInheritedDocumentationAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public ClassName this[string t] { get { return null; } }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerWithoutDocumentationAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 22);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWithDocumentationAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWithInheritedDocumentationAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public ClassName Foo;
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWithoutDocumentationAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 22);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventWithDocumentationAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventWithInheritedDocumentationAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public ClassName Foo;
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventWithoutDocumentationAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 32);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventPropertyWithDocumentationAsync()
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
    event System.Action Foo { add { } remove { } }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventPropertyWithInheritedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    event System.Action Foo { add { } remove { } }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventPropertyNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    event System.Action Foo { add { } remove { } }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventPropertyWithoutDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    ///
    public event System.Action Foo { add { } remove { } }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 32);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedDocumentationAsync()
        {
            var testCode = @"
class Class1
{
    /// <include file='ClassWithoutSummary.xml' path='/Class1/MethodName/*'/>
    public void MethodName()
    {
    }
}
";

            DiagnosticResult expected = Diagnostic().WithLocation(5, 17);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedInheritedDocumentationAsync()
        {
            var testCode = @"
class Class1
{
    /// <include file='ClassWithInheritedSummary.xml' path='/Class1/MethodName/*'/>
    public void MethodName()
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedIncompleteDocumentationAsync()
        {
            var testCode = @"
class Class1
{
    /// <include file='ClassWithSummary.xml' path='/Class1/MethodName/*'/>
    public void MethodName()
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, new[] { expected }, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            string contentWithSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Class1>
  <MethodName>
    <summary>
      Sample method.
    </summary>
    <returns>
      A <see cref=""Task""/> representing the asynchronous operation.
    </returns>
  </MethodName>
</Class1>
";
            string contentWithInheritedSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Class1>
  <MethodName>
    <inheritdoc/>
  </MethodName>
</Class1>
";
            string contentWithoutSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Class1>
  <MethodName>
    <returns>
      A <see cref=""Task""/> representing the asynchronous operation.
    </returns>
  </MethodName>
</Class1>
";

            var test = new StyleCopDiagnosticVerifier<SA1604ElementDocumentationMustHaveSummary>.CSharpTest
            {
                TestCode = source,
                XmlReferences =
                {
                    { "ClassWithSummary.xml", contentWithSummary },
                    { "ClassWithInheritedSummary.xml", contentWithInheritedSummary },
                    { "ClassWithoutSummary.xml", contentWithoutSummary },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}

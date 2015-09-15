// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1604ElementDocumentationMustHaveSummary"/>.
    /// </summary>
    public class SA1604UnitTests : DiagnosticVerifier
    {
        [Theory]
        [InlineData("enum")]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeNoDocumentationAsync(string typeName)
        {
            var testCode = @"
{0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("enum")]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
{0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("enum")]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithInheritedDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <inheritdoc/>
{0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("enum")]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithoutDocumentationAsync(string typeName)
        {
            var testCode = @"
///
{0}
TypeName
{{
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 1);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("partial class")]
        [InlineData("partial struct")]
        [InlineData("partial interface")]
        public async Task TestPartialTypeWithoutDocumentationAsync(string typeName)
        {
            var testCode = @"
///
{0}
TypeName
{{
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateNoDocumentationAsync()
        {
            var testCode = @"
public delegate
void TypeName();";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateWithInheritedDocumentationAsync()
        {
            var testCode = @"
/// <inheritdoc/>
public delegate
void TypeName();";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateWithoutDocumentationAsync()
        {
            var testCode = @"
///
public delegate
void TypeName();";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 6);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 17);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 12);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 6);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 32);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    event System.Action Foo { add { } remove { } }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 25);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1604ElementDocumentationMustHaveSummary();
        }
    }
}

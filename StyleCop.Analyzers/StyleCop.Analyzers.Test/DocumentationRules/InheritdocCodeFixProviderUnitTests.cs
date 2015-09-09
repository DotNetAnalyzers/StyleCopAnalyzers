// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="InheritdocCodeFixProvider"/>.
    /// </summary>
    public class InheritdocCodeFixProviderUnitTests : CodeFixVerifier
    {
        [Theory]
        [InlineData("string TestMember { get; set; }")]
        [InlineData("string TestMember() { return null; }")]
        [InlineData("string this[int a] { get { return \"a\"; } set { } }")]
        [InlineData("event EventHandler TestMember { add { } remove { } }")]
        public async Task TestClassVirtualInheritedMembersAsync(string memberData)
        {
            var testCode = $@"using System;
public class ParentClass
{{
    /// <summary>
    /// Some documentation.
    /// </summary>
    public virtual {memberData}
}}

public class ChildClass : ParentClass
{{
    public override {memberData}
}}
";

            var fixedCode = $@"using System;
public class ParentClass
{{
    /// <summary>
    /// Some documentation.
    /// </summary>
    public virtual {memberData}
}}

public class ChildClass : ParentClass
{{
    /// <inheritdoc/>
    public override {memberData}
}}
";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("string TestMember { get; set; }", "string TestMember { get; set; }")]
        [InlineData("string TestMember();", "string TestMember() { return null; }")]
        [InlineData("string this[int a] { get; set; }", "string this[int a] { get { return \"a\"; } set { } }")]
        [InlineData("event EventHandler TestMember;", "event EventHandler TestMember { add { } remove { } }")]
        public async Task TestInterfaceInheritedMembersAsync(string parentData, string childData)
        {
            var testCode = $@"using System;
public interface IParent
{{
    /// <summary>
    /// Some documentation.
    /// </summary>
    {parentData}
}}

public class ChildClass : IParent
{{
    public {childData}
}}
";

            var fixedCode = $@"using System;
public interface IParent
{{
    /// <summary>
    /// Some documentation.
    /// </summary>
    {parentData}
}}

public class ChildClass : IParent
{{
    /// <inheritdoc/>
    public {childData}
}}
";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("string TestMember { get; set; }")]
        [InlineData("string TestMember() { return null; }")]
        [InlineData("string this[int a] { get { return \"a\"; } set { } }")]
        [InlineData("event EventHandler TestMember { add { } remove { } }")]
        public async Task TestNonvirtualHiddenInheritedMembersAsync(string memberData)
        {
            var testCode = $@"using System;
public class ParentClass
{{
    /// <summary>
    /// Some documentation.
    /// </summary>
    public {memberData}
}}

public class ChildClass : ParentClass
{{
    public new {memberData}
}}
";

            var fixedCode = $@"using System;
public class ParentClass
{{
    /// <summary>
    /// Some documentation.
    /// </summary>
    public {memberData}
}}

public class ChildClass : ParentClass
{{
    public new {memberData}
}}
";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("string TestMember { get; set; }")]
        [InlineData("string TestMember() { return null; }")]
        [InlineData("string this[int a] { get { return \"a\"; } set { } }")]
        [InlineData("event EventHandler TestMember { add { } remove { } }")]
        public async Task TestVirtualHiddenInheritedMembersAsync(string memberData)
        {
            var testCode = $@"using System;
public class ParentClass
{{
    /// <summary>
    /// Some documentation.
    /// </summary>
    public virtual {memberData}
}}

public class ChildClass : ParentClass
{{
    public new {memberData}
}}
";

            var fixedCode = $@"using System;
public class ParentClass
{{
    /// <summary>
    /// Some documentation.
    /// </summary>
    public virtual {memberData}
}}

public class ChildClass : ParentClass
{{
    public new {memberData}
}}
";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1600ElementsMustBeDocumented();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new InheritdocCodeFixProvider();
        }
    }
}

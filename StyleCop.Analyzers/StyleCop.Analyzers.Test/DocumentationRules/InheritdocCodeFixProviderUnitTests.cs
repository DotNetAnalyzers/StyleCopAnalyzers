// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
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
        private static readonly DiagnosticDescriptor SA1600 = new SA1600ElementsMustBeDocumented().SupportedDiagnostics[0];
        private static readonly DiagnosticDescriptor CS1591 =
            new DiagnosticDescriptor(nameof(CS1591), "Title", "Missing XML comment for publicly visible type or member '{0}'", "Category", DiagnosticSeverity.Error, AnalyzerConstants.EnabledByDefault);

        private DiagnosticDescriptor descriptor = SA1600;

        [Theory]
        [InlineData(false, null, "string             TestMember { get; set; }")]
        [InlineData(false, null, "string             TestMember() { return null; }")]
        [InlineData(false, null, "string             this[int a] { get { return \"a\"; } set { } }")]
        [InlineData(false, null, "event EventHandler TestMember { add { } remove { } }")]
        [InlineData(true, "ChildClass.TestMember", "string             TestMember { get; set; }")]
        [InlineData(true, "ChildClass.TestMember()", "string             TestMember() { return null; }")]
        [InlineData(true, "ChildClass.this[int]", "string             this[int a] { get { return \"a\"; } set { } }")]
        [InlineData(true, "ChildClass.TestMember", "event EventHandler TestMember { add { } remove { } }")]
        public async Task TestClassVirtualInheritedMembersAsync(bool compilerWarning, string memberName, string memberData)
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

            if (compilerWarning)
            {
                this.descriptor = CS1591;
            }

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(this.descriptor).WithArguments("ParentClass").WithLocation(2, 14),
                this.CSharpDiagnostic(this.descriptor).WithArguments("ChildClass").WithLocation(10, 14),
                this.CSharpDiagnostic(this.descriptor).WithArguments(memberName).WithLocation(12, 40),
            };

            DiagnosticResult[] expectedFixed =
            {
                this.CSharpDiagnostic(this.descriptor).WithArguments("ParentClass").WithLocation(2, 14),
                this.CSharpDiagnostic(this.descriptor).WithArguments("ChildClass").WithLocation(10, 14),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, expectedFixed, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, numberOfFixAllIterations: 2, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(false, null, "string             TestMember { get; set; }", "string             TestMember { get; set; }")]
        [InlineData(false, null, "string             TestMember();", "string             TestMember() { return null; }")]
        [InlineData(false, null, "string             this[int a] { get; set; }", "string             this[int a] { get { return \"a\"; } set { } }")]
        [InlineData(false, null, "event EventHandler TestMember;", "event EventHandler TestMember { add { } remove { } }")]
        [InlineData(true, "ChildClass.TestMember", "string             TestMember { get; set; }", "string             TestMember { get; set; }")]
        [InlineData(true, "ChildClass.TestMember()", "string             TestMember();", "string             TestMember() { return null; }")]
        [InlineData(true, "ChildClass.this[int]", "string             this[int a] { get; set; }", "string             this[int a] { get { return \"a\"; } set { } }")]
        [InlineData(true, "ChildClass.TestMember", "event EventHandler TestMember;", "event EventHandler TestMember { add { } remove { } }")]
        public async Task TestInterfaceInheritedMembersAsync(bool compilerWarning, string memberName, string parentData, string childData)
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

            if (compilerWarning)
            {
                this.descriptor = CS1591;
            }

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(this.descriptor).WithArguments("IParent").WithLocation(2, 18),
                this.CSharpDiagnostic(this.descriptor).WithArguments("ChildClass").WithLocation(10, 14),
                this.CSharpDiagnostic(this.descriptor).WithArguments(memberName).WithLocation(12, 31),
            };

            DiagnosticResult[] expectedFixed =
            {
                this.CSharpDiagnostic(this.descriptor).WithArguments("IParent").WithLocation(2, 18),
                this.CSharpDiagnostic(this.descriptor).WithArguments("ChildClass").WithLocation(10, 14),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, expectedFixed, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, numberOfFixAllIterations: 2, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("string             TestMember { get; set; }")]
        [InlineData("string             TestMember() { return null; }")]
        [InlineData("string             this[int a] { get { return \"a\"; } set { } }")]
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

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(this.descriptor).WithLocation(2, 14),
                this.CSharpDiagnostic(this.descriptor).WithLocation(10, 14),
                this.CSharpDiagnostic(this.descriptor).WithLocation(12, 35),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, testCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("string             TestMember { get; set; }")]
        [InlineData("string             TestMember() { return null; }")]
        [InlineData("string             this[int a] { get { return \"a\"; } set { } }")]
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

            var fixedCode = testCode;

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(this.descriptor).WithLocation(2, 14),
                this.CSharpDiagnostic(this.descriptor).WithLocation(10, 14),
                this.CSharpDiagnostic(this.descriptor).WithLocation(12, 35),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetDisabledDiagnostics()
        {
            if (this.descriptor == CS1591)
            {
                yield return SA1600.Id;
            }
        }

        /// <inheritdoc/>
        protected override Project ApplyCompilationOptions(Project project)
        {
            project = base.ApplyCompilationOptions(project);

            if (this.descriptor == CS1591)
            {
                var supportedDiagnosticsSpecificOptions = new Dictionary<string, ReportDiagnostic>();
                supportedDiagnosticsSpecificOptions.Add(CS1591.Id, ReportDiagnostic.Error);

                // update the project compilation options
                var modifiedSpecificDiagnosticOptions = project.CompilationOptions.SpecificDiagnosticOptions.SetItem(CS1591.Id, ReportDiagnostic.Error);
                var modifiedCompilationOptions = project.CompilationOptions.WithSpecificDiagnosticOptions(modifiedSpecificDiagnosticOptions);

                Solution solution = project.Solution.WithProjectCompilationOptions(project.Id, modifiedCompilationOptions);
                project = solution.GetProject(project.Id);
            }

            return project;
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

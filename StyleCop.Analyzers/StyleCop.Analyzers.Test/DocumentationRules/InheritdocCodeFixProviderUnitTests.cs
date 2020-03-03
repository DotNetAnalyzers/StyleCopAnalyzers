// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1600ElementsMustBeDocumented,
        StyleCop.Analyzers.DocumentationRules.InheritdocCodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for <see cref="InheritdocCodeFixProvider"/>.
    /// </summary>
    public class InheritdocCodeFixProviderUnitTests
    {
        private static readonly DiagnosticDescriptor SA1600 = new SA1600ElementsMustBeDocumented().SupportedDiagnostics[0];
        private static readonly DiagnosticDescriptor CS1591 =
            new DiagnosticDescriptor(nameof(CS1591), "Title", "Missing XML comment for publicly visible type or member '{0}'", "Category", DiagnosticSeverity.Error, AnalyzerConstants.EnabledByDefault);

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

            var descriptor = compilerWarning ? CS1591 : SA1600;

            var test = new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(descriptor).WithArguments("ParentClass").WithLocation(2, 14),
                    Diagnostic(descriptor).WithArguments("ChildClass").WithLocation(10, 14),
                    Diagnostic(descriptor).WithArguments(memberName).WithLocation(12, 40),
                },
                FixedCode = fixedCode,
                RemainingDiagnostics =
                {
                    Diagnostic(descriptor).WithArguments("ParentClass").WithLocation(2, 14),
                    Diagnostic(descriptor).WithArguments("ChildClass").WithLocation(10, 14),
                },
            };

            if (compilerWarning)
            {
                test.DisabledDiagnostics.Add(SA1600.Id);
                test.SolutionTransforms.Add(SetCompilerDocumentationWarningToError);
            }

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

            var descriptor = compilerWarning ? CS1591 : SA1600;

            var test = new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(descriptor).WithArguments("IParent").WithLocation(2, 18),
                    Diagnostic(descriptor).WithArguments("ChildClass").WithLocation(10, 14),
                    Diagnostic(descriptor).WithArguments(memberName).WithLocation(12, 31),
                },
                FixedCode = fixedCode,
                RemainingDiagnostics =
                {
                    Diagnostic(descriptor).WithArguments("IParent").WithLocation(2, 18),
                    Diagnostic(descriptor).WithArguments("ChildClass").WithLocation(10, 14),
                },
            };

            if (compilerWarning)
            {
                test.DisabledDiagnostics.Add(SA1600.Id);
                test.SolutionTransforms.Add(SetCompilerDocumentationWarningToError);
            }

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(SA1600).WithLocation(2, 14),
                    Diagnostic(SA1600).WithLocation(10, 14),
                    Diagnostic(SA1600).WithLocation(12, 35),
                },
                FixedCode = testCode,
                NumberOfIncrementalIterations = 1,
                NumberOfFixAllIterations = 1,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(SA1600).WithLocation(2, 14),
                    Diagnostic(SA1600).WithLocation(10, 14),
                    Diagnostic(SA1600).WithLocation(12, 35),
                },
                FixedCode = testCode,
                NumberOfIncrementalIterations = 1,
                NumberOfFixAllIterations = 1,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        private static Solution SetCompilerDocumentationWarningToError(Solution solution, ProjectId projectId)
        {
            var project = solution.GetProject(projectId);

            // update the project compilation options
            var modifiedSpecificDiagnosticOptions = project.CompilationOptions.SpecificDiagnosticOptions.SetItem(CS1591.Id, ReportDiagnostic.Error);
            var modifiedCompilationOptions = project.CompilationOptions.WithSpecificDiagnosticOptions(modifiedSpecificDiagnosticOptions);

            return solution.WithProjectCompilationOptions(projectId, modifiedCompilationOptions);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    /// <summary>
    /// A call to a member from an inherited class begins with <c>base.</c>, and the local class does not contain an
    /// override or implementation of the member.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains a call to a member from the base class prefixed
    /// with <c>base.</c>, and there is no local implementation of the member. For example:</para>
    ///
    /// <code language="cs">
    /// string name = base.JoinName("John", "Doe");
    /// </code>
    ///
    /// <para>This rule is in place to prevent a potential source of bugs.Consider a base class which contains the
    /// following virtual method:</para>
    ///
    /// <code language="cs">
    /// public virtual string JoinName(string first, string last)
    /// {
    /// }
    /// </code>
    ///
    /// <para>Another class inherits from this base class but does not provide a local override of this method.
    /// Somewhere within this class, the base class method is called using <c>base.JoinName(...)</c>. This works as
    /// expected. At a later date, someone adds a local override of this method to the class:</para>
    ///
    /// <code language="cs">
    /// public override string JoinName(string first, string last)
    /// {
    ///   return “Bob”;
    /// }
    /// </code>
    ///
    /// <para>At this point, the local call to <c>base.JoinName(...)</c> most likely introduces a bug into the code.
    /// This call will always call the base class method and will cause the local override to be ignored.</para>
    ///
    /// <para>For this reason, calls to members from a base class should not begin with <c>base.</c>, unless a local
    /// override is implemented, and the developer wants to specifically call the base class member. When there is no
    /// local override of the base class member, the call should be prefixed with <c>this.</c> rather than
    /// <c>base.</c>.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1100";
        internal const string Title = "Do not prefix calls with base unless local implementation exists";
        internal const string MessageFormat = "A call to a member from an inherited class begins with ‘base.’, and the local class does not contain an override or implementation of the member";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "A call to a member from an inherited class begins with 'base.', and the local class does not contain an override or implementation of the member.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1100.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeBaseExpression, SyntaxKind.BaseExpression);
        }

        private void AnalyzeBaseExpression(SyntaxNodeAnalysisContext context)
        {
            var baseExpressionSyntax = (BaseExpressionSyntax) context.Node;

            var classDeclarationSyntax = GetParentClass(baseExpressionSyntax);
            if (classDeclarationSyntax == null)
            {
                return;
            }

            var memberAccessExpression = baseExpressionSyntax.Parent as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null)
            {
                return;
            }

            if (!ContainsBaseMember(memberAccessExpression, classDeclarationSyntax))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, baseExpressionSyntax.GetLocation()));
            }
        }

        private static bool ContainsBaseMember(MemberAccessExpressionSyntax memberAccessExpression,
            ClassDeclarationSyntax classDeclarationSyntax)
        {
            var memberName = memberAccessExpression.Name.Identifier.Text;

            var containsBaseMember = classDeclarationSyntax.ChildNodes()
                .OfType<MemberDeclarationSyntax>()
                .SelectMany(m => m.ChildTokens().Where(c => c.CSharpKind() == SyntaxKind.IdentifierToken))
                .Any(i => i.Text == memberName);
            return containsBaseMember;
        }

        private ClassDeclarationSyntax GetParentClass(BaseExpressionSyntax baseExpressionSyntax)
        {
            SyntaxNode parent = baseExpressionSyntax;
            while (parent.CSharpKind() != SyntaxKind.CompilationUnit && parent != null)
            {
                if (parent.CSharpKind() == SyntaxKind.ClassDeclaration)
                {
                    return (ClassDeclarationSyntax) parent;
                   
                }

                parent = parent.Parent;
            }

            return null;
        }
    }
}

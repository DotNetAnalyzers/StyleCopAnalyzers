namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Adjacent C# elements are not separated by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when two adjacent element are not separated by a blank line. For
    /// example:</para>
    ///
    /// <code language="csharp">
    /// public void Method1()
    /// {
    /// }
    /// public bool Property
    /// {
    ///     get { return true; }
    /// }
    /// </code>
    ///
    /// <para>In the example above, the method and property are not separated by a blank line, so a violation of this
    /// rule would occur.</para>
    ///
    /// <code language="csharp">
    /// public event EventHandler SomeEvent
    /// {
    ///     add
    ///     {
    ///         // add event subscriber here
    ///     }
    ///     remove
    ///     {
    ///         // remove event subscriber here
    ///     }
    /// }
    /// </code>
    ///
    /// <para>In the example above, the add and remove of the event need to be separated by a blank line because the add
    /// is multi-line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1516ElementsMustBeSeparatedByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1516ElementsMustBeSeparatedByBlankLine"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1516";
        private const string Title = "Elements must be separated by blank line";
        private const string MessageFormat = "Elements must be separated by blank line";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "Adjacent C# elements are not separated by a blank line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1516.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(this.HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);

            context.RegisterSyntaxNodeAction(this.HandleCompilationUnit, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeAction(this.HandleNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);

            context.RegisterSyntaxNodeAction(this.HandlePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(this.HandlePropertyDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(this.HandlePropertyDeclaration, SyntaxKind.IndexerDeclaration);
        }

        private void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = context.Node as BasePropertyDeclarationSyntax;

            if (propertyDeclaration?.AccessorList?.Accessors != null)
            {
                var accessors = propertyDeclaration.AccessorList.Accessors;

                // We are not interested in properties with only one accessor
                if (accessors.Count == 2)
                {
                    if (accessors[0].Body != null && accessors[1].Body != null)
                    {
                        ReportIfThereIsNoBlankLine(context, accessors[0], accessors[1]);
                    }
                }
            }
        }

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = context.Node as TypeDeclarationSyntax;

            if (typeDeclaration != null)
            {
                var members = typeDeclaration.Members;

                this.HandleMemberList(context, members);
            }
        }

        private void HandleCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = context.Node as CompilationUnitSyntax;

            if (compilationUnit != null)
            {
                var members = compilationUnit.Members;

                this.HandleMemberList(context, members);

                if (members.Count > 0 && compilationUnit.Usings.Count > 0)
                {
                    ReportIfThereIsNoBlankLine(context, compilationUnit.Usings[compilationUnit.Usings.Count - 1], members[0]);
                }
                if (compilationUnit.Usings.Count > 0 && compilationUnit.Externs.Count > 0)
                {
                    ReportIfThereIsNoBlankLine(context, compilationUnit.Externs[compilationUnit.Externs.Count - 1], compilationUnit.Usings[0]);
                }
            }
        }

        private void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = context.Node as NamespaceDeclarationSyntax;

            if (namespaceDeclaration != null)
            {
                var members = namespaceDeclaration.Members;

                this.HandleMemberList(context, members);

                if (members.Count > 0 && namespaceDeclaration.Usings.Count > 0)
                {
                    ReportIfThereIsNoBlankLine(context, namespaceDeclaration.Usings[namespaceDeclaration.Usings.Count - 1], members[0]);
                }
                if (namespaceDeclaration.Usings.Count > 0 && namespaceDeclaration.Externs.Count > 0)
                {
                    ReportIfThereIsNoBlankLine(context, namespaceDeclaration.Externs[namespaceDeclaration.Externs.Count - 1], namespaceDeclaration.Usings[0]);
                }
            }
        }

        private void HandleMemberList(SyntaxNodeAnalysisContext context, SyntaxList<MemberDeclarationSyntax> members)
        {
            for (int i = 1; i < members.Count; i++)
            {
                if (!members[i - 1].ContainsDiagnostics && !members[i].ContainsDiagnostics)
                {
                    // Report if
                    // the previous declaration spans across multiple lines
                    // or the previous declaration is of different type
                    // or the current declaration has documentation
                    // or the current declaration is not a field declaration,
                    if (IsMultiline(members[i - 1])
                        || !members[i - 1].IsKind(members[i].Kind())
                        || !members[i].IsKind(SyntaxKind.FieldDeclaration))
                    {
                        ReportIfThereIsNoBlankLine(context, members[i - 1], members[i]);
                    }
                }
            }
        }

        private static bool IsMultiline(SyntaxNode node)
        {
            var lineSpan = node.GetLocation().GetLineSpan();

            return lineSpan.StartLinePosition.Line != lineSpan.EndLinePosition.Line;
        }

        private static void ReportIfThereIsNoBlankLine(SyntaxNodeAnalysisContext context, SyntaxNode firstNode, SyntaxNode secondNode)
        {
            if (XmlCommentHelper.HasDocumentation(secondNode))
            {
                // This should be reported by SA1514 instead.
                return;
            }

            var parent = firstNode.Parent;

            var allTrivia = parent.DescendantTrivia(TextSpan.FromBounds(firstNode.Span.End, secondNode.Span.Start),
                descendIntoTrivia: true,
                descendIntoChildren: n => true)
                .ToImmutableList();

            if (!HasEmptyLine(allTrivia))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, GetDiagnosticLocation(secondNode)));
            }
        }

        private static Location GetDiagnosticLocation(SyntaxNode node)
        {
            Location nodeLocation = node.GetLocation();

            if (node.HasLeadingTrivia)
            {
                return node.GetLeadingTrivia()[0].GetLocation();
            }
            var firstToken = node.ChildTokens().FirstOrDefault();
            if (firstToken != default(SyntaxToken))
            {
                return node.ChildTokens().First().GetLocation();
            }

            return Location.None;
        }

        private static bool HasEmptyLine(ImmutableList<SyntaxTrivia> allTrivia)
        {
            allTrivia = allTrivia.Where(x => !x.IsKind(SyntaxKind.WhitespaceTrivia)).ToImmutableList();
            for (int i = 1; i < allTrivia.Count; i++)
            {
                if (allTrivia[i].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    if (allTrivia[i - 1].IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        return true;
                    }
                }
                else
                {
                    // We can skip one trivia
                    i++;
                }
            }
            return false;
        }
    }
}

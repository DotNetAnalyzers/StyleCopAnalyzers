namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// A C# code file contains more than one unique class.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a C# file contains more than one class. To increase long-term
    /// maintainability of the code-base, each class should be placed in its own file, and file names should reflect the
    /// name of the class within the file.</para>
    ///
    /// <para>It is possible to place other supporting elements within the same file as the class, such as delegates,
    /// enums, etc., if they are related to the class.</para>
    ///
    /// <para>It is also possible to place multiple parts of the same partial class within the same file.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1402FileMayOnlyContainASingleClass : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1402";
        internal const string Title = "File may only contain a single class";
        internal const string MessageFormat = "File may only contain a single class";
        internal const string Category = "StyleCop.CSharp.MaintainabilityRules";
        internal const string Description = "A C# code file contains more than one unique class.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1402.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

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
            context.RegisterSyntaxTreeAction(HandleSyntaxTree);
        }

        private async void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var syntaxRoot = await context.Tree.GetRootAsync(context.CancellationToken);

            var descentNodes = syntaxRoot.DescendantNodes(descendIntoChildren: node => node != null && !node.IsKind(SyntaxKind.ClassDeclaration));

            string foundClassName = null;
            bool isPartialClass = false;

            foreach (var node in descentNodes)
            {
                if (node.IsKind(SyntaxKind.ClassDeclaration))
                {
                    ClassDeclarationSyntax classDeclaration = node as ClassDeclarationSyntax;
                    if (foundClassName != null)
                    {
                        if (isPartialClass && foundClassName == classDeclaration.Identifier.Text)
                        {
                            continue;
                        }
                        var location = GetClassLocation(node);
                        if (location != null)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                        }
                    }
                    else
                    {
                        foundClassName = classDeclaration.Identifier.Text;
                        isPartialClass = classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword);
                    }
                }
            }
        }

        private Location GetClassLocation(SyntaxNode node)
        {
            var classDeclaration = node as ClassDeclarationSyntax;
            return classDeclaration?.Identifier.GetLocation();
        }
    }
}

// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Generic;
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
        private const string Description = "Adjacent C# elements are not separated by a blank line.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1516.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterCompilationStartAction(HandleCompilationStart);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);

            context.RegisterSyntaxNodeActionHonorExclusions(HandleCompilationUnit, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);

            context.RegisterSyntaxNodeActionHonorExclusions(HandlePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandlePropertyDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandlePropertyDeclaration, SyntaxKind.IndexerDeclaration);
        }

        private static void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context)
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
                        // Don't report a diagnostic if all accessors are single line. Example:
                        //// public string Foo
                        //// {
                        ////    get { return "bar"; }
                        ////    set { }
                        //// }
                        if (IsMultiline(accessors[0]) || IsMultiline(accessors[1]))
                        {
                            ReportIfThereIsNoBlankLine(context, accessors[0], accessors[1]);
                        }
                    }
                }
            }
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = context.Node as TypeDeclarationSyntax;

            if (typeDeclaration != null)
            {
                var members = typeDeclaration.Members;

                HandleMemberList(context, members);
            }
        }

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = context.Node as CompilationUnitSyntax;

            if (compilationUnit != null)
            {
                var members = compilationUnit.Members;

                HandleMemberList(context, members);

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

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = context.Node as NamespaceDeclarationSyntax;

            if (namespaceDeclaration != null)
            {
                var members = namespaceDeclaration.Members;

                HandleMemberList(context, members);

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

        private static void HandleMemberList(SyntaxNodeAnalysisContext context, SyntaxList<MemberDeclarationSyntax> members)
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
            var lineSpan = node.GetLineSpan();

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

            var allTrivia = parent.DescendantTrivia(
                TextSpan.FromBounds(firstNode.Span.End, secondNode.Span.Start),
                descendIntoTrivia: true);

            if (!HasEmptyLine(allTrivia))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, GetDiagnosticLocation(secondNode)));
            }
        }

        private static Location GetDiagnosticLocation(SyntaxNode node)
        {
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

        private static bool HasEmptyLine(IEnumerable<SyntaxTrivia> allTrivia)
        {
            allTrivia = allTrivia.Where(x => !x.IsKind(SyntaxKind.WhitespaceTrivia));

            SyntaxTrivia previousTrivia = default(SyntaxTrivia);

            foreach (var trivia in allTrivia)
            {
                if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    if (previousTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        return true;
                    }
                }

                previousTrivia = trivia;
            }

            return false;
        }
    }
}

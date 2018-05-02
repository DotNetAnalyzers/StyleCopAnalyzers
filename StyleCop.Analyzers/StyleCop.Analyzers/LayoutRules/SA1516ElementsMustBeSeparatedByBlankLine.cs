// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Settings.ObjectModel;

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
    internal class SA1516ElementsMustBeSeparatedByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1516ElementsMustBeSeparatedByBlankLine"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1516";

        /// <summary>
        /// Property key to signal the code fix that blank lines should be removed.
        /// </summary>
        internal const string CodeFixActionKey = "CodeFixAction";
        internal const string RemoveBlankLinesValue = "RemoveBlankLines";
        internal const string InsertBlankLineValue = "InsertBlankLine";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(LayoutResources.SA1516Title), LayoutResources.ResourceManager, typeof(LayoutResources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(LayoutResources.SA1516MessageFormat), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(LayoutResources.SA1516Description), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString MessageFormatRequire = new LocalizableResourceString(nameof(LayoutResources.SA1516MessageFormatRequire), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString DescriptionRequire = new LocalizableResourceString(nameof(LayoutResources.SA1516DescriptionRequire), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString MessageFormatOmit = new LocalizableResourceString(nameof(LayoutResources.SA1516MessageFormatOmit), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString DescriptionOmit = new LocalizableResourceString(nameof(LayoutResources.SA1516DescriptionOmit), LayoutResources.ResourceManager, typeof(LayoutResources));

        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1516.md";

        private static readonly Action<SyntaxNodeAnalysisContext> TypeDeclarationAction = HandleTypeDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> NamespaceDeclarationAction = HandleNamespaceDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> BasePropertyDeclarationAction = HandleBasePropertyDeclaration;

        private static readonly ImmutableDictionary<string, string> DiagnosticProperties = ImmutableDictionary<string, string>.Empty.Add(CodeFixActionKey, InsertBlankLineValue);
        private static readonly ImmutableDictionary<string, string> DiagnosticPropertiesRequire = ImmutableDictionary<string, string>.Empty.Add(CodeFixActionKey, InsertBlankLineValue);
        private static readonly ImmutableDictionary<string, string> DiagnosticPropertiesOmit = ImmutableDictionary<string, string>.Empty.Add(CodeFixActionKey, RemoveBlankLinesValue);

        /// <summary>
        /// Gets the default descriptor for diagnostics generated by this analyzer.
        /// </summary>
        /// <value>The default <see cref="DiagnosticDescriptor"/>.</value>
        public static DiagnosticDescriptor Descriptor { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <summary>
        /// Gets the descriptor for missing blank lines between using directives diagnostics generated by this analyzer.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for missing blank lines between using directives.</value>
        public static DiagnosticDescriptor DescriptorRequire { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormatRequire, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, DescriptionRequire, HelpLink);

        /// <summary>
        /// Gets the descriptor for superfluous blank lines between using directives diagnostics generated by this analyzer.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for superfluous blank lines between using directives.</value>
        public static DiagnosticDescriptor DescriptorOmit { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormatOmit, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, DescriptionOmit, HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(TypeDeclarationAction, SyntaxKinds.TypeDeclaration);
            context.RegisterSyntaxNodeAction(CompilationUnitAction, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeAction(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(BasePropertyDeclarationAction, SyntaxKinds.BasePropertyDeclaration);
        }

        private static void HandleBasePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (BasePropertyDeclarationSyntax)context.Node;

            if (propertyDeclaration.AccessorList?.Accessors != null)
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
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            var members = typeDeclaration.Members;

            HandleMemberList(context, members);
        }

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            var usings = compilationUnit.Usings;
            var members = compilationUnit.Members;

            HandleUsings(context, usings, settings);
            HandleMemberList(context, members);

            if (members.Count > 0 && compilationUnit.Usings.Count > 0)
            {
                ReportIfThereIsNoBlankLine(context, usings[usings.Count - 1], members[0]);
            }

            if (compilationUnit.Usings.Count > 0 && compilationUnit.Externs.Count > 0)
            {
                ReportIfThereIsNoBlankLine(context, compilationUnit.Externs[compilationUnit.Externs.Count - 1], compilationUnit.Usings[0]);
            }
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            var usings = namespaceDeclaration.Usings;
            var members = namespaceDeclaration.Members;

            HandleUsings(context, usings, settings);
            HandleMemberList(context, members);

            if (members.Count > 0 && namespaceDeclaration.Usings.Count > 0)
            {
                ReportIfThereIsNoBlankLine(context, usings[usings.Count - 1], members[0]);
            }

            if (namespaceDeclaration.Usings.Count > 0 && namespaceDeclaration.Externs.Count > 0)
            {
                ReportIfThereIsNoBlankLine(context, namespaceDeclaration.Externs[namespaceDeclaration.Externs.Count - 1], namespaceDeclaration.Usings[0]);
            }
        }

        private static void HandleUsings(SyntaxNodeAnalysisContext context, SyntaxList<UsingDirectiveSyntax> usings, StyleCopSettings settings)
        {
            if (usings.Count < 2)
            {
                return;
            }

            var blankLinesBetweenUsingGroups = settings.OrderingRules.BlankLinesBetweenUsingGroups;

            var previousGroupType = usings[0].GetUsingGroupType(settings);
            var previousLineSpan = usings[0].GetLineSpan();

            for (var i = 1; i < usings.Count; i++)
            {
                var currentGroupType = usings[i].GetUsingGroupType(settings);
                var currentLineSpan = usings[i].GetLineSpan();

                var partOfSameGroup = previousGroupType == currentGroupType;
                var lineDistance = currentLineSpan.StartLinePosition.Line - previousLineSpan.EndLinePosition.Line;

                previousGroupType = currentGroupType;
                previousLineSpan = currentLineSpan;

                if (partOfSameGroup)
                {
                    // if the using statements are part of the same group, there is no need to check.
                    continue;
                }

                if (blankLinesBetweenUsingGroups == OptionSetting.Require)
                {
                    if (lineDistance > 1)
                    {
                        var separatingTrivia = TriviaHelper.MergeTriviaLists(usings[i - 1].GetTrailingTrivia(), usings[i].GetLeadingTrivia());
                        if (separatingTrivia.ContainsBlankLines(false))
                        {
                            continue;
                        }
                    }

                    context.ReportDiagnostic(Diagnostic.Create(DescriptorRequire, usings[i].UsingKeyword.GetLocation(), DiagnosticPropertiesRequire));
                }
                else if (blankLinesBetweenUsingGroups == OptionSetting.Omit)
                {
                    if (lineDistance < 2)
                    {
                        // no point in checking the trivia if the using statements are not separated.
                        continue;
                    }

                    var separatingTrivia = TriviaHelper.MergeTriviaLists(usings[i - 1].GetTrailingTrivia(), usings[i].GetLeadingTrivia());
                    if (!separatingTrivia.ContainsBlankLines(false))
                    {
                        continue;
                    }

                    context.ReportDiagnostic(Diagnostic.Create(DescriptorOmit, usings[i].UsingKeyword.GetLocation(), DiagnosticPropertiesOmit));
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
                    // the current declaration is not a field declaration
                    // or the previous declaration is of different type
                    // or the previous declaration spans across multiple lines
                    //
                    // Note that the order of checking is important, as the call to IsMultiLine requires a FieldDeclarationSyntax,
                    // something that can only be guaranteed if the first two checks fail.
                    if (!members[i].IsKind(SyntaxKind.FieldDeclaration)
                        || !members[i - 1].IsKind(members[i].Kind())
                        || IsMultiline((FieldDeclarationSyntax)members[i - 1]))
                    {
                        ReportIfThereIsNoBlankLine(context, members[i - 1], members[i]);
                    }
                }
            }
        }

        private static bool IsMultiline(FieldDeclarationSyntax fieldDeclaration)
        {
            var lineSpan = fieldDeclaration.GetLineSpan();
            var attributeLists = fieldDeclaration.AttributeLists;

            int startLine;

            // Exclude attributes when determining if a field declaration spans multiple lines
            if (attributeLists.Count > 0)
            {
                var lastAttributeSpan = fieldDeclaration.SyntaxTree.GetLineSpan(attributeLists.Last().FullSpan);
                startLine = lastAttributeSpan.EndLinePosition.Line;
            }
            else
            {
                startLine = lineSpan.StartLinePosition.Line;
            }

            return startLine != lineSpan.EndLinePosition.Line;
        }

        private static bool IsMultiline(AccessorDeclarationSyntax accessorDeclaration)
        {
            var lineSpan = accessorDeclaration.GetLineSpan();
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
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, GetDiagnosticLocation(secondNode), DiagnosticProperties));
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

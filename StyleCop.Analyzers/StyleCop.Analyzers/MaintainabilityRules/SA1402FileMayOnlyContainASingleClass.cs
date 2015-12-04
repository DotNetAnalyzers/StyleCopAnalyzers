﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Settings.ObjectModel;
    using StyleCop.Analyzers.Helpers;

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
    internal class SA1402FileMayOnlyContainASingleClass : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1402FileMayOnlyContainASingleClass"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1402";
        private const string Title = "File may only contain a single class";
        private const string MessageFormat = "File may only contain a single class";
        private const string Description = "A C# code file contains more than one unique class.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1402.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxTreeAnalysisContext, StyleCopSettings> SyntaxTreeAction = HandleSyntaxTree;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(SyntaxTreeAction);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context, StyleCopSettings settings)
        {
            var syntaxRoot = context.Tree.GetRoot(context.CancellationToken);

            var descentNodes = syntaxRoot.DescendantNodes(descendIntoChildren: node => node != null && !node.IsKind(SyntaxKind.ClassDeclaration));
            var classNodes = from descentNode in descentNodes
                                where descentNode.IsKind(SyntaxKind.ClassDeclaration)
                                select descentNode as ClassDeclarationSyntax;

            string suffix;
            var fileName = FileNameHelpers.GetFileNameAndSuffix(context.Tree.FilePath, out suffix);
            var preferredClassNode = classNodes.FirstOrDefault(n => FileNameHelpers.GetConventionalFileName(n, settings.DocumentationRules.FileNamingConvention) == fileName) ?? classNodes.FirstOrDefault();

            if (preferredClassNode == null)
            {
                return;
            }

            string foundClassName = null;
            bool isPartialClass = false;

            foundClassName = preferredClassNode.Identifier.Text;
            isPartialClass = preferredClassNode.Modifiers.Any(SyntaxKind.PartialKeyword);

            foreach (var classNode in classNodes)
            {
                if (classNode == preferredClassNode || (isPartialClass && foundClassName == classNode.Identifier.Text))
                {
                    continue;
                }

                var location = NamedTypeHelpers.GetNameOrIdentifierLocation(classNode);
                if (location != null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                }
            }
        }
    }
}

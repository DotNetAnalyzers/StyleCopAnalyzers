// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// The name of a field or variable in C# uses Hungarian notation.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when Hungarian notation is used in the naming of fields and variables. The
    /// use of Hungarian notation has become widespread in C++ code, but the trend in C# is to use longer, more
    /// descriptive names for variables, which are not based on the type of the variable but which instead describe what
    /// the variable is used for.</para>
    ///
    /// <para>In addition, modern code editors such as Visual Studio make it easy to identify type information for a
    /// variable or field, typically by hovering the mouse cursor over the variable name. This reduces the need for
    /// Hungarian notation.</para>
    ///
    /// <para>StyleCop assumes that any variable name that begins with one or two lower-case letters followed by an
    /// upper-case letter is making use of Hungarian notation, and will flag a violation of this rule in each case. It
    /// is possible to declare certain prefixes as legal, in which case they will be ignored. For example, a variable
    /// named <c>onExecute</c> will appear to StyleCop to be using Hungarian notation, when in reality it is not. Thus,
    /// the <c>on</c> prefix should be flagged as an allowed prefix.</para>
    ///
    /// <para>To configure the list of allowed prefixes, bring up the StyleCop settings for a project, and navigate to
    /// the Hungarian tab, as shown below:</para>
    ///
    /// <para><img alt="" src="Images/HungarianSettings.JPG" border="0"/></para>
    ///
    /// <para>Adding a one or two letter prefix to this list will cause StyleCop to ignore variables or fields which
    /// begin with this prefix.</para>
    ///
    /// <para>If the field or variable name is intended to match the name of an item associated with Win32 or COM, and
    /// thus needs to use Hungarian notation, place the field or variable within a special <c>NativeMethods</c> class. A
    /// <c>NativeMethods</c> class is any class which contains a name ending in <c>NativeMethods</c>, and is intended as
    /// a placeholder for Win32 or COM wrappers. StyleCop will ignore this violation if the item is placed within a
    /// <c>NativeMethods</c> class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1305FieldNamesMustNotUseHungarianNotation : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1305FieldNamesMustNotUseHungarianNotation"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1305";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1305.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(NamingResources.SA1305Title), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(NamingResources.SA1305MessageFormat), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(NamingResources.SA1305Description), NamingResources.ResourceManager, typeof(NamingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<string> CommonPrefixes =
            ImmutableArray.Create("as", "at", "by", "do", "go", "if", "in", "is", "it", "no", "of", "on", "or", "to");

        private static readonly Regex HungarianRegex = new Regex(@"^(?<notation>[a-z]{1,2})[A-Z]");

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> VariableDeclarationAction = Analyzer.HandleVariableDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> ParameterDeclarationAction = Analyzer.HandleParameterDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> CatchDeclarationAction = Analyzer.HandleCatchDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> QueryContinuationAction = Analyzer.HandleQueryContinuation;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> FromClauseAction = Analyzer.HandleFromClause;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> LetClauseAction = Analyzer.HandleLetClause;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> JoinClauseAction = Analyzer.HandleJoinClause;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> JoinIntoClauseAction = Analyzer.HandleJoinIntoClause;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> ForEachStatementAction = Analyzer.HandleForEachStatement;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> SingleVariableDesignationAction = Analyzer.HandleSingleVariableDesignation;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(context =>
            {
                context.RegisterSyntaxNodeAction(VariableDeclarationAction, SyntaxKind.VariableDeclaration);
                context.RegisterSyntaxNodeAction(ParameterDeclarationAction, SyntaxKind.Parameter);
                context.RegisterSyntaxNodeAction(CatchDeclarationAction, SyntaxKind.CatchDeclaration);
                context.RegisterSyntaxNodeAction(QueryContinuationAction, SyntaxKind.QueryContinuation);
                context.RegisterSyntaxNodeAction(FromClauseAction, SyntaxKind.FromClause);
                context.RegisterSyntaxNodeAction(LetClauseAction, SyntaxKind.LetClause);
                context.RegisterSyntaxNodeAction(JoinClauseAction, SyntaxKind.JoinClause);
                context.RegisterSyntaxNodeAction(JoinIntoClauseAction, SyntaxKind.JoinIntoClause);
                context.RegisterSyntaxNodeAction(ForEachStatementAction, SyntaxKind.ForEachStatement);
                context.RegisterSyntaxNodeAction(SingleVariableDesignationAction, SyntaxKindEx.SingleVariableDesignation);
            });
        }

        private static class Analyzer
        {
            public static void HandleVariableDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                var syntax = (VariableDeclarationSyntax)context.Node;

                if (syntax.Parent.IsKind(SyntaxKind.EventFieldDeclaration))
                {
                    return;
                }

                if (NamedTypeHelpers.IsContainedInNativeMethodsClass(syntax))
                {
                    return;
                }

                var declarationType = syntax.Parent.IsKind(SyntaxKind.FieldDeclaration) ? "field" : "variable";
                foreach (var variableDeclarator in syntax.Variables)
                {
                    if (variableDeclarator == null)
                    {
                        continue;
                    }

                    var identifier = variableDeclarator.Identifier;
                    CheckIdentifier(context, identifier, settings, declarationType);
                }
            }

            public static void HandleParameterDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                var parameter = (ParameterSyntax)context.Node;

                if (NamedTypeHelpers.IsContainedInNativeMethodsClass(parameter))
                {
                    return;
                }

                // Only parameters from method declarations can be exempt from this rule
                if (parameter?.Parent?.Parent is MemberDeclarationSyntax memberDeclaration)
                {
                    var semanticModel = context.SemanticModel;
                    var symbol = semanticModel.GetDeclaredSymbol(memberDeclaration);
                    if (symbol != null)
                    {
                        if (symbol.IsOverride || NamedTypeHelpers.IsImplementingAnInterfaceMember(symbol))
                        {
                            return;
                        }
                    }
                }

                CheckIdentifier(context, parameter.Identifier, settings, "parameter");
            }

            public static void HandleCatchDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                CheckIdentifier(context, ((CatchDeclarationSyntax)context.Node).Identifier, settings);
            }

            public static void HandleQueryContinuation(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                CheckIdentifier(context, ((QueryContinuationSyntax)context.Node).Identifier, settings);
            }

            public static void HandleFromClause(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                CheckIdentifier(context, ((FromClauseSyntax)context.Node).Identifier, settings);
            }

            public static void HandleLetClause(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                CheckIdentifier(context, ((LetClauseSyntax)context.Node).Identifier, settings);
            }

            public static void HandleJoinClause(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                CheckIdentifier(context, ((JoinClauseSyntax)context.Node).Identifier, settings);
            }

            public static void HandleJoinIntoClause(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                CheckIdentifier(context, ((JoinIntoClauseSyntax)context.Node).Identifier, settings);
            }

            public static void HandleForEachStatement(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                CheckIdentifier(context, ((ForEachStatementSyntax)context.Node).Identifier, settings);
            }

            public static void HandleSingleVariableDesignation(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                CheckIdentifier(context, ((SingleVariableDesignationSyntaxWrapper)context.Node).Identifier, settings);
            }

            private static void CheckIdentifier(SyntaxNodeAnalysisContext context, SyntaxToken identifier, StyleCopSettings settings, string declarationType = "variable")
            {
                if (identifier.IsMissing)
                {
                    return;
                }

                string name = identifier.ValueText;
                if (string.IsNullOrEmpty(name))
                {
                    return;
                }

                var match = HungarianRegex.Match(name);
                if (!match.Success)
                {
                    return;
                }

                var notationValue = match.Groups["notation"].Value;
                if (settings.NamingRules.AllowCommonHungarianPrefixes && CommonPrefixes.Contains(notationValue))
                {
                    return;
                }

                if (settings.NamingRules.AllowedHungarianPrefixes.Contains(notationValue))
                {
                    return;
                }

                // Variable names should begin with lower-case letter
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), declarationType, name));
            }
        }
    }
}

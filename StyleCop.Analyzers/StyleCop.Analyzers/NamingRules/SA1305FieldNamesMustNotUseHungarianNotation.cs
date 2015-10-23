// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Text.RegularExpressions;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Settings.ObjectModel;

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
        private const string Title = "Field names must not use Hungarian notation";
        private const string MessageFormat = "{0} '{1}' must not use Hungarian notation";
        private const string Description = "The name of a field or variable in C# uses Hungarian notation.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1305.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<string> CommonPrefixes =
            ImmutableArray.Create("as", "at", "by", "do", "go", "if", "in", "is", "it", "no", "of", "on", "or", "to");

        private static readonly Regex HungarianRegex = new Regex(@"^(?<notation>[a-z]{1,2})[A-Z]");

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;

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
            Analyzer analyzer = new Analyzer(context.Options);
            context.RegisterSyntaxNodeActionHonorExclusions(analyzer.HandleVariableDeclarationSyntax, SyntaxKind.VariableDeclaration);
        }

        private sealed class Analyzer
        {
            private readonly NamingSettings namingSettings;

            public Analyzer(AnalyzerOptions options)
            {
                StyleCopSettings settings = options.GetStyleCopSettings();
                this.namingSettings = settings.NamingRules;
            }

            public void HandleVariableDeclarationSyntax(SyntaxNodeAnalysisContext context)
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

                var fieldDeclaration = syntax.Parent.IsKind(SyntaxKind.FieldDeclaration);
                foreach (var variableDeclarator in syntax.Variables)
                {
                    if (variableDeclarator == null)
                    {
                        continue;
                    }

                    var identifier = variableDeclarator.Identifier;
                    if (identifier.IsMissing)
                    {
                        continue;
                    }

                    string name = identifier.ValueText;
                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    var match = HungarianRegex.Match(name);
                    if (!match.Success)
                    {
                        continue;
                    }

                    var notationValue = match.Groups["notation"].Value;
                    if (this.namingSettings.AllowCommonHungarianPrefixes && CommonPrefixes.Contains(notationValue))
                    {
                        continue;
                    }

                    if (this.namingSettings.AllowedHungarianPrefixes.Contains(notationValue))
                    {
                        continue;
                    }

                    // Variable names must begin with lower-case letter
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), fieldDeclaration ? "field" : "variable", name));
                }
            }
        }
    }
}

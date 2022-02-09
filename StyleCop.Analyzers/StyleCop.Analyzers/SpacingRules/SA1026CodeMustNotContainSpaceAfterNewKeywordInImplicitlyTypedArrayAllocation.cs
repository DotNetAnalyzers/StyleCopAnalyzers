// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// An implicitly typed new array allocation within a C# code file is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains an implicitly typed new array allocation which
    /// is not spaced correctly. Within an implicitly typed new array allocation, there should not be any space between
    /// the new keyword and the opening array bracket. For example:</para>
    ///
    /// <code language="cs">
    /// var a = new[] { 1, 10, 100, 1000 };
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1026";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1026.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1026Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1026MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1026Description), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> ImplicitArrayCreationExpressionAction = HandleImplicitArrayCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ImplicitStackAllocArrayCreationExpressionAction = HandleImplicitStackAllocArrayCreationExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(ImplicitArrayCreationExpressionAction, SyntaxKind.ImplicitArrayCreationExpression);
            context.RegisterSyntaxNodeAction(ImplicitStackAllocArrayCreationExpressionAction, SyntaxKindEx.ImplicitStackAllocArrayCreationExpression);
        }

        private static void HandleImplicitArrayCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var arrayCreation = (ImplicitArrayCreationExpressionSyntax)context.Node;
            var newKeywordToken = arrayCreation.NewKeyword;

            if (newKeywordToken.IsFollowedByWhitespace() || newKeywordToken.IsLastInLine())
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, newKeywordToken.GetLocation(), TokenSpacingProperties.RemoveFollowing, "new"));
            }
        }

        private static void HandleImplicitStackAllocArrayCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var arrayCreation = (ImplicitStackAllocArrayCreationExpressionSyntaxWrapper)context.Node;
            var stackAllocKeywordToken = arrayCreation.StackAllocKeyword;

            if (stackAllocKeywordToken.IsFollowedByWhitespace() || stackAllocKeywordToken.IsLastInLine())
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, stackAllocKeywordToken.GetLocation(), TokenSpacingProperties.RemoveFollowing, "stackalloc"));
            }
        }
    }
}

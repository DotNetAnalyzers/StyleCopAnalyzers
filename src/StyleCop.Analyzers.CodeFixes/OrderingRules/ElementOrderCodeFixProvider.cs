// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Implements code fixes for element ordering rules.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ElementOrderCodeFixProvider))]
    [Shared]
    internal class ElementOrderCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1201ElementsMustAppearInTheCorrectOrder.DiagnosticId,
                SA1202ElementsMustBeOrderedByAccess.DiagnosticId,
                SA1203ConstantsMustAppearBeforeFields.DiagnosticId,
                SA1204StaticElementsMustAppearBeforeInstanceElements.DiagnosticId,
                SA1214ReadonlyElementsMustAppearBeforeNonReadonlyElements.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                var transformedDocument = await GetTransformedDocumentAsync(context.Document, diagnostic, context.CancellationToken).ConfigureAwait(false);
                if (transformedDocument == context.Document)
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        OrderingResources.ElementOrderCodeFix,
                        cancellationToken => Task.FromResult(transformedDocument),
                        nameof(ElementOrderCodeFixProvider)),
                    diagnostic);
            }
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, syntaxRoot.SyntaxTree, cancellationToken);
            var elementOrder = settings.OrderingRules.ElementOrder;

            var location = diagnostic.Id == SA1201ElementsMustAppearInTheCorrectOrder.DiagnosticId && diagnostic.AdditionalLocations.Count > 0
                ? diagnostic.AdditionalLocations[0]
                : diagnostic.Location;
            var memberDeclaration = syntaxRoot.FindNode(location.SourceSpan).FirstAncestorOrSelf<MemberDeclarationSyntax>();
            if (memberDeclaration == null)
            {
                return document;
            }

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var updatedRoot = UpdateSyntaxRoot(memberDeclaration, elementOrder, syntaxRoot, settings.Indentation, settings.OrderingRules.MemberKindOrder, semanticModel, cancellationToken);
            if (updatedRoot == syntaxRoot)
            {
                return document;
            }

            return document.WithSyntaxRoot(updatedRoot);
        }

        private static SyntaxNode UpdateSyntaxRoot(
            MemberDeclarationSyntax memberDeclaration,
            ImmutableArray<OrderingTrait> elementOrder,
            SyntaxNode syntaxRoot,
            IndentationSettings indentationSettings,
            MemberKindOrder? configuredOrder,
            SemanticModel? semanticModel = null,
            CancellationToken cancellationToken = default,
            Dictionary<string, Dictionary<string, ImmutableHashSet<string>>>? fieldDependenciesByType = null)
        {
            var parentDeclaration = memberDeclaration.Parent;
            var memberToMove = new MemberOrderHelper(memberDeclaration, elementOrder);

            if (parentDeclaration is TypeDeclarationSyntax syntax)
            {
                return HandleTypeDeclaration(memberToMove, syntax, elementOrder, syntaxRoot, indentationSettings, configuredOrder, semanticModel, cancellationToken, fieldDependenciesByType);
            }

            if (BaseNamespaceDeclarationSyntaxWrapper.IsInstance(parentDeclaration))
            {
                return HandleBaseNamespaceDeclaration(memberToMove, (BaseNamespaceDeclarationSyntaxWrapper)parentDeclaration, elementOrder, syntaxRoot, indentationSettings);
            }

            if (parentDeclaration is CompilationUnitSyntax syntax1)
            {
                return HandleCompilationUnitDeclaration(memberToMove, syntax1, elementOrder, syntaxRoot, indentationSettings);
            }

            return syntaxRoot;
        }

        private static SyntaxNode HandleTypeDeclaration(
            MemberOrderHelper memberOrder,
            TypeDeclarationSyntax typeDeclarationNode,
            ImmutableArray<OrderingTrait> elementOrder,
            SyntaxNode syntaxRoot,
            IndentationSettings indentationSettings,
            MemberKindOrder? configuredOrder,
            SemanticModel? semanticModel,
            CancellationToken cancellationToken,
            Dictionary<string, Dictionary<string, ImmutableHashSet<string>>>? fieldDependenciesByType)
        {
            Dictionary<string, ImmutableHashSet<string>>? fieldDependencies = null;
            if (fieldDependenciesByType != null)
            {
                fieldDependenciesByType.TryGetValue(GetTypeKey(typeDeclarationNode), out fieldDependencies);
            }
            else if (semanticModel != null)
            {
                fieldDependencies = FieldDependencyHelper.ComputeFieldDependencies(semanticModel, typeDeclarationNode.Members, cancellationToken);
            }

            if (configuredOrder != null && elementOrder.Contains(OrderingTrait.Kind))
            {
                return OrderConfiguredMember(memberOrder.Member, typeDeclarationNode.Members, configuredOrder, syntaxRoot, indentationSettings, fieldDependencies, cancellationToken);
            }

            return OrderMember(memberOrder, typeDeclarationNode.Members, elementOrder, syntaxRoot, indentationSettings, fieldDependencies);
        }

        private static SyntaxNode OrderConfiguredMember(
            MemberDeclarationSyntax member,
            SyntaxList<MemberDeclarationSyntax> members,
            MemberKindOrder order,
            SyntaxNode syntaxRoot,
            IndentationSettings indentationSettings,
            Dictionary<string, ImmutableHashSet<string>>? fieldDependencies,
            CancellationToken cancellationToken)
        {
            if (member.ContainsDirectives || member.IsKind(SyntaxKind.IncompleteMember))
            {
                return syntaxRoot;
            }

            var memberIndex = members.IndexOf(member);
            var rank = order.GetRank(member);
            var priority = order.GetPrecedingPriority(member);
            var followingPriority = order.GetFollowingPriority(member);
            var hasInitializer = HasRuntimeInitializer(member);
            var targetIndex = -1;
            var gapIndex = -1;
            for (var i = memberIndex - 1; i >= 0; i--)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var previous = members[i];
                if (previous.IsKind(SyntaxKind.IncompleteMember)
                    || (hasInitializer
                        && HasRuntimeInitializer(previous)
                        && member.GetModifiers().Any(SyntaxKind.StaticKeyword) == previous.GetModifiers().Any(SyntaxKind.StaticKeyword))
                    || FieldDependencyHelper.DependsOn(member, previous, fieldDependencies)
                    || FieldDependencyHelper.DependsOn(previous, member, fieldDependencies))
                {
                    if (!previous.IsKind(SyntaxKind.IncompleteMember)
                        && gapIndex >= 0
                        && rank >= 0
                        && order.GetRank(previous) == rank
                        && order.GetPrecedingPriority(previous) == priority)
                    {
                        targetIndex = gapIndex;
                    }

                    break;
                }

                var previousPriority = order.GetPrecedingPriority(previous);
                if (previousPriority != priority)
                {
                    if (previousPriority > priority)
                    {
                        break;
                    }

                    targetIndex = i;
                    gapIndex = -1;
                    continue;
                }

                var previousRank = order.GetRank(previous);
                if (rank >= 0)
                {
                    if (previousRank < 0)
                    {
                        gapIndex = i;
                        continue;
                    }

                    if (previousRank > rank)
                    {
                        targetIndex = i;
                        gapIndex = -1;
                        continue;
                    }

                    if (previousRank == rank)
                    {
                        if (gapIndex >= 0)
                        {
                            targetIndex = gapIndex;
                        }

                        if (order.GetFollowingPriority(previous) < followingPriority)
                        {
                            targetIndex = i;
                            gapIndex = -1;
                            continue;
                        }
                    }

                    break;
                }

                // Unspecified kinds have no relative kind order, but the remaining traits
                // still apply between declarations of the same kind.
                if (MemberKindOrder.Normalize(previous.Kind()) == MemberKindOrder.Normalize(member.Kind()))
                {
                    if (order.GetFollowingPriority(previous) >= followingPriority)
                    {
                        break;
                    }

                    targetIndex = i;
                }
            }

            if (targetIndex < 0)
            {
                return syntaxRoot;
            }

            for (var i = targetIndex; i < memberIndex; i++)
            {
                if (members[i].ContainsDirectives)
                {
                    return syntaxRoot;
                }
            }

            var span = TextSpan.FromBounds(members[targetIndex].FullSpan.Start, member.FullSpan.End);
            if (syntaxRoot.DescendantTrivia(span).Any(trivia => trivia.IsKind(SyntaxKind.RegionDirectiveTrivia) || trivia.IsKind(SyntaxKind.EndRegionDirectiveTrivia)))
            {
                return syntaxRoot;
            }

            return MoveMember(syntaxRoot, member, members[targetIndex], indentationSettings);
        }

        private static bool HasRuntimeInitializer(MemberDeclarationSyntax member)
        {
            if (member is PropertyDeclarationSyntax property)
            {
                return property.Initializer != null;
            }

            if (member is BaseFieldDeclarationSyntax field && !field.Modifiers.Any(SyntaxKind.ConstKeyword))
            {
                foreach (var variable in field.Declaration.Variables)
                {
                    if (variable.Initializer != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static SyntaxNode HandleCompilationUnitDeclaration(MemberOrderHelper memberOrder, CompilationUnitSyntax compilationUnitDeclaration, ImmutableArray<OrderingTrait> elementOrder, SyntaxNode syntaxRoot, IndentationSettings indentationSettings)
        {
            return OrderMember(memberOrder, compilationUnitDeclaration.Members, elementOrder, syntaxRoot, indentationSettings, fieldDependencies: null);
        }

        private static SyntaxNode HandleBaseNamespaceDeclaration(MemberOrderHelper memberOrder, BaseNamespaceDeclarationSyntaxWrapper namespaceDeclaration, ImmutableArray<OrderingTrait> elementOrder, SyntaxNode syntaxRoot, IndentationSettings indentationSettings)
        {
            return OrderMember(memberOrder, namespaceDeclaration.Members, elementOrder, syntaxRoot, indentationSettings, fieldDependencies: null);
        }

        private static SyntaxNode OrderMember(
            MemberOrderHelper memberOrder,
            SyntaxList<MemberDeclarationSyntax> members,
            ImmutableArray<OrderingTrait> elementOrder,
            SyntaxNode syntaxRoot,
            IndentationSettings indentationSettings,
            Dictionary<string, ImmutableHashSet<string>>? fieldDependencies)
        {
            if (memberOrder.Member.ContainsDirectives)
            {
                return syntaxRoot;
            }

            var memberIndex = members.IndexOf(memberOrder.Member);
            MemberOrderHelper target = default;

            for (var i = memberIndex - 1; i >= 0; --i)
            {
                if (FieldDependencyHelper.DependsOn(memberOrder.Member, members[i], fieldDependencies)
                    || FieldDependencyHelper.DependsOn(members[i], memberOrder.Member, fieldDependencies))
                {
                    break;
                }

                var orderHelper = new MemberOrderHelper(members[i], elementOrder);
                if (orderHelper.Priority < memberOrder.Priority)
                {
                    if (members[i].ContainsDirectives)
                    {
                        return syntaxRoot;
                    }

                    target = orderHelper;
                }
                else
                {
                    break;
                }
            }

            if (target.Member == null)
            {
                return syntaxRoot;
            }

            // Region directives cannot safely move with individual members across a region boundary.
            var span = TextSpan.FromBounds(target.Member.FullSpan.Start, memberOrder.Member.FullSpan.End);
            if (syntaxRoot.DescendantTrivia(span).Any(trivia => trivia.IsKind(SyntaxKind.RegionDirectiveTrivia) || trivia.IsKind(SyntaxKind.EndRegionDirectiveTrivia)))
            {
                return syntaxRoot;
            }

            return MoveMember(syntaxRoot, memberOrder.Member, target.Member, indentationSettings);
        }

        private static string GetTypeKey(TypeDeclarationSyntax typeDeclaration)
        {
            var sb = new StringBuilder(typeDeclaration.Identifier.ValueText);
            for (var parent = typeDeclaration.Parent; parent != null; parent = parent.Parent)
            {
                if (parent is TypeDeclarationSyntax parentType)
                {
                    sb.Insert(0, parentType.Identifier.ValueText + ".");
                }
                else if (BaseNamespaceDeclarationSyntaxWrapper.IsInstance(parent))
                {
                    sb.Insert(0, ((BaseNamespaceDeclarationSyntaxWrapper)parent).Name + ".");
                }
            }

            return sb.ToString();
        }

        private static SyntaxNode MoveMember(SyntaxNode syntaxRoot, MemberDeclarationSyntax member, MemberDeclarationSyntax targetMember, IndentationSettings indentationSettings)
        {
            var firstToken = syntaxRoot.GetFirstToken();
            var fileHeader = GetFileHeader(firstToken.LeadingTrivia);
            syntaxRoot = syntaxRoot.TrackNodes(member, targetMember, firstToken.Parent);
            var memberToMove = syntaxRoot.GetCurrentNode(member);
            var targetMemberTracked = syntaxRoot.GetCurrentNode(targetMember);
            if (!memberToMove.HasLeadingTrivia)
            {
                var targetIndentationLevel = IndentationHelper.GetIndentationSteps(indentationSettings, targetMember);
                var indentationString = IndentationHelper.GenerateIndentationString(indentationSettings, targetIndentationLevel);
                memberToMove = memberToMove.WithLeadingTrivia(SyntaxFactory.Whitespace(indentationString));
            }

            if (!HasLeadingBlankLines(targetMember)
                && HasLeadingBlankLines(member))
            {
                memberToMove = memberToMove.WithTrailingTrivia(memberToMove.GetTrailingTrivia().Add(SyntaxFactory.CarriageReturnLineFeed));
                memberToMove = memberToMove.WithLeadingTrivia(GetLeadingTriviaWithoutLeadingBlankLines(memberToMove));
            }

            syntaxRoot = syntaxRoot.InsertNodesBefore(targetMemberTracked, new[] { memberToMove });
            var fieldToMoveTracked = syntaxRoot.GetCurrentNodes(member).Last();
            syntaxRoot = syntaxRoot.RemoveNode(fieldToMoveTracked, SyntaxRemoveOptions.KeepNoTrivia);
            if (fileHeader.Any())
            {
                var oldFirstToken = syntaxRoot.GetCurrentNode(firstToken.Parent).ChildTokens().First();
                syntaxRoot = syntaxRoot.ReplaceToken(oldFirstToken, oldFirstToken.WithLeadingTrivia(StripFileHeader(oldFirstToken.LeadingTrivia)));
                var newFirstToken = syntaxRoot.GetFirstToken();
                syntaxRoot = syntaxRoot.ReplaceToken(newFirstToken, newFirstToken.WithLeadingTrivia(fileHeader.AddRange(newFirstToken.LeadingTrivia)));
            }

            return syntaxRoot;
        }

        private static SyntaxTriviaList StripFileHeader(SyntaxTriviaList newLeadingTrivia)
        {
            var fileHeader = GetFileHeader(newLeadingTrivia);
            return SyntaxTriviaList.Empty.AddRange(newLeadingTrivia.Skip(fileHeader.Count));
        }

        private static SyntaxTriviaList GetFileHeader(SyntaxTriviaList newLeadingTrivia)
        {
            var onBlankLine = false;
            var hasHeader = false;
            var fileHeader = new List<SyntaxTrivia>();
            for (var i = 0; i < newLeadingTrivia.Count; i++)
            {
                bool done = false;
                switch (newLeadingTrivia[i].Kind())
                {
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    fileHeader.Add(newLeadingTrivia[i]);
                    onBlankLine = false;
                    hasHeader = true;
                    break;

                case SyntaxKind.WhitespaceTrivia:
                    fileHeader.Add(newLeadingTrivia[i]);
                    break;

                case SyntaxKind.EndOfLineTrivia:
                    fileHeader.Add(newLeadingTrivia[i]);

                    if (onBlankLine)
                    {
                        done = true;
                    }
                    else
                    {
                        onBlankLine = true;
                    }

                    break;

                default:
                    done = true;
                    break;
                }

                if (done)
                {
                    break;
                }
            }

            return hasHeader ? SyntaxTriviaList.Empty.AddRange(fileHeader) : SyntaxTriviaList.Empty;
        }

        private static bool HasLeadingBlankLines(SyntaxNode node)
        {
            var firstTriviaIgnoringWhitespace = node.GetLeadingTrivia().FirstOrDefault(x => !x.IsKind(SyntaxKind.WhitespaceTrivia));
            return firstTriviaIgnoringWhitespace.IsKind(SyntaxKind.EndOfLineTrivia);
        }

        private static SyntaxTriviaList GetLeadingTriviaWithoutLeadingBlankLines(SyntaxNode node)
        {
            var leadingTrivia = node.GetLeadingTrivia();

            var skipIndex = 0;
            for (var i = 0; i < leadingTrivia.Count; i++)
            {
                var currentTrivia = leadingTrivia[i];
                if (currentTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    skipIndex = i + 1;
                }
                else if (!currentTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    // Preceded by whitespace
                    skipIndex = i > 0 && leadingTrivia[i - 1].IsKind(SyntaxKind.WhitespaceTrivia) ? i - 1 : i;
                    break;
                }
            }

            return SyntaxFactory.TriviaList(leadingTrivia.Skip(skipIndex));
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } = new FixAll();

            protected override string CodeActionTitle => OrderingResources.ElementOrderCodeFix;

            protected override async Task<SyntaxNode?> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var semanticModel = await document.GetSemanticModelAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, syntaxRoot.SyntaxTree, fixAllContext.CancellationToken);
                var elementOrder = settings.OrderingRules.ElementOrder;

                var fieldDependenciesByType = new Dictionary<string, Dictionary<string, ImmutableHashSet<string>>>();
                if (semanticModel != null)
                {
                    foreach (var typeDeclaration in syntaxRoot.DescendantNodesAndSelf().OfType<TypeDeclarationSyntax>())
                    {
                        var deps = FieldDependencyHelper.ComputeFieldDependencies(semanticModel, typeDeclaration.Members, fixAllContext.CancellationToken);
                        if (deps.Count > 0)
                        {
                            fieldDependenciesByType[GetTypeKey(typeDeclaration)] = deps;
                        }
                    }
                }

                var trackedMembers = new List<MemberDeclarationSyntax>();
                var trackedMembersSet = new HashSet<MemberDeclarationSyntax>();
                foreach (var diagnostic in diagnostics)
                {
                    var memberDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<MemberDeclarationSyntax>();
                    if (memberDeclaration == null)
                    {
                        continue;
                    }

                    if (diagnostic.Id == SA1201ElementsMustAppearInTheCorrectOrder.DiagnosticId
                        || settings.OrderingRules.MemberKindOrder != null)
                    {
                        // Moving one member can create a new ordering violation between previously adjacent siblings.
                        foreach (var sibling in memberDeclaration.Parent.ChildNodes().OfType<MemberDeclarationSyntax>())
                        {
                            if (trackedMembersSet.Add(sibling))
                            {
                                trackedMembers.Add(sibling);
                            }
                        }
                    }
                    else if (trackedMembersSet.Add(memberDeclaration))
                    {
                        trackedMembers.Add(memberDeclaration);
                    }
                }

                syntaxRoot = syntaxRoot.TrackNodes(trackedMembers);

                foreach (var member in trackedMembers)
                {
                    var memberDeclaration = syntaxRoot.GetCurrentNode(member);
                    syntaxRoot = UpdateSyntaxRoot(memberDeclaration, elementOrder, syntaxRoot, settings.Indentation, settings.OrderingRules.MemberKindOrder, semanticModel, fixAllContext.CancellationToken, fieldDependenciesByType);
                }

                return syntaxRoot;
            }
        }
    }
}

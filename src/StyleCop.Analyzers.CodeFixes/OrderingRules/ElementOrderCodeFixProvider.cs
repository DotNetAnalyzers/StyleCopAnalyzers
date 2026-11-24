// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
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
        private static readonly SyntaxAnnotation NameOrderBoundaryAnnotation = new SyntaxAnnotation();
        private static readonly SyntaxAnnotation NameOrderSuppressedAnnotation = new SyntaxAnnotation();

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
            var sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
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

            var endOfLineTrivia = FormattingHelper.GetEndOfLineForCodeFix(memberDeclaration.GetFirstToken(), sourceText, document.Project.Solution.Workspace.Options);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            if (diagnostic.Properties.ContainsKey(SA1201ElementsMustAppearInTheCorrectOrder.MemberNameOrderProperty)
                && memberDeclaration.Parent is TypeDeclarationSyntax type)
            {
                if (semanticModel == null || MemberNameOrder.RequiresStableMemberOrder(type, semanticModel, cancellationToken))
                {
                    return document;
                }

                var preserveInstanceOrder = MemberNameOrder.RequiresStableInstanceOrder(type, semanticModel, cancellationToken);
                var fieldDependencies = memberDeclaration is FieldDeclarationSyntax && elementOrder.Contains(OrderingTrait.Accessibility)
                    ? FieldDependencyHelper.ComputeFieldDependencies(semanticModel, type.Members, cancellationToken)
                    : null;
                var trackedRoot = TrackNameOrderMembers(syntaxRoot, new[] { memberDeclaration }, type.Members, semanticModel, cancellationToken);
                var reordered = OrderMemberGroupByName(trackedRoot, trackedRoot.GetCurrentNode(memberDeclaration), settings.OrderingRules, settings.Indentation, endOfLineTrivia, preserveInstanceOrder, fieldDependencies, cancellationToken);
                return reordered == trackedRoot ? document : document.WithSyntaxRoot(reordered);
            }

            var dependencies = semanticModel != null && memberDeclaration.Parent is TypeDeclarationSyntax parentType
                ? FieldDependencyHelper.ComputeFieldDependencies(semanticModel, parentType.Members, cancellationToken)
                : null;
            var updatedRoot = UpdateSyntaxRoot(memberDeclaration, elementOrder, syntaxRoot, settings.Indentation, endOfLineTrivia, settings.OrderingRules.MemberKindOrder, settings.OrderingRules.AccessibilityOrder, dependencies, cancellationToken);
            if (updatedRoot == syntaxRoot)
            {
                return document;
            }

            return document.WithSyntaxRoot(updatedRoot);
        }

        private static SyntaxNode OrderMemberGroupByName(
            SyntaxNode syntaxRoot,
            MemberDeclarationSyntax member,
            OrderingSettings orderingSettings,
            IndentationSettings indentationSettings,
            SyntaxTrivia endOfLineTrivia,
            bool preserveInstanceOrder,
            Dictionary<string, ImmutableHashSet<string>>? fieldDependencies,
            CancellationToken cancellationToken)
        {
            if (member.Parent is not TypeDeclarationSyntax type || !CanOrderByName(member, preserveInstanceOrder))
            {
                return syntaxRoot;
            }

            var members = type.Members;
            var priority = MemberNameOrder.GetGroupPriority(member, orderingSettings);
            var kind = MemberNameOrder.GetGroupKind(member, orderingSettings);
            var start = members.IndexOf(member);
            while (start > 0 && IsInNameOrderGroup(members[start - 1], kind, priority, orderingSettings, preserveInstanceOrder))
            {
                start--;
            }

            var group = new List<MemberDeclarationSyntax>();
            for (var i = start; i < members.Count && IsInNameOrderGroup(members[i], kind, priority, orderingSettings, preserveInstanceOrder); i++)
            {
                group.Add(members[i]);
            }

            syntaxRoot = syntaxRoot.TrackNodes(group);
            foreach (var item in group)
            {
                cancellationToken.ThrowIfCancellationRequested();
                syntaxRoot = OrderMemberByName(syntaxRoot, syntaxRoot.GetCurrentNode(item), orderingSettings, indentationSettings, endOfLineTrivia, preserveInstanceOrder, fieldDependencies);
            }

            return syntaxRoot;
        }

        private static SyntaxNode TrackNameOrderMembers(
            SyntaxNode syntaxRoot,
            IEnumerable<SyntaxNode> nodesToTrack,
            IEnumerable<MemberDeclarationSyntax> members,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var annotations = new Dictionary<MemberDeclarationSyntax, SyntaxAnnotation>();
            var candidates = new Dictionary<TextSpan, MemberDeclarationSyntax>();
            var diagnostics = new List<Diagnostic>();
            foreach (var member in members)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (MemberNameOrder.ContainsModuleInitializer(member, semanticModel, cancellationToken))
                {
                    annotations.Add(member, NameOrderBoundaryAnnotation);
                    continue;
                }

                var identifier = MemberNameOrder.GetSortableIdentifier(member);
                if (identifier.RawKind == 0)
                {
                    continue;
                }

                if (MemberNameOrder.IsInHiddenRegion(identifier, cancellationToken))
                {
                    annotations.Add(member, NameOrderBoundaryAnnotation);
                    continue;
                }

                if (MemberNameOrder.IsGeneratedDeclaration(member, semanticModel, cancellationToken))
                {
                    annotations.Add(member, NameOrderSuppressedAnnotation);
                    continue;
                }

                candidates.Add(identifier.Span, member);
                diagnostics.Add(Diagnostic.Create(SA1201ElementsMustAppearInTheCorrectOrder.MemberNameOrderDescriptor, identifier.GetLocation()));
            }

            // Check the original identifiers, including members that only become out of order after a move.
            // Roslyn applies both local and global suppressions as well as pragma directives.
            var compilation = semanticModel.Compilation;
            foreach (var diagnostic in CompilationWithAnalyzers.GetEffectiveDiagnostics(diagnostics, compilation))
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (diagnostic.IsSuppressed)
                {
                    var annotation = diagnostic.GetSuppressionInfo(compilation)?.Attribute == null
                        ? NameOrderBoundaryAnnotation
                        : NameOrderSuppressedAnnotation;
                    annotations.Add(candidates[diagnostic.Location.SourceSpan], annotation);
                }
            }

            syntaxRoot = syntaxRoot.TrackNodes(nodesToTrack.Concat(annotations.Keys).Distinct());
            var trackedAnnotations = annotations.ToDictionary(entry => syntaxRoot.GetCurrentNode(entry.Key), entry => entry.Value);
            return syntaxRoot.ReplaceNodes(
                trackedAnnotations.Keys,
                (original, rewritten) => rewritten.WithAdditionalAnnotations(trackedAnnotations[original]));
        }

        private static bool CanOrderByName(MemberDeclarationSyntax member, bool preserveInstanceOrder)
        {
            return !member.HasAnnotation(NameOrderBoundaryAnnotation)
                && MemberNameOrder.GetSortableIdentifier(member).RawKind != 0
                && (!preserveInstanceOrder || !MemberNameOrder.RequiresAutoLayout(member));
        }

        private static bool IsInNameOrderGroup(
            MemberDeclarationSyntax member,
            SyntaxKind kind,
            int priority,
            OrderingSettings orderingSettings,
            bool preserveInstanceOrder)
        {
            return MemberNameOrder.GetGroupKind(member, orderingSettings) == kind
                && CanOrderByName(member, preserveInstanceOrder)
                && MemberNameOrder.GetGroupPriority(member, orderingSettings) == priority;
        }

        private static SyntaxNode OrderMemberByName(
            SyntaxNode syntaxRoot,
            MemberDeclarationSyntax member,
            OrderingSettings orderingSettings,
            IndentationSettings indentationSettings,
            SyntaxTrivia endOfLineTrivia,
            bool preserveInstanceOrder,
            Dictionary<string, ImmutableHashSet<string>>? fieldDependencies)
        {
            // An attributed member can still be an insertion target for an unsuppressed member.
            if (member.HasAnnotation(NameOrderSuppressedAnnotation)
                || member.Parent is not TypeDeclarationSyntax type
                || !CanOrderByName(member, preserveInstanceOrder))
            {
                return syntaxRoot;
            }

            var members = type.Members;
            var priority = MemberNameOrder.GetGroupPriority(member, orderingSettings);
            var name = MemberNameOrder.GetSortableIdentifier(member).ValueText;
            var kind = MemberNameOrder.GetGroupKind(member, orderingSettings);
            MemberDeclarationSyntax? target = null;

            var index = members.IndexOf(member);
            var checkFieldAccessibility = fieldDependencies != null && member is FieldDeclarationSyntax field && FieldDependencyHelper.IsDependency(field, fieldDependencies);
            for (var i = index - 1; i >= 0; i--)
            {
                var previous = members[i];
                if (!IsInNameOrderGroup(previous, kind, priority, orderingSettings, preserveInstanceOrder)
                    || string.CompareOrdinal(MemberNameOrder.GetSortableIdentifier(previous).ValueText, name) <= 0
                    || MemberNameOrder.HasOrderSensitiveInitializers(member, previous)
                    || MemberNameOrder.StartsOutOfOrderGroup(members, i, orderingSettings)
                    || (checkFieldAccessibility && MemberNameOrder.CouldExposeFieldAccessibilityInversion(members, index, i, orderingSettings, fieldDependencies!)))
                {
                    break;
                }

                target = previous;
            }

            return target == null ? syntaxRoot : MoveMember(syntaxRoot, member, target, indentationSettings, endOfLineTrivia);
        }

        private static SyntaxNode UpdateSyntaxRoot(
            MemberDeclarationSyntax memberDeclaration,
            ImmutableArray<OrderingTrait> elementOrder,
            SyntaxNode syntaxRoot,
            IndentationSettings indentationSettings,
            SyntaxTrivia endOfLineTrivia,
            MemberKindOrder? configuredOrder,
            ImmutableArray<AccessLevel> accessibilityOrder,
            Dictionary<string, ImmutableHashSet<string>>? fieldDependencies,
            CancellationToken cancellationToken)
        {
            var parentDeclaration = memberDeclaration.Parent;
            var memberToMove = new MemberOrderHelper(memberDeclaration, elementOrder, accessibilityOrder);

            if (parentDeclaration is TypeDeclarationSyntax syntax)
            {
                return HandleTypeDeclaration(memberToMove, syntax, elementOrder, syntaxRoot, indentationSettings, endOfLineTrivia, configuredOrder, accessibilityOrder, fieldDependencies, cancellationToken);
            }

            if (BaseNamespaceDeclarationSyntaxWrapper.IsInstance(parentDeclaration))
            {
                return HandleBaseNamespaceDeclaration(memberToMove, (BaseNamespaceDeclarationSyntaxWrapper)parentDeclaration, elementOrder, accessibilityOrder, syntaxRoot, indentationSettings, endOfLineTrivia);
            }

            if (parentDeclaration is CompilationUnitSyntax syntax1)
            {
                return HandleCompilationUnitDeclaration(memberToMove, syntax1, elementOrder, accessibilityOrder, syntaxRoot, indentationSettings, endOfLineTrivia);
            }

            return syntaxRoot;
        }

        private static SyntaxNode HandleTypeDeclaration(
            MemberOrderHelper memberOrder,
            TypeDeclarationSyntax typeDeclarationNode,
            ImmutableArray<OrderingTrait> elementOrder,
            SyntaxNode syntaxRoot,
            IndentationSettings indentationSettings,
            SyntaxTrivia endOfLineTrivia,
            MemberKindOrder? configuredOrder,
            ImmutableArray<AccessLevel> accessibilityOrder,
            Dictionary<string, ImmutableHashSet<string>>? fieldDependencies,
            CancellationToken cancellationToken)
        {
            if (configuredOrder != null && elementOrder.Contains(OrderingTrait.Kind))
            {
                return OrderConfiguredMember(memberOrder.Member, typeDeclarationNode.Members, configuredOrder, syntaxRoot, indentationSettings, endOfLineTrivia, fieldDependencies, cancellationToken);
            }

            return OrderMember(memberOrder, typeDeclarationNode.Members, elementOrder, accessibilityOrder, syntaxRoot, indentationSettings, endOfLineTrivia, fieldDependencies);
        }

        private static SyntaxNode OrderConfiguredMember(
            MemberDeclarationSyntax member,
            SyntaxList<MemberDeclarationSyntax> members,
            MemberKindOrder order,
            SyntaxNode syntaxRoot,
            IndentationSettings indentationSettings,
            SyntaxTrivia endOfLineTrivia,
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
            var hasInitializer = MemberNameOrder.HasRuntimeInitializer(member);
            var targetIndex = -1;
            var gapIndex = -1;
            for (var i = memberIndex - 1; i >= 0; i--)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var previous = members[i];
                if (previous.IsKind(SyntaxKind.IncompleteMember)
                    || (hasInitializer
                        && MemberNameOrder.HasRuntimeInitializer(previous)
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

            return MoveMember(syntaxRoot, member, members[targetIndex], indentationSettings, endOfLineTrivia);
        }

        private static SyntaxNode HandleCompilationUnitDeclaration(MemberOrderHelper memberOrder, CompilationUnitSyntax compilationUnitDeclaration, ImmutableArray<OrderingTrait> elementOrder, ImmutableArray<AccessLevel> accessibilityOrder, SyntaxNode syntaxRoot, IndentationSettings indentationSettings, SyntaxTrivia endOfLineTrivia)
        {
            return OrderMember(memberOrder, compilationUnitDeclaration.Members, elementOrder, accessibilityOrder, syntaxRoot, indentationSettings, endOfLineTrivia, fieldDependencies: null);
        }

        private static SyntaxNode HandleBaseNamespaceDeclaration(MemberOrderHelper memberOrder, BaseNamespaceDeclarationSyntaxWrapper namespaceDeclaration, ImmutableArray<OrderingTrait> elementOrder, ImmutableArray<AccessLevel> accessibilityOrder, SyntaxNode syntaxRoot, IndentationSettings indentationSettings, SyntaxTrivia endOfLineTrivia)
        {
            return OrderMember(memberOrder, namespaceDeclaration.Members, elementOrder, accessibilityOrder, syntaxRoot, indentationSettings, endOfLineTrivia, fieldDependencies: null);
        }

        private static SyntaxNode OrderMember(
            MemberOrderHelper memberOrder,
            SyntaxList<MemberDeclarationSyntax> members,
            ImmutableArray<OrderingTrait> elementOrder,
            ImmutableArray<AccessLevel> accessibilityOrder,
            SyntaxNode syntaxRoot,
            IndentationSettings indentationSettings,
            SyntaxTrivia endOfLineTrivia,
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

                var orderHelper = new MemberOrderHelper(members[i], elementOrder, accessibilityOrder);
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

            return MoveMember(syntaxRoot, memberOrder.Member, target.Member, indentationSettings, endOfLineTrivia);
        }

        private static SyntaxNode MoveMember(SyntaxNode syntaxRoot, MemberDeclarationSyntax member, MemberDeclarationSyntax targetMember, IndentationSettings indentationSettings, SyntaxTrivia endOfLineTrivia)
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
                memberToMove = memberToMove.WithTrailingTrivia(memberToMove.GetTrailingTrivia().Add(endOfLineTrivia));
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
                var sourceText = await document.GetTextAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var semanticModel = await document.GetSemanticModelAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, syntaxRoot.SyntaxTree, fixAllContext.CancellationToken);
                var elementOrder = settings.OrderingRules.ElementOrder;

                var fieldDependenciesByType = new Dictionary<TypeDeclarationSyntax, Dictionary<string, ImmutableHashSet<string>>>();
                if (semanticModel != null)
                {
                    foreach (var typeDeclaration in syntaxRoot.DescendantNodesAndSelf().OfType<TypeDeclarationSyntax>())
                    {
                        var deps = FieldDependencyHelper.ComputeFieldDependencies(semanticModel, typeDeclaration.Members, fixAllContext.CancellationToken);
                        if (deps.Count > 0)
                        {
                            fieldDependenciesByType[typeDeclaration] = deps;
                        }
                    }
                }

                var trackedMembers = new List<MemberDeclarationSyntax>();
                var trackedMembersSet = new HashSet<MemberDeclarationSyntax>();
                var typesToOrderByName = new Dictionary<TypeDeclarationSyntax, bool>();
                var visitedTypes = new HashSet<TypeDeclarationSyntax>();
                var orderByName = semanticModel != null && elementOrder.Contains(OrderingTrait.Name);
                foreach (var diagnostic in diagnostics)
                {
                    var memberDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<MemberDeclarationSyntax>();
                    if (memberDeclaration == null)
                    {
                        continue;
                    }

                    if (orderByName
                        && diagnostic.Id == SA1201ElementsMustAppearInTheCorrectOrder.DiagnosticId
                        && memberDeclaration.Parent is TypeDeclarationSyntax type
                        && visitedTypes.Add(type)
                        && !MemberNameOrder.RequiresStableMemberOrder(type, semanticModel, fixAllContext.CancellationToken))
                    {
                        var preserveInstanceOrder = MemberNameOrder.RequiresStableInstanceOrder(type, semanticModel, fixAllContext.CancellationToken);
                        typesToOrderByName.Add(type, preserveInstanceOrder);
                    }

                    if (diagnostic.Properties.ContainsKey(SA1201ElementsMustAppearInTheCorrectOrder.MemberNameOrderProperty))
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

                syntaxRoot = typesToOrderByName.Count > 0 && semanticModel != null
                    ? TrackNameOrderMembers(syntaxRoot, trackedMembers.Concat(typesToOrderByName.Keys), typesToOrderByName.Keys.SelectMany(type => type.Members), semanticModel, fixAllContext.CancellationToken)
                    : syntaxRoot.TrackNodes(trackedMembers);

                foreach (var member in trackedMembers)
                {
                    var memberDeclaration = syntaxRoot.GetCurrentNode(member);
                    var endOfLineTrivia = FormattingHelper.GetEndOfLineForCodeFix(memberDeclaration.GetFirstToken(), sourceText, document.Project.Solution.Workspace.Options);
                    Dictionary<string, ImmutableHashSet<string>>? fieldDependencies = null;
                    if (member.Parent is TypeDeclarationSyntax originalType)
                    {
                        fieldDependenciesByType.TryGetValue(originalType, out fieldDependencies);
                    }

                    syntaxRoot = UpdateSyntaxRoot(memberDeclaration, elementOrder, syntaxRoot, settings.Indentation, endOfLineTrivia, settings.OrderingRules.MemberKindOrder, settings.OrderingRules.AccessibilityOrder, fieldDependencies, fixAllContext.CancellationToken);
                }

                foreach (var entry in typesToOrderByName)
                {
                    fieldDependenciesByType.TryGetValue(entry.Key, out var fieldDependencies);
                    var type = syntaxRoot.GetCurrentNode(entry.Key);
                    var members = type.Members.Where(member => CanOrderByName(member, entry.Value)).ToList();
                    var reorderedText = syntaxRoot.GetText(encoding: sourceText.Encoding);
                    syntaxRoot = syntaxRoot.TrackNodes(members);
                    foreach (var member in members)
                    {
                        fixAllContext.CancellationToken.ThrowIfCancellationRequested();
                        var trackedMember = syntaxRoot.GetCurrentNode(member);
                        var endOfLineTrivia = FormattingHelper.GetEndOfLineForCodeFix(member.GetFirstToken(), reorderedText, document.Project.Solution.Workspace.Options);
                        syntaxRoot = OrderMemberByName(syntaxRoot, trackedMember, settings.OrderingRules, settings.Indentation, endOfLineTrivia, entry.Value, fieldDependencies);
                    }
                }

                return syntaxRoot;
            }
        }
    }
}
